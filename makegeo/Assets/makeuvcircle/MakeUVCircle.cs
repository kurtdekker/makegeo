/*
    The following license supersedes all notices in the source code.
*/

/*
	Copyright (c) 2018 Kurt Dekker/PLBM Games All rights reserved.

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

public class MakeUVCircle
{
	public static GameObject Create( Vector3 dimensions, AxisDirection direction, int sectors)
	{
		GameObject go = new GameObject ("MakeUVCircle.Create();");

		MeshFilter mf = go.AddComponent<MeshFilter> ();
		Mesh mesh = new Mesh();

		List<Vector3> verts = new List<Vector3> ();
		List<int> tris = new List<int> ();
		List<Vector2> uvs = new List<Vector2> ();

		// start by adding the center
		verts.Add( Vector3.zero);
		uvs.Add (Vector2.one / 2);
		bool flipTris = true;

		for (int i = 0; i <= sectors; i++)
		{
			int n = verts.Count;

			float angle = (Mathf.PI * 2 * i) / sectors;

			Vector3 sphericalPoint = Vector3.zero;
			Vector2 uvPoint = Vector2.zero;

			switch (direction)
			{
			case AxisDirection.ZPLUS:		// facing away from the default Unity camera
				flipTris = false;
				goto case AxisDirection.ZMINUS;
			case AxisDirection.ZMINUS:		// facing back at the default Unity camera
				sphericalPoint = new Vector3 (
					Mathf.Cos (angle) * dimensions.x,
					Mathf.Sin (angle) * dimensions.y);
				uvPoint = new Vector2 (
					(1 + Mathf.Cos (angle)) / 2,
					(1 + Mathf.Sin (angle)) / 2);
				break;
			default :
				throw new System.NotImplementedException (
					"Sorry, I haven't gotten to these axes yet!");
			}

			verts.Add (sphericalPoint);

			uvs.Add (uvPoint);

			if (i > 0)
			{
				tris.Add (flipTris ? n : n - 1);
				tris.Add (flipTris ? n - 1 : n);
				tris.Add (0);	// center
			}
		}

		mesh.vertices = verts.ToArray ();
		mesh.triangles = tris.ToArray ();
		mesh.uv = uvs.ToArray ();

		mesh.RecalculateBounds ();
		mesh.RecalculateNormals ();

		mf.mesh = mesh;

		go.AddComponent<MeshRenderer> ();

		return go;
	}
}
