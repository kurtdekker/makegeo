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

using UnityEngine;
using System.Collections;

public static class Extensions
{
	public static bool isDown( this TouchPhase phase)
	{
		return (phase == TouchPhase.Began) ||
			(phase == TouchPhase.Moved) ||
			(phase == TouchPhase.Stationary);
	}
	public static bool cameUp( this TouchPhase phase)
	{
		return (phase == TouchPhase.Ended) ||
			(phase == TouchPhase.Canceled);
	}
}

public class MicroTouch
{
	public int fingerId;
	public TouchPhase phase;
	public Vector3 position;

	public static MicroTouch[] GatherMicroTouches()
	{
		// decide if we need extra room for mouse touches
		int mouseTouches = 0;
		bool includeMouse0 = false;
		bool includeMouse1 = false;
		switch (Application.platform)
		{
		case RuntimePlatform.WindowsEditor:
		case RuntimePlatform.WindowsPlayer:
		case RuntimePlatform.OSXEditor:
		case RuntimePlatform.OSXPlayer:
		case RuntimePlatform.WebGLPlayer :
			if (Input.GetMouseButton(0) || Input.GetMouseButtonUp (0))
			{
				includeMouse0 = true;
				mouseTouches++;
			}
			if (Input.GetMouseButton(1) || Input.GetMouseButtonUp (1))
			{
				includeMouse1 = true;
				mouseTouches++;
			}
			break;
		}
		
		int numTouches = Input.touches.Length;

		numTouches += mouseTouches;

		MicroTouch[] mts = new MicroTouch[numTouches];

		int n = 0;

		if (includeMouse0)
		{
			MicroTouch mt = new MicroTouch();
			mt.fingerId = -99;
			mt.position = Input.mousePosition;
			mt.phase = TouchPhase.Moved;
			if (Input.GetMouseButtonDown(0))
			{
				mt.phase = TouchPhase.Began;
			}
			if (Input.GetMouseButtonUp(0))
			{
				mt.phase = TouchPhase.Ended;
			}
			mts[n++] = mt;
		}

		if (includeMouse1)
		{
			MicroTouch mt = new MicroTouch();
			mt.fingerId = -98;
			mt.position = Input.mousePosition;
			mt.phase = TouchPhase.Moved;
			if (Input.GetMouseButtonDown(1))
			{
				mt.phase = TouchPhase.Began;
			}
			if (Input.GetMouseButtonUp(1))
			{
				mt.phase = TouchPhase.Ended;
			}
			mts[n++] = mt;
		}

		foreach (Touch t in Input.touches)
		{
			MicroTouch mt = new MicroTouch();
			mt.fingerId = t.fingerId;
			mt.position = t.position;
			mt.phase = t.phase;
			mts[n++] = mt;
		}
		
		return mts;
	}
}
