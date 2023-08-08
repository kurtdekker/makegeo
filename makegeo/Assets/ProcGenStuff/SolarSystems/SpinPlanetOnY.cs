using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// @kurtdekker - used for both orbital motion and local motion!

public class SpinPlanetOnY : MonoBehaviour
{
	float angle;
	float angularSpeed;

	public static SpinPlanetOnY Attach( GameObject pivot, float angle, float angularSpeed)
	{
		var spinner = pivot.AddComponent<SpinPlanetOnY>();
		spinner.angle = angle;
		// note: calling Update BEFORE setting angular speed, just to set angle
		spinner.Update();
		spinner.angularSpeed = angularSpeed;
		return spinner;
	}

	void Update ()
	{
		angle += angularSpeed * Time.deltaTime;

		angle = angle % 360.0f;

		transform.localRotation = Quaternion.Euler( 0, angle, 0);
	}
}
