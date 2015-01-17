using UnityEngine;
using System.Collections;

public class HelpGUI : MonoBehaviour {

	public CameraControl cam;

	int line;

	void OnGUI()
	{
		line = 0;

		GUI.Label(getRect(), "Toggle camera mode: Tab");
		GUI.Label(getRect(), "Spawn enemy: E");

		line++;

		switch (cam.cameraMode) {
		case CameraControl.CameraMode.ThirdPerson:
			GUI.Label(getRect(), "CAMERA CONTROLS - third person mode");
			GUI.Label(getRect(), "Pan camera: right mouse button + move mouse");
			GUI.Label(getRect(), "Zoom camera: mouse scroll wheel");
			break;

		case CameraControl.CameraMode.Isometric:
			GUI.Label(getRect(), "CAMERA CONTROLS - isometric mode");
			GUI.Label(getRect(), "Zoom camera: mouse scroll wheel");
			break;

		case CameraControl.CameraMode.FreeLook:
			GUI.Label(getRect(), "CAMERA CONTROLS - free look mode");
			GUI.Label(getRect(), "Look with camera: right mouse button + move mouse");
			GUI.Label(getRect(), "Move forward: W/S - horizontal: A/D - vertical: R/F");
			break;
		}

		if (cam.cameraMode != CameraControl.CameraMode.FreeLook) {
			line++;
			GUI.Label(getRect(), "PLAYER CONTROLS");
			GUI.Label(getRect(), "Move: WASD");
			GUI.Label(getRect(), "Place mine: F - explodes upon contact with enemy");
			GUI.Label(getRect(), "Place/remove wall: R - enemies path around walls");
			GUI.Label(getRect(), "Shoot: left mouse button");
		}
	}

	Rect getRect()
	{
		return new Rect(10, 10 + 15 * line++, 500, 25);
	}

	string coords(Vector3 pos)
	{
		return "(" + pos.x + ", " + pos.y + ")";
	}
}
