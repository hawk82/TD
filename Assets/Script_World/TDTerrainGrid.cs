﻿using UnityEngine;
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
			m_aLines = new GameObject[grid.nbCellsX*grid.nbCellsY];
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
		for (uint i=0; i<grid.nbCellsX; i++)
		{
			for (uint j=0; j<grid.nbCellsY; j++)
			{
				GameObject line = m_aLines[lrCount];
				lrCount++;
				LineRenderer lineRenderer = line.GetComponent<LineRenderer>();
				lineRenderer.SetWidth(0.3f, 0.3f);
				lineRenderer.SetColors(Color.blue, Color.blue);
				lineRenderer.SetVertexCount(4);

				Vector3 bottomLeft  = new Vector3(startX + ((float) i)*stepX, startY + ((float) j)*stepY, 0);
				Vector3 bottomRight = new Vector3(bottomLeft.x + stepX, bottomLeft.y, 0);
				Vector3 upRight     = new Vector3(bottomLeft.x + stepX, bottomLeft.y + stepY, 0);
				Vector3 upLeft      = new Vector3(bottomLeft.x, bottomLeft.y + stepY, 0);
				lineRenderer.SetPosition(0, TDWorld.getWorld().from2dTo3d(bottomLeft));
				lineRenderer.SetPosition(1, TDWorld.getWorld().from2dTo3d(bottomRight));
				lineRenderer.SetPosition(2, TDWorld.getWorld().from2dTo3d(upRight));
				lineRenderer.SetPosition(3, TDWorld.getWorld().from2dTo3d(upLeft));
			}
		}
	}

	GameObject [] m_aLines;
}
