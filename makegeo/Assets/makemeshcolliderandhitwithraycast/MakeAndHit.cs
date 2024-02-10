using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// @kurtdekker - you CAN hit geometry you just created.
// to see, drop this on a blank GameObject and press PLAY

public class MakeAndHit : MonoBehaviour
{
	void Start ()
	{
		GameObject tri = new GameObject();

		tri.name = "Target!";

		Mesh mesh = new Mesh();

		mesh.vertices = new Vector3[] {
			new Vector3( -1, -1, 1),
			new Vector3( -1, +1, 1),
			new Vector3( +2, +0, 1),
		};
		mesh.triangles = new int[] {
			0, 1, 2,
		};

		MeshCollider mc = tri.AddComponent<MeshCollider>();
		mc.sharedMesh = mesh;

		Ray ray = new Ray( origin: Vector3.zero, direction: Vector3.forward);

		RaycastHit hit;
		if (Physics.Raycast( ray, out hit, Mathf.Infinity))
		{
			Debug.Log( "Hit " + hit.collider.name);
		}
		else
		{
			Debug.Log( "Missed.");
		}
	}
}
