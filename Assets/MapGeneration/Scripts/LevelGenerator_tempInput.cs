using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelGenerator_tempInput : MonoBehaviour {
    public GameObject map;
	public bool CanStartCoroutine;
	private GameObject CurrentOrientation, NewOrientation, NewNewOrientation, StartOrientation;
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
		// clear and populate map array
		map.GetComponent<GameobjectArray>().Clear();
//		map.GetComponent<PopulateMap>().populate(Random.Range(5,13), Random.Range(5,13));
//		map.GetComponent<PopulateMap>().populate(12, 20, true);
		map.GetComponent<PopulateMap>().populate(9, 16, true);
//		map.GetComponent<PopulateMap>().populate(2, 2, true);
		yield return null;

		// place a random startposition
		int StartPos = map.GetComponent<GameobjectArray> ().GetRandom ();

		map.GetComponent<GameobjectArray>().Cells [StartPos].GetComponentInChildren<Type>().SetValue((int)Type.ValueEnum.Start);
		map.GetComponent<StartPositions>().NewPositions.Add(StartPos);
		map.GetComponent<GameobjectArray>().Cells[StartPos].GetComponent<UsedAsStartPosition>().used = true;
		yield return null;

		// iterate through all "StartPositions"
		steps = 0;
		difficulty = 0;
		while (map.GetComponent<StartPositions> ().NewPositions.Count > 0) {
			steps++;

			map.GetComponent<StartPositions> ().CurrentPositions =  new List<int>(map.GetComponent<StartPositions> ().NewPositions);
			map.GetComponent<StartPositions> ().NewPositions.Clear ();
			foreach (int pos in map.GetComponent<StartPositions>().CurrentPositions) {
				for (int orientation = 0; orientation < 4; orientation++) { // 0 = right, 1 = down, 2 = left, 3 = up
					map.GetComponent<GameobjectArray>().Cells[pos].GetComponent<BreadCrumbsObject>().Touched.GetComponent<Touched>().Visit();
					map.GetComponent<GameobjectArray>().Cells[pos].GetComponent<BreadCrumbsObject>().Orientation[orientation].GetComponent<Touched>().Visit();
					if (map.GetComponent<GameobjectArray> ().Cells [pos].GetComponent<TypeObject> ().To.GetComponent<Type> ().Value == (int)Type.ValueEnum.Teleporter) {
						NewOrientation = map.GetComponent<GameobjectArray> ().Cells[map.GetComponent<GameobjectArray> ().Cells [pos].GetComponent<Teleporter> ().Teleposition];
					} else {
						NewOrientation = map.GetComponent<GameobjectArray> ().Cells [pos].GetComponent<NextPositionObject> ().Orientation [orientation].GetComponent<NextPosition> ().nextObject;
					}
					NewNewOrientation = NewOrientation.GetComponent<NextPositionObject> ().Orientation[orientation].GetComponent<NextPosition> ().nextObject;
					CurrentOrientation = map.GetComponent<GameobjectArray> ().Cells [pos];
					StartOrientation = CurrentOrientation;
					CanTestStart = false;
					// iterating steps
					while (CurrentOrientation != NewOrientation) {
						if (CurrentOrientation == StartOrientation && CanTestStart) {
							break;
						}
						CanTestStart = true;
						CurrentOrientation.GetComponent<BreadCrumbsObject> ().Orientation [orientation].GetComponent<Touched> ().Visit ();
						inc++;
						goingIntoTeleporter = false;
						if (CurrentOrientation.GetComponent<TypeObject> ().To.GetComponent<Type> ().Value == (int)Type.ValueEnum.Teleporter) {
							goingIntoTeleporter = true;
							CurrentOrientation.GetComponent<BreadCrumbsObject>().Orientation[orientation].GetComponent<Touched> ().UnVisit ();
						}
						if (NewOrientation.GetComponent<BreadCrumbsObject> ().Orientation[orientation].GetComponent<Touched> ().IsVisited ()) {
							break;
						}
						// colliding with types
						if (NewOrientation.GetComponent<TypeObject> ().To.Value == (int)Type.ValueEnum.Clingy) {
							if (!CurrentOrientation.GetComponent<UsedAsStartPosition> ().used) {
								map.GetComponent<StartPositions> ().NewPositions.Add (NewOrientation.GetComponent<ThisPosition> ().position);
								NewOrientation.GetComponent<UsedAsStartPosition> ().used = true;
							}
							break;
						} else if (NewOrientation.GetComponent<TypeObject> ().To.Value == (int)Type.ValueEnum.Block) {
							if (!CurrentOrientation.GetComponent<UsedAsStartPosition> ().used) {
								map.GetComponent<StartPositions> ().NewPositions.Add (CurrentOrientation.GetComponent<ThisPosition> ().position);
								CurrentOrientation.GetComponent<UsedAsStartPosition> ().used = true;
							}
							break;
						} else if (NewOrientation.GetComponent<TypeObject> ().To.Value == (int)Type.ValueEnum.Oneway) {
							if (orientation != NewOrientation.GetComponent<Orientation> ().Value) {
								if (!CurrentOrientation.GetComponent<UsedAsStartPosition> ().used) {
									map.GetComponent<StartPositions> ().NewPositions.Add (CurrentOrientation.GetComponent<ThisPosition> ().position);
									CurrentOrientation.GetComponent<UsedAsStartPosition> ().used = true;
								}
								break;
							}
						}
						// add types
						if (!NewOrientation.GetComponent<BreadCrumbsObject> ().Touched.GetComponent<Touched> ().IsVisited ()) {
							if (NewOrientation.GetComponent<TypeObject>().To.Value == (int)Type.ValueEnum.Empty){
								// add block?
								if (Random.value>0.85f){
									NewOrientation.GetComponent<TypeObject> ().To.SetValue((int)Type.ValueEnum.Block);
									map.GetComponent<StartPositions> ().NewPositions.Add (CurrentOrientation.GetComponent<ThisPosition> ().position);
									CurrentOrientation.GetComponent<UsedAsStartPosition> ().used = true;
									break;
								}
								// add oneway?
								if (Random.value>0.85f){
									if (NewNewOrientation != NewOrientation) { // are we at edge?
										if (NewNewOrientation.GetComponent<TypeObject> ().To.Value == (int)Type.ValueEnum.Empty) { // need to have one free space infront
											if (!CurrentOrientation.GetComponent<UsedAsStartPosition> ().used) {
												//NewNewOrientation.GetComponent<BreadCrumbsObject> ().Touched.GetComponent<Touched> ().Visit();
												NewOrientation.GetComponent<TypeObject> ().To.SetValue ((int)Type.ValueEnum.Oneway);
												NewOrientation.GetComponent<Orientation> ().SetOrientation (orientation);
											}
										}
									}
								}
								// add clingy?
								if (Random.value > 0.98f) {
									NewOrientation.GetComponent<TypeObject> ().To.SetValue((int)Type.ValueEnum.Clingy);
									map.GetComponent<StartPositions> ().NewPositions.Add (NewOrientation.GetComponent<ThisPosition> ().position);
									NewOrientation.GetComponent<UsedAsStartPosition> ().used = true;
	//								NewOrientation.GetComponent<CounterObject> ().steps.setValue (steps);
									break;
								}
								// add teleporter?
								if (Random.value > 0.99f) {
									if (!goingIntoTeleporter) {
										temporaryTeleporterPosition = map.GetComponent<GameobjectArray> ().GetValidTeleporterRandom (NewOrientation.GetComponent<ThisPosition>().position);
										if (temporaryTeleporterPosition != -1) {
											NewOrientation.GetComponent<TypeObject> ().To.SetValue ((int)Type.ValueEnum.Teleporter);
											map.GetComponent<GameobjectArray> ().Cells [temporaryTeleporterPosition].GetComponent<TypeObject> ().To.SetValue ((int)Type.ValueEnum.Teleporter);
											NewOrientation.GetComponent<Teleporter> ().Teleposition = temporaryTeleporterPosition;
											map.GetComponent<GameobjectArray> ().Cells [temporaryTeleporterPosition].GetComponent<Teleporter> ().Teleposition = NewOrientation.GetComponent<ThisPosition> ().position;
											map.GetComponent<GameobjectArray> ().Cells [temporaryTeleporterPosition].GetComponent<BreadCrumbsObject> ().Touched.GetComponent<Touched> ().Visit ();
										}
									}
								}
							}
						}

						NewOrientation.GetComponent<BreadCrumbsObject> ().Touched.GetComponent<Touched> ().Visit ();
						if (NewOrientation.GetComponent<TypeObject> ().To.Value != (int)Type.ValueEnum.Teleporter) {
							CurrentOrientation.GetComponent<BreadCrumbsObject> ().Orientation[orientation].GetComponent<Touched> ().Visit();
						}

						if (steps < NewOrientation.GetComponent<CounterObject> ().steps.getValue () || NewOrientation.GetComponent<CounterObject> ().steps.getValue () == 0) {
							NewOrientation.GetComponent<CounterObject> ().steps.setValue (steps);
						}

						CurrentOrientation = NewOrientation;
//						if (CurrentOrientation == StartOrientation){
//							break;
//						}
						if (CurrentOrientation.GetComponent<TypeObject> ().To.GetComponent<Type> ().Value == (int)Type.ValueEnum.Teleporter && !goingIntoTeleporter) {
							NewOrientation = map.GetComponent<GameobjectArray> ().Cells [CurrentOrientation.GetComponent<Teleporter> ().Teleposition];
						} else {
							NewOrientation = NewOrientation.GetComponent<NextPositionObject> ().Orientation [orientation].GetComponent<NextPosition> ().nextObject;
						}
//						NewOrientation = NewNewOrientation;
						NewNewOrientation = NewOrientation.GetComponent<NextPositionObject> ().Orientation[orientation].GetComponent<NextPosition> ().nextObject;
						yield return new WaitForSeconds (0.08f);
					}
					if (CurrentOrientation == NewOrientation) {
						if (!CurrentOrientation.GetComponent<UsedAsStartPosition> ().used) {
							map.GetComponent<StartPositions> ().NewPositions.Add (NewOrientation.GetComponent<ThisPosition> ().position);
							CurrentOrientation.GetComponent<UsedAsStartPosition> ().used = true;
						}
					}
				}
			}
			yield return new WaitForSeconds (0.03f);
		}
		yield return null;

		CanStartCoroutine = true; // prep for coroutine to start again
	}
}

