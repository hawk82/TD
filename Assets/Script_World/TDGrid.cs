using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class TDGrid
{
	public TDGrid()
	{
		m_ncx = m_ncy = 1;
		reallocate();
		m_length = m_width = 1f;
		m_startX = m_startY = 0;
	}

	public bool initialize(uint nbCellsX, uint nbCellsY,
		float startX, float startY, float length, float width)
	{
		m_ncx = nbCellsX;
		m_ncy = nbCellsY;
		m_startX = startX;
		m_startY = startY;
		m_length = length;
		m_width  = width;
		reallocate();
		recalcGrid();
		return true;
	}

	public uint nbCellsX
	{
		get {return m_ncx;}
		set {m_ncx = value; reallocate(); recalcGrid();}
	}
	public uint nbCellsY
	{
		get {return m_ncy;}
		set {m_ncy = value; reallocate(); recalcGrid();}
	}

	public float length
	{
		get {return m_length;}
		set {m_length = value; recalcGrid();}
	}

	public float width
	{
		get {return m_width;}
		set {m_width = value; recalcGrid();}
	}

	public enum CellState {eFree = 0, eBusy, ePlayer, eEnemyRespawn};
	public struct Cell
	{
		Cell(uint i, uint j) {m_i = i; m_j = j;}
		public uint m_i, m_j;
	}

	public CellState cellState(Cell cell)
	{
		if (cell.m_i < 0 || cell.m_i >= m_ncx)
			return CellState.eBusy;
		if (cell.m_j < 0 || cell.m_j >= m_ncy)
			return CellState.eBusy;
		return m_aCells[cell.m_i, cell.m_j];
	}

	public void setCellState(Cell cell, CellState state)
	{
		m_aCells[cell.m_i, cell.m_j] = state;
	}

	public Vector2 getCenter(Cell cell)
	{
		float cx = m_gridX*((float)(cell.m_i) + 0.5f) + m_startX;
		float cy = m_gridY*((float)(cell.m_j) + 0.5f) + m_startY;
		return new Vector2(cx, cy);
	}

	public Cell getCell(Vector2 pos)
	{
		Cell cell = new Cell();
		pos.x -= m_startX;
		if (pos.x < 0f)
			cell.m_i = 0;
		else			
			cell.m_i = (uint)((pos.x/m_gridX));
		pos.y -= m_startY;
		if (pos.y < 0f)
			cell.m_j = 0;
		else			
			cell.m_j = (uint)((pos.y/m_gridX));
		return cell;
	}

	class PathNode
	{
		public Cell m_cell;
		public PathNode m_parentNode;

		public int m_stepsFromStart; 
		public int m_heuristicRating;
		public int m_priority // less is better
		{
			get
			{
				return m_stepsFromStart + m_heuristicRating;
			}
		}
	}

	//Gets neighborhoods if exists
	List<Cell> getSuccessors(Cell sourceCell)
	{
		sbyte[,] direction = new sbyte[4, 2] { { 0, -1 }, { 1, 0 }, { 0, 1 }, { -1, 0 } };
		List<Cell> successors = new List<Cell>();

		for (int i = 0; i < 4; ++i)
		{
			int posI = (int)sourceCell.m_i + direction[i, 0];
			int posJ = (int)sourceCell.m_j + direction[i, 1];

			if ( (0 <= posI) && (0 <= posJ) && (m_ncx > posI) && (m_ncy > posJ))
			{
				if (CellState.eBusy == m_aCells[posI, posJ])
					continue;

				Cell cell = new Cell();
				cell.m_i = (uint)posI;
				cell.m_j = (uint)posJ;
				successors.Add(cell);
			}
		}

		return successors;
	}

	// Manhattan distance to end cell
	int heuristicRating(Cell curCell, Cell endCell)
	{
		int di = (int)(curCell.m_i - endCell.m_i);
		int dj = (int)(curCell.m_j - endCell.m_j);
		int rating = System.Math.Abs(di) + System.Math.Abs(dj);
		return rating;
	}

	public bool buildAirPath(Cell startCell, Cell endCell, out Cell[] path)
	{
		path = new Cell[2];
		path[0] = startCell;
		path[1] = endCell;
		return true;
	}

	public bool buildPath(Cell startCell, Cell endCell, out Cell[] path)
	{
		List<PathNode> closedCells = new List<PathNode>();
		SortedDictionary<int, List<PathNode>> openCells = new SortedDictionary<int, List<PathNode>>(); // int - priority
		
		PathNode startPath = new PathNode();
		startPath.m_cell = startCell;
		startPath.m_parentNode = null;
		startPath.m_stepsFromStart = 0;
		startPath.m_heuristicRating = heuristicRating(startCell, endCell);

		List<PathNode> startList = new List<PathNode>();
		startList.Add(startPath);
		openCells.Add(startPath.m_priority, startList);

		bool pathFound = false;
		PathNode endPathNode = null;
		
		while (0 < openCells.Count)
		{
			int key = openCells.Keys.First();            
			List<PathNode> parentListNodes = openCells[key];
			PathNode parentNode = parentListNodes.First();

			closedCells.Add(parentNode);
			parentListNodes.RemoveAt(0);
			if (0 == parentListNodes.Count)
			{
				openCells.Remove(key);
			}
			
			if (endCell.Equals(parentNode.m_cell))
			{
				pathFound = true;
				endPathNode = parentNode;
				break;
			}

			List<Cell> successors = getSuccessors(parentNode.m_cell);
			foreach (Cell nextCell in successors)
			{
				PathNode nextPath = new PathNode();
				nextPath.m_cell = nextCell;
				nextPath.m_parentNode = parentNode;
				nextPath.m_stepsFromStart = parentNode.m_stepsFromStart + 1;

				bool cellProcessed = false;
				foreach (KeyValuePair<int, List<PathNode>> iterator in openCells)
				{
					bool stop = false;
					List<PathNode> listNodes = iterator.Value;
					foreach (PathNode openedNode in listNodes)
					{
						if (openedNode.m_cell.Equals(nextPath.m_cell))
						{
							if (nextPath.m_stepsFromStart < openedNode.m_stepsFromStart)
							{
								openCells.Remove(iterator.Key);
							}
							else
							{
								cellProcessed = true;
							}
							stop = true;
							break;
						}   
					}
					if (stop)
						break;
				}

				if (cellProcessed)
					continue;

				foreach (PathNode closedNode in closedCells)
				{
					if (closedNode.m_cell.Equals(nextPath.m_cell))
					{
						cellProcessed = true;
						break;
					}
				}

				if (cellProcessed)
					continue;

				nextPath.m_heuristicRating = heuristicRating(nextCell, endCell);

				if (openCells.ContainsKey(nextPath.m_priority))
				{
					openCells[nextPath.m_priority].Add(nextPath);
				}
				else 
				{
					List<PathNode> listNode = new List<PathNode>();
					listNode.Add(nextPath);
					openCells.Add(nextPath.m_priority, listNode);
				}
			}
		}

		if (pathFound)
		{
			path = new Cell[endPathNode.m_stepsFromStart+1];
			PathNode curNode = endPathNode;
			for (int i = path.Length - 1; i >= 0; --i)
			{
				path[i] = curNode.m_cell;
				curNode = curNode.m_parentNode;
			}
			return true;
		}
		else
		{
			path = null;
			return false;
		}
	}

	void reallocate()
	{
		m_aCells = new CellState[m_ncx, m_ncy];
		for (uint i=0; i<m_ncx; i++)
			for (uint j=0; j<m_ncy; j++)
				m_aCells[i, j] = CellState.eFree;
	}

	void recalcGrid()
	{
		m_gridX = m_length/(float)m_ncx;
		m_gridY = m_width/(float)m_ncy;
	}

	CellState[,] m_aCells;

	uint m_ncx;
	uint m_ncy;

	float m_length;
	float m_width;

	public float m_startX;
	public float m_startY;
	
	public float m_gridX;
	public float m_gridY;
}
