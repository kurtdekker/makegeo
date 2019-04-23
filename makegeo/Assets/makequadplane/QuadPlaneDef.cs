using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuadPlaneDef
{
    public int xCells;
    public int zCells;

    // caution: x controls x dimension, y controls scaling of height, and z controls z direction
    public Vector3 WorldSize = Vector3.one;

    public Vector2 UVScale = Vector2.one;

    public System.Func<Vector2, float> HeightFunction;  // optional

    public QuadPlaneDef(int x, int y)
    {
        xCells = x;
        zCells = y;

        WorldSize = Vector3.one;
    }

    public QuadPlaneDef(int x, int z, Vector3 size)
    {
        xCells = x;
        zCells = z;

        WorldSize = size;
    }
}
