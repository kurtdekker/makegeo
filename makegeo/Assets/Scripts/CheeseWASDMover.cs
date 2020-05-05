/*
	The following license supersedes all notices in the source code.

	Copyright (c) 2019 Kurt Dekker/PLBM Games All rights reserved.

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

public class CheeseWASDMover : MonoBehaviour
{
	public enum CheeseWASDViewType
	{
		FirstPerson,
		IsometricView,
		ThirdPerson,
	}

	CheeseWASDViewType viewType;

	const float FPSEyeHeight = 1.5f;
	const float FPSUpDownGazeLimit = 60.0f;

	const float MoveSpeed = 30.0f;

	const float LateralMouse = 500.0f;
	const float VerticalMouse = 400.0f;

	Camera cam;
	float heading;

	float UpDown;

	Transform CamAnchor;

	// in case anyone wants to know how we're moving, such as for tossing weapons
	public Vector3 LastVelocity { get; private set; }

	public static CheeseWASDMover Create( Camera cam, Vector3 position, float heading, CheeseWASDViewType viewType)
	{
		var cheese = new GameObject("CheeseWASDMover.Create()").AddComponent<CheeseWASDMover>();

		cheese.viewType = viewType;

		cheese.transform.position = position;
		cheese.heading = heading;
		cheese.cam = cam;

		cheese.CamAnchor = new GameObject("CamAnchor").transform;
		cheese.CamAnchor.SetParent( cheese.transform);
		cheese.CamAnchor.localPosition = Vector3.up * FPSEyeHeight;
		cheese.CamAnchor.localRotation = Quaternion.identity;

		switch( viewType)
		{
		default :
			throw new System.NotImplementedException( "Only have FirstPerson implemented so far.");

		case CheeseWASDViewType.FirstPerson :
			cam.transform.SetParent( cheese.CamAnchor);
			cam.transform.localPosition = Vector3.zero;
			break;
		}

		cheese.UpdateCameraFacing();

		return cheese;
	}

	void UpdateMouse()
	{
		float mx = Input.GetAxis( "Mouse X");
		float my = Input.GetAxis( "Mouse Y");

		my *= -1;

		mx *= LateralMouse;
		my *= VerticalMouse;

		heading += mx * Time.deltaTime;
		UpDown += my * Time.deltaTime;

		if (UpDown < -FPSUpDownGazeLimit) UpDown = -FPSUpDownGazeLimit;
		if (UpDown >  FPSUpDownGazeLimit) UpDown =  FPSUpDownGazeLimit;
	}

	void UpdateMotion()
	{
		var movement = new Vector3( Input.GetAxis( "Horizontal"), 0, Input.GetAxis( "Vertical"));

		movement = Quaternion.Euler( 0, heading, 0) * movement;

		movement *= MoveSpeed;

		LastVelocity = movement;

		transform.position += movement * Time.deltaTime;

		Ray ray = new Ray( transform.position + Vector3.up * 2.0f, Vector3.down);
		RaycastHit rch;
		if (Physics.Raycast( ray, out rch, 100))
		{
			float yHeight = rch.point.y;

			Vector3 position = transform.position;
			//position.y = Mathf.Lerp( position.y, yHeight, Time.deltaTime * 10);
			position.y = yHeight;
			transform.position = position;
		}
	}

	void UpdateCameraFacing()
	{
		cam.transform.localRotation = Quaternion.Euler( UpDown, heading, 0);
	}

	int dropFrames = 1;

	void Update()
	{
		if (dropFrames > 0)
		{
			dropFrames--;
			return;
		}

		UpdateMouse();

		UpdateMotion();

		UpdateCameraFacing();
	}
}
