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
		GetComponent<TextMesh>().color = new Vector4(1,Mathf.Clamp((Value*0.1f)-1,0,1),0,Mathf.Clamp(Value*0.1f,0,1));
    }
	public void setTeleporterValue(int newValue){
		Value = newValue;
		GetComponent<TextMesh>().text = Value.ToString();
		GetComponent<TextMesh> ().color = Color.white;
	}

    public int getValue(){
		
        return Value;
    }
}
