using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// @kurtdekker - some random cheeseball procgen stuff

public static partial class MakeStuff
{
	// This is a horribly-designed function with
	// nearly-fully-interchangeable arguments.
	//
	//  USE NAMED ARGUMENTS when you call this!
	public static GameObject MakeSpiralStairs(
		int count = 36,
		float stepHeight = 0.1f,
		float degreesPerStep = 10,		// this times count is total spiral
		float depth = 0.3f,				// depth in the direction your foot lands
		float radius = 2,
		float inwardRadius = 0.2f,		// overlap at the core; negative to make an empty core
		int direction = +1				// +1: turn right going up, -1: turn left going up
	)			
	{
		GameObject go = new GameObject("MakeSpiralStairs");

		float angle = 0;
		for (int i = 0; i < count; i++)
		{
			var stepRoot = new GameObject( "StepRoot " + i);
			stepRoot.transform.SetParent( go.transform);

			// up we go, first one at y == 0
			stepRoot.transform.localPosition = Vector3.up * (stepHeight * i);

			// replace with your own prefab if you like
			var step = GameObject.CreatePrimitive( PrimitiveType.Cube);

			// shape it like a step
			step.transform.localScale = new Vector3( radius + inwardRadius, stepHeight, depth);
				
			// hook it to the root rotator
			step.transform.SetParent( stepRoot.transform);

			// move it out into position (might not apply if you offset your prefab nicely)
			step.transform.localPosition = new Vector3(
				(radius - inwardRadius) / 2,
				stepHeight / 2, 0);

			// set rotation at the root, swinging the stair around
			stepRoot.transform.rotation = Quaternion.Euler( 0, angle, 0);

			// and around we go!!
			angle += degreesPerStep * direction;
		}

		return go;
	}
}
