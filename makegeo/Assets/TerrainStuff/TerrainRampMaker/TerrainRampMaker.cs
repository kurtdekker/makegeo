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

#if UNITY_EDITOR
using UnityEditor;
#endif

// Feature list:
//	_x_	lets you put a series of child objects under this
//	_x_	shows them in the editor window:
//		_x_	gizmos at each one
//		_x_	gizmo handles at each one!
//		_x_	simple gizmo line going between them
//		___	multiple lines showing width
//		___	Make a mesh, use Gizmos.DrawWireMesh()!
//	_x_	button to make intervening heights linearly track between ends
//	_x_	button to make all endpoints track similar terrain height
//		___	field entry for terrain height
//	_w_	raycasts height of first and last
//	_x_	Has a button that:
//		_x_	slings a ramp along that path
//		_x_	handles undo properly
//	_x_	handles specification of path width
//	___	handles specification of sharpness of edges of ramp
//		___	splatmap paint differently on the ramp
//		___	splatmap paint differently on the edges
//	___	drop a "Terrain Ramp Control Point" on each child:
//		_x_	allow varying widths of the ramp
//		___	allow varying opacity of the ramp imprint?
//	___	Better edge smoothing:
//		___	Accumulate a "stack" of heights at a given point
//		___	use their average

[ExecuteInEditMode]
public class TerrainRampMaker : MonoBehaviour
{
	public enum HeightFollowingModeType
	{
		OFF,
		ENDPOINTS_ONLY,
		ENDPOINTS_RAMP,
		ALLPOINTS_TOUCH,
	}

	// These get copied to each TerrainRampNode sub-object.
	// These are in worldspace coordinates:
	public	float	ModelRampWidth = 10.0f;
	// Total affected ramp width is RampWidth + 2 * RampEdges:
	public	float	ModelRampEdges = 2.5f;

	// Allows blending of the path into the terrain, i.e., preserve some
	// of the underlying terrain contouring in your path.
	public	float	CutOpacity = 1.0f;

	public	bool	OnlyDrawWhenSelected = false;

	[Space(20)]
	[Header( "Warning: enabling this can make ramp cuts SLOW!")]
	public	bool	EnableUndoFunctionality = true;
	[Space(50)]

	public	HeightFollowingModeType		HeightFollowingMode = HeightFollowingModeType.ENDPOINTS_RAMP;

	bool AllOnSameTerrain;
	Terrain[] Terrains;

	const float HeightScanRange = 250.0f;

	Mesh GizmoMesh;

	void Update()
	{
		AllOnSameTerrain = true;
		if (GetCount() < 2) return;

		if (Terrains == null || Terrains.Length != GetCount())
		{
			Terrains = new Terrain[ GetCount()];
		}

		// This only auto-levels the end points...
		// <WIP> might be useful to only auto-level on command
		// <WIP> might be useful to interpolate the middle position heights on command
		for (int passno = 1; passno <= 2; passno++)
		{
			Vector3 pfirst = GetChild(0).position;
			Vector3 plast = GetChild( GetCount() - 1).position;

			for (int i = 0; i < GetCount(); i++)
			{
				Terrains[i] = null;

				var tr = GetChild( i);

				if (passno == 1) tr.name = i.ToString();

				Ray ray = new Ray( tr.position + Vector3.up * HeightScanRange, Vector3.down);
				RaycastHit rch;
				if (Physics.Raycast( ray, out rch, HeightScanRange * 2))
				{
					bool SetByRaycast = false;
					bool SetByInterpolation = false;

					if (HeightFollowingMode == HeightFollowingModeType.ALLPOINTS_TOUCH)
					{
						SetByRaycast = true;
					}

					if (HeightFollowingMode == HeightFollowingModeType.ENDPOINTS_ONLY)
					{
						SetByRaycast = true;
					}

					if (HeightFollowingMode == HeightFollowingModeType.ENDPOINTS_RAMP)
					{
						SetByRaycast = (i == 0) || (i == GetCount() - 1);
						SetByInterpolation = !SetByRaycast;
					}

					SetByRaycast = SetByRaycast && (passno == 1);
					SetByInterpolation = SetByInterpolation && (passno == 2);

					if (SetByRaycast)
					{
						tr.position = rch.point;
					}

					if (SetByInterpolation)
					{
						float distance1 = 0;
						float distance2 = 0;
						for (int j = 0; j < GetCount() - 1; j++)
						{
							Vector3 delta1 = GetChild(j + 1).position - GetChild(j).position;
							delta1.y = 0;

							if (j >= i)
							{
								distance2 += delta1.magnitude;
							}
							else
							{
								distance1 += delta1.magnitude;
							}
						}

						float terpedY = tr.position.y;

						if (distance1 + distance2 > 0.1f)
						{
							float terpAlpha = distance1 / (distance1 + distance2);
							terpedY = Mathf.Lerp( pfirst.y, plast.y, terpAlpha);
						}

						tr.position = new Vector3( tr.position.x, terpedY, tr.position.z);
					}

					Terrains[i] = rch.collider.gameObject.GetComponent<Terrain>();
				}
			}
		}

		AllOnSameTerrain = true;
		for (int i = 1; i < Terrains.Length; i++)
		{
			if (Terrains[i] != Terrains[0])
			{
				AllOnSameTerrain = false;
				break;
			}
		}

		FabricateGizmoMesh();
	}

