using UnityEngine;
using System.Collections;

public class TDImp : TDEnemy {

	public override uint getStartHP()
	{
		return TDWorld.getWorld().m_configuration.enemyImpHP;
	}
	public override float getStartSpeed()
	{
		return TDWorld.getWorld().m_configuration.enemyImpSpeed;
	}
	public override bool canFly()
	{
		return false;
	}
	protected override float flyHeight()
	{
		return 5.0f;
	}
	public override float getResistance(TDDamage.Type type)
	{
		return 0f;
	}
	protected override uint killReward()
	{
		return TDWorld.getWorld().m_configuration.enemyImpKillReward;
	}
	protected override float heroHostileChance()
	{
		return TDWorld.getWorld().m_configuration.enemyImpHeroHostileChance;
	}
	protected override float heroHostileRadius()
	{
		return TDWorld.getWorld().m_configuration.enemyImpHeroHostileRadius;
	}
	protected override float fightRadius()
	{
		return TDWorld.getWorld().m_configuration.enemyImpFightRadius;
	}
	protected override float physicalDamage()
	{
		return TDWorld.getWorld().m_configuration.enemyImpPhysicalDamagePerSec;
	}
}
