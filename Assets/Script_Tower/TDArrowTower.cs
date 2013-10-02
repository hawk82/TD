using UnityEngine;
using System.Collections;

public class TDArrowTower : TDTower
{
	public override float getRestoration()
	{
		return TDWorld.getWorld().m_configuration.towerArcherRestoration;
	}
	public override float getEfficientRadius()
	{
		return TDWorld.getWorld().m_configuration.towerArcherRadius;
	}
	public override TDProjectile createProjectile()
	{
		GameObject projectile = (GameObject) Instantiate(m_prefabArrow, gameObject.transform.position, Quaternion.identity);
		return (TDProjectile) projectile.GetComponent<TDProjectile>();
	}
	public override TDDamage getTowerDamage()
	{
		return new TDDamage(TDDamage.Type.ePhysical, TDWorld.getWorld().m_configuration.towerArcherPhysicalDamage, 0);
	}
	public override bool shootsFlying()
	{
		return true;
	}
	public override uint price()
	{
		return TDWorld.getWorld().m_configuration.towerArcherPrice;
	}
	public GameObject m_prefabArrow;
}
