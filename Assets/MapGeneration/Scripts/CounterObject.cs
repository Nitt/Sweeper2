using UnityEngine;
using System.Collections;

public class CounterObject : MonoBehaviour {
	public Counter steps, difficulty, teleporters;
	// Use this for initialization
	public void Reset(){
		steps.setValue(0);
		difficulty.setValue(0);
		//teleporters.setValue(0);
	}
	public void ResetTeleporters(){
//		steps.setValue(0);
//		difficulty.setValue(0);
		teleporters.setValue(0);
	}

	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