	void FabricateGizmoMesh()
	{
		if (GizmoMesh == null)
		{
			GizmoMesh = new Mesh();
		}

		using( VertexHelper vh = new VertexHelper())
		{
			for (int i = 0; i < GetCount() - 1; i++)
			{
				TerrainRampNode n1 = GetNode(i);

				Vector3 a = GetChild(i).position;
				Vector3 b = GetChild(i+1).position;

				Vector3 delta = b - a;
				delta.y = 0;

				Vector3 lateral = Quaternion.Euler( 0, 90, 0) * delta;

				Vector3 left = a + lateral.normalized * n1.RampWidth;
				Vector3 right = a - lateral.normalized * n1.RampWidth;

				// TODO: add above to the ramp geometry mesh
			}
		}
	}

	// This is the mapping between points and how they are stored
	IEnumerable<Transform> Children()
	{
		for( int i = 0; i < GetCount(); i++)
		{
			yield return GetChild(i);
		}
	}
	int GetCount()
	{
		return transform.childCount;
	}
	Transform GetChild( int i)
	{
		return transform.GetChild(i);
	}
	TerrainRampNode GetNode( int i)
	{
		Transform tr = GetChild( i);
		var node = tr.GetComponent<TerrainRampNode>();
		if (!node)
		{
			node = tr.gameObject.AddComponent<TerrainRampNode>();
			node.RampWidth = ModelRampWidth;
			node.RampEdges = ModelRampEdges;
		}
		return node;
	}
	// end mapping

	void GizmosDrawHelper()
	{
		for (int i = 0; i < GetCount(); i++)
		{
			var tr = GetChild( i);

			Gizmos.DrawWireSphere( tr.position, 0.5f);

			if (i > 0)
			{
				Gizmos.DrawLine( tr.position, GetChild(i - 1).position);
			}
		}
	}

	void OnDrawGizmosSelected()
	{
		GizmosDrawHelper();
	}

	void OnDrawGizmos()
	{
		if (!OnlyDrawWhenSelected) GizmosDrawHelper();
	}

