using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelGenerator_tempInput : MonoBehaviour {
	public int mapSeed;
	public float buildSpeed;
    public GameObject map;
	[HideInInspector]
	public bool CanStartCoroutine;
	private GameObject currentCell, nextCell, nextNextCell, startCell;
	private int difficulty, steps, teleporterCount;
	private int temporaryTeleporterPosition;
	private bool goingIntoTeleporter, CanTestStart;
	private int inc;
	private int seedCounter = 0;

	// Use this for initialization
	void Start () {
		CanStartCoroutine = true;
		steps = 0;
		difficulty = 0;
		teleporterCount = 10;
		seedCounter = 0;
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
		mapSeed++;
		GameobjectArray mapCellArray =  map.GetComponent<GameobjectArray>();
		StartPositions startPositions = map.GetComponent<StartPositions> ();
		seedCounter = mapSeed;



		// clear counters:
		steps = 0;
		difficulty = 0;
		teleporterCount = 10;


		// clear and populate map array:
		mapCellArray.Clear();
//		map.GetComponent<PopulateMap>().populate(Random.Range(5,13), Random.Range(5,13));
//		map.GetComponent<PopulateMap>().populate(12, 20, true);
//		map.GetComponent<PopulateMap>().populate(9, 16, true);
		map.GetComponent<PopulateMap>().populate(16, 9, false);
//		map.GetComponent<PopulateMap>().populate(6, 12, true);
//		map.GetComponent<PopulateMap>().populate(3, 3, true);
		yield return null;

		Random.seed = seedCounter;
		seedCounter++;
		Debug.Log ("startrandom:" + Random.value);
		int StartPos = mapCellArray.GetRandom(seedCounter);

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

				startCell = mapCellArray.Cells[current];

				currentBreadCrumbObject.visitTouched();

				for (int orientation = 0; orientation < 4; orientation++) { // 0 = right, 1 = down, 2 = left, 3 = up
//					Debug.Log("startpos:" + current + " orientation:" + orientation);
					// variable decalarations:
					Type currentType;
					Type nextType;

					currentCell = mapCellArray.Cells[current];
					currentType = currentCell.GetComponent<TypeObject>().To;
					currentBreadCrumbObject = currentCell.GetComponent<BreadCrumbsObject> ();
					currentBreadCrumbObject.visitOrientation (orientation);

//					// what to do when starting on top of a teleporter
//					if (currentType.Value == (int)Type.ValueEnum.Teleporter) {
//						nextCell = mapCellArray.Cells[currentCell.GetComponent<Teleporter> ().Teleposition];
//					} else {
//						nextCell = currentCell.GetComponent<NextPositionObject> ().Orientation [orientation].GetComponent<NextPosition> ().nextObject;
//					}
					nextCell = currentCell.GetComponent<NextPositionObject> ().Orientation [orientation].GetComponent<NextPosition> ().nextObject;
					nextNextCell = nextCell.GetComponent<NextPositionObject> ().Orientation[orientation].GetComponent<NextPosition> ().nextObject;
					nextType = nextCell.GetComponent<TypeObject>().To;

					CanTestStart = false;

					while (currentCell != nextCell) {
						currentType = currentCell.GetComponent<TypeObject>().To;
						currentBreadCrumbObject = currentCell.GetComponent<BreadCrumbsObject> ();
						currentBreadCrumbObject.visitOrientation(orientation);

						goingIntoTeleporter = false;
						if (currentType.Value == (int)Type.ValueEnum.Teleporter) {
							if (currentCell != startCell){
								goingIntoTeleporter = true;

								currentBreadCrumbObject.unVisitOrientation(orientation);
								currentCell = mapCellArray.Cells [currentCell.GetComponent<Teleporter> ().Teleposition];
								currentType = currentCell.GetComponent<TypeObject>().To;
								nextCell = currentCell.GetComponent<NextPositionObject> ().Orientation [orientation].GetComponent<NextPosition> ().nextObject;
	//							nextBreadCrumbObject = nextCell.GetComponent<BreadCrumbsObject> ();
							}
						}

						nextType = nextCell.GetComponent<TypeObject>().To;
						currentBreadCrumbObject = currentCell.GetComponent<BreadCrumbsObject> ();
						nextBreadCrumbObject = nextCell.GetComponent<BreadCrumbsObject> ();

//						Debug.Log ("I'm at:" + currentCell.GetComponent<ThisPosition> ().position);

						if (currentCell == startCell && CanTestStart) {
							break;
						}
						CanTestStart = true;


						inc++;


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
								seedCounter++;
								Random.seed = seedCounter;
								if (Random.value>0.85f){
									nextType.SetValue((int)Type.ValueEnum.Block);
									startPositions.NewPositions.Add (currentCell.GetComponent<ThisPosition> ().position);
									currentCell.GetComponent<UsedAsStartPosition> ().used = true;
									break;
								}
								// add oneway?
								seedCounter++;
								Random.seed = seedCounter;
								if (Random.value>0.85f){
									if (nextNextCell != nextCell) { // are we at edge?
										if (nextNextCell.GetComponent<TypeObject> ().To.Value == (int)Type.ValueEnum.Empty) { // need to have one free space infront
											if (!currentCell.GetComponent<UsedAsStartPosition> ().used) {
												nextNextCell.GetComponent<BreadCrumbsObject> ().Touched.GetComponent<Touched> ().Visit();
												nextType.SetValue ((int)Type.ValueEnum.Oneway);
												nextCell.GetComponent<Orientation> ().SetOrientation (orientation);
											}
										}
									}
								}
								// add clingy?
								seedCounter++;
								Random.seed = seedCounter;
								if (Random.value > 0.98f) {
									nextType.SetValue((int)Type.ValueEnum.Clingy);
									startPositions.NewPositions.Add (nextCell.GetComponent<ThisPosition> ().position);
									nextCell.GetComponent<UsedAsStartPosition> ().used = true;
	//								nextCell.GetComponent<CounterObject> ().steps.setValue (steps);
									break;
								}
								// add teleporter?
								seedCounter++;
								Random.seed = seedCounter;
								if (Random.value > 0.98f) {
									if (!goingIntoTeleporter) {
										seedCounter++;
										Random.seed = seedCounter;
										temporaryTeleporterPosition = mapCellArray.GetValidTeleporterRandom (nextCell.GetComponent<ThisPosition>().position, seedCounter);
										if (temporaryTeleporterPosition != -1) {
											teleporterCount++;
											nextType.SetValue ((int)Type.ValueEnum.Teleporter);
											nextCell.GetComponent<CounterObject> ().teleporters.setValue (teleporterCount);
											mapCellArray.Cells [temporaryTeleporterPosition].GetComponent<CounterObject>().teleporters.setValue (teleporterCount);
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
//						if (nextType.Value != (int)Type.ValueEnum.Teleporter) {
//							currentBreadCrumbObject.visitOrientation(orientation);
//						}

						if (steps < nextCell.GetComponent<CounterObject> ().steps.getValue () || nextCell.GetComponent<CounterObject> ().steps.getValue () == 0) {
							if (nextType.Value == (int)Type.ValueEnum.Empty) {
								nextCell.GetComponent<CounterObject> ().steps.setValue (steps);
							}
						}

						currentCell = nextCell;
//						if (currentCell == startCell){
//							break;
//						}
						nextCell = nextCell.GetComponent<NextPositionObject> ().Orientation [orientation].GetComponent<NextPosition> ().nextObject;
//						Debug.Log("before checking teleporter");
//						if (currentType.Value == (int)Type.ValueEnum.Teleporter && !goingIntoTeleporter) {
//							nextCell = mapCellArray.Cells [currentCell.GetComponent<Teleporter> ().Teleposition];
//							Debug.Log ("nextcCell:" + nextCell.GetComponent<ThisPosition> ().position);
//							Debug.Log ("We are now teleporting!! to index:" + nextCell.GetComponent<ThisPosition>().position );
//						} else {
//							nextCell = nextCell.GetComponent<NextPositionObject> ().Orientation [orientation].GetComponent<NextPosition> ().nextObject;
//						}
//						nextCell = nextNextCell;
						nextNextCell = nextCell.GetComponent<NextPositionObject> ().Orientation[orientation].GetComponent<NextPosition> ().nextObject;
						yield return new WaitForSeconds (0.082f/Mathf.Clamp(buildSpeed,0.08f,100));
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

