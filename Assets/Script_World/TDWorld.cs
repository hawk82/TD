using UnityEngine;
using System.Collections.Generic;

public class TDWorld : MonoBehaviour {

	void Awake()
	{
		m_defaultStrategy = new TDDefaultTowerStrategy();
		m_fairStrategy    = new TDFairTowerStrategy();
		m_stupidStrategy  = new TDStupidTowerStrategy();
		m_configuration = new TDConfiguration();
		m_configuration.readFromResource();

		GameObject terrain = getTerrain();
		Bounds terrainBounds = terrain.collider.bounds;
		Vector3 lowPnt = from3dTo2d(terrainBounds.min);
		Vector3 highPnt = from3dTo2d(terrainBounds.max);
		m_grid = new TDGrid();
		m_grid.initialize(m_configuration.gridNbCellsX, m_configuration.gridNbCellsY,
						  lowPnt.x + 5f, lowPnt.y + 5f, highPnt.x - lowPnt.x - 10f, highPnt.y - lowPnt.y - 10f);

		// Take into account obstacles
		GameObject [] aObstacles = getAllObstacles();
		foreach (GameObject obj in aObstacles)
		{
			TDGrid.CellState cellState = TDGrid.CellState.eBusy;
			switch (obj.tag)
			{
				case "Player":
					cellState = TDGrid.CellState.ePlayer;
					break;
				case "EnemyRespawn":
					cellState = TDGrid.CellState.eEnemyRespawn;
					break;
			}
			Bounds b = obj.renderer.bounds;
			occupyRegion3d(b.min, b.max, cellState);
		}

		// put the dynamic obstacles
		uint treesBuilt = 0;
		while (treesBuilt < m_configuration.nbTrees)
		{
			uint i = (uint)((Random.value)*(float)(m_grid.nbCellsX));
			uint j = (uint)((Random.value)*(float)(m_grid.nbCellsY));
			TDGrid.Cell cell = new TDGrid.Cell();
			cell.m_i = i;
			cell.m_j = j;
			if (m_grid.cellState(cell) != TDGrid.CellState.eFree)
				continue;
			Vector3 pos = m_grid.getCenter(cell);
			pos = from2dTo3d(pos);
			occupyPosition3d(pos, TDGrid.CellState.eBusy);
			addTree3d(pos);
			treesBuilt++;
		}
	}

	// Use this for initialization
	void Start () {
		m_startTime = -1;
		m_frequency = 1;
	}
	
	// Update is called once per frame
	void Update () {
		if (m_startTime != (int) ((float)(m_frequency)*Time.time))
		{
			// Random position
			GameObject [] aRespawnPoint = getAllEnemyRespawnPoints();
			uint respawnIndex = (uint)(Random.value*aRespawnPoint.Length);
			Vector3 pos = aRespawnPoint[respawnIndex].transform.position;
			if (Random.value < 0.3)
				addEnemy3d(TDEnemy.Type.eGargoyle, pos);
			else
				addEnemy3d(TDEnemy.Type.eImp, pos);
			m_startTime = (int) ((float)(m_frequency)*Time.time);
		}
	}

	void LateUpdate()
	{
		m_aCachedEnemies = null;
	}
	
	public static TDWorld getWorld()
	{
		GameObject [] aWorlds = GameObject.FindGameObjectsWithTag("World");
		if (0 == aWorlds.Length)
			return null;
		TDWorld world = (TDWorld) (aWorlds[0].GetComponent("TDWorld"));
		return world;
	}

	public GameObject getPlayer()
	{
		GameObject [] aPlayers = GameObject.FindGameObjectsWithTag("Player");
		if (0 == aPlayers.Length)
			return null;
		return aPlayers[0];
	}

	public GameObject getHero()
	{
		GameObject [] aHeros = GameObject.FindGameObjectsWithTag("Hero");
		if (0 == aHeros.Length)
			return null;
		return aHeros[0];
	}

