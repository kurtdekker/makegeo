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

// If you don't provide one of these in your scene, the activation of any
// script that implements the IDatasack3DSensor to your scene should
// automatically add one of these and default it to Camera.main.

public class Datasack3DManager : MonoBehaviour
{
	[Tooltip( "You may override this or it will presume Camera.main at runtime.")]
	public Camera cam;

	static Datasack3DManager _Instance;

	void Awake()
	{
		if (_Instance)
		{
			if (this != _Instance)
			{
				Destroy( this);
				return;
			}
		}
		_Instance = this;

		if (cam == null)
		{
			cam = Camera.main;
		}
	}

	public static Datasack3DManager Instance
	{
		get
		{
			if (!_Instance)
			{
				_Instance = new GameObject( "Datasack3DManager.Create();").AddComponent<Datasack3DManager>();
			}
			return _Instance;
		}
	}

	void UpdateSimpleMouseInput()
	{
		bool clicked = false;
		Vector3 position = Vector3.zero;

		if (Input.GetMouseButtonDown(0))
		{
			clicked = true;
		}

		// <WIP> option to handle Input.GetMouseButtonDown(0) instead?
		// <WIP> handle must-touch--first-then-release enforcement
		// <WIP> these choices might likely want to come from the specific
		// objects we touched, which means we need to get them first before
		// we even know if we care about them... It's complicated.

		if (clicked)
		{
			position = Input.mousePosition;

			Ray ray = cam.ScreenPointToRay( position);
			RaycastHit rch;
			if (Physics.Raycast( ray, out rch))
			{
				MonoBehaviour[] allBehaviors = rch.collider.gameObject.GetComponents<MonoBehaviour>();
				foreach( var mb in allBehaviors)
				{
					IDatasackTouchable sensor = mb as IDatasackTouchable;
					if (sensor != null)
					{
						sensor.Touched();
					}
				}
			}
		}
	}

	void Update ()
	{
		UpdateSimpleMouseInput();
	}
}
