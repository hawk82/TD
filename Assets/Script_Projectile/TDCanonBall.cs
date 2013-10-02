using UnityEngine;
using System.Collections;

public class TDCanonBall : TDProjectile {

	public override void setTarget(TDActor target)
	{
		GameObject newTarget = (GameObject) Instantiate(m_prefabFakeTarget, target.gameObject.transform.position, new Quaternion());
		TDActor actor = newTarget.GetComponent<TDActor>();
		base.setTarget(actor);
		m_target.transform.position = new Vector3(m_target.transform.position.x, 0f, m_target.transform.position.z);
		Vector3 dir = m_target.transform.position - transform.position;
		dir.y = 0f;
		m_startDistance = dir.magnitude;
		m_pathGone = 0f;
	}

	public override float speed()
	{
		return TDWorld.getWorld().m_configuration.towerCanonProjectileSpeed;
	}
	public override void moveToTarget()
	{
		Vector3 dir = m_target.transform.position - transform.position;
		dir.y = 0f;
		float pathGone = 1.0f - dir.magnitude/m_startDistance;
		if (m_pathGone > pathGone)
		{
			onTargetReached();
			return;
		}
		m_pathGone = pathGone;
		dir.Normalize();
		dir *= speed()*Time.deltaTime;
		transform.Translate(dir);
		// Parabola y = ax^2 + c
		float a = 0f, b = 0f, c = 0f;
		if (pathGone < 0.5f)
		{
			a = 4.0f*(-2.5f + TDWorld.getWorld().m_configuration.towerCanonBallParabolaHeight);
			b = 1f;
			c = 2.0f;
		}
		else
		{
			pathGone -= 0.5f;
			a = -4.0f*TDWorld.getWorld().m_configuration.towerCanonBallParabolaHeight;
			c = TDWorld.getWorld().m_configuration.towerCanonBallParabolaHeight;		
		}
		float y = a*(pathGone*pathGone) + b*pathGone + c;
		transform.position = new Vector3(transform.position.x, y, transform.position.z);
	}
	public override void onTargetReached()
	{
		Vector3 explodePosition = m_target.transform.position;
		if (m_target != null)
		{
			DestroyObject(m_target);
		}
		GameObject [] aAllEnemies = TDWorld.getWorld().getAllEnemiesUnsafe();
		foreach(GameObject thisObject in aAllEnemies)
		{
			if (thisObject == null)
				continue;
			if ((thisObject.transform.position - explodePosition).magnitude > TDWorld.getWorld().m_configuration.towerCanonBallDamageRadius)
				continue;
			TDEnemy enemy = TDWorld.getWorld().getTDEnemy(thisObject);
			if (enemy == null)
				continue;
			if (enemy.canFly())
				continue;
			enemy.receiveDamage(m_damage.clone(enemy), null);
		}
	}
	
	float m_pathGone;
	float m_startDistance;
	public GameObject m_prefabFakeTarget;
}
