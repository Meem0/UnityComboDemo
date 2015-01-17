using UnityEngine;
using System.Collections.Generic;

using Pathfinding;

public class Enemy : MonoBehaviour {

	public List<Vector3> targetPosition;
	public float speed;
	public float reachedNodeDistance;
	public float maxNodeTravelTime;
	public int health = 100;

	Seeker seeker;
	Path path;
	Vector3 movement;
	Vector3 nodeDistance;
	bool pathRequested = false;
	int pathNodeIndex = 0;
	int targetPosIndex = 0;
	int currentNodeTravelTime = 0;

	public void AddHealth(int amount) {
		health += amount;

		if (health <= 0) {
			Destroy(gameObject);
		}
	}

	void Start() {
		Debug.Log(targetPosition);

		seeker = GetComponent<Seeker>();
	}

	public void OnPathComplete(Path p) {
		path = p;

		pathRequested = false;
		pathNodeIndex = 0;
		currentNodeTravelTime = 0;
	}

	void FixedUpdate() {
		// path not set
		if (path == null) {

			if (!pathRequested) {
				// create a path to targetPosition, return result to OnPathComplete
				seeker.StartPath(transform.position, targetPosition[targetPosIndex], OnPathComplete);
				pathRequested = true;
			}

			return;
		}

		// last node of current path reached
		if (pathNodeIndex >= path.vectorPath.Count) {
			path = null;

			// advance to the next position
			targetPosIndex++;
			if (targetPosIndex >= targetPosition.Count)
				targetPosIndex = 0;

			return;
		}

		// current step's distance to travel
		movement = path.vectorPath[pathNodeIndex] - transform.position;
		movement = movement.normalized * speed * Time.fixedDeltaTime;

		// move toward current node
		rigidbody.MovePosition(transform.position + movement);

		// get distance from current node, not considering y
		nodeDistance = path.vectorPath[pathNodeIndex] - transform.position;
		nodeDistance.y = 0f;

		// if node was reached, go to the next one
		if (nodeDistance.sqrMagnitude < reachedNodeDistance) {
			pathNodeIndex++;
			currentNodeTravelTime = 0;
		}
		// otherwise, check if it's taken too long to get to the next node
		else if (++currentNodeTravelTime > maxNodeTravelTime) {
			path = null;
		}
	}
}