	public GameObject getHeroRespawnPoint()
	{
		GameObject [] aPts = GameObject.FindGameObjectsWithTag("HeroRespawnPoint");
		if (0 == aPts.Length)
			return null;
		return aPts[0];
	}

	public GameObject getTerrain()
	{
		GameObject [] aTerrains = GameObject.FindGameObjectsWithTag("Terrain");
		if (0 == aTerrains.Length)
			return null;
		return aTerrains[0];
	}

	public TDPlayer getTDPlayer()
	{
		GameObject player = getPlayer();
		if (null == player)
			return null;
		return (TDPlayer) player.GetComponent<TDPlayer>();
	}

	public TDHero getTDHero()
	{
		GameObject player = getHero();
		if (null == player)
			return null;
		return (TDHero) player.GetComponent<TDHero>();
	}

	public TDEnemy getTDEnemy(GameObject obj)
	{
		if (null == obj)
			return null;
		return (TDEnemy) obj.GetComponent<TDEnemy>();
	}

	public TDTower getTDTower(GameObject obj)
	{
		return (TDTower) obj.GetComponent<TDTower>();
	}

	// May contain incomplete list or some deleted entries
	public GameObject [] getAllEnemiesUnsafe()
	{
		if (m_aCachedEnemies == null)
		{
			m_aCachedEnemies = GameObject.FindGameObjectsWithTag("Enemy");
		}
		return m_aCachedEnemies;
	}
	GameObject [] m_aCachedEnemies;


	public GameObject [] getAllTowers()
	{
		return GameObject.FindGameObjectsWithTag("Tower");
	}

	public GameObject [] getAllObstacles()
	{
		GameObject [] aObstacles = GameObject.FindGameObjectsWithTag("Obstacle");
		GameObject [] aPlayer = GameObject.FindGameObjectsWithTag("Player");
		GameObject [] aEnemyRespawn = GameObject.FindGameObjectsWithTag("EnemyRespawn");
		List<GameObject> aAllObstacles = new List<GameObject>();
		aAllObstacles.AddRange(aObstacles);
		aAllObstacles.AddRange(aPlayer);
		aAllObstacles.AddRange(aEnemyRespawn);
		return aAllObstacles.ToArray();
	}

	public GameObject [] getAllEnemyRespawnPoints()
	{
		return GameObject.FindGameObjectsWithTag("EnemyRespawn");
	}
	
	public bool isFakeTarget(GameObject obj)
	{
		if (null == obj)
			return false;
		return (obj.tag == "FakeTarget");
	}

	public TDGrid.CellState positionState3d(Vector3 pos)
	{
		Vector3 res = from3dTo2d(pos);
		TDGrid.Cell cell = m_grid.getCell(res);
		return m_grid.cellState(cell);
	}

	public TDGrid.CellState positionState2d(Vector3 pos)
	{
		TDGrid.Cell cell = m_grid.getCell(pos);
		return m_grid.cellState(cell);
	}

	public void occupyPosition3d(Vector3 pos, TDGrid.CellState occupyState)
	{
		Vector3 res = from3dTo2d(pos);
		TDGrid.Cell cell = m_grid.getCell(res);
		m_grid.setCellState(cell, occupyState);
	}

	public void occupyRegion3d(Vector3 minPos, Vector3 maxPos, TDGrid.CellState occupyState)
	{
		minPos = from3dTo2d(minPos);
		TDGrid.Cell minCell = m_grid.getCell(minPos);
		maxPos = from3dTo2d(maxPos);
		TDGrid.Cell maxCell = m_grid.getCell(maxPos);
		TDGrid.Cell cell = new TDGrid.Cell();
		for (uint i=minCell.m_i; i<=maxCell.m_i; i++)
			for (uint j=minCell.m_j; j<=maxCell.m_j; j++)
			{
				cell.m_i = i; cell.m_j = j;
				m_grid.setCellState(cell, occupyState);
			}
	}

