using UnityEngine;
using System.Collections;

public class Type : MonoBehaviour {
	public enum ValueEnum{Block,Clingy,Empty,Goal,Oneway,Start,Teleporter, Dead};
	public int Value;
	public GameObject Visual;
	public Material[] Mats;
	// Use this for initialization
	void Start () {
		Value = (int)ValueEnum.Empty;
	}

	public void SetValue(int NewValue){
		Value = NewValue;
		switch (NewValue) {
		case ((int)ValueEnum.Empty):
			transform.GetComponent<TextMesh> ().text = "E";
			Visual.GetComponent<Renderer> ().material = Mats [(int)NewValue];
			break;
		case ((int)ValueEnum.Block):
			transform.GetComponent<TextMesh>().text = "B";
			Visual.GetComponent<Renderer> ().material = Mats [(int)NewValue];
			break;
		case ((int)ValueEnum.Clingy):
			transform.GetComponent<TextMesh>().text = "C";
			Visual.GetComponent<Renderer> ().material = Mats [(int)NewValue];
			break;
		case ((int)ValueEnum.Oneway):
			transform.GetComponent<TextMesh>().text = "O";
			Visual.GetComponent<Renderer> ().material = Mats [(int)NewValue];
			break;
		case ((int)ValueEnum.Teleporter):
			transform.GetComponent<TextMesh>().text = "T";
			Visual.GetComponent<Renderer> ().material = Mats [(int)NewValue];
			break;
		case ((int)ValueEnum.Start):
			transform.GetComponent<TextMesh>().text = "S";
			Visual.GetComponent<Renderer> ().material = Mats [(int)NewValue];
			break;
		case ((int)ValueEnum.Goal):
			transform.GetComponent<TextMesh>().text = "G";
			Visual.GetComponent<Renderer> ().material = Mats [(int)NewValue];
			break;
		case ((int)ValueEnum.Dead):
			transform.GetComponent<TextMesh>().text = "D";
			Visual.GetComponent<Renderer> ().material = Mats [(int)NewValue];
			break;
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
