using UnityEngine;
using System.Collections;

public class UsedAsStartPosition : MonoBehaviour {
	public GameObject visual;
	private bool used;
	// Use this for initialization
	void Start () {
		used = false;
	}

	public void Reset(){
		used = false;
		visual.SetActive (false);
	}
	
	public void IsUsed(){
		used = true;
		visual.SetActive (true);
	}
	public bool GetUsed(){
		return used;
	}
}
