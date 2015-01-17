using UnityEngine;
using System.Collections;

public class CameraControl : MonoBehaviour
{
	public enum CameraMode {
		ThirdPerson,
		Isometric,
		FreeLook
	}

	public float sensitivityX;
	public float sensitivityY;
	public float panSpeed;
	public float isometricZoomSpeed;
	public float thirdPersonZoomSpeed;
	public Transform target;
	public CameraMode cameraMode { get; set; }

	Vector3 lastTargetPos;
	Vector3 isoOffset;
	Quaternion isoRotation;
	float targetDistance;

	void Start()
	{
		cameraMode = CameraMode.ThirdPerson;
		// isometric mode needs to know positional offset from target and rotation
		isoOffset = transform.position - target.position;
		isoRotation = transform.rotation;
		// third person mode needs to know distance from target
		targetDistance = isoOffset.magnitude;
	}

	void Update()
	{
		// check for camera mode switch
		if (Input.GetButtonUp("ToggleCameraMode")) {
			lastTargetPos = target.position;

			// switch camera mode
			switch (cameraMode) {
			case CameraMode.ThirdPerson:
				cameraMode = CameraMode.Isometric;
				camera.orthographic = true;
				camera.transform.rotation = isoRotation;
				break;

			case CameraMode.Isometric:
				cameraMode = CameraMode.FreeLook;
				camera.orthographic = false;
				break;

			case CameraMode.FreeLook:
				cameraMode = CameraMode.ThirdPerson;
				break;
			}
		}

		// update camera
		switch (cameraMode) {
		case CameraMode.ThirdPerson: thirdPersonMode(); break;
		case CameraMode.Isometric: isometricMode(); break;
		case CameraMode.FreeLook: lookMode(); break;
		}
	}
	
	void thirdPersonMode()
	{
		transform.position += target.position - lastTargetPos;
		lastTargetPos = target.position;
		targetDistance += -1 * Input.GetAxis("CameraZoom") * thirdPersonZoomSpeed * Time.deltaTime;

		if (Input.GetButton("CameraReset")) {
			transform.Translate(-1 * Input.GetAxis("CameraLookX") * sensitivityX * Time.deltaTime,
			                    -1 * Input.GetAxis("CameraLookY") * sensitivityY * Time.deltaTime, 0f);
		}

		transform.Translate(0f, 0f, (transform.position - target.position).magnitude
		                    - targetDistance);

		transform.LookAt(target.position);
	}

	void isometricMode()
	{
		transform.position = target.position + isoOffset;

		camera.orthographicSize += Input.GetAxis("CameraZoom") * isometricZoomSpeed;
	}
	
	void lookMode()
	{
		if (Input.GetButton("CameraReset")) {
			Vector3 rot = transform.eulerAngles;
			rot.x += angleClamp(Input.GetAxis("CameraLookY") * sensitivityY * Time.deltaTime);
			rot.y += angleClamp(Input.GetAxis("CameraLookX") * sensitivityX * Time.deltaTime);
			
			transform.eulerAngles = rot;
		}
		
		transform.Translate(Input.GetAxis("CameraHorizontal") * panSpeed * Time.deltaTime,
		                    Input.GetAxis("CameraVertical") * panSpeed * Time.deltaTime,
		                    Input.GetAxis("CameraForward") * panSpeed * Time.deltaTime);
	}

	float angleClamp(float angle)
	{
		if (angle < 0)
			return ((angle % 360) + 360) % 360;
		return angle % 360;
	}
}
