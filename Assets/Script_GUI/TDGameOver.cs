using UnityEngine;
using System.Collections;

public class TDGameOver : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnGUI () {
		// Make a background box
		GUI.Box(new Rect(10, 10, 800, 600), "GAME OVER");

		if (GUI.Button(new Rect(400, 40, 80, 20), "Play again")) {
			Application.LoadLevel("MainScene");
		}

	}
}
