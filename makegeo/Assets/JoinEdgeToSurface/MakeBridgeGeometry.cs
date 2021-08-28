/*
	The following license supersedes all notices in the source code.

	Copyright (c) 2021 Kurt Dekker/PLBM Games All rights reserved.

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

public static class MakeBridgeGeometry
{
	public static GameObject Create(
//		Vector3 position,
		IVertexProvider VertEdge,
		Vector3 upDirection,
		Vector3 bridgeDirection,
		Material material)
	{
		var go = new GameObject( "MakeBridgeGeometry.Create();");

		var mf = go.AddComponent<MeshFilter>();
		var mesh = mf.mesh;

		List<Vector3> newVerts = new List<Vector3>();
		List<int> newTris = new List<int>();
		List<Vector2> newUVs = new List<Vector2>();

		float bridgeDistance = bridgeDirection.magnitude;

		// make a strip going down
		Vector3 prevVert = Vector3.zero;

		Vector2 downUV = new Vector2( 0, 1);

		Vector2 leftUV = new Vector2( 0, 0);
		Vector2 rightUV = new Vector2( 1, 0);

		// compute where we should go
		Vector3 location = go.transform.position;
		{
			var centroid = Vector3.zero;
			int vertCount = 0;
			foreach( var v in VertEdge.GetVertices())
			{
				centroid += v;
				vertCount++;
			}
			if (vertCount > 0)
			{
				centroid /= vertCount;
			}

			// stick this bridge geometry origin at the centroid of
			// the edges plus half the bridge distance
			location = centroid + bridgeDirection / 2;;

			go.transform.position = location;
		}

		foreach( var v in VertEdge.GetVertices())
		{
			var v2 = v + bridgeDirection;

			// raycast this part down to match the first collider we hit
			var ray = new Ray( origin: v2, direction: -upDirection);
			RaycastHit hit;
			if (Physics.Raycast( ray: ray, hitInfo: out hit, maxDistance: 100))
			{
				v2 = hit.point;
			}

			int n = newVerts.Count;

			float vDistance = (v - prevVert).magnitude;

			float step = vDistance / bridgeDistance;

			newVerts.Add( v);
			newVerts.Add( v2);

			newUVs.Add( leftUV);
			newUVs.Add( rightUV);

			leftUV += downUV * step;
			rightUV += downUV * step;

			if (n > 0)
			{
				newTris.Add( n - 2);
				newTris.Add( n);
				newTris.Add( n - 1);

				newTris.Add( n);
				newTris.Add( n + 1);
				newTris.Add( n - 1);
			}

			prevVert = v;
		}

		// back out our local position from these points,
		// since points are in local space
		for( int i = 0; i < newVerts.Count; i++)
		{
			newVerts[i] -= location;
		}

		mesh.vertices = newVerts.ToArray();
		mesh.triangles = newTris.ToArray();
		mesh.uv = newUVs.ToArray();

		mesh.RecalculateBounds();
		mesh.RecalculateNormals();

		mf.mesh = mesh;

		var mr = go.AddComponent<MeshRenderer>();
		mr.material = material;

		return go;
	}
}