	public Vector3 truncate3d(Vector3 pos)
	{
		Vector3 res = from3dTo2d(pos);
		TDGrid.Cell cell = m_grid.getCell(res);
		res = m_grid.getCenter(cell);
		res = from2dTo3d(res);
		return res;
	}

	public GameObject addEnemy3d(TDEnemy.Type type, Vector3 pos)
	{
		GameObject enemy = null;
		pos = from3dTo2d(pos);
		pos = from2dTo3d(pos);
		switch (type)
		{
			case TDEnemy.Type.eImp:
				enemy = (GameObject) Instantiate(m_prefabEnemyImp, pos, Quaternion.identity);
				break;
			case TDEnemy.Type.eGargoyle:
				enemy = (GameObject) Instantiate(m_prefabEnemyGargoyle, pos, Quaternion.identity);
				break;
				
		}
		return enemy;
	}

	public GameObject addTower3d(TDTower.Type type, Vector3 pos)
	{
		pos = from3dTo2d(pos);
		pos = from2dTo3d(pos);
		GameObject tower = null;
		switch (type)
		{
			case TDTower.Type.eArrowTower:
				tower = (GameObject) Instantiate(m_prefabArrowTower, pos, Quaternion.identity);
				break;
			case TDTower.Type.eCanonTower:
				tower = (GameObject) Instantiate(m_prefabCanonTower, pos, Quaternion.identity);
				break;
			case TDTower.Type.eIceTower:
				tower = (GameObject) Instantiate(m_prefabIceTower, pos, Quaternion.identity);
				break;
				
		}
		return tower;
	}

	public GameObject addTree3d(Vector3 pos)
	{
		pos = from3dTo2d(pos);
		pos = from2dTo3d(pos);
		return (GameObject) Instantiate(m_prefabTree, pos, Quaternion.identity);
	}

	public bool canTowerBeBuiltAt3d(Vector3 pos)
	{
		GameObject player = getPlayer();
		GameObject [] aRespawnPoints = getAllEnemyRespawnPoints();

		TDGrid.Cell startCell = m_grid.getCell(from3dTo2d(player.transform.position));
		TDGrid.Cell checkCell = m_grid.getCell(from3dTo2d(pos));

		if (m_grid.cellState(checkCell) != TDGrid.CellState.eFree)
			return false;

		bool canBuild = true;
		try
		{
			m_grid.setCellState(checkCell, TDGrid.CellState.eBusy);
			foreach (GameObject respawnPoint in aRespawnPoints)
			{
				TDGrid.Cell endCell = m_grid.getCell(from3dTo2d(respawnPoint.transform.position));
				TDGrid.Cell[] path;
				bool pathExists = m_grid.buildPath(startCell, endCell, out path);
				if (!pathExists)
				{
					canBuild = false;
					break;
				}
			}
		}
		catch (UnityException)
		{
		}
		finally
		{
			m_grid.setCellState(checkCell, TDGrid.CellState.eFree);
		}

		return canBuild;
	}

	public Vector3 from2dTo3d(Vector3 vec2d)
	{
		float height = Terrain.activeTerrain.SampleHeight(new Vector3(vec2d.x, 0, vec2d.y));
		return new Vector3(vec2d.x, height, vec2d.y);
	}

	public Vector3 from3dTo2d(Vector3 vec3d)
	{
		return new Vector3(vec3d.x, vec3d.z, 0);
	}

	public TDConfiguration m_configuration;
	public TDTowerStrategy m_defaultStrategy;
	public TDTowerStrategy m_fairStrategy;
	public TDTowerStrategy m_stupidStrategy;
	public TDGrid m_grid;

	public GameObject m_prefabEnemyImp;
	public GameObject m_prefabEnemyGargoyle;

	public GameObject m_prefabArrowTower;
	public GameObject m_prefabCanonTower;
	public GameObject m_prefabIceTower;
	public GameObject m_prefabTree;

	int m_frequency;
    int m_startTime;

}
