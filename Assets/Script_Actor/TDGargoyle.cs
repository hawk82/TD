using UnityEngine;
using System.Collections;

public class TDGargoyle : TDEnemy {

	public override uint getStartHP()
	{
		return TDWorld.getWorld().m_configuration.enemyGargoyleHP;
	}
	public override float getStartSpeed()
	{
		return TDWorld.getWorld().m_configuration.enemyGargoyleSpeed;
	}
	public override bool canFly()
	{
		return true;
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
		return TDWorld.getWorld().m_configuration.enemyGargoyleKillReward;
	}
	protected override float heroHostileChance()
	{
		return TDWorld.getWorld().m_configuration.enemyGargoyleHeroHostileChance;
	}
	protected override float heroHostileRadius()
	{
		return TDWorld.getWorld().m_configuration.enemyGargoyleHeroHostileRadius;
	}
	protected override float fightRadius()
	{
		return TDWorld.getWorld().m_configuration.enemyGargoyleFightRadius;
	}
	protected override float physicalDamage()
	{
		return TDWorld.getWorld().m_configuration.enemyGargoylePhysicalDamagePerSec;
	}
}
