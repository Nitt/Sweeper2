using UnityEngine;
using System.Collections;

public class Counter : MonoBehaviour {
    private int Value = 0;

	// Use this for initialization
	void Start () {
	
	}
	
	public void setValue(int newValue){
        Value = newValue;
        GetComponent<TextMesh>().text = Value.ToString();
    }

    public int getValue(){
		GetComponent<TextMesh>().color = new Vector4(1,Mathf.Clamp((Value*0.1f)-1,0,1),0,Mathf.Clamp(Value*0.1f,0,1));
        return Value;
    }
}
