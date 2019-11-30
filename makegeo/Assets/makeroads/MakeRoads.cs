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
	// How high to lift up so we can raycast down, and how far to cast down.
	// If your point provider gives results that are close to the surface,
	// these can be made smaller.
	const float CastPullUp = 25.0f;
	const float CastRayDown = 50.0f;

	public static GameObject Create( RoadConfiguration Config, IEnumerable<PositionAndHeading> PointProvider)
	{
		GameObject go = new GameObject ("MakeRoad1.Create();");

		MeshFilter mf = go.AddComponent<MeshFilter> ();
		Mesh mesh = new Mesh();

		List<Vector3> verts = new List<Vector3> ();
		List<int> tris0 = new List<int> ();
		List<int> tris1 = new List<int> ();
		List<Vector2> uvs = new List<Vector2> ();

		bool first = true;

		float HalfWidth = Config.Width / 2;

		Vector3 prevLeft = Vector3.zero;
		Vector3 prevRight = Vector3.zero;

		int prevNTopLeft = -1;

		int prevNEdgeLeft = -1;
		int prevNEdgeRight = -1;

		Vector3 prevPosition = Vector3.zero;

		float u = 0.0f;

		foreach( var pt in PointProvider)
		{
			Vector3 position = pt.Position;
			float heading = pt.Heading;

			Vector3 leftFlatLateral = Quaternion.Euler( 0, heading - 90, 0) * Vector3.forward * HalfWidth;
			Vector3 rightFlatLateral = Quaternion.Euler( 0, heading + 90, 0) * Vector3.forward * HalfWidth;;

			Vector3 left = position + leftFlatLateral;
			Vector3 right = position + rightFlatLateral;

			Vector3 leftEdgeBottom = left;
			Vector3 rightEdgeBottom = right;

			if (Config.FollowUnderlyingSurface)
			{
				Ray rayLeft = new Ray( left + Vector3.up * CastPullUp, Vector3.down);
				Ray rayRight = new Ray( right + Vector3.up * CastPullUp, Vector3.down);
				RaycastHit rchLeft, rchRight;
				bool hitLeft = Physics.Raycast( rayLeft, out rchLeft, CastRayDown);
				bool hitRight = Physics.Raycast( rayRight, out rchRight, CastRayDown);

				if (hitLeft && hitRight)
				{
					left = rchLeft.point;
					right = rchRight.point;

					if (!Config.TiltWithUnderlyingSurface)
					{
						float highestPoint = Mathf.Max( left.y, right.y);

						float originalHighestPoint = highestPoint;

						// To make sure we don't have ground poking through, we can
						// check spanwise going across, and adjust upwards more.
						for (int spanWiseCheckNo = 0; spanWiseCheckNo < Config.ExtraSpanWiseHeightSamples; spanWiseCheckNo++)
						{
							int n = spanWiseCheckNo + 1;

							float fraction = (float)n / (Config.ExtraSpanWiseHeightSamples + 2);

							Ray rayCenter = new Ray( Vector3.Lerp( left, right, fraction) + Vector3.up * CastPullUp, Vector3.down);
							RaycastHit rch;
							if (Physics.Raycast( rayCenter, out rch, CastRayDown))
							{
								if (rch.point.y > highestPoint)
								{
									highestPoint = rch.point.y;
								}
							}
						}

						left.y += highestPoint - originalHighestPoint;
						right.y += highestPoint - originalHighestPoint;
					}

					// keep roadway flat?
					if (!Config.TiltWithUnderlyingSurface)
					{
						if (left.y > right.y)
						{
							right.y = left.y;
						}
						if (right.y > left.y)
						{
							left.y = right.y;
						}
					}
				}
			}

			left += Vector3.up * Config.Height;
			right += Vector3.up * Config.Height;
			 
			int nTopLeft = verts.Count;

			verts.Add( left);
			verts.Add( right);

			uvs.Add( new Vector2( u, 0));
			uvs.Add( new Vector2( u, 1));

			if (!first)
			{
				tris0.Add( prevNTopLeft);
				tris0.Add( nTopLeft);
				tris0.Add( prevNTopLeft + 1);

				tris0.Add( nTopLeft);
				tris0.Add( nTopLeft + 1);
				tris0.Add( prevNTopLeft + 1);
			}

			// we need this early so we can scale the edge UVs properly and keep them nearly square
			float uAdvance = Vector3.Distance( position, prevPosition) / Config.Width;

			if (Config.MakeEdges)
			{
				leftEdgeBottom += Vector3.down * Config.EdgeExtraHeight;
				rightEdgeBottom += Vector3.down * Config.EdgeExtraHeight;

				float leftRaise = left.y - leftEdgeBottom.y;
				float rightRaise = right.y - rightEdgeBottom.y;

				float EdgeTangentOutwards = Mathf.Tan( (90 - Config.EdgeAngle) * Mathf.Deg2Rad);
				float leftOutward = leftRaise * EdgeTangentOutwards;
				float rightOutward = rightRaise * EdgeTangentOutwards;

				leftEdgeBottom += leftFlatLateral.normalized * leftOutward;
				rightEdgeBottom += rightFlatLateral.normalized * rightOutward;

				{
					int nEdgeLeft = verts.Count;

					verts.Add( leftEdgeBottom);
					verts.Add( left);

					float v = uAdvance * Vector3.Distance( leftEdgeBottom, left) / Vector3.Distance( prevLeft, left);

					uvs.Add( new Vector2( u, -v));
					uvs.Add( new Vector2( u, 0));

					if (!first)
					{
						tris1.Add( prevNEdgeLeft + 1);
						tris1.Add( prevNEdgeLeft);
						tris1.Add( nEdgeLeft);

						tris1.Add( prevNEdgeLeft + 1);
						tris1.Add( nEdgeLeft);
						tris1.Add( nEdgeLeft + 1);
					}

					prevNEdgeLeft = nEdgeLeft;
				}

				{
					int nEdgeRight = verts.Count;

					verts.Add( right);
					verts.Add( rightEdgeBottom);

					float v = uAdvance * Vector3.Distance( rightEdgeBottom, right) / Vector3.Distance( prevRight, right);

					uvs.Add( new Vector2( -u, 0));
					uvs.Add( new Vector2( -u, -v));

					if (!first)
					{
						tris1.Add( prevNEdgeRight + 1);
						tris1.Add( prevNEdgeRight);
						tris1.Add( nEdgeRight);

						tris1.Add( prevNEdgeRight + 1);
						tris1.Add( nEdgeRight);
						tris1.Add( nEdgeRight + 1);
					}

					prevNEdgeRight = nEdgeRight;
				}
			}

			u -= uAdvance;

			prevPosition = position;

			prevLeft = left;
			prevRight = right;

			first = false;
			prevNTopLeft = nTopLeft;

//			var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
//			cube.transform.position = position;
//			cube.transform.rotation = Quaternion.Euler( 0, heading, 0);
//
//			var capsule = GameObject.CreatePrimitive(PrimitiveType.Capsule);
//			capsule.transform.position = left;
//			capsule.transform.rotation = Quaternion.Euler( 0, heading, 0) * Quaternion.Euler( 90, 0, 0);
//
//			var cylinder = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
//			cylinder.transform.position = right;
//			cylinder.transform.rotation = Quaternion.Euler( 0, heading, 0) * Quaternion.Euler( 90, 0, 0);
		}

		mesh.subMeshCount = 2;

		mesh.vertices = verts.ToArray ();
		mesh.uv = uvs.ToArray ();

		mesh.SetTriangles( tris0.ToArray (), 0);
		mesh.SetTriangles( tris1.ToArray (), 1);

		mesh.RecalculateBounds ();
		mesh.RecalculateNormals ();

		mf.mesh = mesh;

		var mr = go.AddComponent<MeshRenderer> ();
		mr.materials = new Material[] {
			Config.RoadSurfaceMaterial,
			Config.RoadEdgeMaterial,
		};

		return go;
	}
}
