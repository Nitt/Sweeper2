using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StartPositions : MonoBehaviour {
	public List<int> CurrentPositions;
	public List<int> NewPositions;

	public void copyNewToCurrent() {
		CurrentPositions =  new List<int>(NewPositions);
		NewPositions.Clear();
	}
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
