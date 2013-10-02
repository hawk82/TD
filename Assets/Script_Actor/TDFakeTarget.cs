using UnityEngine;
using System.Collections;

public class TDFakeTarget : TDActor {

	// Use this for initialization
	new void Start () {
		
	}
	
	// Update is called once per frame
	new void Update () {
	
	}

	protected override void onTargetReached(GameObject obj)
	{
	}

	protected override void onTargetDestroyed()
	{
	}

	public override uint getStartHP()
	{
		return 1;
	}
	public override float getStartSpeed()
	{
		return 0.0f;
	}
	public override bool canFly()
	{
		return false;
	}
	protected override float flyHeight()
	{
		return 0.0f;
	}
	public override float getResistance(TDDamage.Type type)
	{
		return 0.0f;
	}
}
