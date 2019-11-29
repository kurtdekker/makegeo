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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MakeRoads
{
	public static GameObject Create( RoadConfiguration Config, IEnumerable<PositionAndHeading> PointProvider)
	{
		GameObject go = new GameObject ("MakeRoad1.Create();");

		MeshFilter mf = go.AddComponent<MeshFilter> ();
		Mesh mesh = new Mesh();

		List<Vector3> verts = new List<Vector3> ();
		List<int> tris = new List<int> ();
		List<Vector2> uvs = new List<Vector2> ();

		bool first = true;

		float HalfWidth = Config.Width / 2;

		foreach( var pt in PointProvider)
		{
			Vector3 position = pt.Position;
			float heading = pt.Heading;

// placeholder test/debugging to make sure we're getting good points
			Debug.Log( pt);

			var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
			cube.transform.position = position;
			cube.transform.rotation = Quaternion.Euler( 0, heading, 0);

			Vector3 left = position + Quaternion.Euler( 0, heading - 90, 0) * Vector3.forward * HalfWidth;
			Vector3 right = position + Quaternion.Euler( 0, heading + 90, 0) * Vector3.forward * HalfWidth;

			var capsule = GameObject.CreatePrimitive(PrimitiveType.Capsule);
			capsule.transform.position = left;
			capsule.transform.rotation = Quaternion.Euler( 0, heading, 0) * Quaternion.Euler( 90, 0, 0);

			var cylinder = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
			cylinder.transform.position = right;
			cylinder.transform.rotation = Quaternion.Euler( 0, heading, 0) * Quaternion.Euler( 90, 0, 0);;
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
