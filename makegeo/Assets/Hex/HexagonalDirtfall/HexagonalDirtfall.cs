using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexagonalDirtfall : MonoBehaviour
{
	Camera cam;

	GameObject testSphereRaw;
	GameObject testSphereSnapped;

	HexGrid hg;

	void Start ()
	{
		cam = Camera.main;

		testSphereRaw = GameObject.CreatePrimitive( PrimitiveType.Sphere);
		testSphereSnapped = GameObject.CreatePrimitive( PrimitiveType.Sphere);

		hg = new HexGrid();
	}
	
	void UpdateMouse ()
	{
		bool fingerdown = Input.GetMouseButton(0);

		Vector3 mousePosition = Input.mousePosition;

		if (fingerdown)
		{
			Plane p = new Plane( inNormal: Vector3.back, inPoint: Vector3.zero);

			Ray ray = cam.ScreenPointToRay( mousePosition);

			float enter = 0;
			if (p.Raycast( ray, out enter))
			{
				Vector2 point = ray.GetPoint(enter);

				testSphereRaw.transform.position = point;

				Vector3 snapped = hg.SnapToGrid( point);

				testSphereSnapped.transform.position = snapped;
			}
		}
	}

	void Update()
	{
		UpdateMouse();
	}
}
