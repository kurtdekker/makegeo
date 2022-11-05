using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// @kurtdekker - this is a C# port of code found at:
//
// https://answers.unity.com/questions/9583/carving-a-mesh.html
//
// the purpose appears to be to show a triangle you hover over
//
// make sure you have gizmos enabled for Game window or you won't see anything.

public class ClickTriangle : MonoBehaviour
{
	void Update ()
	{
		RaycastHit hit;

		var camera = Camera.main;

		if (!Physics.Raycast (camera.ScreenPointToRay(Input.mousePosition), out hit))
		{
			return;
		}

		// Just in case, also make sure the collider also has a renderer material and texture
		var meshCollider = hit.collider as MeshCollider;

		if (meshCollider == null || meshCollider.sharedMesh == null)
			return;
		
		var mesh = meshCollider.sharedMesh;
		var vertices = mesh.vertices;
		var triangles = mesh.triangles;

		// Extract local space vertices that were hit
		var p0 = vertices[triangles[hit.triangleIndex * 3 + 0]];
		var p1 = vertices[triangles[hit.triangleIndex * 3 + 1]];
		var p2 = vertices[triangles[hit.triangleIndex * 3 + 2]];

		// Transform local space vertices to world space
		var hitTransform = hit.collider.transform;
		p0 = hitTransform.TransformPoint(p0);
		p1 = hitTransform.TransformPoint(p1);
		p2 = hitTransform.TransformPoint(p2);

		// Display with Debug.DrawLine
		Debug.DrawLine(p0, p1);
		Debug.DrawLine(p1, p2);
		Debug.DrawLine(p2, p0);
	}
}
