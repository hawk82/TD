using UnityEngine;
using System.Collections;

public class TDTerrainGrid : MonoBehaviour {

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		if (TDWorld.getWorld().m_configuration.drawGrid == 0)
			return;
		TDGrid grid = TDWorld.getWorld().m_grid;
		if (grid == null)
			return;
		if (m_aLines == null)
		{
			m_aLines = new GameObject[grid.nbCellsX + grid.nbCellsY + 2];
			GameObject terrainGridLinePrefab = (GameObject) Resources.Load("TerrainGridLinePrefab");
			for (int i=0; i<m_aLines.Length; i++)
			{
				GameObject obj = (GameObject) Instantiate(terrainGridLinePrefab, new Vector3(), new Quaternion());
				m_aLines[i] = obj;
				obj.AddComponent<LineRenderer>();
			}
		}
		
		int lrCount = 0;
		float startX = grid.m_startX;
		float startY = grid.m_startY;
		float stepX  = grid.m_gridX;
		float stepY  = grid.m_gridY;
		for (uint i=0; i<=grid.nbCellsX; i++)
		{
			GameObject line = m_aLines[lrCount];
			lrCount++;
			LineRenderer lineRenderer = line.GetComponent<LineRenderer>();
			lineRenderer.SetWidth(1, 1);
			lineRenderer.SetColors(Color.blue, Color.blue);
			lineRenderer.SetVertexCount(2);
			Vector3 startLine = new Vector3(startX + ((float) i)*stepX, startY, 0);
			Vector3 endLine   = new Vector3(startX + ((float) i)*stepX, startY + grid.width, 0);
			lineRenderer.SetPosition(0, TDWorld.getWorld().from2dTo3d(startLine));
			lineRenderer.SetPosition(1, TDWorld.getWorld().from2dTo3d(endLine));
		}

		for (uint i=0; i<=grid.nbCellsY; i++)
		{
			GameObject line = m_aLines[lrCount];
			lrCount++;
			LineRenderer lineRenderer = line.GetComponent<LineRenderer>();
			lineRenderer.SetWidth(1, 1);
			lineRenderer.SetColors(Color.blue, Color.blue);
			lineRenderer.SetVertexCount(2);
			Vector3 startLine = new Vector3(startX, startY + ((float) i)*stepY, 0);
			Vector3 endLine   = new Vector3(startX + grid.length, startY + ((float) i)*stepY, 0);
			lineRenderer.SetPosition(0, TDWorld.getWorld().from2dTo3d(startLine));
			lineRenderer.SetPosition(1, TDWorld.getWorld().from2dTo3d(endLine));
		}
	}

	GameObject [] m_aLines;
}
