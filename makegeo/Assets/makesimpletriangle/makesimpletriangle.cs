/*
	The following license supersedes all notices in the source code.

	Copyright (c) 2020 Kurt Dekker/PLBM Games All rights reserved.
	
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

// Ultra-simple example of producing geometry in Unity3D.

public class makesimpletriangle : MonoBehaviour
{
	public Material materialToUse;

	GameObject MakeTriangleByHand( bool AddBackface = false)
	{
		// place to hold temporary stuff as we build our things
		List<Vector3> verts = new List<Vector3>();
		List<Vector2> uvs = new List<Vector2>();
		List<int> tris = new List<int>();

		// lower left corner
		verts.Add( new Vector3( -1, 0, 0));
		uvs.Add( new Vector2( 0.0f, 0.0f));
		// top center point
		verts.Add( new Vector3( 0, 2, 0));
		uvs.Add( new Vector2( 0.5f, 1.0f));
		// lower right point
		verts.Add( new Vector3( 1, 0, 0));
		uvs.Add( new Vector2( 1.0f, 0.0f));
			
		// windings must go clockwise to face back at you (left hand rule)
		tris.Add( 0);
		tris.Add( 1);
		tris.Add( 2);

		// if you like, add a backface by winding the other way
		if (AddBackface)
		{
			tris.Add( 2);
			tris.Add( 1);
			tris.Add( 0);
		}

		// transfer temporary variables into mesh
		Mesh mesh = new Mesh();

		mesh.vertices = verts.ToArray();
		mesh.uv = uvs.ToArray();
		mesh.triangles = tris.ToArray();		// implies only one submesh; all one big submesh

		// you can also supply these yourself in a separate Vector3 array...
		mesh.RecalculateBounds();
		mesh.RecalculateNormals();
		mesh.RecalculateTangents();

		// at this stage the mesh is complete

		// now make the Unity GameObject and all
		var go = new GameObject( "Made by " + GetType() + ".MakeTriangleByHand();");

		var mf = go.AddComponent<MeshFilter>();
		mf.mesh = mesh;

		var mr = go.AddComponent<MeshRenderer>();
		mr.material = materialToUse;

		// and back it goes to the caller
		return go;
	}

	IEnumerator Start ()
	{
		// doing it all by hand
		var t1 = MakeTriangleByHand();
		// and one with a backface
		var t2 = MakeTriangleByHand( AddBackface: true);

		// slide them to the left so we can make something else
		for (float distance = 0; distance >= -2; distance -= Time.deltaTime)
		{
			t1.transform.Translate( new Vector3( -1, -0.7f, 0) * Time.deltaTime);
			t2.transform.Translate( new Vector3( -1,  0.7f, 0) * Time.deltaTime);
			yield return null;
		}

		// tack on a spin-in-place script
		SpinMeY.Attach( t1, RateOfSpin: 150);
		SpinMeY.Attach( t2, RateOfSpin: 90);

		// coming soon: example using UnityEngine.UI.VertexHelper() class
	}
}
