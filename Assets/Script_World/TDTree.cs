using UnityEngine;
using System.Collections;

public class TDTree : MonoBehaviour {

	// Use this for initialization
	void Start () {
		gameObject.renderer.material.color = new Color(0.7f, 0.1f, 0, 0);
		Transform pTransform = gameObject.GetComponent<Transform>();
		foreach (Transform trs in pTransform)
		{
			if (trs.gameObject.name != "Tree base")
				trs.gameObject.renderer.material.color = new Color(0.1f, 0.7f, 0, 0);
          }
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
