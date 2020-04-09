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
using UnityEngine.UI;

public static class MakeCollider2D
{
	// Supply the verts in go-around-the-edge order. Unity winds clockwise.
	// It's up to you to supply at least three points.
	// No idea what Physics2D might do with whacky interleaved colliders... try it!!
	public static GameObject Create( Vector2[] SourcePoints, bool DoubleSided = false)
	{
		Vector2[] Points = new Vector2[ SourcePoints.Length];
		System.Array.Copy( SourcePoints, Points, SourcePoints.Length);

		GameObject go = new GameObject("MakeCollider2D");

		Mesh mesh = new Mesh();

		Vector2 centroid = Vector3.zero;

		for (int i = 0; i < Points.Length; i++)
		{
			centroid += Points[i];
		}

		centroid /= Points.Length;

		for (int i = 0; i < Points.Length; i++)
		{
			Points[i] -= centroid;
		}

		go.transform.position = centroid;

		int numSides = DoubleSided ? 2 : 1;

		using (var vh = new VertexHelper())
		{
			int vertCounter = 0;

			for (int sideNo = 0; sideNo < numSides; sideNo++)
			{
				for (int i = 0; i < Points.Length; i++)
				{
					// second side just considers the points in reverse
					int iMapped = (sideNo == 0) ? i : ((Points.Length - 1) - i);

					Vector2 vert = Points[iMapped];

					UIVertex vtx = new UIVertex();

					vtx.position =  new Vector3( vert.x, vert.y, 0);

					vtx.uv0 = new Vector2( vert.x, vert.y);

					vh.AddVert(vtx);

					if (((i > 1) && (i < Points.Length)) ||
						(i > Points.Length + 1))
					{
						// topology is a fan
						if (sideNo == 0)
						{
							vh.AddTriangle( 0, vertCounter - 1, vertCounter);
						}
						if (sideNo == 1)
						{
							vh.AddTriangle( Points.Length, vertCounter - 1, vertCounter);
						}
					}

					vertCounter++;
				}
			}

			vh.FillMesh(mesh);
		}

		var pc2d = go.AddComponent<PolygonCollider2D>();
		pc2d.points = Points;

		mesh.RecalculateBounds();
		mesh.RecalculateNormals();

		MeshFilter mf = go.AddComponent<MeshFilter>();
		mf.mesh = mesh;

		go.AddComponent<MeshRenderer>();

		return go;
	}
}
