using UnityEngine;
using System.Collections;

public class PopulateMap : MonoBehaviour {
	// Use this for initialization
	GameobjectArray MapArray;
	NextPosition NextPos;
	void Start () {
		
	}

	public void populate(int Width, int Height, bool looping){ // x:1, y:1
		MapArray = transform.GetComponent<GameobjectArray>();
		MapArray.Cells = new GameObject[Width*Height];

        for (int x = 0; x < Width; x++){
			for (int y = 0; y < Height; y++) {
				MapArray.Cells [(y + (x * Height))] = (GameObject)Instantiate (MapArray.Cell, new Vector3 (x, y, 0), Quaternion.identity);
				MapArray.Cells [(y + (x * Height))].transform.parent = transform;
				MapArray.Cells [(y + (x * Height))].transform.name = "Cell_x:" + x as string + " y;" + y as string + " index:" + (y + (x * Height));
				MapArray.Cells [(y + (x * Height))].GetComponent<ThisPosition> ().position = (y + (x * Height));
				// Right
				for (int orientation = 0; orientation < 4; orientation++) { // 0 = right, 1 = down, 2 = left, 3 = up
					NextPos = MapArray.Cells [(y + (x * Height))].GetComponent<NextPositionObject> ().Orientation[orientation].GetComponent<NextPosition> ();
					switch (orientation) {
					case((int)Orientation.ValueEnum.Right):
						NextPos.nextY = y;
						if (x + 1 < Width) {
							NextPos.nextX = x + 1;
						} else {
							if (looping) {
								NextPos.nextX = 0;
							} else {
								NextPos.nextX = x;
							}
						}
						break;
					case((int)Orientation.ValueEnum.Down):
						NextPos.nextX = x;
						if (y > 0) {
							NextPos.nextY = y - 1;
						} else {
							if (looping) {
								NextPos.nextY = Height-1;
							} else {
								NextPos.nextY = y;
							}
						}
						break;
					case((int)Orientation.ValueEnum.Left):
						NextPos.nextY = y;
						if (x > 0) {
							NextPos.nextX = x - 1;
						} else {
							if (looping) {
								NextPos.nextX = Width-1;
							} else {
								NextPos.nextX = x;
							}
						}
						break;
					case((int)Orientation.ValueEnum.Up):
						NextPos.nextX = x;
						if (y + 1 < Height) {
							NextPos.nextY = y + 1;
						} else {
							if (looping) {
								NextPos.nextY = 0;
							} else {
								NextPos.nextY = y;
							}
						}
						break;
					}
				}
            }
        }
		for (int x = 0; x < Width; x++) {
			for (int y = 0; y < Height; y++) {
				for (int orientation = 0; orientation < 4; orientation++) { // 0 = right, 1 = down, 2 = left, 3 = up
					NextPos = MapArray.Cells [(y + (x * Height))].GetComponent<NextPositionObject>().Orientation[orientation].GetComponent<NextPosition> ();
					NextPos.nextObject = MapArray.Cells [NextPos.nextY + Height * NextPos.nextX];
				}
			}
		}
    }
}
