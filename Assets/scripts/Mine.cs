using UnityEngine;
using System.Collections;

public class Mine : MonoBehaviour {

	public GameObject explosion;

	bool exploded = false;

	public void Explode()
	{
		if (!exploded) {
			exploded = true;

			GameObject myExplosion = Instantiate(explosion) as GameObject;
			myExplosion.transform.position = transform.position;
			
			Destroy(gameObject);
		}
	}

	void OnTriggerEnter(Collider col)
	{
		if (col.gameObject.CompareTag("Enemy")) {
			Explode ();
		}
	}
}
