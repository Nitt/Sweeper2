using UnityEngine;
using System.Collections;

public class ResetScript : MonoBehaviour {

	public void Reset(){
		GetComponent<UsedAsStartPosition> ().Reset ();
		GetComponent<BreadCrumbsObject> ().Reset ();
		GetComponent<CounterObject> ().Reset ();
		GetComponent<CounterObject> ().ResetTeleporters ();
		GetComponent<TypeObject> ().To.SetValue((int)Type.ValueEnum.Empty);
	}

	public void ResetBreadCrumbs(){
		GetComponent<BreadCrumbsObject> ().Reset ();
		GetComponent<CounterObject> ().Reset ();
	}

	public void ResetStartPositions(){
		GetComponent<UsedAsStartPosition> ().Reset ();
//		GetComponent<BreadCrumbsObject> ().Reset ();
//		GetComponent<CounterObject> ().Reset ();
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
