using UnityEngine;
using System.Collections.Generic;

public class Player : MonoBehaviour {

	public GraphUpdate graphUpdate;
	public CameraControl followCamera;
	public Projectile projectile;
	public GameObject mine;
	public GameObject wall;
	public float speed;
	public float wallDistance;
	public float maxRayLength;

	Vector3 movement;
	Vector3 lookDirection;
	CameraControl.CameraMode cameraMode;

	void Start ()
	{
		cameraMode = followCamera.cameraMode;
	}
	
	// Update is called once per frame
	void Update ()
	{
		// no actions when camera is in look mode
		if (cameraMode == CameraControl.CameraMode.FreeLook)
			return;

		// shoot a projectile
		if (Input.GetButtonUp("Shoot")) {
			Shoot();
		}

		// create a mine
		if (Input.GetButtonUp("Mine")) {
			PlaceMine();
		}

		// create or delete a wall
		if (Input.GetButtonUp ("Wall")) {
			PlaceOrRemoveWall();
		}
	}

	void FixedUpdate()
	{
		// camera mode changed
		if (followCamera.cameraMode != cameraMode) {
			cameraMode = followCamera.cameraMode;

			if (cameraMode == CameraControl.CameraMode.Isometric) {
				transform.forward = Vector3.forward;
				rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
			}
			else {
				rigidbody.constraints = RigidbodyConstraints.FreezeRotationX |
										RigidbodyConstraints.FreezeRotationZ;
			}
		}

		// no movement when camera is in look mode
		if (cameraMode == CameraControl.CameraMode.FreeLook)
			return;

		movement.x = Input.GetAxisRaw("Horizontal");
		movement.z = Input.GetAxisRaw("Vertical");

		if (!movement.Equals(Vector3.zero)) {
			movement = movement.normalized * speed * Time.fixedDeltaTime;

			// if camera is in third-person mode, rotate player to move in direction of camera
			if (cameraMode == CameraControl.CameraMode.ThirdPerson) {
				lookDirection = followCamera.transform.forward;
				lookDirection.y = 0f;
				transform.forward = lookDirection;
				movement = Quaternion.FromToRotation(Vector3.forward, transform.forward) * movement;
			}

			rigidbody.MovePosition(transform.position + movement);
		}
	}

	void Shoot()
	{
		Vector3 mouseRayPoint = MouseRaycast();

		// no vertical aiming in isometric mode
		if (cameraMode == CameraControl.CameraMode.Isometric) {
			mouseRayPoint.y = transform.position.y;
		}

		// aim the projectile at the mouse
		Vector3 projectileDirection = (mouseRayPoint - transform.position).normalized;
		Projectile newProjectile =
			Instantiate(projectile,
			            transform.position,
			            Quaternion.identity) as Projectile;
		newProjectile.SetDirection(projectileDirection);
		newProjectile.owner = collider;
	}

	void PlaceMine()
	{
		// create the mine
		GameObject newMine = Instantiate(mine) as GameObject;
		
		// position the mine in the ground below the player
		Vector3 placePos = transform.position;
		placePos.y = 0f;
		
		newMine.transform.position = placePos;
	}

	void PlaceOrRemoveWall()
	{
		bool deleted = false; // true if the raycast hit a wall, which will be deleted
		Vector3 mouseRayPoint = Vector3.zero;
		Collider mouseRayCol = null;

		// cast a ray from the mouse
		if (MouseRaycast(out mouseRayPoint, out mouseRayCol)) {
			// if it hit a wall, delete the wall
			if (mouseRayCol.CompareTag("Wall")) {
				GameObject.Destroy(mouseRayCol.gameObject);
				
				//update the graph
				graphUpdate.AddUpdateBounds(mouseRayCol.bounds);
				
				deleted = true;
			}
		}
		
		// raycast didn't hit a wall; create a wall instead of deleting one
		if (!deleted) {
			// create the wall
			GameObject newWall = Instantiate (wall) as GameObject;
			
			// wall position is based on player position
			Vector3 placePos = transform.position;
			placePos.y = 1.5f;
			
			// distance from player to mouse click
			Vector3 clickDistance = mouseRayPoint - transform.position;
			
			// mouse click was farther in the x direction
			if (Mathf.Abs(clickDistance.x) >= Mathf.Abs(clickDistance.z)) {
				if (clickDistance.x < 0f)
					placePos.x -= wallDistance;
				else
					placePos.x += wallDistance;
				
				// rotate wall to face x direction
				newWall.transform.rotation = Quaternion.Euler(0f, 90f, 0f);
			}
			// mouse click was farther in the z direction
			else {
				if (clickDistance.z < 0f)
					placePos.z -= wallDistance;
				else
					placePos.z += wallDistance;
			}
			
			newWall.transform.position = placePos;
			
			// update the graph
			graphUpdate.AddUpdateBounds(newWall.collider.bounds);
		}
	}

	// casts a ray from the mouse with maximum length maxRayLength
	// returns true if an object was hit, in which case hitCollider contains the hit object's collider
	// hitPos always contains the end point of the ray
	bool MouseRaycast(out Vector3 hitPos, out Collider hitCollider)
	{
		bool hitObject = false;
		hitCollider = null;

		RaycastHit hitInfo;
		Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
		float distance = maxRayLength;

		// raycast hit something; fill hitCollider and set distance accordingly
		if (Physics.Raycast(mouseRay, out hitInfo, maxRayLength)) {
			hitObject = true;
			hitCollider = hitInfo.collider;
			distance = hitInfo.distance;
		}

		// get the endpoint of the ray
		hitPos = mouseRay.GetPoint(distance);

		return hitObject;
	}

	// overload for MouseRaycast; just returns endpoint
	Vector3 MouseRaycast()
	{
		Collider col = null;
		Vector3 endpoint = Vector3.zero;
		MouseRaycast(out endpoint, out col);
		return endpoint;
	}
}
