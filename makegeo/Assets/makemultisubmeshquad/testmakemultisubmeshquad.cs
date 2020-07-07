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

public class testmakemultisubmeshquad : MonoBehaviour
{
	public Material mtl0;
	public Material mtl1;

	void Start()
	{
		var mesh = new Mesh();

		using (var vh = new VertexHelper())
		{
			UIVertex vtx = new UIVertex();

			float size = 4;

			// winding order: lower left, upper left, upper right, lower right

			vtx.position = new Vector3(-size, -size, 0);
			vtx.uv0 = new Vector2(0, 0);
			vh.AddVert(vtx);

			vtx.position = new Vector3(-size, size, 0);
			vtx.uv0 = new Vector2(0, 1);
			vh.AddVert(vtx);

			vtx.position = new Vector3(size, size, 0);
			vtx.uv0 = new Vector2(1, 1);
			vh.AddVert(vtx);

			vtx.position = new Vector3(size, -size, 0);
			vtx.uv0 = new Vector2(1, 0);
			vh.AddVert(vtx);

			vh.FillMesh(mesh);
		}

		// At this stage we have four verts in our mesh, no triangles

		// make room for 2 submeshes
		mesh.subMeshCount = 2;

		// define two triangles, each one in a submesh
		// NOTE: shared verts must by necessity share UVs. See note above.
		// If you want different UV coordinates at the shared verts, you
		// must re-add those verts again, with different UV coordintates.
		mesh.SetTriangles(
			new int[] {
				0, 1, 2,	// upper left triangle
			}, 0);
		mesh.SetTriangles(
			new int[] {
				0, 2, 3,	// lower right triangle
			}, 1);

		// handy
		mesh.RecalculateBounds();
		mesh.RecalculateNormals();

		// and now stick it in a MeshFilter, add a MeshRenderer, and inject
		// the two materials above dragged in from the inspector.
		GameObject go = new GameObject("MultiMeshQuad");
		var mf = go.AddComponent<MeshFilter>();
		mf.mesh = mesh;
		var mr = go.AddComponent<MeshRenderer>();
		mr.materials = new Material[] {
			mtl0,
			mtl1
		};

	}
}
