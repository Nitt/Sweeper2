using UnityEngine;
using System.Collections;

public class Movement : MonoBehaviour {
	public enum stateEnum{Right, Down, Left, Up, Still, Win};
	public float movementSpeed;
	public GameObject map;
	public int currentPositionIndex, state;
	private GameobjectArray mapCellArray;
	private int nextPositionIndex;
	private NextPosition nextPosition;
	private bool canStartCoroutine;
	// Use this for initialization
	void Start () {
		state = (int)stateEnum.Still;
		mapCellArray = map.GetComponent<GameobjectArray> ();
		canStartCoroutine = true;
	}
	
	// Update is called once per frame
	void Update () {
		if (state == (int)stateEnum.Still && canStartCoroutine) {
			if (Input.GetAxis ("Horizontal") > 0) {
				state = (int)stateEnum.Right;
			} else if (Input.GetAxis ("Horizontal") < 0) {
				state = (int)stateEnum.Left;
			} else if (Input.GetAxis ("Vertical") > 0) {
				state = (int)stateEnum.Up;
			} else if (Input.GetAxis ("Vertical") < 0) {
				state = (int)stateEnum.Down;
			}
		} else {
			nextStep (state);
		}
		if (Input.GetKey ("r")) {
			transform.position = mapCellArray.cells [map.GetComponent<StartIndex> ().startIndex].transform.position;
			currentPositionIndex = map.GetComponent<StartIndex> ().startIndex;
			state = (int)stateEnum.Still;
		}
	}

	public void nextStep(int state){
		if (canStartCoroutine) {
			canStartCoroutine = false;
			StartCoroutine (movingToIndex (state));
		}
	}

	public IEnumerator movingToIndex (int orientation){
		int localStartIndex = currentPositionIndex;
		Vector3 startPosition;
		Vector3 targetPosition;
		float distance;
		bool shouldContinue = true, canTestStartIndex = false;
		Type to;
		Orientation nextOrientation;
		while (shouldContinue) {
			nextPosition = mapCellArray.cells [currentPositionIndex].GetComponent<NextPositionObject> ()
				.Orientation [orientation].GetComponent<NextPosition> ();
			nextPositionIndex = nextPosition.nextObject.GetComponent<ThisPosition> ().position;
			to = mapCellArray.cells [nextPositionIndex].GetComponent<TypeObject> ().To;
			nextOrientation = mapCellArray.cells [nextPositionIndex].GetComponent<Orientation> ();

//			Debug.Log ("to.value:" + to.Value);
			if (localStartIndex == currentPositionIndex && canTestStartIndex) {
				shouldContinue = false;
				state = (int)stateEnum.Still;
				break;
			}
			canTestStartIndex = true;
			if (nextPositionIndex == currentPositionIndex) {
				shouldContinue = false;
				state = (int)stateEnum.Still;
				break;
			} else if (to.Value != (int)Type.ValueEnum.Empty && to.Value != (int)Type.ValueEnum.Start) {
				if (to.Value == (int)Type.ValueEnum.Oneway) {
					if (nextOrientation.Value != orientation) {
						shouldContinue = false;
						state = (int)stateEnum.Still;
						break;
					}
				} else if (to.Value == (int)Type.ValueEnum.Clingy) {
					shouldContinue = false;
				} else if (to.Value == (int)Type.ValueEnum.Teleporter) {
					startPosition = transform.position;
					targetPosition = mapCellArray.cells [nextPositionIndex].transform.position;
					distance = Vector3.Distance (startPosition, targetPosition);
					for (float i = 0.0f; i < 1.0f; i += movementSpeed / distance) {
						transform.position = Vector3.Lerp (startPosition, targetPosition, i);
						yield return new WaitForSeconds (1.0f / 120.0f);
					}
					transform.position = targetPosition;
					yield return new WaitForSeconds (0.1f);
					nextPositionIndex = mapCellArray.cells [nextPositionIndex].GetComponent<Teleporter> ().Teleposition;
					transform.position = mapCellArray.cells [nextPositionIndex].transform.position;
					yield return new WaitForSeconds (0.1f);
				} else if (to.Value == (int)Type.ValueEnum.Goal) {
					startPosition = transform.position;
					targetPosition = mapCellArray.cells [nextPositionIndex].transform.position;
					distance = Vector3.Distance (startPosition, targetPosition);
					for (float i = 0.0f; i < 1.0f; i += movementSpeed / distance) {
						transform.position = Vector3.Lerp (startPosition, targetPosition, i);
						yield return new WaitForSeconds (1.0f / 120.0f);
					}
					transform.position = targetPosition;
					shouldContinue = false;
					state = (int)stateEnum.Win;
					yield return new WaitForSeconds (0.4f);

					break;
				} else if (to.Value == (int)Type.ValueEnum.Dead) {
					startPosition = transform.position;
					targetPosition = mapCellArray.cells [nextPositionIndex].transform.position;
					distance = Vector3.Distance (startPosition, targetPosition);
					for (float i = 0.0f; i < 1.0f; i += movementSpeed / distance) {
						transform.position = Vector3.Lerp (startPosition, targetPosition, i);
						yield return new WaitForSeconds (1.0f / 120.0f);
					}
					transform.position = targetPosition;
					yield return new WaitForSeconds (0.4f);
					transform.position = mapCellArray.cells [map.GetComponent<StartIndex> ().startIndex].transform.position;
					currentPositionIndex = map.GetComponent<StartIndex> ().startIndex;
					shouldContinue = false;
					state = (int)stateEnum.Still;
					yield return new WaitForSeconds (0.4f);
					break;
				} else {
					shouldContinue = false;
					state = (int)stateEnum.Still;
					break;
				}
			}

			startPosition = transform.position;
			targetPosition = mapCellArray.cells [nextPositionIndex].transform.position;
			distance = Vector3.Distance (startPosition, targetPosition);
			for (float i = 0.0f; i < 1.0f; i += movementSpeed / distance) {
				transform.position = Vector3.Lerp (startPosition, targetPosition, i);
//				yield return new WaitForSeconds (1.0f / 120.0f);
				yield return null;
			}
			transform.position = targetPosition;
			if (!shouldContinue) {
				yield return new WaitForSeconds (0.18f);
			}
			state = (int)stateEnum.Still;
			currentPositionIndex = nextPositionIndex;

			
		}
		canStartCoroutine = true;
	}
}
