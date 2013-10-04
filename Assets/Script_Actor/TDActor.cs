using UnityEngine;
using System.Collections.Generic;

public abstract class TDActor : TDMovable {

	// Use this for initialization
	protected virtual void Start () {
		m_HP = (int) getStartHP();
		cleanPath();
		m_aModifier = new List<TDModifier>();
	}
	
	// Update is called once per frame
	protected virtual void Update () {
		if (!isAlive())
			return;
		m_momentalSpeedFactor = 1.0f;
		List<TDModifier> itemsToRemove = new List<TDModifier>();
		foreach (TDModifier m in m_aModifier)
		{
			if (m != null)
			{
				m.apply();
				if (m_HP <= 0)
				{
					die();
					break;
				}
				if (m_HP > getStartHP())
				{
					m_HP = getStartHP();
				}
				if (m.finished())
					itemsToRemove.Add(m);
			}
			else
				itemsToRemove.Add(m);
		}

		foreach (TDModifier m in itemsToRemove)
			m_aModifier.Remove(m);

		if (m_path != null)
			walkByPath();
	}

	public virtual bool isAlive()
	{
		return true;
	}

	public abstract bool canFly();
 	public override Vector3 getElevation()
	{
		return new Vector3(0f, 0f, 0f);
	}
	
	// Caches the path in case of success
	public bool hasPathTo(GameObject target)
	{
		return buildPath(target);
	}
	
    // Returns false if target is not reachable
	public bool walkByPath()
	{
		if (null == m_path)
			return false;

		if ((m_currentCellIndex < 0) ||	(m_currentCellIndex >= m_path.Length))
			return false;

		if (target() == null) // nowhere to go
		{
			cleanPath();
			onTargetDestroyed();
			return true;
		}

		TDWorld world = TDWorld.getWorld();
		int cellTo = (m_currentCellIndex == m_path.Length - 1) ? m_currentCellIndex : (m_currentCellIndex + 1);

		Vector3 nextCellPos = world.m_grid.getCenter(m_path[cellTo]);
		Vector3 dir = nextCellPos - getGridPosition();
		
		if (!canFly())
		{
			if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y))
			{
				dir.y = 0;
			}
			else
			{
				dir.x = 0;
			}
		}
		dir.Normalize();

		float effSpeed = m_momentalSpeedFactor*getStartSpeed();
		Vector3 move = effSpeed*Time.deltaTime*dir;
		Vector3 nextPos = getGridPosition() + move;
		if (!canFly())
			if (TDGrid.CellState.eBusy == TDWorld.getWorld().positionState2d(nextPos))
			{
				return buildPath(target()); // To make things easy walk there next step
			}

		Vector3 move3d = world.from2dTo3d(move);

		transform.Translate(move3d);
		fixPosition();

		if ( (world.from3dTo2d(transform.position) - nextCellPos).magnitude < world.m_configuration.hitDistance )
			++m_currentCellIndex;
		
		if (m_currentCellIndex == m_path.Length)
		{
			onTargetReached(target());
			cleanPath();
			return true;
		}

		return true;
	}

	protected void cleanPath()
	{
		m_path = null;
		m_currentCellIndex = -1;
	}

	protected abstract void onTargetReached(GameObject obj);

	protected abstract void onTargetDestroyed();


	protected virtual void die()
	{
		Destroy(gameObject);
	}

	virtual public void receiveDamage(TDDamage damage, TDActor source)
	{
		receiveModifier(damage);
	}

	public void receiveModifier(TDModifier modifier)
	{
		if (modifier != null)
			modifier.setTarget(this);
		if (m_aModifier != null)
			m_aModifier.Add(modifier);
	}

	public abstract uint getStartHP();
	public abstract float getStartSpeed();

	public float health()
	{
		return m_HP;
	}

	public void heal(float val)
	{
		m_HP += val;
	}

	public void receiveDamage(TDDamage.Type type, float val)
	{
		m_HP -= (1.0f - getResistance(type))*val;
	}
	public abstract float getResistance(TDDamage.Type type);
	
    // 0 < factor <= 1
    // Slows speed on Update event
	public void slowSpeed(float factor)
	{
		m_momentalSpeedFactor *= factor;
	}

	bool buildPath(GameObject target)
	{
		m_currentCellIndex = -1;
		setTarget(null);
		TDWorld world = TDWorld.getWorld();
		TDGrid grid = world.m_grid;
		TDGrid.Cell startCell = grid.getCell(world.from3dTo2d(gameObject.transform.position));
		TDGrid.Cell endCell = grid.getCell(world.from3dTo2d(target.transform.position));
		bool pathExists = false;
		if (canFly())
			pathExists = grid.buildAirPath(startCell, endCell, out m_path);
		else
			pathExists = grid.buildPath(startCell, endCell, out m_path);
		m_currentCellIndex = 0;
		setTarget(target);
		return pathExists;
	}

	protected virtual void setTarget(GameObject newTarget)
	{
		m_target = newTarget;
	}

	protected GameObject target()
	{
		return m_target;
	}

	GameObject m_target;

	protected List<TDModifier> m_aModifier;
	protected float m_HP;
	protected float m_momentalSpeedFactor;

	int m_currentCellIndex;
	protected TDGrid.Cell[] m_path;
}
