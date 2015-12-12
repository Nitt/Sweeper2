using UnityEngine;
using System.Collections;

public class Touched : MonoBehaviour {
    private bool Visited = false;
	// Use this for initialization
	void Start () {
	
	}
	
	public void Visit(){
        Visited = true;
		GetComponent<TextMesh>().color = new Vector4(1,.5f,0,.4f);
    }
    public bool IsVisited(){
        return Visited;
    }
	public void UnVisit(){
		Visited = false;
		GetComponent<TextMesh>().color = new Vector4(0,0,0,0);
	}
}
