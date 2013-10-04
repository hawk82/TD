using UnityEngine;
using System.Collections;

public abstract class TDMovable : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public Vector3 getGridPosition()
	{
		return TDWorld.getWorld().from3dTo2d(transform.position);
	}

	public Vector3 getWorldPosition()
	{
		return TDWorld.getWorld().from2dTo3d(getGridPosition()) + getElevation();
	}

	public void fixPosition()
	{
		transform.position = getWorldPosition();
	}

 	public abstract Vector3 getElevation();
}
