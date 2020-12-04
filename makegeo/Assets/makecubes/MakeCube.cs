/*
	The following license supersedes all notices in the source code.

	Copyright (c) 2020 Kurt Dekker/PLBM Games All rights reserved.

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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MakeCube
{
	public static GameObject Create( Vector3 dimensions, Material[] materials)
	{
		GameObject go = new GameObject ("MakeCube.Create();");

		MeshFilter mf = go.AddComponent<MeshFilter> ();
		Mesh mesh = new Mesh();

		Vector3[] verts = new Vector3[]
		{
			// back
			new Vector3( -1,  1, -1),
			new Vector3(  1,  1, -1),
			new Vector3(  1, -1, -1),
			new Vector3( -1, -1, -1),

			// right
			new Vector3(  1,  1, -1),
			new Vector3(  1,  1,  1),
			new Vector3(  1, -1,  1),
			new Vector3(  1, -1, -1),

			// front
			new Vector3(  1,  1,  1),
			new Vector3( -1,  1,  1),
			new Vector3( -1, -1,  1),
			new Vector3(  1, -1,  1),

			// left
			new Vector3( -1,  1,  1),
			new Vector3( -1,  1, -1),
			new Vector3( -1, -1, -1),
			new Vector3( -1, -1,  1),

			// top
			new Vector3( -1,  1,  1),
			new Vector3(  1,  1,  1),
			new Vector3(  1,  1, -1),
			new Vector3( -1,  1, -1),

			// bottom
			new Vector3( -1, -1, -1),
			new Vector3(  1, -1, -1),
			new Vector3(  1, -1,  1),
			new Vector3( -1, -1,  1),
		};

		for (int i = 0; i < verts.Length; i++)
		{
			var p = verts[i];

			p.x *= dimensions.x / 2;
			p.y *= dimensions.y / 2;
			p.z *= dimensions.z / 2;

			verts[i] = p;
		}

		mesh.vertices = verts;

		Vector2[] uvPattern = new Vector2[]
		{
			new Vector2( 0, 1),
			new Vector2( 1, 1),
			new Vector2( 1, 0),
			new Vector2( 0, 0),
		};

		int subMeshCount = 6;

		List<Vector2> uvs = new List<Vector2>();
		for (int i = 0; i < subMeshCount; i++)
		{
			for (int j = 0; j < uvPattern.Length; j++)
			{
				uvs.Add( uvPattern[j]);
			}
		}

		for (int i = 0; i < uvs.Count; i++)
		{
			var p = uvs[i];

			int face = i / 4;
			switch( face)
			{
			case 0 :
			case 2 :
				p.x *= dimensions.x;
				p.y *= dimensions.y;
				break;
			case 1 :
			case 3 :
				p.x *= dimensions.z;
				p.y *= dimensions.y;
				break;
			case 4 :
			case 5 :
				p.x *= dimensions.x;
				p.y *= dimensions.z;
				break;
			}

			uvs[i] = p;
		}

		mesh.uv = uvs.ToArray();

		int[] triPattern = new int[]
		{
			0, 1, 2,
			0, 2, 3,
		};
		mesh.subMeshCount = subMeshCount;
		for (int i = 0; i < subMeshCount; i++)
		{
			int offset = i * 4;
			List<int> tris = new List<int>();
			for (int j = 0; j < triPattern.Length; j++)
			{
				tris.Add( offset + triPattern[j]);
			}
			mesh.SetTriangles( tris, i);
		}

		mesh.RecalculateBounds ();
		mesh.RecalculateNormals ();

		mf.mesh = mesh;

		var mr = go.AddComponent<MeshRenderer> ();
		mr.materials = materials;

		return go;
	}
}
