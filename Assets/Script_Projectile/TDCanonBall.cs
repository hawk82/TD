using UnityEngine;
using System.Collections;

public class TDCanonBall : TDProjectile {

	public override void setTarget(TDActor target)
	{
		GameObject newTarget = (GameObject) Instantiate(m_prefabFakeTarget, target.gameObject.transform.position, new Quaternion());
		TDActor actor = newTarget.GetComponent<TDActor>();
		base.setTarget(actor);
		actor.transform.position = target.gameObject.transform.position;
		Vector3 startDir = m_target.transform.position - transform.position;
		startDir.y = 0;
		m_startDistance = startDir.magnitude;
		m_pathGone = 0f;
		m_startHeight = transform.position.y;
		m_endHeight = m_target.transform.position.y;
	}

	public override float speed()
	{
		return TDWorld.getWorld().m_configuration.towerCanonProjectileSpeed;
	}
	public override void moveToTarget()
	{
		Vector3 dir = m_target.transform.position - transform.position;
		dir.y = 0;
		float pathGone = 1.0f - dir.magnitude/m_startDistance;
		if (m_pathGone > pathGone)
		{
			onTargetReached();
			Destroy(gameObject);
			return;
		}
		m_pathGone = pathGone;
		dir.Normalize();
		dir *= speed()*Time.deltaTime;
		transform.Translate(dir);
		float y = TDWorld.getWorld().m_configuration.towerCanonBallParabolaHeight*Mathf.Sin(Mathf.PI*pathGone);
		y += Mathf.Lerp(m_startHeight, m_endHeight, m_pathGone);
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
	
	float m_startHeight;
	float m_endHeight;
	float m_pathGone;
	float m_startDistance;
	public GameObject m_prefabFakeTarget;
}
