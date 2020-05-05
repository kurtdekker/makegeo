using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MC2 : MonoBehaviour
{
	void Start ()
	{
		var filters = GetComponentsInChildren<MeshFilter>();	

		List<Material> materials = new List<Material>();

		List<CombineInstance> combines = new List<CombineInstance>();

		for (int i = 0; i < filters.Length; i++)
		{
			var filter = filters[i];

			var mesh = filter.sharedMesh;

			var rndrr = filter.GetComponent<MeshRenderer>();

			for (int j = 0; j < mesh.subMeshCount; j++)
			{
				var ci = new CombineInstance();

				ci.mesh = mesh;
				ci.subMeshIndex = j;

				combines.Add( ci);

				// I assume the end meshes are in linear order?
				materials.Add( rndrr.materials[j]);
			}
		}

		var go = new GameObject("combined");

		go.transform.position += Vector3.right * 3;

		var mf = go.AddComponent<MeshFilter>();
		mf.mesh = new Mesh();
		mf.mesh.CombineMeshes( combines.ToArray(), false);

		var mr = go.AddComponent<MeshRenderer>();
		mr.materials = materials.ToArray();

		transform.position += Vector3.right * -3;
	}
}