	void CutStraightRamp()
	{
		if (!EnableUndoFunctionality)
		{
			int result = EditorUtility.DisplayDialogComplex(
				"Warning! Permanent Terrain Action!",
				"UNDO functionality is currently OFF. This operation will " +
				"permanently change your terrain asset to cut the ramp.\n\n" +
				"You can turn on UNDO functionality on the TerrainRampMaker root instance.\n\n" +
				"UNDO functionality may cause the terrain ramp cut to be very slow on some versions of Unity3D.\n\n" +
				"I recommend you use source control at all times to avoid data loss.\n\n" +
				"Are you sure you want to cut this ramp?",
				"CUT RAMP", "Cancel", null);

			if (result != 0)
			{
				Debug.LogWarning( "Terrain cut cancelled.");
				return;
			}
		}

		// all points in world position
		Vector3[] WorldPositions = new Vector3[ GetCount()];
		for (int i = 0; i < GetCount(); i++)
		{
			WorldPositions[i] = GetChild(i).position;
		}

		Terrain ter = Terrains[0];
		TerrainData td = ter.terrainData;

		// all points in terrain position
		Vector3[] TerrainPositions = new Vector3[ GetCount()];
		for (int i = 0; i < GetCount(); i++)
		{
			Vector3 one = WorldPositions[i];

			// take the object's terrain-relative height and scale it by the terrain's size.
			float scaledUndersideHeight = one.y / td.size.y;

			one.y = scaledUndersideHeight;

			TerrainPositions[i] = one;
		}

		int w = td.heightmapResolution;
		int h = w;

		// We will paint the height of our ramp on here
		float[,] paintedRampHeightField = new float[ w, h];

		// We will paint 1.0 on the parts of the ramp we want precisely at
		// the ramp height, and then taper off all around it.
		float[,] rampOpacityField = new float[ w, h];

		int[,] pixelOverdraw = new int[w,h];

		// Now we traverse our ramp line and set those TerrainData
		// heightmap pixels to the required height.

		// Fabricate out the edges of the ramp geometry, from the point of
		// view of looking straight down on it: this involves making edges
		// that are "away" from the ramp centerline by the ramp's width.
		Vector3[] LeftPoints = new Vector3[ GetCount()];
		Vector3[] RightPoints = new Vector3[ GetCount()];
		for (int i = 0; i < GetCount(); i++)
		{
			Vector3 LateralNormal = Vector3.zero;
			if (i == 0)
			{
				LateralNormal = (GetChild(i + 1).position - GetChild(i).position);
			}
			else
			{
				if (i == GetCount() - 1)
				{
					LateralNormal = (GetChild(i).position - GetChild(i - 1).position);
				}
				else
				{
					LateralNormal = (
						(GetChild(i + 1).position - GetChild(i).position).normalized +
						(GetChild(i).position - GetChild(i - 1).position).normalized);
				}
			}
			LateralNormal.y = 0;
			LateralNormal = Quaternion.Euler( 0, 90, 0) * LateralNormal;
			LateralNormal.Normalize();

			// half of the ramp's width plus one of its full edges
			float LateralSpanOfRamp = ModelRampWidth / 2 + ModelRampEdges;

			LeftPoints[i] = TerrainPositions[i] + LateralNormal * LateralSpanOfRamp;
			RightPoints[i] = TerrainPositions[i] - LateralNormal * LateralSpanOfRamp;

			// move from world coordinate space into pixel heightmap coordinate space
			LeftPoints[i].x = (LeftPoints[i].x * td.heightmapResolution) / td.size.x;
			LeftPoints[i].z = (LeftPoints[i].z * td.heightmapResolution) / td.size.z;
			RightPoints[i].x = (RightPoints[i].x * td.heightmapResolution) / td.size.x;
			RightPoints[i].z = (RightPoints[i].z * td.heightmapResolution) / td.size.z;
		}

		// convert the edges specification from world coordinates to heightmap coordinates
		float HMRampEdges = (ModelRampEdges * td.heightmapResolution) / td.size.x;

		// this term is to make sure we traverse the terrain heightmap
		// pixels densely enough that we don't leave gaps. We could make
		// a polygon fill instead but I can't be arsed right now.
		float ExtraStepTraversalFactor = 0.5f;

		// This traverses each of the ramp control points
		for (int j = 0; j < GetCount() - 1; j++)
		{
			float LeftDistance = (LeftPoints[j + 1] - LeftPoints[j]).magnitude;
			float RightDistance = (RightPoints[j + 1] - RightPoints[j]).magnitude;

			int StepsAlongSegment = (int)( Mathf.Max( LeftDistance, RightDistance) / ExtraStepTraversalFactor);

			// This traverses a single linear segment of the ramp
			for ( int i = 0; i < StepsAlongSegment; i++)
			{
				float FractionAlongSegment = (float)i / StepsAlongSegment;

				// Lerp along our edges, finding left and right extent of ramp, including edges
				Vector3 LeftPos = Vector3.Lerp(
					LeftPoints[j],
					LeftPoints[j + 1],
					FractionAlongSegment);

				Vector3 RightPos = Vector3.Lerp(
					RightPoints[j],
					RightPoints[j + 1],
					FractionAlongSegment);

				float TotalRampWidth = (LeftPos - RightPos).magnitude;

				int StepsAcrossRampWidth = (int)((RightPos - LeftPos).magnitude / ExtraStepTraversalFactor);

				// This traverses the lateral span of the ramp, including its edges
				for (int k = 0; k <= StepsAcrossRampWidth; k++)
				{
					float FractionAcrossRamp = (float)k / StepsAcrossRampWidth;

					Vector3 position = Vector3.Lerp(
						LeftPos, RightPos, FractionAcrossRamp);

					// Handle the opacity falloff on the edges of the ramp, going
					// across its face. This term is the mirrored half fraction,
					// going from 0.0 to 0.5 and back down to 0.0 laterally.
					float fractionFromEdge = FractionAcrossRamp;
					if (fractionFromEdge > 0.5f)
					{
						fractionFromEdge = 1.0f - fractionFromEdge;
					}

					// presume its the middle flattish part of the ramp
					float opacity = 1.0f;
					if (fractionFromEdge <= HMRampEdges / TotalRampWidth)
					{
						opacity = fractionFromEdge / (HMRampEdges / TotalRampWidth);
					}

					int v = (int)position.x;
					int u = (int)position.z;

					if (u >= 0 && u < h)
					{
						if (v >= 0 && v < w)
						{
							paintedRampHeightField[u,v] += position.y;
							pixelOverdraw[u,v]++;

							rampOpacityField[u,v] = opacity;
						}
					}
				}
			}
		}

		float[,] originalTerrainHeightField = td.GetHeights( 0, 0, w, h);

		// now we apply the painted ramp to the underlying terrain
		for (int j = 0; j < h; j++)
		{
			for (int i = 0; i < w; i++)
			{
				float opacity = rampOpacityField[i,j] * CutOpacity;

				if (pixelOverdraw[i,j] > 0)
				{
					float paintedSample = paintedRampHeightField[i,j] / pixelOverdraw[i,j];

					float sample = Mathf.Lerp(
						originalTerrainHeightField[i,j],
						paintedSample,
						opacity);

					originalTerrainHeightField[i,j] = sample;
				}
			}
		}

#if UNITY_EDITOR
		if (EnableUndoFunctionality)
		{
			Undo.RecordObjects( new Object[] {
				td
			}, "Terrain Ramp Cut");
		}
#endif

		// save the whole heightmap back. CAUTION! This modifies the on-disk terrain asset!
		td.SetHeights( 0, 0, originalTerrainHeightField);
	}

#if UNITY_EDITOR
	[CustomEditor( typeof( TerrainRampMaker))]
	public class TerrainRampMakerEditor : Editor
	{
		public override void OnInspectorGUI()
		{
			TerrainRampMaker trm = (TerrainRampMaker)target;

			DrawDefaultInspector();

			GUILayout.BeginVertical();

			GUI.color = Color.white;
			if (trm.GetCount() < 2)
			{
				GUI.color = Color.yellow;
				GUILayout.Label( "ERROR: Need at least two children points for a ramp.");
			}
			else
			{
				if (trm.AllOnSameTerrain)
				{
					GUILayout.Label( "Adjust settings below.");

					if (GUILayout.Button( "CUT STRAIGHT RAMP"))
					{
						trm.CutStraightRamp();
					}
				}
				else
				{
					GUI.color = Color.yellow;
					GUILayout.Label( "ERROR: All points must drop to the same terrain!");
				}
			}
			GUI.color = Color.white;

			GUILayout.EndVertical();
		}

		void OnSceneGUI()
		{
			TerrainRampMaker trm = (TerrainRampMaker)target;

			for (int i = 0; i < trm.GetCount(); i++)
			{
				var tr = trm.GetChild( i);

				EditorGUI.BeginChangeCheck();
				Vector3 position = Handles.PositionHandle( tr.position, Quaternion.identity );
				if( EditorGUI.EndChangeCheck() )
				{
					Undo.RecordObject( tr, "Move Ramp Control Point" );
					tr.position = position;
				}
			}
		}
	}
#endif
}
