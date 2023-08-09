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

		x += CellSize / 2;
		y += (CellSize * HeightRatio) / 2;

		int iy = (int)y;
		if (y < 0) iy--;

		if ((iy & 1) != 0)
		{
			x -= CellSize / 2;
		}

		int ix = (int)x;
		if (x < 0) ix--;

		Vector2 outPoint = new Vector2( ix * CellSize, iy * CellSize * HeightRatio);

		if ((iy & 1) != 0)
		{
			outPoint.x += CellSize / 2;
		}

		return outPoint;
	}
}
