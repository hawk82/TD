using UnityEngine;
using System.Collections;

public class TDArrow : TDProjectile {

	public override float speed()
	{
		return TDWorld.getWorld().m_configuration.towerArcherProjectileSpeed;
	}
	public override void moveToTarget()
	{
		Vector3 dir = m_target.transform.position - transform.position;
		dir.Normalize();
		float angleY = -90f + Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;
		if (angleY < 0)
			angleY += 360f;
		if (angleY > 360f)
			angleY -= 360f;
		dir *= speed()*Time.deltaTime;
		transform.position = transform.position + dir; // Don't change to Translate else it will produce wrong transformation in combination with rotation
		transform.rotation = Quaternion.Euler(0, angleY, 0);
	}
	public override void onTargetReached()
	{
		if (m_target != null)
		{
			m_target.receiveDamage(m_damage, null);
		}
	}
}
