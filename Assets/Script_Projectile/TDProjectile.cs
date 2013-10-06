using UnityEngine;
using System.Collections;

public abstract class TDProjectile : MonoBehaviour {

	// Use this for initialization
	void Start () {
		m_previousDir = new Vector3();
	}
	
	// Update is called once per frame
	void Update () {

		if (transform.position.y < TDWorld.getWorld().getHeightAt3d(transform.position))
		{
			Destroy(gameObject);
			return;
		}

		if (m_target == null)
		{
			m_previousDir.Set(m_previousDir.x, 0.0f, m_previousDir.z);
			m_previousDir.Normalize();
			transform.position = transform.position + (m_previousDir*(speed()*Time.deltaTime)) +
								 new Vector3(0f, -TDWorld.getWorld().m_configuration.projectileFallSpeed*Time.deltaTime, 0f);
			
			return;
		}
		moveToTarget();
		m_previousDir = m_target.transform.position - transform.position;
		float dist = m_previousDir.magnitude;
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
	
	Vector3 m_previousDir;
	float m_recDistance;
	public TDDamage m_damage;
	protected TDActor m_target;
}
