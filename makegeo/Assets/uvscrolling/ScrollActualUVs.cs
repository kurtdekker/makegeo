using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// @kurtdekker - scrolls the actual UV coordinates
// within the mesh in a given direction.
// Expects to find a MeshFilter

public class ScrollActualUVs : MonoBehaviour
{
	public Vector2 UVDirection;

	public float UVSpeed;

	Mesh mesh;
	Vector2[] WorkingUVs;

	void Reset()
	{
		float angle = Random.Range( 0.0f, 306.0f);
		Quaternion rotation = Quaternion.Euler( 0, 0, angle);
		UVDirection = rotation * Vector2.up;

		UVSpeed = 1.0f;
	}

	void Start()
	{
		var mf = GetComponent<MeshFilter>();

		mesh = mf.mesh;

		var uvs = mesh.uv;

		// copy them
		WorkingUVs = new Vector2[ uvs.Length];
		for( int i = 0; i < WorkingUVs.Length; i++)
		{
			WorkingUVs[i] = uvs[i];
		}

		mesh.uv = WorkingUVs;
	}

	void Update ()
	{
		bool oob = false;
		Vector2 fix = Vector2.zero;

		var change = UVDirection * UVSpeed * Time.deltaTime;

		for (int i = 0; i < WorkingUVs.Length; i++)
		{
			var uv = WorkingUVs[i];
			uv += change;

			// keep wrapping nearish
			if (uv.x < -1)
			{
				oob = true;
				fix.x = +1;
			}
			if (uv.x > +1)
			{
				oob = true;
				fix.x = -1;
			}
			if (uv.y < -1)
			{
				oob = true;
				fix.y = +1;
			}
			if (uv.y > +1)
			{
				oob = true;
				fix.y = -1;
			}

			WorkingUVs[i] = uv;
		}

		if (oob)
		{
			for (int i = 0; i < WorkingUVs.Length; i++)
			{
				var uv = WorkingUVs[i];
				uv += fix;
				WorkingUVs[i] = uv;
			}
		}

		mesh.uv = WorkingUVs;
	}
}
