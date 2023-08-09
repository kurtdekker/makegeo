using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexGrid
{
	Vector2 ZeroPoint = Vector3.zero;

	float CellSize = 1.0f;

	float HeightRatio { get { return Mathf.Sin( Mathf.PI / 3); } }

	public Vector2 SnapToGrid( Vector2 inPoint)
	{
		inPoint -= ZeroPoint;

		float x = inPoint.x;
		float y = inPoint.y / HeightRatio;

		int iy = (int)y;

		if ((iy & 1) != 0)
		{
			x -= 0.5f;
		}

		int ix = (int)x;

		Vector2 outPoint = new Vector2( ix * CellSize, iy * CellSize * HeightRatio);

		if ((iy & 1) != 0)
		{
			outPoint.x += 0.5f;
		}

		return outPoint;
	}
}
