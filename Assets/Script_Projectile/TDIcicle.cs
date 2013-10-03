using UnityEngine;
using System.Collections;

public class TDIcicle : TDProjectile {

	public override float speed()
	{
		return TDWorld.getWorld().m_configuration.towerIceProjectileSpeed;
	}
	public override void moveToTarget()
	{
		Vector3 dir = m_target.transform.position - transform.position;
		dir.Normalize();
		dir *= speed()*Time.deltaTime;
		transform.position = transform.position + dir;
	}
	public override void onTargetReached()
	{
		if (m_target != null)
		{
			m_target.receiveDamage(m_damage, null);
			TDIceSlow iceSlow = new TDIceSlow(TDWorld.getWorld().m_configuration.towerIceTime, TDWorld.getWorld().m_configuration.towerIceSpeedFactor);
			m_target.receiveModifier(iceSlow);
		}
	}
}
