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

public static class MakeUVCapsule
{
	// dimensions is radial size in X,Y,Z (Y is polar axis)
	//	NOTE: UVs will only look correct for uniform dimensional values (n,n,n)
	// sectors are how many longitudinal dividers (full equatorial)
	// meridians are how many latitudinal dividers (pole to pole)
	// equatorialHeight is the flat central part height (we call this the equator)
	public static GameObject Create( Vector3 dimensions, int sectors, int meridians, float equatorialHeight)
	{
		GameObject go = new GameObject ("MakeUVCapsule.Create();");

		MeshFilter mf = go.AddComponent<MeshFilter> ();
		Mesh mesh = new Mesh();

		List<Vector3> verts = new List<Vector3> ();
		List<int> tris = new List<int> ();
		List<Vector2> uvs = new List<Vector2> ();

		int equatorialMeridian = meridians / 2;

		for (int i = 0; i <= sectors; i++)
		{
			float longitude = (Mathf.PI * 2 * i) / sectors;

			float verticalOffset = -equatorialHeight / 2;

			// This sequences multiple times through on the equatorial meridian:
			//	- the last band of verts on the lower hemisphere
			//	- the bottom band of verts on the equator
			//	- the top band of verts on the equator
			//	- finally to continue to draw the first band of verts on the upper hemisphere

			const int extraMeridians = 4;

			int createEquator = extraMeridians - 1;

			// total V distance along the curved portion (upper and lower hemispheres combined)
			// see note about elliptical distance: this distorts with non-uniform dimensions.
			float curvedVDistance = dimensions.x * Mathf.PI;

			for (int j = 0; j <= meridians; j++)
			{
				bool emitTriangles = true;

				int effectiveJ = j;

				if (j == equatorialMeridian)
				{
					if (createEquator > 0)
					{
						// last (topmost) band of verts on lower hemisphere
						if (createEquator == 3)
						{
						}
						// bottom band of verts on the equator band
						if (createEquator == 2)
						{
							// don't want these zero-height polys as we transition
							// from the lower hemisphere to the equatorial band.
							emitTriangles = false;
						}
						// top band of verts on the equator band
						if (createEquator == 1)
						{
							verticalOffset = -verticalOffset;
						}

						createEquator--;

						j--;
					}
					else
					{
						// don't want these zero-height polys as we transition
						// from the equatorial band to the upper hemisphere
						emitTriangles = false;
					}
				}

				int n = verts.Count;

				float latitude = (Mathf.PI * effectiveJ) / meridians - Mathf.PI / 2;

				Vector3 sphericalPoint = new Vector3 (
					Mathf.Cos (longitude) *
						Mathf.Cos( latitude) * dimensions.x,
					Mathf.Sin( latitude) * dimensions.y + verticalOffset,
					Mathf.Sin (longitude) *
						Mathf.Cos( latitude) * dimensions.z);

				verts.Add (sphericalPoint);

				// WARNING: this is a cheap and cheerful linear map of
				// normalized local Y to the V texture coordinate
				float v = sphericalPoint.y / (dimensions.y * 2 + equatorialHeight) + 0.5f;

				Vector2 uvPoint = new Vector2 ((float)i / sectors, v);
				uvs.Add (uvPoint);

				if (emitTriangles)
				{
					if (i > 0 && j > 0)
					{
						int effectiveMeridians = meridians + extraMeridians;

						tris.Add (n);
						tris.Add (n - effectiveMeridians - 1);
						tris.Add (n - effectiveMeridians);

						tris.Add (n);
						tris.Add (n - 1);
						tris.Add (n - effectiveMeridians - 1);
					}
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
