// code originally from this Unity3D website:
// https://docs.unity3d.com/ScriptReference/Mesh.CombineMeshes.html
//
// Improved by Kurt Dekker @kurtdekker
// 
// Usage:
// - make a blank object
// - put this script on it
// - parent all other mesh GameObjects below this one
// - run
// - voila!

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class ExampleCombinerImproved : MonoBehaviour
{
	void Start()
	{
		MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();
		List<CombineInstance> combines = new List<CombineInstance>();

		// KurtFixed: handle materials... I mean, they're kind of important!
		List<Material> materials = new List<Material>();

		for (int i = 0; i < meshFilters.Length; i++)
		{
			// KurtFixed: we gotta ignore ourselves or our count would be off!
			if (meshFilters[i] == GetComponent<MeshFilter>())
			{
				continue;
			}

			// KurtFixed: tally up the materials, since each mesh could have multiple
			var mr = meshFilters[i].GetComponent<MeshRenderer>();
			for (int j = 0; j < mr.materials.Length; j++)
			{
				var combine = new CombineInstance();

				combine.mesh = meshFilters[i].sharedMesh;
				combine.subMeshIndex = j;

				combine.transform = meshFilters[i].transform.localToWorldMatrix;
				meshFilters[i].gameObject.SetActive(false);

				combines.Add(combine);

				materials.Add( mr.materials[j]);
			}
		}
		transform.GetComponent<MeshFilter>().mesh = new Mesh();
		transform.GetComponent<MeshFilter>().mesh.CombineMeshes(combines.ToArray(), false);
		transform.gameObject.SetActive(true);

		// KurtFixed: inject the original materials
		gameObject.GetComponent<MeshRenderer>().materials = materials.ToArray();
	}
}
