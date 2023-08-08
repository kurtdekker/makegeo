using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// @kurtdekker

public class SolarSystems : MonoBehaviour
{
	public Material baseMaterial;

	public Transform GalaxyPlane;

	GameObject CreateCount( int numPlanets, float maxRadius)
	{
		GameObject root = new GameObject( "root");

		// always make a sun
		{
			GameObject sun = GameObject.CreatePrimitive( PrimitiveType.Sphere);
			sun.transform.SetParent( root.transform);
			sun.transform.localScale = Vector3.one * Random.Range( 1.0f, 2.0f);

			var rndrr = sun.GetComponent<Renderer>();
			if (rndrr)
			{
				Material mtl = new Material( baseMaterial);
				mtl.color = new Color( 1.0f, Random.Range( 0.2f, 1.0f), Random.Range( 0.2f, 0.6f));
				rndrr.material = mtl;
			}
		}

		for (int i = 0; i < numPlanets; i++)
		{
			GameObject planetPivot = new GameObject("Pivot #" + i);
			planetPivot.transform.SetParent( root.transform);

			// orbit
			{
				float angle = Random.Range( 0.0f, 360.0f);
				// random speed, randomly negative / positive
				float angularSpeed = Random.Range( 10.0f, 200.0f) * (Random.Range( 0, 2) * 2 - 1);
				SpinPlanetOnY.Attach( planetPivot, angle: angle, angularSpeed: angularSpeed);
			}


			GameObject planet = GameObject.CreatePrimitive( PrimitiveType.Sphere);
			planet.transform.SetParent( planetPivot.transform);

			float orbitalRadius = (maxRadius * (i + 1)) / numPlanets;
			planet.transform.localPosition = GalaxyPlane.root.forward * orbitalRadius;
			planet.transform.localScale = Vector3.one * Random.Range( 0.2f, 0.8f);

			// spin
			{
				float angle = Random.Range( 0.0f, 360.0f);
				// random speed, randomly negative / positive
				// yes, I know that's not how orbital mechanics work. But it looks cool.
				float angularSpeed = Random.Range( 10.0f, 50.0f) * (Random.Range( 0, 2) * 2 - 1);
				SpinPlanetOnY.Attach( planet, angle: angle, angularSpeed: angularSpeed);
			}

			var rndrr = planet.GetComponent<Renderer>();
			if (rndrr)
			{
				Material mtl = new Material( baseMaterial);
				mtl.color = Random.ColorHSV();
				rndrr.material = mtl;
			}
		}

		return root;
	}

	GameObject CreateRandom()
	{
		int a = Random.Range( 3, 10);
		int b = Random.Range( 3, 10);
		int numPlanets = Mathf.Min(a,b);

		return CreateCount( numPlanets, 10);
	}

	Camera cam;
	void Start()
	{
		cam = Camera.main;
	}

	void Update()
	{
		if (Input.GetMouseButtonDown(0))
		{
			Ray ray = cam.ScreenPointToRay( Input.mousePosition);

			Plane p = new Plane( inNormal: GalaxyPlane.up, inPoint: GalaxyPlane.position);

			float enter = 0;
			if (p.Raycast( ray: ray, enter: out enter))
			{
				Vector3 position = ray.GetPoint( enter);

				var solarSystem = CreateRandom();
				solarSystem.transform.position = position;
			}
		}
	}
}
