using UnityEngine;
using System.Collections;

public abstract class TDProjectile : MonoBehaviour {

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		if (m_target == null)
		{
			Destroy (gameObject);
			return;
		}
		moveToTarget();
		Vector3 dir = m_target.transform.position - transform.position;
		float dist = dir.magnitude;
		// Second condition prevents round-off situation when target is almost reached but not damaged
		if ((dist < TDWorld.getWorld().m_configuration.hitDistance) ||
			(dist > (m_recDistance + TDWorld.getWorld().m_configuration.hitDistance)))
		{
			onTargetReached();
			Destroy(gameObject);
		}
		m_recDistance = dist;
	}

	public virtual void setTarget(TDActor target)
	{
		m_target = target;
		m_recDistance = (m_target.transform.position - transform.position).magnitude;
	}

	public abstract float speed();
	public abstract void moveToTarget();
	public abstract void onTargetReached();
	
	float m_recDistance;
	public TDDamage m_damage;
	protected TDActor m_target;
}
