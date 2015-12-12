using UnityEngine;
using System.Collections;

public class Orientation : MonoBehaviour {
	public enum ValueEnum{Right,Down,Left,Up};
	public int Value;
	public GameObject Visual;

	public void SetOrientation(int newValue){
		Value = newValue;
		Visual.transform.rotation = Quaternion.Euler (0, 0, newValue*-90);
	}
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
