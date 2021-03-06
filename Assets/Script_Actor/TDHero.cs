﻿using UnityEngine;
using System.Collections;

public class TDHero : TDActor {

	// Use this for initialization
	protected override void Start () {
		base.Start();
		GameObject respawnPoint = TDWorld.getWorld().getHeroRespawnPoint();
		transform.position = respawnPoint.transform.position;
		fixPosition();
		m_state = State.ePatrol;
		receiveModifier(new TDHealthRegeneration(10000.0f, TDWorld.getWorld().m_configuration.heroAutoHealPerSec));
	}
	
	// Update is called once per frame
	protected override void Update () {
		base.Update();
		switch (m_state)
		{
			case State.eWalk:
				walk();
				break;
			case State.eFight:
				fight();
				break;
			case State.ePatrol:
				patrol();
				break;
			case State.eDead:
				deadTime();
				break;
		}
	}

	protected override void setTarget(GameObject newTarget)
	{
		if (null != target())
		{
			TDWorld world = TDWorld.getWorld();
			if (world.isFakeTarget(target()))
			{
				DestroyObject(target());
			}
		}
		base.setTarget(newTarget);
	}

	private void fight()
	{
		if (!isAlive())
			return;
		TDWorld world = TDWorld.getWorld();
		TDEnemy tdEnemy = world.getTDEnemy(target());
		if (null == tdEnemy)
		{
			m_state = State.ePatrol;
			return;
		}
		if ((tdEnemy.getGridPosition() - getGridPosition()).magnitude > world.m_configuration.heroFightRadius)
		{
			if (hasPathTo(target()))
			{
				m_state = State.eWalk;
			}
			else
			{
				m_state = State.ePatrol;
			}
			return;
		}
		TDDamage damage = new TDDamage(TDDamage.Type.ePhysical, world.m_configuration.heroPhysicalDamagePerSec*Time.deltaTime, 0f);
		tdEnemy.receiveDamage(damage, this);
	}

	private void walk()
	{
		if (!isAlive())
			return;
		if (null == target())
		{
			cleanPath();
		}
		if (null == m_path)
		{
			m_state = State.ePatrol;
			return;
		}
		if ((null != target()) && (TDWorld.getWorld().isFakeTarget(target()))) // Special treatment since fake target is destroyed
		{
			patrol(target().transform.position);
			return;
		}
		if (hasPathTo(target()))
		{
			if (1 == m_path.Length)
			{
				cleanPath();
				m_state = State.eFight;
			}
			walkByPath();
			return;
		}
		m_state = State.ePatrol;
	}

	private void patrol()
	{
		if (!isAlive())
			return;
		m_path = null;
		TDWorld world = TDWorld.getWorld();
		GameObject [] aEnemies = world.getAllEnemiesUnsafe();
		foreach (GameObject enemy in aEnemies)
		{
			TDEnemy tdEnemy = TDWorld.getWorld().getTDEnemy(enemy);
			if (tdEnemy == null)
				continue;
			if (tdEnemy.canFly())
				continue;
			if ((tdEnemy.getGridPosition() - getGridPosition()).magnitude < world.m_configuration.heroPatrolRadius)
			{
				if (hasPathTo(enemy))
				{
					m_state = State.eWalk;
					break;
				}
			}
		}
		
	}

	protected override void onTargetReached(GameObject obj)
	{
		if (!isAlive())
			return;

		TDWorld world = TDWorld.getWorld();
		GameObject player = world.getPlayer();
		if (obj == player)
		{
			return;
		}
		
		TDEnemy tdEnemy = world.getTDEnemy(obj);
		if (null != tdEnemy)
		{
			m_state = State.eFight;
			return;
		}

		if (world.isFakeTarget(obj))
		{
			DestroyObject(obj);
		}

		m_state = State.ePatrol;
	}

	protected override void onTargetDestroyed()
	{
		if (!isAlive())
			return;
		m_state = State.ePatrol;
	}

	public void patrol(Vector3 pos)
	{
		if (!isAlive())
			return;
		GameObject fakeTarget = (GameObject) Instantiate(m_prefabFakeTarget, pos, new Quaternion());
		if (hasPathTo(fakeTarget))
		{
			m_state = State.eWalk;
		}
		else
		{
			m_state = State.ePatrol;
			DestroyObject(fakeTarget);
		}
	}

	public void runToBase()
	{
		if (!isAlive())
			return;
		GameObject player = TDWorld.getWorld().getPlayer();
		if (hasPathTo(player))
			m_state = State.eWalk;
		else
			m_state = State.ePatrol;
	}

	public override uint getStartHP()
	{
		return TDWorld.getWorld().m_configuration.heroHP;
	}
	public override float getStartSpeed()
	{
		return TDWorld.getWorld().m_configuration.heroSpeed;
	}
	public override bool canFly()
	{
		return false;
	}
	public override float getResistance(TDDamage.Type type)
	{
		return 0f;
	}

	public override bool isAlive()
	{
		return m_state != State.eDead;
	}

	protected override void die()
	{
		if (m_state == State.eDead)
			throw new System.Exception();
		if (m_cross != null)
			throw new System.Exception();
		m_state = State.eDead;
		renderer.enabled = false;
		m_deathTime = Time.time;
		m_cross = (GameObject) Instantiate(m_prefabCross, getWorldPosition(), new Quaternion());
	}

	protected void deadTime()
	{
		if (Time.time - m_deathTime > TDWorld.getWorld().m_configuration.heroDeathTime)
			resurrect();
	}

	protected void resurrect()
	{
		m_HP = getStartHP();
		GameObject respawnPoint = TDWorld.getWorld().getHeroRespawnPoint();
		transform.position = respawnPoint.transform.position;
		fixPosition();
		m_state = State.ePatrol;
		renderer.enabled = true;
		DestroyObject(m_cross);
	}

	enum State
	{
		ePatrol       = 0,
		eWalk         = 1,
		eFight        = 2,
		eDead         = 3,
	}
	State m_state;
	public GameObject m_prefabFakeTarget;
	public GameObject m_prefabCross;
	float m_deathTime;
	public GameObject m_cross;
}
