using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// @kurtdekker - sprinkle this anywhere you want some
// physics-enabled cubes to play with.

public class SprinkleSomePlayCubesOn : MonoBehaviour
{
	[Header( "How many?")]
	public int CubeCount = 20;

	[Header("Where should these be?")]
	[Header( "Uses the Y to lift only.")]
	public Vector3 ArealExtent = new Vector3( 100, 10, 100);

	[Header( "Range of size:")]
	public float MinSize = 1;
	public float MaxSize = 10;

	void Start ()
	{
		for (int i = 0; i < CubeCount; i++)
		{
			float fx = 0;
			if (CubeCount > 1)
			{
				fx = (float)i / (CubeCount - 1);
			}
			float fz = Random.value;

			var pos = new Vector3(
				(fx - 0.5f) * ArealExtent.x,
				ArealExtent.y,
				(fz - 0.5f) * ArealExtent.z);

			pos += transform.position;

			var cube = GameObject.CreatePrimitive( PrimitiveType.Cube);
			cube.transform.position = pos;
			cube.transform.rotation = Random.rotation;
			cube.transform.localScale = new Vector3(
				Random.Range( MinSize, MaxSize),
				Random.Range( MinSize, MaxSize),
				Random.Range( MinSize, MaxSize));

			var rb = cube.AddComponent<Rigidbody>();
			rb.sleepThreshold = 0;
		}
	}
}
