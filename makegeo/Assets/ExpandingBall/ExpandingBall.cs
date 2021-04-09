/*
	The following license supersedes all notices in the source code.

	Copyright (c) 2021 Kurt Dekker/PLBM Games All rights reserved.

	http://www.twitter.com/kurtdekker

	Redistribution and use in source and binary forms, with or without
	modification, are permitted provided that the following conditions are
	met:

	Redistributions of source code must retain the above copyright notice,
	this list of conditions and the following disclaimer.

	Redistributions in binary form must reproduce the above copyright
	notice, this list of conditions and the following disclaimer in the
	documentation and/or other materials provided with the distribution.

	Neither the name of the Kurt Dekker/PLBM Games nor the names of its
	contributors may be used to endorse or promote products derived from
	this software without specific prior written permission.

	THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS
	IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED
	TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A
	PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT
	HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL,
	SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED
	TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR
	PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF
	LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING
	NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
	SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExpandingBall : MonoBehaviour
{
	static IEnumerator ExplodeOutwards( GameObject unitSphere, float maxScale, float interval)
	{
		float minScale = 1.0f;

		Collider c = unitSphere.GetComponent<Collider>();
		bool savedColliderEnabled = false;
		if (c)
		{
			savedColliderEnabled = c.enabled;
			c.enabled = false;
		}

		float fraction = 0.0f;
		float time = 0.0f;

		MeshFilter mf = unitSphere.GetComponent<MeshFilter>();
		Mesh mesh = mf.mesh;

		Vector3[] OriginalVertices = new Vector3[ mesh.vertexCount];
		System.Array.Copy( mesh.vertices, OriginalVertices, mesh.vertexCount);

		Ray[] OriginalRays = new Ray[ mesh.vertexCount];
		for (int i = 0; i < mesh.vertexCount; i++)
		{
			OriginalRays[i] = new Ray( unitSphere.transform.position, OriginalVertices[i]);
		}

		Vector3[] WorkVertices = new Vector3[ mesh.vertexCount];

		do
		{
			time += Time.deltaTime;
			fraction = time / interval;

			float distance = Mathf.Lerp( minScale, maxScale, fraction);

			for (int i = 0; i < OriginalVertices.Length; i++)
			{
				Ray ray = OriginalRays[i];

				Vector3 worldPoint = Vector3.zero;

				RaycastHit rch;
				if (Physics.Raycast( ray, out rch, distance))
				{
					worldPoint = rch.point;
				}
				else
				{
					worldPoint = ray.GetPoint( distance);
				}

				Vector3 localPoint = unitSphere.transform.InverseTransformPoint( worldPoint);

				WorkVertices[i] = localPoint;
			}

			mesh.vertices = WorkVertices;

			mesh.RecalculateNormals();

			mf.mesh = mesh;

			yield return null;
		}
		while( fraction < 1.0f);

		if (c) c.enabled = savedColliderEnabled;

		yield return null;
	}

	// running / testing the above

	public UnityEngine.UI.Text TextSize;

	public Material BallMaterial;

	void Start()
	{
		SetMaximumSize( 3);
	}

	float MaximumSize;

	void SetMaximumSize(int sz)
	{
		MaximumSize = sz;

		TextSize.text = "Size:" + MaximumSize; 
	}

	List<GameObject> balls = new List<GameObject>();

	void Update ()
	{
		if (Input.GetMouseButtonDown(0))
		{
			Ray ray = Camera.main.ScreenPointToRay( Input.mousePosition);
			RaycastHit rch;
			if (Physics.Raycast( ray, out rch, 1000))
			{
				GameObject ball = GameObject.CreatePrimitive( PrimitiveType.Sphere);

				ball.transform.position = rch.point;

				ball.GetComponent<Renderer>().material = BallMaterial;

				StartCoroutine( ExplodeOutwards(
					ball,
					maxScale: MaximumSize,
					interval: 1.0f)
				);

				balls.Add( ball);
			}
		}

		for (int i = 2; i < 10; i++)
		{
			var kc = (KeyCode)(KeyCode.Alpha0 + i);
			if (Input.GetKeyDown(kc))
			{
				SetMaximumSize( i);
			}
		}

		if (Input.GetKeyDown(KeyCode.R))
		{
			foreach( var ball in balls)
			{
				Destroy(ball);
			}
			balls.Clear();
		}
	}
}
