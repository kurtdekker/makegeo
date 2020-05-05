using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshCombiner : MonoBehaviour
{
	void Start()
	{
		MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();
		MeshRenderer[] meshRenderers = GetComponentsInChildren<MeshRenderer>();
		CombineInstance[] combine = new CombineInstance[meshFilters.Length];
		//Material[] SharedMats = meshRenderers[1].sharedMaterials;
		Material MainMaterial = meshRenderers[1].materials[0];
		Material[] SubMaterial = new Material[meshRenderers[1].materials.Length-1];
		List<CombineInstance> combine2 = new List<CombineInstance>();

		// gotta get this big enough
		SubMaterial = new Material[2];

		for (int i = 0; i < meshFilters.Length; i++)
		{
			Mesh mShared = meshFilters[i].sharedMesh;

			combine[i].mesh = mShared;
			combine[i].transform = meshFilters[i].transform.localToWorldMatrix;

			if (mShared.subMeshCount > 1)
			{
				// combine submeshes
				for (int j = 0; j < mShared.subMeshCount; j++)
				{
					if (j < mShared.subMeshCount - 1)
					{
						SubMaterial[i] = meshRenderers[1].materials[0];
					}
					CombineInstance ci = new CombineInstance();

					ci.mesh = mShared;
					ci.subMeshIndex = j;
					ci.transform = meshFilters[i].transform.localToWorldMatrix;

					combine2.Add(ci);
				}
			}

			meshFilters[i].gameObject.SetActive(false);
		}
		GameObject go = new GameObject(gameObject.name);
		go.AddComponent<MeshFilter>();
		go.AddComponent<MeshRenderer>();

		var goFilter = go.GetComponent<MeshFilter>();
		var goRenderer = go.GetComponent<MeshRenderer>();

		go.SetActive(false);
		goFilter.mesh = new Mesh();
		goFilter.mesh.CombineMeshes(combine);
		go.SetActive(true);

		//goRenderer.materials = Mats;
		goRenderer.material = MainMaterial;

		if (SubMaterial.Length >= 1)
		{
			GameObject go2 = new GameObject(gameObject.name + "Sub");
			go2.AddComponent<MeshFilter>();
			go2.AddComponent<MeshRenderer>();

			var goFilter2 = go2.GetComponent<MeshFilter>();
			var goRenderer2 = go2.GetComponent<MeshRenderer>();

			go2.SetActive(false);
			goFilter2.mesh = new Mesh();
			goFilter2.mesh.CombineMeshes(combine2.ToArray(), false);
			go2.SetActive(true);

			//goRenderer.materials = Mats;
			goRenderer2.materials = SubMaterial;
		}

		Destroy(gameObject);
	}
}