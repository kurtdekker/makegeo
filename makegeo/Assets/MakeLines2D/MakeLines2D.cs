using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// @kurtdekker
// Just curious what kind of horsepower it takes
// to run a bunch of straight lines on a single mesh,
// simply by tracking their endpoints, sort of like
// a super-simple Vector graphics video board.

public class MakeLines2D : MonoBehaviour
{
	public Transform LowerLeft;
	public Transform UpperRight;

	public Material mtl;

	Vector2 LL { get { return new Vector2( LowerLeft.position.x, LowerLeft.position.y); } }
	Vector2 UR { get { return new Vector2( UpperRight.position.x, UpperRight.position.y); } }

	const float MinSpeed = 1;
	const float MaxSpeed = 3;

	const int MAXLINESEGMENTS = 1000;

	const float MinWidth = 0.01f;
	const float MaxWidth = 0.015f;

	class LineSegment
	{
		public Vector2 p1, p2;
		public Vector2 v1, v2;
		public float width;
		public Vector2 uv;
	}
	LineSegment[] LineSegments;
	// kept in perfect synchrony with the above, 4 verts per line
	Vector3[] VertBuffer;
	int[] TriBuffer;
	Vector2[] UVBuffer;

	MeshFilter mf;
	Mesh mesh;

	void Start ()
	{
		LineSegments = new LineSegment[ MAXLINESEGMENTS];
		for (int i = 0; i < LineSegments.Length; i++)
		{
			var ls = new LineSegment();

			// random endpoints
			ls.p1 = new Vector2(
				Mathf.Lerp( LL.x, UR.x, Random.value),
				Mathf.Lerp( LL.x, UR.x, Random.value));
			ls.p2 = new Vector2(
				Mathf.Lerp( LL.x, UR.x, Random.value),
				Mathf.Lerp( LL.x, UR.x, Random.value));

			// random endpoint velocities
			ls.v1 = Random.insideUnitCircle.normalized *
				Random.Range( MinSpeed, MaxSpeed) * (Random.Range( 0, 2) * 2 - 1);
			ls.v2 = Random.insideUnitCircle.normalized *
				Random.Range( MinSpeed, MaxSpeed) * (Random.Range( 0, 2) * 2 - 1);

			// random width
			ls.width = Random.Range(  MinWidth, MaxWidth);

			// random color (via UV into source texture)
			ls.uv = new Vector2( Random.value, Random.value);

			LineSegments[i] = ls;
		}

		mf = gameObject.AddComponent<MeshFilter>();
		mesh = new Mesh();
		mf.mesh = mesh;

		MeshRenderer mr = gameObject.AddComponent<MeshRenderer>();
		mr.material = mtl;

		VertBuffer = new Vector3[MAXLINESEGMENTS * 4];
		UVBuffer = new Vector2[ MAXLINESEGMENTS * 4];

		TriBuffer = new int[ MAXLINESEGMENTS * 2 * 3];

		int tn = 0;
		int vn = 0;
		for (int i = 0; i < MAXLINESEGMENTS; i++)
		{
			TriBuffer[tn++] = vn + 0;
			TriBuffer[tn++] = vn + 1;
			TriBuffer[tn++] = vn + 2;

			TriBuffer[tn++] = vn + 1;
			TriBuffer[tn++] = vn + 3;
			TriBuffer[tn++] = vn + 2;

			vn += 4;
		}

		mesh.vertices = VertBuffer;
		mesh.triangles = TriBuffer;

		UpdateLineSegments();
		UpdateUVColors();
	}

	// only need this if the line colors change
	void UpdateUVColors()
	{
		int un = 0;
		for (int i = 0; i < MAXLINESEGMENTS; i++)
		{
			var ls = LineSegments[i];

			var uv = ls.uv;

			UVBuffer[ un++] = uv;
			UVBuffer[ un++] = uv;
			UVBuffer[ un++] = uv;
			UVBuffer[ un++] = uv;
		}

		mesh.uv = UVBuffer;
	}

	void MoveClampBounce( ref Vector2 p, ref Vector2 v)
	{
		p += v * Time.deltaTime;

		if (p.x < LL.x)
		{
			p.x = LL.x;
			v.x = +Mathf.Abs( v.x);
		}
		if (p.x > UR.x)
		{
			p.x = UR.x;
			v.x = -Mathf.Abs( v.x);
		}

		if (p.y < LL.y)
		{
			p.y = LL.y;
			v.y = +Mathf.Abs( v.y);
		}
		if (p.y > UR.y)
		{
			p.y = UR.y;
			v.y = -Mathf.Abs( v.y);
		}
	}

	Quaternion rotl = Quaternion.Euler( 0, 0, 90);
	Quaternion rotr = Quaternion.Euler( 0, 0, -90);

	void UpdateLineSegments()
	{
		int vn = 0;

		for (int i = 0; i < LineSegments.Length; i++)
		{
			var ls = LineSegments[i];

			MoveClampBounce( ref ls.p1, ref ls.v1);
			MoveClampBounce( ref ls.p2, ref ls.v2);

			Vector2 delta = (ls.p2 - ls.p1).normalized;

			VertBuffer[vn++] = ls.p1 + (Vector2)(rotr * delta * ls.width);
			VertBuffer[vn++] = ls.p1 + (Vector2)(rotl * delta * ls.width);

			VertBuffer[vn++] = ls.p2 + (Vector2)(rotr * delta * ls.width);
			VertBuffer[vn++] = ls.p2 + (Vector2)(rotl * delta * ls.width);
		}

		mesh.RecalculateBounds();

		mesh.vertices = VertBuffer;

		mf.mesh = mesh;
	}

	void Update()
	{
		UpdateLineSegments();
	}
}
