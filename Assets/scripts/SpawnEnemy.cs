using UnityEngine;
using System.Collections.Generic;

public class SpawnEnemy : MonoBehaviour {

	public GameObject enemyType;

	Vector3 spawnHeightOffset;

	void Start()
	{
		spawnHeightOffset = new Vector3(0f, 0.9f, 0f);
	}

	void Update ()
	{
		if (Input.GetButtonUp("SpawnEnemy")) {
			CreateEnemy();
		}
	}

	void CreateEnemy()
	{
		// make the list of waypoints
		int waypointAmount = Random.Range(2, 4);
		List<Vector3> waypointList = new List<Vector3>(waypointAmount);

		// spawn point
		waypointList.Add(randomDirection() * Random.Range(8f, 14f));

		// rest of waypoints
		for (int i = 1; i < waypointAmount; i++) {
			waypointList.Add(waypointList[i - 1] + (randomDirection() * Random.Range(7f, 12f)));
		}

		Enemy enemy = (GameObject.Instantiate(enemyType, waypointList[0] + spawnHeightOffset,
		                                     Quaternion.identity) as GameObject).GetComponent<Enemy>();
		enemy.targetPosition = waypointList;
	}

	// returns a random normalized Vector3 with 0 y
	Vector3 randomDirection()
	{
		return new Vector3(Random.Range(-1f, 1f), 0f, Random.Range(-1f, 1f)).normalized;
	}
}
