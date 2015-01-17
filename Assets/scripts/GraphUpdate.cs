using UnityEngine;
using System.Collections.Generic;

public class GraphUpdate : MonoBehaviour {

	LinkedList<Bounds> updateBounds;

	void Start()
	{
		updateBounds = new LinkedList<Bounds>();
	}
	
	void LateUpdate () 
	{
		while (updateBounds.Count != 0) {
			AstarPath.active.UpdateGraphs(updateBounds.First.Value);
			updateBounds.RemoveFirst();
		}
	}

	public void AddUpdateBounds(Bounds bounds)
	{
		updateBounds.AddLast(bounds);
	}
}
