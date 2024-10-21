using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// @kurtdekker - a quick procgen dominoes demo
//
// let's get familiar with the problem space, shall we?
//
// to use:
//	- make an empty scene with a Main Camera and Light
//	- drop this script on a GameObject (empty or otherwise, doesn't matter)
//	- press PLAY

public class Dominoes : MonoBehaviour
{
	IEnumerator Start ()
	{
		// floor
		GameObject floor = GameObject.CreatePrimitive( PrimitiveType.Plane);
		floor.name = "Floor";
		floor.GetComponent<Renderer>().material.color = new Color( 0.3f, 0.5f, 0.3f);

		// my first eyeball attempt ... about twice the size of a real domino
		float width = 0.030f;
		float height = 0.100f;
		float thickness = 0.010f;

		// between domino centers
		float spacing = 0.040f;

		if (true)
		{
			// based on what ChatGPT told me (48mm, 24mm, 7.5mm) I was not
			// easily able to get the dominoes to consistently fall so I
			// made them about half as thick so they fall over better.

			width = 0.024f;
			height = 0.048f;
			thickness = 0.0075f;

			// perhaps by making them thinner?
			thickness = 0.0040f;

			spacing = 0.04f;
		}

		// they should probably be slippery too
		PhysicMaterial pm = new PhysicMaterial("Domino");
		pm.bounciness = 0.1f;
		pm.staticFriction = 0.5f;
		pm.dynamicFriction = 0.2f;
		pm.bounceCombine = PhysicMaterialCombine.Average;
		pm.frictionCombine = PhysicMaterialCombine.Minimum;

		// make us proud Mr Physics System!!!
		int dominoCount = 100;

		// degrees y rotation
		float heading = 90;

		// degrees per domino
		float curvature = 3.0f;

		// flat on the ground; we'll lift each one
		Vector3 cursor = new Vector3( -1.0f, 0, 0);

		Rigidbody firstDomino = null;

		// the initial kick will be in the direction of the first domino
		Vector3 kickDirection = Quaternion.Euler( 0, heading, 0) * Vector3.forward;

		// how strong of a kick?
		float kickStrength = 5.0f;

		// how far up from center to kick?
		float kickHeight = height / 3;

		float camViewHeight = 10;
		float camViewBack = -10;

		// make the dominoes
		for (int i = 0; i < dominoCount; i++)
		{
			GameObject domino = GameObject.CreatePrimitive( PrimitiveType.Cube);
			domino.name = "Domino #" + (i + 1);

			domino.GetComponent<Collider>().sharedMaterial = pm;

			domino.transform.localScale = new Vector3( width, height, thickness);

			// stand them so the bottom precisely touches the ground
			domino.transform.position = cursor + Vector3.up * height * 0.5f;

			// align to heading
			domino.transform.localRotation = Quaternion.Euler (0, heading, 0);

			// step cursor
			Vector3 stepDistance = Vector3.forward * spacing;
			cursor += Quaternion.Euler( 0, heading, 0) * stepDistance;

			// physics
			Rigidbody rb = domino.AddComponent<Rigidbody>();
			rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
			// note: I'm leaving the default mass of 1.0f so these dominoes
			// are probably made of depleted uranium...

			// TODO: we may want to add some drag / angularDrag AFTER they fall and land to help them sleep?
			rb.drag = 0;
			rb.angularDrag = 0;

			// turn
			float turnAmount = curvature;
			// wiggle the turn a bit
			float theta = i / 5.0f;
			turnAmount += Mathf.Sin( theta) * 8.0f;
			// turn
			heading += turnAmount;

			if (i == 0)
			{
				firstDomino = rb;
			}

			// look at center domino
			if (i == dominoCount / 2)
			{
				Camera cam = Camera.main;

				cam.transform.position = new Vector3( 0, camViewHeight, camViewBack);

				Vector3 lookAt = cursor;
				// fudge forward
				lookAt += Vector3.forward * 0.7f;

				cam.transform.LookAt( lookAt);

				// zoom in a bunch
				cam.fieldOfView = 8.0f;
			}
		}

		// wait for it...

		yield return new WaitForSeconds( 1.0f);

		// kick the first domino
		Vector3 kick = kickDirection * kickStrength;

		// where on the domino do we hit it?
		Vector3 kickPosition = firstDomino.position + Vector3.up * kickHeight;

		firstDomino.AddForceAtPosition( kick, kickPosition);
	}
}
