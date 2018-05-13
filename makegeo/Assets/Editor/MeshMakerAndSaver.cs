
// This stub is just to make mesh assets and save them
// to disk so you can grab them and use them in other projects.

#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;

class MeshMakerAndSaver
{
	static Mesh CreateSimpleSquare()
	{
		Mesh m = new Mesh();
		m.vertices = new Vector3[] {
			new Vector3( 0, 0, 0),
			new Vector3( 1, 0, 0),
			new Vector3( 1, 1, 0),
			new Vector3( 0, 1, 0),
		};
		m.triangles = new int[] {
			0, 2, 1,
			0, 3, 2,
		};
		m.RecalculateNormals();
		m.RecalculateBounds();

		return m;
	}

	[MenuItem("Assets/Create Procedural Mesh")]
	static void Create () 
	{   
		string filePath = 
			EditorUtility.SaveFilePanelInProject("Save Procedural Mesh", "Procedural Mesh", "asset", "");

		if (filePath == "") return;

		GameObject go = null;
		Mesh m = null;

		m = CreateSimpleSquare();

		go = MakeUVSphere.Create( Vector3.one, 12, 12);
		m = go.GetComponent<MeshFilter>().sharedMesh;

		AssetDatabase.CreateAsset( m, filePath);

		GameObject.DestroyImmediate(go);
	}
}

#endif
