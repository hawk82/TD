using UnityEngine;
using System.Collections;

public class TDGUILogic : MonoBehaviour {

	// Use this for initialization
	void Start () {
		m_mode = Mode.eNone;
		m_skipThisUpdate = false;
	}
	
	// Update is called once per frame
	void Update () {
		
		if (m_skipThisUpdate)
		{
			m_skipThisUpdate = false;
			return;
		}
		TDWorld world = TDWorld.getWorld();
		TDPlayer tdPlayer = world.getTDPlayer();
		if (tdPlayer.health() <= 0)
		{
			Application.LoadLevel("GameOver");
		}

		Camera camera = Camera.main;
        if (Input.GetMouseButtonDown(0))
        {
            m_dragOrigin = Input.mousePosition;
            return;
        }

        if (Input.GetMouseButton(0))
		{
			Ray ray = camera.ScreenPointToRay(Input.mousePosition);
			Ray oldRay = camera.ScreenPointToRay(m_dragOrigin);
			m_dragOrigin = Input.mousePosition;

			GameObject terrain = TDWorld.getWorld().getTerrain();
			
			RaycastHit hit, oldHit;
			if (terrain.collider.Raycast(ray, out hit, 10000f) && terrain.collider.Raycast(oldRay, out oldHit, 10000f))
			{
				Vector3 dir = hit.point - oldHit.point;
 	        	Vector3 move = new Vector3(-dir.x, 0f, 0f);
 				camera.transform.Translate(move, Space.World); 
			}
			return;
		}

		if (Mode.eNone == m_mode)
			return;

		if (Input.GetMouseButtonUp(0))
		{
			Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			if (Physics.Raycast(mouseRay, out hit))
			{
				if (hit.transform.gameObject.Equals(GameObject.Find("Terrain")))
				{
					Vector3 pos = hit.point;
					pos = TDWorld.getWorld().truncate3d(pos);
					GameObject newTower = null;
					if (TDGrid.CellState.eFree == world.positionState3d(pos))
					{
						switch (m_mode)
						{
							case Mode.eArcher:
								if (!world.canTowerBeBuiltAt3d(pos))
									break;
								newTower = world.addTower3d(TDTower.Type.eArrowTower, pos);
								break;
							case Mode.eCanon:
								if (!world.canTowerBeBuiltAt3d(pos))
									break;
								newTower = world.addTower3d(TDTower.Type.eCanonTower, pos);
								break;
							case Mode.eIce:
								if (!world.canTowerBeBuiltAt3d(pos))
									break;
								newTower = world.addTower3d(TDTower.Type.eIceTower, pos);
								break;
							case Mode.eHeroPatrol:
								TDHero tdHero = world.getTDHero();
								tdHero.patrol(pos);
								break;
							default:
								return;
						}
						if (newTower != null)
						{
							TDTower tdTower = world.getTDTower(newTower);
							if (!tdPlayer.affords(tdTower.price()))
							{
								DestroyObject(newTower);
								return;
							}
							tdPlayer.expense(tdTower.price());
							TDWorld.getWorld().occupyPosition3d(pos, TDGrid.CellState.eBusy);
						}
					}
				}
			}
		}
	}

	void OnGUI ()
	{
		float screenWidth = Screen.width;
		float screenHeight = Screen.height;

		float outfit = 20f;
		float width = 0.3f*screenWidth;
		float height = 0.1f*screenHeight;
		float textHeight = 0.4f*height;

		GUI.Box(new Rect(outfit, outfit, width, height), "");

		string healthString;
		healthString = TDWorld.getWorld().getTDPlayer().health() + " HP";
		GUI.Label(new Rect(2*outfit, 2*outfit, 50, textHeight), healthString);

		string moneyString;
		moneyString = TDWorld.getWorld().getTDPlayer().money() + " $";
		GUI.Label(new Rect(100, 2*outfit, 50, textHeight), moneyString);

		GUI.Box(new Rect(screenWidth - outfit - width, screenHeight - outfit - height, width, height), "Towers");
		
		GUI.SetNextControlName("Archer");
		if (GUI.Button(new Rect(screenWidth - width, screenHeight - height, 80, textHeight), "Archer"))
		{
			m_mode = Mode.eArcher;
			m_skipThisUpdate = true;
		}

		GUI.SetNextControlName("Canonier");
		if (GUI.Button(new Rect(screenWidth - width + 80 + outfit, screenHeight - height, 80, textHeight), "Canonier"))
		{
			m_mode = Mode.eCanon;
			m_skipThisUpdate = true;
		}

		GUI.SetNextControlName("Ice");
		if (GUI.Button(new Rect(screenWidth - width + 180 + outfit, screenHeight - height, 80, textHeight), "Ice"))
		{
			m_mode = Mode.eIce;
			m_skipThisUpdate = true;
		}

		GUI.Box(new Rect(outfit, screenHeight - height - outfit, width, height), "Eric the Strongblade");

		string heroHealthString;
		heroHealthString = (int) Mathf.Ceil(TDWorld.getWorld().getTDHero().health()) + " HP";
		GUI.Label(new Rect(2*outfit, screenHeight - height, 50, textHeight), heroHealthString);
		
		GUI.SetNextControlName("Patrol");
		if (GUI.Button(new Rect(50f+2f*outfit, screenHeight - height, 50, textHeight), "Patrol"))
		{
			m_mode = Mode.eHeroPatrol;
			m_skipThisUpdate = true;
		}

		GUI.SetNextControlName("To base!");
		if (GUI.Button(new Rect(100f+3f*outfit, screenHeight - height, 50, textHeight), "Base!"))
		{
			m_mode = Mode.eHeroToBase;
			TDHero tdHero = TDWorld.getWorld().getTDHero();
			tdHero.runToBase();
			m_skipThisUpdate = true;
		}

		switch (m_mode)
		{
			case Mode.eArcher:
				GUI.FocusControl("Archer");
				break;
			case Mode.eCanon:
				GUI.FocusControl("Canonier");
				break;
			case Mode.eHeroPatrol:
				GUI.FocusControl("Patrol");
				break;
			case Mode.eHeroToBase:
				GUI.FocusControl("To base!");
				break;
		}
	}

	enum Mode
	{
		eNone       = 0,
		eArcher     = 1,
		eCanon      = 2,
		eIce        = 3,
		eHeroPatrol = 20,
		eHeroToBase = 21
	}
	
	bool m_skipThisUpdate;
	Mode m_mode;
    private Vector3 m_dragOrigin;
}
