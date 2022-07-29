using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// @kurtdekker - put this on a blank GameObject, press PLAY

public class SpawnInACircle : MonoBehaviour
{
	void Fabricate ()
	{
		// destroy any previous that might have been present
		for (int i = transform.childCount - 1; i >= 0; i--)
		{
			Destroy( transform.GetChild( i).gameObject);
		}

		// the center of our universe
		var cube = GameObject.CreatePrimitive( PrimitiveType.Cube);

		// parent underneath us
		cube.transform.SetParent( transform);

		// move this wherever you like
		cube.transform.position = new Vector3( 0.0f, 0.0f, 0.0f);

		// the radius at which the evenly-spaced spheres will spawn
		float fixedRadius = 2.5f;

		// the radius at which the randomly-spaced capsules will spawn
		float randomRadius = 5.0f;

		// how many things?
		int count = Random.Range( 5, 15);

		for (int i = 0; i < count; i++)
		{
			// we'll spawn spheres at an even fixed spacing:
			var sphere = GameObject.CreatePrimitive( PrimitiveType.Sphere);

			// parent underneath us
			sphere.transform.SetParent( transform);

			// evenly spaced angles all around the compass
			float angle = (i * 360.0f) / count;

			// start from the position of the cube, which we will encircle
			Vector3 centerPosition = cube.transform.position;

			// compute the "up" offset vector
			Vector3 upOffset = Vector3.up * fixedRadius;

			// rotate the "up" offset vector by the angle (around +Z)
			upOffset = Quaternion.Euler( 0, 0, angle) * upOffset;

			// compute and set the sphere to the final position
			sphere.transform.position = centerPosition + upOffset;

			// we'll spawn capsules at irregular (random)
			// spacing, at a different radius
			angle = Random.Range( 0.0f, 360.0f);

			// make the capsule
			var capsule = GameObject.CreatePrimitive( PrimitiveType.Capsule);

			// make them short and squat
			capsule.transform.localScale = new Vector3( 1.0f, 0.5f, 1.0f);

			// parent underneath us
			capsule.transform.SetParent( transform);

			// compute the "up" offset vector
			upOffset = Vector3.up * randomRadius;

			// rotate the "up" offset vector by the angle (around +Z)
			upOffset = Quaternion.Euler( 0, 0, angle) * upOffset;

			// compute and set the capsule to the final position
			capsule.transform.position = centerPosition + upOffset;
		}
	}

	void Start()
	{
		Fabricate();
	}

	void Update()
	{
		if (Input.GetKeyDown( KeyCode.Space))
		{
			Fabricate();
		}
	}
}
