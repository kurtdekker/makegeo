/*
	The following license supersedes all notices in the source code.

	Copyright (c) 2022 Kurt Dekker/PLBM Games All rights reserved.

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

public static class MakeUVCylinder
{
	// cylinder is +Y axis oriented...
	// dimensions is x/z for ellipse, y is height of cylinder
	// sectors are how many longitudinal dividers (full equatorial)
	// meridians are how many latitudinal dividers (pole to pole)
	public static GameObject Create( Vector3 dimensions, int sectors, int meridians)
	{
		GameObject go = new GameObject ("MakeUVCylinder.Create();");

		MeshFilter mf = go.AddComponent<MeshFilter> ();
		Mesh mesh = new Mesh();

		List<Vector3> verts = new List<Vector3> ();
		List<int> tris = new List<int> ();
		List<Vector2> uvs = new List<Vector2> ();

		for (int i = 0; i <= sectors; i++)
		{
			float longitude = (Mathf.PI * 2 * i) / sectors;

			float x = Mathf.Cos (longitude) * dimensions.x;
			float z = Mathf.Sin (longitude) * dimensions.z;

			for (int j = 0; j <= meridians; j++)
			{
				int n = verts.Count;

				float y = Mathf.Lerp( -dimensions.y, dimensions.y, (float)j / meridians);

				Vector3 cylindricalPoint = new Vector3 ( x, y, z);

				verts.Add (cylindricalPoint);

				Vector2 uvPoint = new Vector2 ((float)i / sectors, (float)j / meridians);
				uvs.Add (uvPoint);

				if (i > 0 && j > 0)
				{
//					tris.Add (n);
//					tris.Add (n - meridians);
//					tris.Add (n - meridians - 1);

					tris.Add (n);
					tris.Add (n - 1);
					tris.Add (n - meridians - 1);

					tris.Add (n - 1);
					tris.Add (n - meridians - 2);
					tris.Add (n - meridians - 1);
				}
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
