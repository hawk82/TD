using UnityEngine;
using System.Collections.Generic;

public class TDTowerStrategy
{
	public bool shouldShootAt(TDEnemy enemy, TDDamage potentialDamage)
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

	public void shootingAt(TDEnemy enemy, TDDamage potentialDamage)
	{
		m_followedEnemies[enemy] += potentialDamage.estimatedFirstDamage();
	}

	void destroyCallback(TDEnemy enemy)
	{
		enemy.OnEventDestroy -= destroyCallback;
		if (m_followedEnemies.ContainsKey(enemy))
		{
			m_followedEnemies.Remove(enemy);
		}
	}
	
	Dictionary<TDEnemy, float> m_followedEnemies;
}
