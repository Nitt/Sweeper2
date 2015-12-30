using UnityEngine;
using System.Collections;

public class GameobjectArray : MonoBehaviour {
	public GameObject cellPrefab;
	public GameObject[] cells;

	// GetValidTeleporterRandom
	int teleporterIndex;

	// GetValidGoal
	int goalIndex, currentDifficulty;

	public void ResetStartPositions(){
		foreach (GameObject go in cells) {
			go.GetComponent<ResetScript> ().ResetStartPositions();
		}
	}
	public void ResetBreadCrumbs(){
		foreach (GameObject go in cells) {
			go.GetComponent<ResetScript> ().ResetBreadCrumbs ();
		}
	}
	public void Reset(){
		foreach (GameObject go in cells) {
			go.GetComponent<ResetScript> ().Reset ();
		}
	}
	public void Clear(){
		foreach (GameObject go in cells) {
			Destroy (go);
		}
		System.Array.Clear (cells, 0, cells.Length);
	}
	public int GetRandom(int randomSeed){
		Random.seed = randomSeed;
		return Random.Range (0, cells.Length);
	}

	public int GetValidTeleporterRandom(int originTeleporter, int randomSeed){
		for (int i = 0; i < 8; i++) {
			randomSeed++;
			Random.seed = randomSeed;
			teleporterIndex = Random.Range (0, cells.Length);
			if (teleporterIndex != originTeleporter) {
				if (!cells [teleporterIndex].GetComponent<BreadCrumbsObject> ().Touched.GetComponent<Touched> ().IsVisited () &&
						cells [teleporterIndex].GetComponent<TypeObject> ().To.Value == (int)Type.ValueEnum.Empty) {
					return teleporterIndex;
				}
			}
		}
		return -1; // failed
	}

	public int GetValidGoal (int goalMinDifficulty, int goalMaxDifficulty, int goalAimDifficulty){
		goalIndex = -1; // failed state
		foreach (GameObject cell in cells) {
			if (cell.GetComponent<CounterObject> ().difficulty.getValue() > goalMinDifficulty &&
					cell.GetComponent<CounterObject> ().difficulty.getValue() < goalMaxDifficulty) {
				if (goalIndex == -1) {
					goalIndex = cell.GetComponent<ThisPosition> ().position;
				} else {
					currentDifficulty = cell.GetComponent<CounterObject> ().difficulty.getValue();
					if (Mathf.Abs (cells [goalIndex].GetComponent<CounterObject> ().difficulty.getValue() - goalAimDifficulty) > currentDifficulty) {
						goalIndex = cell.GetComponent<ThisPosition> ().position;
					}
				}
			}
		}
		return goalIndex;
	}
}
