using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelGenerator_tempInput : MonoBehaviour {
	public GameObject playerPrefab, player;
	public int mapSeed, mapBigSeed;
    public GameObject map;
	[HideInInspector]
	public int finalStartPos;
	public bool canStartCoroutine, canTestStart;
	private GameObject currentCell, nextCell, nextNextCell, startCell;
	private int difficulty, steps, teleporterCount, temporaryTeleporterPosition, inc, seedCounter,
		goalIndex, currentGoalDifficulty, closestDifficulty, mapMin, mapMax, mapAim, finalMapSeed, iterativeStrength;
	private GameobjectArray mapCellArray;
	private List<int> deadEnds, startIndexes;
	private bool startedGame;

	// Use this for initialization
	void Start () {
		mapSeed = 0;
		mapBigSeed = 0;
		canStartCoroutine = true;
		steps = 0;
		difficulty = 1;
		teleporterCount = 0;
		seedCounter = 0;
		inc = 0;
		mapCellArray = map.GetComponent<GameobjectArray> ();
		mapCellArray.Clear();
		map.GetComponent<PopulateMap>().populate(16, 9, false);

		iterativeStrength = 30;
		mapMin = 5;
		mapMax = 160;
		mapAim = 32;
		deadEnds = new List<int> ();
		startIndexes = new List<int> ();
		startedGame = false;
//		nextLevel ();
	}
	
	// Update is called once per frame
	void Update () {
		if (startedGame == false || player.GetComponent<Movement>().state == (int)Movement.stateEnum.Win) {
			nextLevel ();
			startedGame = true;
			player.GetComponent<Movement> ().state = (int)Movement.stateEnum.Still;
		}
	}

	public void nextLevel(){
		deadEnds = new List<int> ();
		startIndexes = new List<int> ();
		mapSeed = mapBigSeed*(iterativeStrength+1);
		mapBigSeed++;
		finalMapSeed = -1;
		closestDifficulty = 0;
		for (int i = 0; i < iterativeStrength; i++) {
			GenerateLevel (true, -1);
			goalIndex = mapCellArray.GetValidGoal (mapMin, mapMax, mapAim);
			if (goalIndex != -1) {
				currentGoalDifficulty = mapCellArray.cells [goalIndex].GetComponent<CounterObject> ().difficulty.getValue();
				if (Mathf.Abs(closestDifficulty - mapAim) > Mathf.Abs(currentGoalDifficulty - mapAim)){
					closestDifficulty = currentGoalDifficulty;
					finalMapSeed = mapSeed;
				}
			}
			mapSeed++;
		}
		if (finalMapSeed != -1) {
			mapSeed = finalMapSeed;
			finalStartPos = GenerateLevel (true, -1);
			goalIndex = mapCellArray.GetValidGoal (mapMin, mapMax, mapAim);
			mapCellArray.cells [goalIndex].GetComponent<TypeObject> ().To.SetValue ((int)Type.ValueEnum.Goal);
			mapCellArray.ResetBreadCrumbs ();
			foreach (GameObject cell in mapCellArray.cells) {
				if (cell.GetComponent<UsedAsStartPosition> ().GetUsed ()) {
					startIndexes.Add (cell.GetComponent<ThisPosition> ().position);
				}
			}
			foreach (int cellIndex in startIndexes) {
				//					Debug.Log (cellIndex);
				mapCellArray.ResetStartPositions();
				mapCellArray.ResetBreadCrumbs();
				GenerateLevel (false, cellIndex);
				if (!mapCellArray.cells [goalIndex].GetComponent<BreadCrumbsObject> ().isVisitedTouched ()) {
					// Can't get to Goal from here
					//						Debug.Log("This is a deadEnd:" + cellIndex);
					deadEnds.Add (cellIndex);
				}
			}
			mapCellArray.ResetStartPositions();
			mapCellArray.ResetBreadCrumbs ();
			foreach (int deadEnd in deadEnds) {
				if (mapCellArray.cells [deadEnd].GetComponent<TypeObject> ().To.Value == (int)Type.ValueEnum.Empty) {
					mapCellArray.cells [deadEnd].GetComponent<TypeObject> ().To.SetValue ((int)Type.ValueEnum.Dead);
				}
			}

			// create Player
			if (player == null) {
				player = (GameObject)Instantiate (playerPrefab, mapCellArray.cells [finalStartPos].transform.position, Quaternion.identity); 
				player.GetComponent<Movement> ().currentPositionIndex = finalStartPos;
				player.GetComponent<Movement> ().map = map;
			} else {
				player.transform.position = mapCellArray.cells [finalStartPos].transform.position;
				player.GetComponent<Movement> ().currentPositionIndex = finalStartPos;
			}
		} else {
			Debug.Log ("Failed to create map");
		}
	}

	int GenerateLevel(bool shouldAddTypes, int startHere){
		// variable declaration:
		StartPositions startPositions = map.GetComponent<StartPositions>();
		int startPos;
		seedCounter = mapSeed;

		// clear counters:
		steps = 0;
		difficulty = 1;
		teleporterCount = 1;

		if (shouldAddTypes) {
			mapCellArray.Reset ();
		}


		Random.seed = seedCounter;
		seedCounter++;
		if (startHere == -1) {
			startPos = mapCellArray.GetRandom (seedCounter);
			map.GetComponent<StartIndex> ().startIndex = startPos;
		} else {
			startPos = startHere;
		}

		// place a random startposition:
		startPositions.NewPositions.Add (startPos);
		startPositions.NewDifficulties.Add (difficulty);
		startCell = mapCellArray.cells [startPos];
		if (shouldAddTypes) {
			startCell.GetComponent<TypeObject> ().To.SetValue ((int)Type.ValueEnum.Start);
		}
		startCell.GetComponent<UsedAsStartPosition> ().IsUsed ();

		while (startPositions.NewPositions.Count > 0) {
			steps++;
			startPositions.copyNewToCurrent();


			for (int currentIndex = 0; currentIndex < startPositions.CurrentPositions.Count; currentIndex++) { // <- iterate through all "StartPositions"
				// variable declarations:
				int current = startPositions.CurrentPositions [currentIndex];
				int currentDifficulty = startPositions.CurrentDifficulties [currentIndex];;
				GameObject currentCell = mapCellArray.cells[current];
				BreadCrumbsObject currentBreadCrumbObject = currentCell.GetComponent<BreadCrumbsObject> ();
				BreadCrumbsObject nextBreadCrumbObject;

				startCell = mapCellArray.cells[current];

				currentBreadCrumbObject.visitTouched();

				for (int orientation = 0; orientation < 4; orientation++) { // 0 = right, 1 = down, 2 = left, 3 = up
					// variable decalarations:
					Type currentType;
					Type nextType;

					currentDifficulty = startPositions.CurrentDifficulties [currentIndex];
					currentCell = mapCellArray.cells[current];
					currentType = currentCell.GetComponent<TypeObject>().To;
					currentBreadCrumbObject = currentCell.GetComponent<BreadCrumbsObject> ();
					if (currentType.Value != (int)Type.ValueEnum.Teleporter) {
						currentBreadCrumbObject.visitOrientation (orientation);
					}

					nextCell = currentCell.GetComponent<NextPositionObject> ().Orientation [orientation].GetComponent<NextPosition> ().nextObject;
					nextNextCell = nextCell.GetComponent<NextPositionObject> ().Orientation[orientation].GetComponent<NextPosition> ().nextObject;
					nextType = nextCell.GetComponent<TypeObject>().To;

					canTestStart = false;

					while (currentCell != nextCell || currentCell.GetComponent<TypeObject>().To.Value == (int)Type.ValueEnum.Teleporter) {
						currentType = currentCell.GetComponent<TypeObject>().To;
						currentBreadCrumbObject = currentCell.GetComponent<BreadCrumbsObject> ();
						if (currentType.Value != (int)Type.ValueEnum.Teleporter) {
							currentBreadCrumbObject.visitOrientation (orientation);
						}

						if (currentType.Value == (int)Type.ValueEnum.Teleporter) { // going into teleporter
							if (currentCell != startCell) {
								currentDifficulty += 2; // this will be double in practice (since going through two teleporters, one start and one exit)
								currentBreadCrumbObject.unVisitOrientation (orientation);
								currentCell = mapCellArray.cells [currentCell.GetComponent<Teleporter> ().Teleposition];
								currentType = currentCell.GetComponent<TypeObject> ().To;
								nextCell = currentCell.GetComponent<NextPositionObject> ().Orientation [orientation].GetComponent<NextPosition> ().nextObject;
								if (currentCell == nextCell) { //ramming into edge of map
									startPositions.NewPositions.Add (nextCell.GetComponent<ThisPosition> ().position);
									startPositions.NewDifficulties.Add (currentDifficulty);
									nextCell.GetComponent<UsedAsStartPosition> ().IsUsed();
									break;
								}
							}
						}
						nextType = nextCell.GetComponent<TypeObject>().To;
						currentBreadCrumbObject = currentCell.GetComponent<BreadCrumbsObject> ();
						nextBreadCrumbObject = nextCell.GetComponent<BreadCrumbsObject> ();

						if (currentCell == startCell && canTestStart) {
							break;
						}
						canTestStart = true;

						inc++;


						if (nextBreadCrumbObject.isVisitedOrientation(orientation)) {
							break;
						}
						// colliding with types
						if (nextType.Value == (int)Type.ValueEnum.Clingy) {
//							if (!currentCell.GetComponent<UsedAsStartPosition> ().GetUsed()) {
								currentDifficulty += 1;
								startPositions.NewPositions.Add (nextCell.GetComponent<ThisPosition> ().position);
								startPositions.NewDifficulties.Add (currentDifficulty);
								nextCell.GetComponent<UsedAsStartPosition> ().IsUsed();
//							}
							break;
						} else if (nextType.Value == (int)Type.ValueEnum.Block) {
							if (!currentCell.GetComponent<UsedAsStartPosition> ().GetUsed()) {
								currentDifficulty += 2;
								startPositions.NewPositions.Add (currentCell.GetComponent<ThisPosition> ().position);
								startPositions.NewDifficulties.Add (currentDifficulty);
								currentCell.GetComponent<UsedAsStartPosition> ().IsUsed();
							}
							break;
						} else if (nextType.Value == (int)Type.ValueEnum.Oneway) {
							if (orientation != nextCell.GetComponent<Orientation> ().Value) {
								if (!currentCell.GetComponent<UsedAsStartPosition> ().GetUsed()) {
									currentDifficulty += 3;
									startPositions.NewPositions.Add (currentCell.GetComponent<ThisPosition> ().position);
									startPositions.NewDifficulties.Add (currentDifficulty);
									currentCell.GetComponent<UsedAsStartPosition> ().IsUsed();
								}
								break;
							}
						}
						// add types
						if (shouldAddTypes) {
							if (!nextBreadCrumbObject.isVisitedTouched ()) {
								if (nextType.Value == (int)Type.ValueEnum.Empty) {
									// add block?
									seedCounter++;
									Random.seed = seedCounter;
									if (Random.value > 0.79f) {
										currentDifficulty += 2;
										nextType.SetValue ((int)Type.ValueEnum.Block);
										startPositions.NewPositions.Add (currentCell.GetComponent<ThisPosition> ().position);
										startPositions.NewDifficulties.Add (currentDifficulty);
										currentCell.GetComponent<UsedAsStartPosition> ().IsUsed ();
										break;
									}
									// add oneway?
									seedCounter++;
									Random.seed = seedCounter;
									if (Random.value > 0.9f) {
										//currentDifficulty += 1;
										if (currentCell.GetComponent<TypeObject> ().To.Value != (int)Type.ValueEnum.Teleporter) {
											if (nextNextCell != nextCell) { // are we at edge?
												if (nextNextCell.GetComponent<TypeObject> ().To.Value == (int)Type.ValueEnum.Empty) { // need to have one free space infront
													if (!currentCell.GetComponent<UsedAsStartPosition> ().GetUsed ()) {
														nextNextCell.GetComponent<BreadCrumbsObject> ().Touched.GetComponent<Touched> ().Visit ();
														nextType.SetValue ((int)Type.ValueEnum.Oneway);
														nextCell.GetComponent<Orientation> ().SetOrientation (orientation);
													}
												}
											}
										}
									}
									// add clingy?
									seedCounter++;
									Random.seed = seedCounter;
									if (Random.value > 0.98f) {
										currentDifficulty += 1;
										nextType.SetValue ((int)Type.ValueEnum.Clingy);
										startPositions.NewPositions.Add (nextCell.GetComponent<ThisPosition> ().position);
										startPositions.NewDifficulties.Add (currentDifficulty);
										nextCell.GetComponent<UsedAsStartPosition> ().IsUsed ();
										break;
									}
									// add teleporter?
									seedCounter++;
									Random.seed = seedCounter;
									if (Random.value > 0.994f) {
										seedCounter++;
										Random.seed = seedCounter;
										temporaryTeleporterPosition = mapCellArray.GetValidTeleporterRandom (nextCell.GetComponent<ThisPosition> ().position, seedCounter);
										if (temporaryTeleporterPosition != -1) {
											teleporterCount++;
											nextType.SetValue ((int)Type.ValueEnum.Teleporter);
											nextCell.GetComponent<CounterObject> ().teleporters.setTeleporterValue (teleporterCount);
											mapCellArray.cells [temporaryTeleporterPosition].GetComponent<CounterObject> ().teleporters.setTeleporterValue (teleporterCount);
											mapCellArray.cells [temporaryTeleporterPosition].GetComponent<TypeObject> ().To.SetValue ((int)Type.ValueEnum.Teleporter);
											nextCell.GetComponent<Teleporter> ().Teleposition = temporaryTeleporterPosition;
											mapCellArray.cells [temporaryTeleporterPosition].GetComponent<Teleporter> ().Teleposition = nextCell.GetComponent<ThisPosition> ().position;
											mapCellArray.cells [temporaryTeleporterPosition].GetComponent<BreadCrumbsObject> ().visitTouched ();
										}
									}
								}
							}
						}

						nextBreadCrumbObject.visitTouched ();

						if (steps < nextCell.GetComponent<CounterObject> ().steps.getValue () || nextCell.GetComponent<CounterObject> ().steps.getValue () == 0) {
							if (nextType.Value == (int)Type.ValueEnum.Empty) {
								nextCell.GetComponent<CounterObject> ().steps.setValue (steps);
							}
						}
						if (currentDifficulty < nextCell.GetComponent<CounterObject> ().difficulty.getValue () || nextCell.GetComponent<CounterObject> ().difficulty.getValue () == 0) {
							if (nextType.Value == (int)Type.ValueEnum.Empty) {
								nextCell.GetComponent<CounterObject> ().difficulty.setValue (currentDifficulty);
							}
						}

						currentCell = nextCell;
						nextCell = nextCell.GetComponent<NextPositionObject> ().Orientation [orientation].GetComponent<NextPosition> ().nextObject;
						nextNextCell = nextCell.GetComponent<NextPositionObject> ().Orientation[orientation].GetComponent<NextPosition> ().nextObject;
					}
					if (currentCell == nextCell) {
						if (!currentCell.GetComponent<UsedAsStartPosition> ().GetUsed()) {
							startPositions.NewPositions.Add (nextCell.GetComponent<ThisPosition> ().position);
							startPositions.NewDifficulties.Add (currentDifficulty);
							currentCell.GetComponent<UsedAsStartPosition> ().IsUsed();
						}
					}
				}
			}
		}
		return startPos;
	}
}

