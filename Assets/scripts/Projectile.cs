﻿using UnityEngine;
using System.Collections;

public class Projectile : MonoBehaviour {
	public float speed;
	public float timeToDestruction;
	public int damage;

	Vector3 direction;

	void Start() {
		Destroy (gameObject, timeToDestruction);
	}

	void OnTriggerEnter(Collider other) {
		if (other.CompareTag("Enemy")) {
			other.GetComponent<Enemy>().AddHealth(-damage);
			Destroy(gameObject);
		}
	}
	
	public void SetDirection(Vector3 dir) {
		direction = dir;
		rigidbody.velocity = direction * speed;
	}
}
