/*
	The following license supersedes all notices in the source code.

	Copyright (c) 2019 Kurt Dekker/PLBM Games All rights reserved.

	http://www.twitter.com/kurtdekker

	Redistribution and use in source and binary forms, with or without
	modification, are permitted provided that the following conditions are
	met:

	Redistributions of source code must retain the above copyright notice,
	this list of conditions and the following disclaimer.

	Redistributions in binary form must reproduce the above copyright
	notice, this list of conditions and the following disclaimer in the
	documentation and/or other materials provided with the distribution.

	Neither the name of the Kurt Dekker/PLBM Games nor the names of its
	contributors may be used to endorse or promote products derived from
	this software without specific prior written permission.

	THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS
	IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED
	TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A
	PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT
	HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL,
	SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED
	TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR
	PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF
	LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING
	NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
	SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/


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

	[MenuItem("Assets/Create Procedural Mesh")]
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
