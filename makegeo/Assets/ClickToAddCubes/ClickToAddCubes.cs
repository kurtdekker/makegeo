using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// @kurtdekker
// to use in the simplest form:
//	- make a blank scene with camera / light
//	- drop this script on a blank GameObject
//	- press PLAY
//
// Everything else is optional. See comments / headers
//
// Works for mobile touch or mouse (see MicroTouch class).

public class ClickToAddCubes : MonoBehaviour
{
	[Header( "Provide or we'll guess.")]
	public float CubeSize = 1;

	[Header( "Provide it or we guess.")]
	public Camera cam;

	[Header( "Cube or else we use the default:")]
	public GameObject CubePrefab;

	[Header( "Optional place to parent everything.")]
	public Transform Parent;

	[Header( "Central camera pivot, if you want cam control.")]
	public Transform CenterCameraPivot;

	[Header( "Map of screen fraction to degrees:")]
	public float ScrollSensitivity = 1000;

	void AddCube ( Vector3 position)
	{
		GameObject copy = null;

		if (CubePrefab)
		{
			Instantiate<GameObject>( CubePrefab, Parent);
		}
		else
		{
			copy = GameObject.CreatePrimitive( PrimitiveType.Cube);
			copy.transform.localScale = Vector3.one * CubeSize;
		}

		copy.transform.position = position;
		copy.transform.rotation = Quaternion.identity;
	}

	void Start()
	{
		if (!cam)
		{
			cam = Camera.main;
		}

		if (CubeSize == 0)
		{
			CubeSize = 1.0f;
		}

		// kick it off with one in the center!!
		AddCube( Vector3.zero);
	}

	bool fingerDown;
	Vector3 fingerStartPosition;
	Vector3 fingerPreviousPosition;

	// kinda means 'rotating' or 'scrolling' I suppose...
	bool moving;

	void UpdateTouchAndMouse()
	{
		var touches = MicroTouch.GatherMicroTouches();
		if (touches.Length < 1)
		{
			return;
		}

		var mt = touches[0];

		if (mt.phase.isDown())
		{
			if (!fingerDown)
			{
				moving = false;
				fingerStartPosition = mt.position;
				fingerPreviousPosition = mt.position;
			}

			float distance = Vector3.Distance( mt.position, fingerStartPosition);

			if (distance >= MR.MINAXIS * 0.05f)
			{
				moving = true;
			}

			if (moving)
			{
				// handle camera rotation?
				if (CenterCameraPivot)
				{
					Vector3 delta = mt.position - fingerPreviousPosition;

					// scale down to minaxis screen pixels (normalize to minaxis)
					delta /= MR.MINAXIS;

					// scale up to sensitivity
					delta *= ScrollSensitivity;

					// do the rotation
					CenterCameraPivot.Rotate( cam.transform.right * -delta.y);
					CenterCameraPivot.Rotate( cam.transform.up * +delta.x);
				}
			}

			fingerDown = true;
		}
		else
		{
			if (fingerDown)
			{
				// were we scrolling? if so, that's not a click
				if (moving)
				{
					// nothing to do
				}
				else
				{
					// we were not scrolling; this is a click
					Ray ray = cam.ScreenPointToRay(mt.position);
					RaycastHit hit;
					if (Physics.Raycast( ray: ray, hitInfo: out hit))
					{
						Vector3 center = hit.transform.position;

						Vector3 direction = hit.normal;

						Vector3 position = center + direction.normalized * CubeSize;

						AddCube( position);
					}
					else
					{
						Debug.LogWarning( "Didn't click on anything!");
					}
				}
			}
			fingerDown = false;
		}

		fingerPreviousPosition = mt.position;
	}

	void Update()
	{
		UpdateTouchAndMouse();
	}
}
