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

using UnityEngine;
using System.Collections;

// This was created in response to this Unity3D forum post:
// http://forum.unity3d.com/threads/terrain-cutout.291836/
//
// BE SURE YOU BACK UP YOUR TERRAIN DATA FIRST! USE SOURCE CONTROL!
// I will not be responsible for damage to your terrainData.
//
// To use, place this script on the Terrain itself and make sure
// all "buildings" are parented to the terrain.
//
// There are MANY embedded assumptions here. Some of them are:
//	- All cut/raise objects should be parented below the terrain
//	- All objects both cut and raise (see boolean arguments).
//	- The only object-level scaling is done at the leaf objects of the tree

public class TerrainCutter : MonoBehaviour
{
	void ModifyTerrain( MeshFilter mf, bool raise, bool lower)
	{
		Terrain ter = gameObject.GetComponent<Terrain>();
		TerrainData td = ter.terrainData;

		Bounds bounds = mf.mesh.bounds;

		Vector3 scaledSize = new Vector3 (
			bounds.size.x * mf.transform.localScale.x,
			bounds.size.y * mf.transform.localScale.y,
			bounds.size.z * mf.transform.localScale.z) * 0.5f;
		Quaternion rot = mf.transform.rotation;

		Vector3[] bottoms = new Vector3[4];

		int n = 0;
		for (int i = -1; i <= 1; i += 2)
		{
			for (int j = -1; j <= 1; j += 2)
			{
				for (int k = -1; k <= 1; k += 2)
				{
					Vector3 pos = mf.transform.localPosition +
						bounds.center +
						rot * new Vector3(
							scaledSize.x * i,
							scaledSize.y * j,
							scaledSize.z * k);
					// only record the bottom corners
					if (j == -1)
					{
						bottoms[n++] = pos;
					}

					// enable if you want to see the corners we calculated as spheres
					GameObject marker = GameObject.CreatePrimitive( PrimitiveType.Sphere);
					marker.transform.position = pos + transform.position;
				}
			}
		}

		// take the object's terrain-relative height and scale it by the terrain's size.
		float scaledUndersideHeight = bottoms[0].y / td.size.y;

		float[,] heightField = td.GetHeights( 0, 0, td.heightmapResolution, td.heightmapResolution);

		// Now we traverse our bottom corners (in object bounds space) and set
		// the affected heightmaps to the new scaledUndersideHeight

		// This calculates number of steps and is naively overkill, as one only needs
		// to make sure that you don't skip any interim heightmap datapoints.
		int uSteps = (int)Vector3.Distance( bottoms[0], bottoms[1]);
		int vSteps = (int)Vector3.Distance( bottoms[0], bottoms[2]);

		// You need to hit points beyond the bounds or else your building
		// will be perched upon a too-narrow terrain pedestal, especially
		// as LOD kicks in at further viewing distances.
		const int extraCellsBeyond = 5;

		for (int v = -extraCellsBeyond; v <= vSteps + extraCellsBeyond; v++)
		{
			for (int u = -extraCellsBeyond; u <= uSteps + extraCellsBeyond; u++)
			{
				Vector3 position = bottoms[0] +
					((bottoms[1] - bottoms[0]) * u) / uSteps +
					((bottoms[2] - bottoms[0]) * v) / vSteps;

				position = new Vector3(
					(position.x * td.heightmapResolution) / td.size.x,
					0,		// do not care
					(position.z * td.heightmapResolution) / td.size.z);

				int terrainS = (int)position.x;
				int terrainR = (int)position.z;

				if (terrainR >= 0 && terrainR < heightField.GetLength(0))
				{
					if (terrainS >= 0 && terrainS < heightField.GetLength(1))
					{
						Debug.Log( "" + terrainR + " ======= " +  terrainS);

						if (raise && (heightField[terrainR,terrainS] < scaledUndersideHeight))
						{
							heightField[terrainR,terrainS] = scaledUndersideHeight;
						}

						if (lower && (heightField[terrainR,terrainS] > scaledUndersideHeight))
						{
							heightField[terrainR,terrainS] = scaledUndersideHeight;
						}
					}
				}
			}
		}

		// save the whole heightmap back. CAUTION! This modifies the on-disk terrain asset!
		td.SetHeights( 0, 0, heightField);
	}

	IEnumerator Start ()
	{
		// remove this delay - this is just so you can see it happen...
		yield return new WaitForSeconds( 2.0f);

		MeshFilter[] AllMeshFilters = gameObject.GetComponentsInChildren<MeshFilter>();
		Debug.Log( System.String.Format (
			"Found {0} MeshFilter components.", AllMeshFilters.Length));

		foreach(MeshFilter mf in AllMeshFilters)
		{
			ModifyTerrain( mf, true, true);
		}
	}
}
