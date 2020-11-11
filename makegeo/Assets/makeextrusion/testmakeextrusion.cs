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

public class testmakeextrusion : MonoBehaviour
{
	public Material FrontMaterial;
	public Material BackMaterial;
	public Material SideMaterial;

	void Start ()
	{
		int n = 0;

		{
			List<Vector3> points = new List<Vector3>();

			// 0
			points.Add( new Vector3( -0.7f, -2));
			points.Add( new Vector3( -0.7f, 2));
			points.Add( new Vector3( 0, 2));
			points.Add( new Vector3( 0, 1));
			points.Add( new Vector3( 1, 2));
			// 5
			points.Add( new Vector3( 1.5f, 1.5f));
			points.Add( new Vector3( 0, 0));
			points.Add( new Vector3( 1.5f, -1.5f));
			points.Add( new Vector3( 1, -2));
			points.Add( new Vector3( 0, -1));
			// 10
			points.Add( new Vector3( 0, -2));

			List<int> triangles = new List<int>();
			triangles.Add3<int>( 0, 1, 2);
			triangles.Add3<int>( 0, 2, 3);
			triangles.Add3<int>( 0, 3, 6);
			triangles.Add3<int>( 6, 3, 4);
			triangles.Add3<int>( 6, 4, 5);
			triangles.Add3<int>( 9, 6, 7);
			triangles.Add3<int>( 9, 7, 8);
			triangles.Add3<int>( 0, 6, 9);
			triangles.Add3<int>( 0, 9, 10);

			var go = Extrude2DShape.ExtrudeLoop(
				points.ToArray(),
				1.0f,
				offset: -0.5f,
				triangles: triangles.ToArray());
			go.GetComponent<MeshRenderer> ().materials = new Material[]
			{
				FrontMaterial,
				BackMaterial,
				SideMaterial,
			};
			go.transform.position = Vector3.left * 1.4f;
			 SpinMeY.Attach(go);
			n++;
		}
	}
}
