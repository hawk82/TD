using UnityEngine;
using System.Collections;

public abstract class TDTower : MonoBehaviour {

	public enum Type
	{
		eArrowTower = 0,
		eCanonTower = 1,
		eIceTower   = 2
	}

	// Use this for initialization
	void Start () {
		m_restTime = Time.time;
	}
	
	// Update is called once per frame
	void Update () {
		float currentTime = Time.time;
		float respawn = getRestoration();
		if (currentTime < (m_restTime + respawn))
			return;
		GameObject [] aAllEnemies = TDWorld.getWorld().getAllEnemiesUnsafe();
		double recDist = -1;
		GameObject recObject = null;
		TDDamage damage = getTowerDamage();
		foreach(GameObject thisObject in aAllEnemies)
		{
			if (thisObject == null)
				continue;
			float dist = (transform.position - thisObject.transform.position).magnitude;
			float efficientRadius = getEfficientRadius();
		   	if (dist < efficientRadius)
			{
				TDEnemy enemy = TDWorld.getWorld().getTDEnemy(thisObject);
				if (enemy.canFly())
					if (!shootsFlying())
						continue;
				if (!getStrategy().shouldShootAt(enemy, damage))
					continue;
				if ((recDist < 0) || (recDist > dist))
				{
					recDist = dist;
					recObject = thisObject;
				}
			}
		}
		if (recObject == null)
			return;
		TDEnemy recEnemy = TDWorld.getWorld().getTDEnemy(recObject);
		getStrategy().shootingAt(recEnemy, damage);
		shootTo(recEnemy);
		m_restTime = Time.time;
	}

	public virtual TDTowerStrategy getStrategy()
	{
		return TDWorld.getWorld().m_defaultStrategy;
	}

	public virtual Vector3 getTowerShootHeight()
	{
		return new Vector3(0f, 2f, 0f);
	}

	public abstract float getRestoration();
	public abstract float getEfficientRadius();
	public abstract TDProjectile createProjectile();
	public abstract bool shootsFlying();
	public abstract uint price();

	public abstract TDDamage getTowerDamage();
	public void shootTo(TDActor actor)
	{
		TDDamage damage = getTowerDamage();
		TDProjectile projectile = createProjectile();
		projectile.m_damage = damage;
		projectile.setTarget(actor);
	}

	float m_restTime;
}
