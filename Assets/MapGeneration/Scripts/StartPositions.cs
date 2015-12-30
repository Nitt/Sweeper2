using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StartPositions : MonoBehaviour {
	[HideInInspector]
	public List<int> CurrentPositions, NewPositions, CurrentDifficulties, NewDifficulties;

	public void copyNewToCurrent() {
		CurrentPositions =  new List<int>(NewPositions);
		CurrentDifficulties =  new List<int>(NewDifficulties);
		NewPositions.Clear();
		NewDifficulties.Clear();
	}
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
