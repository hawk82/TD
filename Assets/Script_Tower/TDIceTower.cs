using UnityEngine;
using System.Collections;

public class TDIceTower : TDTower
{
	public override float getRestoration()
	{
		return TDWorld.getWorld().m_configuration.towerIceRestoration;
	}
	public override float getEfficientRadius()
	{
		return TDWorld.getWorld().m_configuration.towerIceRadius;
	}
	public override TDProjectile createProjectile()
	{
		GameObject projectile = (GameObject) Instantiate(m_prefabIcicle, gameObject.transform.position + getTowerShootHeight(), Quaternion.identity);
		return (TDProjectile) projectile.GetComponent<TDProjectile>();
	}
	public override TDDamage getTowerDamage()
	{
		return new TDDamage(TDDamage.Type.ePhysical, TDWorld.getWorld().m_configuration.towerIcePhysicalDamage, 0);
	}
	public override bool shootsFlying()
	{
		return true;
	}
	public override uint price()
	{
		return TDWorld.getWorld().m_configuration.towerIcePrice;
	}
	public override TDTowerStrategy getStrategy()
	{
		return TDWorld.getWorld().m_fairStrategy;
	}
	public GameObject m_prefabIcicle;
}

