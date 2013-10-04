using UnityEngine;
using System.Collections.Generic;

public abstract class TDTowerStrategy
{
	public abstract bool shouldShootAt(TDEnemy enemy, TDDamage potentialDamage);
	public abstract void shootingAt(TDEnemy enemy, TDDamage potentialDamage);
	public abstract void destroyCallback(TDEnemy enemy);
}

public class TDDefaultTowerStrategy : TDTowerStrategy
{
	public override bool shouldShootAt(TDEnemy enemy, TDDamage potentialDamage)
	{
		if (m_followedEnemies == null)
			m_followedEnemies = new Dictionary<TDEnemy, float>();
		if (!m_followedEnemies.ContainsKey(enemy))
		{
			m_followedEnemies[enemy] = 0;
			enemy.OnEventDestroy += destroyCallback;
			return true;
		}
		float followedDamage = m_followedEnemies[enemy];
		if (followedDamage > enemy.getStartHP())
			return false;
		return true;
	}

	public override void shootingAt(TDEnemy enemy, TDDamage potentialDamage)
	{
		m_followedEnemies[enemy] += potentialDamage.estimatedFirstDamage();
	}

	public override void destroyCallback(TDEnemy enemy)
	{
		enemy.OnEventDestroy -= destroyCallback;
		if (m_followedEnemies.ContainsKey(enemy))
		{
			m_followedEnemies.Remove(enemy);
		}
	}
	
	Dictionary<TDEnemy, float> m_followedEnemies;
}


public class TDStupidTowerStrategy : TDTowerStrategy
{
	public override bool shouldShootAt(TDEnemy enemy, TDDamage potentialDamage)
	{
		return true;
	}

	public override void shootingAt(TDEnemy enemy, TDDamage potentialDamage)
	{
	}

	public override void destroyCallback(TDEnemy enemy)
	{
	}
}


public class TDFairTowerStrategy : TDTowerStrategy
{
	public TDFairTowerStrategy() {m_counter = 0; m_counterLimit = 10;}

	public override bool shouldShootAt(TDEnemy enemy, TDDamage potentialDamage)
	{
		if (m_followedEnemies == null)
			m_followedEnemies = new List<TDEnemy>();
		m_counter++;
		if (m_counter > m_counterLimit)
		{
			m_counter = 0;
			m_followedEnemies.Clear();
		}
		if (!m_followedEnemies.Contains(enemy)) // New enemies always have preference
			return true;
		return false;
	}

	public override void shootingAt(TDEnemy enemy, TDDamage potentialDamage)
	{
		if (!m_followedEnemies.Contains(enemy))
			m_followedEnemies.Add(enemy);
	}

	public override void destroyCallback(TDEnemy enemy)
	{
		enemy.OnEventDestroy -= destroyCallback;
		if (m_followedEnemies.Contains(enemy))
		{
			m_followedEnemies.Remove(enemy);
		}
	}
	
	int m_counter;
	int m_counterLimit;
	List<TDEnemy> m_followedEnemies;
}
