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

// @kurtdekker
// You can slap this on a single piece of geometry or on an
// entire hierarchy. It will reassign world-space UVs to match
// the three coordinate axes. The purpose is just to be able
// to make evenly-textured placeholder 3D graphics.

public class SetUVToWorld : MonoBehaviour
{
	// set this if you are putting it on a parent object, otherwise
	// this script operates only on the current GameObject.
	public	bool		DriveToAllChildren;

	public	Material	MaterialToUse;

	public	bool		PreserveColor;

	public	bool		IncludeRotation;

	void Reset()
	{
		IncludeRotation = true;
		MaterialToUse = null;
	}

	public static void AddToAllMeshRenderersWithMeshFilters(
		GameObject go,
		bool _PreserveColor = false,
		Material _MaterialToUse = null)
	{
		MeshRenderer[] rndrrs = go.GetComponentsInChildren<MeshRenderer> ();
		foreach( var mr in rndrrs)
		{
			MeshFilter mf = mr.GetComponent<MeshFilter> ();
			if (mf != null)
			{
				var uvsetter = mr.gameObject.AddComponent<SetUVToWorld> ();
				uvsetter.PreserveColor = _PreserveColor;
				uvsetter.MaterialToUse = _MaterialToUse;
			}
		}
	}

	void Start()
	{
		// be careful: we are intentionally adding instances of this
		// script that don't have the DriveToAllChildren bit set, in
		// order to benefit from setting things once.
		if (DriveToAllChildren)
		{
			AddToAllMeshRenderersWithMeshFilters(
				gameObject, PreserveColor, MaterialToUse);
			return;
		}

		MeshFilter mf = GetComponent<MeshFilter> ();
		if (mf)
		{
			Vector2[] uvs = mf.mesh.uv;
			Vector3[] verts = mf.mesh.vertices;
			int[] tris = mf.mesh.triangles;

			if (uvs.Length != verts.Length)
			{
				uvs = new Vector2[ verts.Length];
			}

			for (int i = 0; i < verts.Length; i++)
			{
				verts [i] = transform.TransformPoint (verts [i]);
				if (!IncludeRotation)
				{
					verts [i] = Quaternion.Inverse (transform.rotation) * verts [i];
				}
			}

			for ( int i = 0; i < tris.Length; i += 3)
			{
				Vector3 norm = Vector3.Cross (
					verts [tris [i + 1]] - verts [tris [i + 0]],
					verts [tris [i + 1]] - verts [tris [i + 2]]).normalized;

				float dotX = Mathf.Abs (Vector3.Dot (norm, Vector3.right));
				float dotY = Mathf.Abs (Vector3.Dot (norm, Vector3.up));
				float dotZ = Mathf.Abs (Vector3.Dot (norm, Vector3.forward));

				if (dotX > dotY && dotX > dotZ)
				{
					for ( int j = 0; j < 3; j++)
					{
						uvs [tris[i + j]] = new Vector2 (verts [tris[i + j]].z, verts [tris[i + j]].y);
					}
				}
				else
				{
					if (dotY > dotX && dotY > dotZ)
					{
						for ( int j = 0; j < 3; j++)
						{
							uvs [tris[i + j]] = new Vector2 (verts [tris[i + j]].x, verts [tris[i + j]].z);
						}
					}
					else
					{
						for ( int j = 0; j < 3; j++)
						{
							uvs [tris[i + j]] = new Vector2 (verts [tris[i + j]].x, verts [tris[i + j]].y);
						}
					}
				}
			}
			mf.mesh.uv = uvs;

			if (MaterialToUse)
			{
				Dictionary<Color, Material> ColorDictToSaveDrawcalls = new Dictionary<Color, Material>();

				Renderer rndrr = GetComponent<Renderer>();
				if (rndrr)
				{
					Material[] allNewMaterials = new Material[ rndrr.materials.Length];

					for (int i = 0; i < allNewMaterials.Length; i++)
					{
						Color preservedColor = Color.white;

						Material originalMaterial = rndrr.materials[i];

						var instancedMaterial = MaterialToUse;

						if (originalMaterial)
						{
							if (PreserveColor)
							{
								preservedColor = originalMaterial.color;

								if (ColorDictToSaveDrawcalls.ContainsKey( preservedColor))
								{
									instancedMaterial = ColorDictToSaveDrawcalls[preservedColor];
								}
								else
								{
									instancedMaterial = new Material( MaterialToUse);
									instancedMaterial.color = preservedColor;
									ColorDictToSaveDrawcalls[preservedColor] = instancedMaterial;
								}
							}
						}

						allNewMaterials[i] = instancedMaterial;
					}

					rndrr.materials = allNewMaterials;
				}
			}
		}
		else
		{
			Debug.LogError (
				GetType() + ": there is no MeshFilter on GameObject '" + name + "'!");
		}
	}
}
