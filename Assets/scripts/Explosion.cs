using UnityEngine;
using System.Collections;

public class Explosion : MonoBehaviour {

	public float explosionForce;
	public float explosionRadius;
	public float upwardsModifier;
	public float maxDamage;

	float duration = 1f;

	// Use this for initialization
	void Start () {
		float explosionRadiusSqrd = explosionRadius * explosionRadius;

		// get all colliders within explosion radius
		Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);

		// iterate over all hit colliders
		foreach (Collider hit in colliders) {
			// push and damage all hit enemies
			if (hit && hit.rigidbody && hit.CompareTag("Enemy")) {
				hit.rigidbody.AddExplosionForce(explosionForce, transform.position, explosionRadius, upwardsModifier);

				/*Debug.Log(Mathf.Lerp (maxDamage, 0f, 0.5f));
				Debug.Log(Mathf.Lerp (maxDamage, 0f, 0.1f));
				Debug.Log(Mathf.Lerp (maxDamage, 0f, 0.9f));
				Debug.Log(Mathf.Lerp (maxDamage, 0f, 1f));
				Debug.Log(Mathf.Lerp (maxDamage, 0f, 0f));*/

				hit.GetComponent<Enemy>().AddHealth( -1 * (int)
					// damage is linear between maxDamage (enemy at same position as explosion)
					//   and 0 (enemy is as far from explosion as possible)
					Mathf.Lerp(maxDamage, 0f,
				           // distance from enemy as a percentage of max explosion range
				           Mathf.Clamp01((hit.transform.position - transform.position).sqrMagnitude
				              			 / explosionRadiusSqrd)));
			}
			else if (hit && hit.CompareTag("Mine")) {
				hit.GetComponent<Mine>().Explode();
			}
		}

		Destroy(gameObject, duration);
	}
}
