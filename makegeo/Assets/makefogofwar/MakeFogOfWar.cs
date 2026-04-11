
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// @kurtdekker

public class MakeFogOfWar
{
	// The fog of war is two quad strips wrapped around a central hollow.
	//
	// Sectors defines how much geometry to make (subdivisions)
	// The hollow is of radius "innerRadius"
	// The gradient portion is of "gradientWidth"
	// rays are sent outward to "totalRadius"
	// clip is an optional way to clamp the outside verts to a rectangle
	//
	// This is NOT a general solution for arbitrary-shaped fog of war!
	//
	// Think of it as an "anti-circle" or a "gradient-lined hole."
	//
	public static GameObject Create(
		int sectors,
		float innerRadius,
		float gradientWidth,
		float totalRadius,
		Rect clip = default)
	{
		GameObject go = new GameObject("MakeFogOfWar.Create();");

		MeshFilter mf = go.AddComponent<MeshFilter>();
		Mesh mesh = new Mesh();

		List<Vector3> verts = new List<Vector3>();
		List<int> innerTris = new List<int>();
		List<Vector2> uvs = new List<Vector2>();

		List<int> outerTris = new List<int>();

		float middleRadius = innerRadius + gradientWidth;

		// fabricate!
		//
		// we go around making inner, middle for the inner band
		// we go around making middle, outer for the outer band
		//

		// the index of the previous ray inner vert
		int prevN = 0;

		for (int i = 0; i <= sectors; i++)
		{
			// index of this ray inner vert
			int n = verts.Count;

			float angle = (Mathf.PI * 2 * i) / sectors;

			// this is to wrap the V of the UV around the circle
			float phi = (float)i / sectors;

			// these verts are involved in the inner (gradient)band of quads
			Vector3 innerPoint = new Vector3(
				Mathf.Cos(angle) * innerRadius,
				Mathf.Sin(angle) * innerRadius);
			Vector2 innerUV = new Vector2(1, phi);		// u == 1 is the clear transparent side (right)

			verts.Add(innerPoint);
			uvs.Add(innerUV);

			Vector3 middlePoint = new Vector3(
				Mathf.Cos(angle) * middleRadius,
				Mathf.Sin(angle) * middleRadius);
			Vector2 middleUV = new Vector2(0, phi);     // u == 0 is the solid opaque side (left)

			verts.Add(middlePoint);
			uvs.Add(middleUV);

			// add the inner quads
			if (i > 0)
			{
				innerTris.Add(prevN + 0);
				innerTris.Add(n + 0);
				innerTris.Add(n + 1);

				innerTris.Add(prevN + 0);
				innerTris.Add(n + 1);
				innerTris.Add(prevN + 1);
			}

			// these verts are involved in the outer (solid) band of quads
			verts.Add(middlePoint);
			uvs.Add(middleUV);

			Vector3 outerPoint = new Vector3(
				Mathf.Cos(angle) * totalRadius,
				Mathf.Sin(angle) * totalRadius);
			Vector2 outerUV = new Vector2(1, phi);

			// we only bother to clip the outer one, and I don't bother with UV-matching!
			if (clip.width > 0)
			{
				if (outerPoint.x < clip.xMin)
				{
					outerPoint.x = clip.xMin;
				}
				if (outerPoint.x > clip.xMax)
				{
					outerPoint.x = clip.xMax;
				}
				if (outerPoint.y < clip.yMin)
				{
					outerPoint.y = clip.yMin;
				}
				if (outerPoint.y > clip.yMax)
				{
					outerPoint.y = clip.yMax;
				}
			}

			verts.Add(outerPoint);
			uvs.Add(outerUV);

			// add the outer quads
			if (i > 0)
			{
				outerTris.Add(prevN + 2);
				outerTris.Add(n + 2);
				outerTris.Add(n + 3);

				outerTris.Add(prevN + 2);
				outerTris.Add(n + 3);
				outerTris.Add(prevN + 3);
			}

			prevN = n;
		}

		mesh.vertices = verts.ToArray();
		mesh.uv = uvs.ToArray();

		mesh.subMeshCount = 2;
		mesh.SetTriangles(innerTris.ToArray(), 0);
		mesh.SetTriangles(outerTris.ToArray(), 1);

		mesh.RecalculateBounds();
		mesh.RecalculateNormals();

		mf.mesh = mesh;

		go.AddComponent<MeshRenderer>();

		return go;
	}
}
