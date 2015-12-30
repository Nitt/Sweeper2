using UnityEngine;
using System.Collections;

public class BreadCrumbsObject : MonoBehaviour {
	public GameObject Touched;
	public GameObject[] Orientation;

	public void Reset(){
		for (int i = 0; i < 4; i++) {
			Orientation[i].GetComponent<Touched>().UnVisit();
			Touched.GetComponent<Touched> ().UnVisit();
		}
	}

	public void visitTouched(){
		Touched.GetComponent<Touched>().Visit();
	}

	public void visitOrientation(int orientation){
		Orientation[orientation].GetComponent<Touched>().Visit();
	}

	public void unVisitOrientation(int orientation){
		Orientation[orientation].GetComponent<Touched>().UnVisit();
	}

	public bool isVisitedOrientation(int orientation){
		return Orientation[orientation].GetComponent<Touched>().IsVisited();
	}

	public bool isVisitedTouched(){
		return Touched.GetComponent<Touched>().IsVisited();
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
