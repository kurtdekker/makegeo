/*
	The following license supersedes all notices in the source code.

	Copyright (c) 2023 Kurt Dekker/PLBM Games All rights reserved.

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

// Triggered from this forum post:
// https://forum.unity.com/threads/draw-object-add-collider-and-rigidbody-to-it.750794/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testfingerpolygons : MonoBehaviour
{
	public Material mtlPolygon;

	public LineRenderer LR;

	bool fingerDown;
	Vector2 lastFingerPosition;

	Vector2 worldFingerPosition;

	List<Vector2> Points;

	Vector2 ScreenCenter
	{
		get
		{
			return new Vector2( Screen.width, Screen.height) / 2;
		}
	}

	float CameraOrthoSize
	{
		get
		{
			return Camera.main.orthographicSize;
		}
	}

	Vector2 CameraCenterAxis
	{
		get
		{
			return new Vector2( Camera.main.transform.position.x, Camera.main.transform.position.y);
		}
	}

	float MinDistanceToConsiderAnEdge
	{
		get
		{
			return Mathf.Min( Screen.width, Screen.height) / 20f;
		}
	}

	void UpdateReadFingerCreatePoints()
	{
		var touches = MicroTouch.GatherMicroTouches();

		if (touches.Length < 1)
		{
			return;
		}

		var mt = touches[0];

		Vector2 fingerPos = mt.position;

		// NOTE: this works because we're using an orthographic camera.
		// Per Unity's camera setup, orthoSize is half the screen height.

		// If you are using a perspective camera, cast a ray into the scene
		// to where your zero plane is and get the position that way.

		fingerPos -= ScreenCenter;
		worldFingerPosition = (fingerPos * CameraOrthoSize) / (Screen.height / 2);
		worldFingerPosition += CameraCenterAxis;

		if (mt.phase == TouchPhase.Began)
		{
			fingerDown = true;

			lastFingerPosition = fingerPos;

			Points = new List<Vector2>();

			Points.Add( worldFingerPosition);
		}

		if (fingerDown)
		{
			if (mt.phase.isDown())
			{
				float distance = Vector2.Distance( fingerPos, lastFingerPosition);

				if (distance > MinDistanceToConsiderAnEdge)
				{
					Points.Add( worldFingerPosition);
					lastFingerPosition = fingerPos;
				}
			}
			else
			{
				if (mt.phase.cameUp())
				{
					if (Points.Count > 2)
					{
						Points.Add( worldFingerPosition);

						var go = MakeCollider2D.Create( Points.ToArray(), DoubleSided: true);

						go.GetComponent<Renderer>().material = mtlPolygon;

						go.AddComponent<Rigidbody2D>();
					}

					fingerDown = false;
					Points = null;
				}
			}
		}
	}

	List<Vector2> LRPoints;

	void UpdateDriveLineRenderer()
	{
		if (Points == null)
		{
			LR.positionCount = 0;
			LR.enabled = false;
			return;
		}

		LR.enabled = true;

		// what we have so far plus your finger
		int numPoints = Points.Count + 1;

		if (numPoints != LR.positionCount)
		{
			LRPoints = new List<Vector2>( Points);
			LRPoints.Add( worldFingerPosition);
		}

		LR.positionCount = numPoints;

		LRPoints[ LRPoints.Count - 1] = worldFingerPosition;

		for (int i = 0; i < LRPoints.Count; i++)
		{
			LR.SetPosition( i, LRPoints[i]);
		}
	}

	void Update()
	{
		UpdateReadFingerCreatePoints();

		UpdateDriveLineRenderer();
	}
}
