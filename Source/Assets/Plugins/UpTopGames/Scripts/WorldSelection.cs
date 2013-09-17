using UnityEngine;
using System.Collections;

public class WorldSelection : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void StartGame()
	{
		Flow.currentGame.worldID = 1;
		Application.LoadLevel("Game");
	}
}
