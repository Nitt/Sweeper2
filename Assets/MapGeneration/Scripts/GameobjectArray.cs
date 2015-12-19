using UnityEngine;
using System.Collections;

public class GameobjectArray : MonoBehaviour {
	public GameObject Cell;
	public GameObject[] Cells;
	int TeleporterIndex;

	public void Clear(){
		foreach (GameObject go in Cells) {
			Destroy (go);
		}
		System.Array.Clear (Cells, 0, Cells.Length);
	}
	public int GetRandom(int randomSeed){
		Random.seed = randomSeed;
		return Random.Range (0, Cells.Length);
	}

	public int GetValidTeleporterRandom(int originTeleporter, int randomSeed){
		for (int i = 0; i < 8; i++) {
			randomSeed++;
			Random.seed = randomSeed;
			TeleporterIndex = Random.Range (0, Cells.Length);
			if (TeleporterIndex != originTeleporter) {
				if (!Cells [TeleporterIndex].GetComponent<BreadCrumbsObject> ().Touched.GetComponent<Touched> ().IsVisited () && Cells [TeleporterIndex].GetComponent<TypeObject> ().To.Value == (int)Type.ValueEnum.Empty) {
					return TeleporterIndex;
					break;
				}
			}
		}
		return -1;
	}
}
