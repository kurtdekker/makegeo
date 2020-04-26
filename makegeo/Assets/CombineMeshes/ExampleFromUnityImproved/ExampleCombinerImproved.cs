// code originally from this Unity3D website:
// https://docs.unity3d.com/ScriptReference/Mesh.CombineMeshes.html
//
// improved by Kurt Dekker @kurtdekker
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
		CombineInstance[] combine = new CombineInstance[meshFilters.Length];

		List<Material> materials = new List<Material>();

		for (int i = 0; i < meshFilters.Length; i++)
		{
			// we gotta ignore ourselves or our count would be off!
			if (meshFilters[i] == GetComponent<MeshFilter>())
			{
				continue;
			}

			combine[i].mesh = meshFilters[i].sharedMesh;
			combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
			meshFilters[i].gameObject.SetActive(false);

			// tally up the materials
			var mr = meshFilters[i].GetComponent<MeshRenderer>();
			for (int j = 0; j < mr.materials.Length; j++)
			{
				materials.Add( mr.materials[j]);
			}
		}
		transform.GetComponent<MeshFilter>().mesh = new Mesh();
		transform.GetComponent<MeshFilter>().mesh.CombineMeshes(combine, false);
		transform.gameObject.SetActive(true);

		// inject the original materials
		gameObject.GetComponent<MeshRenderer>().materials = materials.ToArray();
	}
}
