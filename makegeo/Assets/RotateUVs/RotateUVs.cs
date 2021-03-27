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

public class RotateUVs : MonoBehaviour
{
	public Material mtl;

	[Header( "In UV space, the center around which we rotate.")]
	public Vector2 Center = new Vector2( 0.5f, 0.5f);

	[Header( "Degrees per second of rotation.")]
	public float RateOfRotation = 50;

	GameObject quad;
	MeshFilter filter;
	Mesh mesh;
	Vector2[] originalUVs;
	Vector2[] rotatedUVs;		// used again and again as "scratch"

	float angle;

	void Start ()
	{
		quad = GameObject.CreatePrimitive( PrimitiveType.Quad);

		filter = quad.GetComponent<MeshFilter>();
		mesh = filter.mesh;
		originalUVs = new Vector2[ mesh.uv.Length];
		System.Array.Copy( mesh.uv, originalUVs, originalUVs.Length);

		rotatedUVs = new Vector2[ originalUVs.Length];

		quad.GetComponent<Renderer>().material = mtl;
	}

	void Update()
	{
		angle += RateOfRotation * Time.deltaTime;

		// rotate around Z, which is orthogonal to Vector2's X/Y
		Quaternion rotation = Quaternion.Euler( 0, 0, angle);

		for (int i = 0; i < originalUVs.Length; i++)
		{
			var uv = originalUVs[i];

			// back out the center
			uv -= Center;

			// rotate (now effectively around (0,0))
			uv = rotation * uv;

			// put the center back in
			uv += Center;

			rotatedUVs[i] = uv;
		}

		// now the rotatedUVs are all created, time to
		// put them back into the mesh
		mesh.uv = rotatedUVs;
		filter.mesh = mesh;
	}
}
