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

public static class Extrude2DShape
{
	// clockwisePoints: points going clockwise around the shape top, in X,Y plane.
	// thickness: how far to extrude in the +Z plane
	// offset: how far up (negative) or down (positive) down the +Z
	// triangles: optional how to wind the top triangles (otherwise they won't be)
	public static GameObject ExtrudeLoop(
		Vector3[] clockwisePoints,
		float thickness,
		float offset = 0,
		int[] triangles = null)
	{
		GameObject go = new GameObject ("Extrude2DShape.ExtrudeLoop();");

		MeshFilter mf = go.AddComponent<MeshFilter> ();
		Mesh mesh = new Mesh();

		UIVertex vtx = new UIVertex();

		var extrudeDirection = Vector3.forward;

		List<int> FrontTris = new List<int>();
		List<int> BackTris = new List<int>();
		List<int> SideTris = new List<int>();

		using (var vh = new VertexHelper())
		{
			if (offset != 0)
			{
				for (int i = 0; i < clockwisePoints.Length; i++)
				{
					clockwisePoints[i] += extrudeDirection * offset;
				}
			}

			if (triangles != null)
			{
				//front
				{
					int n = vh.currentVertCount;
					for (int i = 0; i < clockwisePoints.Length; i++)
					{
						vtx.position = clockwisePoints[i];
						vtx.uv0 = vtx.position;
						vh.AddVert( vtx);
					}
					for (int i = 0; i < triangles.Length; i += 3)
					{
						FrontTris.Add3(
							n + triangles[i + 0],
							n + triangles[i + 1],
							n + triangles[i + 2]);
					}
				}

				// back (wound in reverse)
				{
					int n = vh.currentVertCount;
					for (int i = 0; i < clockwisePoints.Length; i++)
					{
						vtx.position = clockwisePoints[i] + extrudeDirection * thickness;
						vtx.uv0 = vtx.position;
						vh.AddVert( vtx);
					}
					for (int i = 0; i < triangles.Length; i += 3)
					{
						BackTris.Add3(
							n + triangles[i + 0],
							n + triangles[i + 2],
							n + triangles[i + 1]);
					}
				}

			}

			Vector3 prevTop = Vector3.zero;
			Vector3 prevBottom = Vector3.zero;

			float masterU = 0;
			float prevU = 0;

			for (int i = 0; i <= clockwisePoints.Length; i++)
			{
				Vector3 pointTop = clockwisePoints[i % clockwisePoints.Length];

				Vector3 pointBottom = pointTop + extrudeDirection * thickness;

				if (i > 0)
				{
					float deltaU =  Vector3.Distance( pointTop, prevTop);
					masterU += deltaU;

					int n = vh.currentVertCount;

					vtx.position = prevTop;
					vtx.uv0 = new Vector2( prevU, 0);
					vh.AddVert( vtx);

					vtx.position = prevBottom;
					vtx.uv0 = new Vector2( prevU, thickness);
					vh.AddVert( vtx);

					vtx.position = pointBottom;
					vtx.uv0 = new Vector2( masterU, thickness);
					vh.AddVert( vtx);

					vtx.position = pointTop;
					vtx.uv0 = new Vector2( masterU, 0);
					vh.AddVert( vtx);

					SideTris.Add3( n, n + 1, n + 2);
					SideTris.Add3( n, n + 2, n + 3);
				}

				prevTop = pointTop;
				prevBottom = pointBottom;
				prevU = masterU;
			}

			vh.FillMesh( mesh);

			mesh.subMeshCount = 3;
			mesh.SetTriangles( FrontTris, 0);
			mesh.SetTriangles( BackTris, 1);
			mesh.SetTriangles( SideTris, 2);
		}

		mesh.RecalculateBounds ();
		mesh.RecalculateNormals ();

		mf.mesh = mesh;

		go.AddComponent<MeshRenderer> ();

		return go;
	}
}
