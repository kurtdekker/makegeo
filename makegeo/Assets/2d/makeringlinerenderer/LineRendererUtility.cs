using UnityEngine;

// @kurtdekker - just some cheap and cheerful LineRenderer-centric utils.

public class LineRendererUtility
{
	// pass in your LineRenderer
	//	- make sure it has a valid material
	//	- control the width of it however you like
	//	- make sure it is set to loop
	public static void MakeRing( LineRenderer lineRenderer, float radius, int segments)
	{
		Vector3[] points = new Vector3[segments];

		for (int i = 0; i < segments; i++)
		{
			float angle = (i * Mathf.PI * 2) / segments;

			float x = Mathf.Cos( angle) * radius;
			float y = Mathf.Sin( angle) * radius;

			points[i] = new Vector3( x, y, 0);
		}

		lineRenderer.positionCount = segments;
		lineRenderer.SetPositions( points);
	}
}
