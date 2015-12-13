using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelGenerator_tempInput : MonoBehaviour {
    public GameObject map;
	public bool CanStartCoroutine;
	private GameObject currentCell, nextCell, nextNextCell, startCell;
	private int steps;
	private int difficulty;
	private int temporaryTeleporterPosition;
	private bool goingIntoTeleporter, CanTestStart;
	private int inc;

	// Use this for initialization
	void Start () {
		CanStartCoroutine = true;
		steps = 0;
		difficulty = 0;
		inc = 0;
		goingIntoTeleporter = false;
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKey ("space")) {
			if (CanStartCoroutine) {
				StartCoroutine ("GenerateLevel");
				CanStartCoroutine = false;
			}
		}
	}

	IEnumerator GenerateLevel(){
		// variable declaration:
		GameobjectArray mapCellArray =  map.GetComponent<GameobjectArray>();
		StartPositions startPositions = map.GetComponent<StartPositions> ();
		int StartPos = mapCellArray.GetRandom ();

		// clear counters:
		steps = 0;
		difficulty = 0;

		// clear and populate map array:
		mapCellArray.Clear();
//		map.GetComponent<PopulateMap>().populate(Random.Range(5,13), Random.Range(5,13));
//		map.GetComponent<PopulateMap>().populate(12, 20, true);
		map.GetComponent<PopulateMap>().populate(9, 16, true);
//		map.GetComponent<PopulateMap>().populate(2, 2, true);
		yield return null;

		// place a random startposition:
		startPositions.NewPositions.Add(StartPos);
		startCell = mapCellArray.Cells [StartPos];
		startCell.GetComponent<TypeObject>().To.SetValue((int)Type.ValueEnum.Start);
		startCell.GetComponent<UsedAsStartPosition>().used = true;
		yield return null;


		while (startPositions.NewPositions.Count > 0) {
			steps++;
			startPositions.copyNewToCurrent();

			foreach (int current in startPositions.CurrentPositions) { // <- iterate through all "StartPositions"
				// variable declarations:
				GameObject currentCell = mapCellArray.Cells[current];
				BreadCrumbsObject currentBreadCrumbObject = currentCell.GetComponent<BreadCrumbsObject> ();
				BreadCrumbsObject nextBreadCrumbObject;

				startCell = currentCell;

				currentBreadCrumbObject.visitTouched();

				for (int orientation = 0; orientation < 4; orientation++) { // 0 = right, 1 = down, 2 = left, 3 = up
					// variable decalarations:
					Type currentType = currentCell.GetComponent<TypeObject>().To;
					Type nextType;

					currentBreadCrumbObject.visitOrientation (orientation);

					if (currentType.Value == (int)Type.ValueEnum.Teleporter) {
						nextCell = mapCellArray.Cells[currentCell.GetComponent<Teleporter> ().Teleposition];
					} else {
						nextCell = currentCell.GetComponent<NextPositionObject> ().Orientation [orientation].GetComponent<NextPosition> ().nextObject;
					}
					nextNextCell = nextCell.GetComponent<NextPositionObject> ().Orientation[orientation].GetComponent<NextPosition> ().nextObject;
					nextType = nextCell.GetComponent<TypeObject>().To;

					CanTestStart = false;
					// iterating steps

					while (currentCell != nextCell) {
						
						currentType = currentCell.GetComponent<TypeObject>().To;
						nextType = nextCell.GetComponent<TypeObject>().To;
						currentBreadCrumbObject = currentCell.GetComponent<BreadCrumbsObject> ();
						nextBreadCrumbObject = nextCell.GetComponent<BreadCrumbsObject> ();

						if (currentCell == startCell && CanTestStart) {
							break;
						}
						CanTestStart = true;

						currentBreadCrumbObject.visitOrientation(orientation);
						inc++;
						goingIntoTeleporter = false;

						if (currentType.Value == (int)Type.ValueEnum.Teleporter) {
							goingIntoTeleporter = true;
							currentBreadCrumbObject.unVisitOrientation(orientation);
						}
						if (nextBreadCrumbObject.isVisitedOrientation(orientation)) {
							break;
						}
						// colliding with types
						if (nextType.Value == (int)Type.ValueEnum.Clingy) {
							if (!currentCell.GetComponent<UsedAsStartPosition> ().used) {
								startPositions.NewPositions.Add (nextCell.GetComponent<ThisPosition> ().position);
								nextCell.GetComponent<UsedAsStartPosition> ().used = true;
							}
							break;
						} else if (nextType.Value == (int)Type.ValueEnum.Block) {
							if (!currentCell.GetComponent<UsedAsStartPosition> ().used) {
								startPositions.NewPositions.Add (currentCell.GetComponent<ThisPosition> ().position);
								currentCell.GetComponent<UsedAsStartPosition> ().used = true;
							}
							break;
						} else if (nextType.Value == (int)Type.ValueEnum.Oneway) {
							if (orientation != nextCell.GetComponent<Orientation> ().Value) {
								if (!currentCell.GetComponent<UsedAsStartPosition> ().used) {
									startPositions.NewPositions.Add (currentCell.GetComponent<ThisPosition> ().position);
									currentCell.GetComponent<UsedAsStartPosition> ().used = true;
								}
								break;
							}
						}
						// add types
						if (!nextBreadCrumbObject.isVisitedTouched()) {
							if (nextType.Value == (int)Type.ValueEnum.Empty){
								// add block?
								if (Random.value>0.85f){
									nextType.SetValue((int)Type.ValueEnum.Block);
									startPositions.NewPositions.Add (currentCell.GetComponent<ThisPosition> ().position);
									currentCell.GetComponent<UsedAsStartPosition> ().used = true;
									break;
								}
								// add oneway?
								if (Random.value>0.85f){
									if (nextNextCell != nextCell) { // are we at edge?
										if (nextNextCell.GetComponent<TypeObject> ().To.Value == (int)Type.ValueEnum.Empty) { // need to have one free space infront
											if (!currentCell.GetComponent<UsedAsStartPosition> ().used) {
												//nextNextCell.GetComponent<BreadCrumbsObject> ().Touched.GetComponent<Touched> ().Visit();
												nextType.SetValue ((int)Type.ValueEnum.Oneway);
												nextCell.GetComponent<Orientation> ().SetOrientation (orientation);
											}
										}
									}
								}
								// add clingy?
								if (Random.value > 0.98f) {
									nextType.SetValue((int)Type.ValueEnum.Clingy);
									startPositions.NewPositions.Add (nextCell.GetComponent<ThisPosition> ().position);
									nextCell.GetComponent<UsedAsStartPosition> ().used = true;
	//								nextCell.GetComponent<CounterObject> ().steps.setValue (steps);
									break;
								}
								// add teleporter?
								if (Random.value > 0.99f) {
									if (!goingIntoTeleporter) {
										temporaryTeleporterPosition = mapCellArray.GetValidTeleporterRandom (nextCell.GetComponent<ThisPosition>().position);
										if (temporaryTeleporterPosition != -1) {
											nextType.SetValue ((int)Type.ValueEnum.Teleporter);
											mapCellArray.Cells [temporaryTeleporterPosition].GetComponent<TypeObject> ().To.SetValue ((int)Type.ValueEnum.Teleporter);
											nextCell.GetComponent<Teleporter> ().Teleposition = temporaryTeleporterPosition;
											mapCellArray.Cells [temporaryTeleporterPosition].GetComponent<Teleporter> ().Teleposition = nextCell.GetComponent<ThisPosition> ().position;
											mapCellArray.Cells [temporaryTeleporterPosition].GetComponent<BreadCrumbsObject> ().visitTouched();
										}
									}
								}
							}
						}

						nextBreadCrumbObject.visitTouched ();
						if (nextType.Value != (int)Type.ValueEnum.Teleporter) {
							currentBreadCrumbObject.visitOrientation(orientation);
						}

						if (steps < nextCell.GetComponent<CounterObject> ().steps.getValue () || nextCell.GetComponent<CounterObject> ().steps.getValue () == 0) {
							nextCell.GetComponent<CounterObject> ().steps.setValue (steps);
						}

						currentCell = nextCell;
//						if (currentCell == startCell){
//							break;
//						}
						if (currentType.Value == (int)Type.ValueEnum.Teleporter && !goingIntoTeleporter) {
							nextCell = mapCellArray.Cells [currentCell.GetComponent<Teleporter> ().Teleposition];
						} else {
							nextCell = nextCell.GetComponent<NextPositionObject> ().Orientation [orientation].GetComponent<NextPosition> ().nextObject;
						}
//						nextCell = nextNextCell;
						nextNextCell = nextCell.GetComponent<NextPositionObject> ().Orientation[orientation].GetComponent<NextPosition> ().nextObject;
						//yield return new WaitForSeconds (0.08f);
					}
					if (currentCell == nextCell) {
						if (!currentCell.GetComponent<UsedAsStartPosition> ().used) {
							startPositions.NewPositions.Add (nextCell.GetComponent<ThisPosition> ().position);
							currentCell.GetComponent<UsedAsStartPosition> ().used = true;
						}
					}
				}
			}
			//yield return new WaitForSeconds (0.03f);
		}
		yield return null;

		CanStartCoroutine = true; // prep for coroutine to start again
	}
}

