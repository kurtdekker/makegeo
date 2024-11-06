using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//
// @kurtdekker - testing "changing out" the vertices in a
// Mesh object without doing anything else. Who notices?
//
// To run: place this on an empty GameObject, optionally
// provide your own MeshFilter/Collider to test, or we make one.
//
// In the scene you will see what's going on via Gizmos.
//
// This will pause itself, requiring you to un-pause it.
//
// To see what is happening, it may be helpful to enable/disable
// the MeshRenderer so you can see the green wireframe Collider.
//
// Based upon this forum post:
//
// https://discussions.unity.com/t/physics-ignores-meshes-on-memory/1548808/2
//
// Testing in Unity 2018.4.19, the MeshRenderer immediately notices
// that the vertices change.
//
// The physics system seems to need being re-told what the (same)
// Mesh() is, or else just be enabled / disabled, which likely
// runs the same bake code.
//

public class ChangeColliderMeshVertices : MonoBehaviour
{
	Mesh mesh;

	[Header( "Make or we will AddComponent<T>() here...")]
	public MeshFilter _MeshFilter;
	public MeshCollider _Collider;

	IEnumerator Start()
	{
		if (!_MeshFilter)
		{
			_MeshFilter = gameObject.AddComponent<MeshFilter>();
			// we presume you also failed to add a MeshRenderer
			gameObject.AddComponent<MeshRenderer>();
		}

		if (!_Collider)
		{
			_Collider = gameObject.AddComponent<MeshCollider>();

		}


		mesh = new Mesh();

		Vector3[] verts = new Vector3[]
		{
			new Vector3( -2, -2, 0),
			new Vector3( -2, +2, 0),
			new Vector3( +2, +0, 0),
		};
		int[] tris = new int[]
		{
			0, 1, 2,
		};

		mesh.vertices = verts;
		mesh.triangles = tris;

		mesh.RecalculateBounds();
		mesh.RecalculateNormals();


		// give the mesh to our renderer and collider
		_MeshFilter.sharedMesh = mesh;
		_Collider.sharedMesh = mesh;


		// press PAUSE to resume
		yield return null;
		Debug.Log("PRESS PAUSE TO CONTINUE!");
		Debug.Break();
		yield return null;

		// shifts the above original verts to the right, only in the original array
		for (int i = 0; i < verts.Length; i++)
		{
			Vector3 v = verts[i];
			v.x += 4;
			verts[i] = v;
		}

		// NOTE: the above does not change anything as far as Unity is concerned.
		// Apparently the API has "taken" the verts into the mesh when I first did
		// it, which makes sense.

		// press PAUSE to resume
		yield return null;
		Debug.Log("PRESS PAUSE TO CONTINUE!");
		Debug.Break();
		yield return null;

		// send up dated position to the Mesh
		mesh.vertices = verts;

		// now leave it running go look at what changes in the scene

		// NOTE: the rendered Mesh moves, but the Collider's
		// mesh (green wireframe) does not, until you move on from here.
		//
		// This is consistent with the physics subsystem baking the verts.
		//

		// press PAUSE to resume
		yield return null;
		Debug.Log("PRESS PAUSE TO CONTINUE!");
		Debug.Break();
		yield return null;


		// I tested both these ways separately and both ways
		// work to update the Collider to the new Mesh verts:

		// one way to make the collider update
		_Collider.sharedMesh = mesh;

		// another way to make the physics notice:
		//_Collider.enabled = false;
		//_Collider.enabled = true;
	}
}
