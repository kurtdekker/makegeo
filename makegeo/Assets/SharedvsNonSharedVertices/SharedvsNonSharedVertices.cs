using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SharedvsNonSharedVertices : MonoBehaviour
{
	public Material material;

	void MakeTwoObjects()
	{
		Vector3[] verts = new Vector3[] {
			// 0
			new Vector3( -7.0f,  0.0f,  0.0f),
			new Vector3( -4.0f,  2.0f, -2.0f),	// shared
			new Vector3( -4.0f,  2.0f,  2.0f),	// shared
			new Vector3( -1.0f,  0.0f,  0.0f),

			// 4
			new Vector3(  1.0f,  0.0f,  0.0f),
			new Vector3(  4.0f,  2.0f, -2.0f),
			new Vector3(  4.0f,  2.0f,  2.0f),
			// 7
			new Vector3(  4.0f,  2.0f, -2.0f),	// dupe so we don't share
			new Vector3(  4.0f,  2.0f,  2.0f),	// dupe so we don't share
			new Vector3(  7.0f,  0.0f,  0.0f),
		};

		int[] tris = new int[]
		{
			0, 2, 1,
			1, 2, 3,

			4, 6, 5,
			7, 8, 9,
		};

		var go = new GameObject( "CombinedExample");
		var mf = go.AddComponent<MeshFilter>();

		var mesh = new Mesh();
		mesh.vertices = verts;
		mesh.triangles = tris;

		mesh.RecalculateBounds();
		mesh.RecalculateNormals();
		mf.mesh = mesh;

		var mr = go.AddComponent<MeshRenderer>();
		mr.material = material;

		go.AddComponent<SetUVToWorld>();		// lazy
	}
	
	void Start()
	{
		MakeTwoObjects();
	}
}
