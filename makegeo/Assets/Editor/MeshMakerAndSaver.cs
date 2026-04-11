
// @kurtdekker

// This stub is just to make mesh assets and save them
// to disk so you can grab them and use them in other projects.

#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;

class MeshMakerAndSaver
{
	static Mesh CreateSimpleSquare()
	{
		Mesh mesh = new Mesh();
		mesh.vertices = new Vector3[] {
			new Vector3( 0, 0, 0),
			new Vector3( 1, 0, 0),
			new Vector3( 1, 1, 0),
			new Vector3( 0, 1, 0),
		};
		mesh.uv = new Vector2[] {
			new Vector2( 0, 0),
			new Vector2( 1, 0),
			new Vector2( 1, 1),
			new Vector2( 0, 1),
		};
		mesh.triangles = new int[] {
			// front side
			0, 2, 1,
			0, 3, 2,
			// back side
			0, 1, 2,
			0, 2, 3,
		};
		mesh.RecalculateNormals();
		mesh.RecalculateBounds();

		return mesh;
	}

	[MenuItem("Assets/MakeGeo/Create Procedural Mesh")]
	static void Create () 
	{   
		string filePath = 
			EditorUtility.SaveFilePanelInProject("Save Procedural Mesh", "Procedural Mesh", "asset", "");

		if (filePath == "") return;

		GameObject go = null;

		Mesh mesh= CreateSimpleSquare();

		// enable if you want to produce a sphere to save:
		//go = MakeUVSphere.Create( Vector3.one / 2, 16, 16);
		//mesh = go.GetComponent<MeshFilter>().sharedMesh;

		AssetDatabase.CreateAsset( mesh, filePath);

		GameObject.DestroyImmediate(go);
	}
}

#endif
