/*
	The following license supersedes all notices in the source code.

	Copyright (c) 2018 Kurt Dekker/PLBM Games All rights reserved.

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
using UnityEngine.UI;

public class DSKeycodeInput : MonoBehaviour
{
	[Tooltip("Defaults to UISack datavar if none supplied.")]
	public	Datasack	dataSack;

	public	enum KeyActivity
	{
		DOWN,
		UP,
		HOLD,
	}

	[Tooltip( "List of keycodes you want tracked.")]
	public	KeyCode[]	KeysToTrack;

	[Tooltip( "What type of key activity to track.")]
	public	KeyActivity	Activity;

	[Tooltip( "Leave blank to send the Keycode.ToString()")]
	public	string		Output;

	[Tooltip( "Check box to send GameObject.name")]
	public	bool		SendGameObjectName;

	void	Reset()
	{
		KeysToTrack = new KeyCode[] {
			KeyCode.Return,
			KeyCode.Space,
		};

		Activity = KeyActivity.DOWN;

		Output = "";

		SendGameObjectName = false;

		if (GetComponent<Button>())
		{
			SendGameObjectName = true;
		}
	}

	void	Update()
	{
		// default is KeyActivity.DOWN
		System.Func<KeyCode,bool> InputFunction = Input.GetKeyDown;

		switch( Activity)
		{
		case KeyActivity.UP :
			InputFunction = Input.GetKeyUp;
			break;
		case KeyActivity.HOLD :
			InputFunction = Input.GetKey;
			break;
		}

		bool triggered = false;
		string tempOutput = "";

		foreach( var key in KeysToTrack)
		{
			if (InputFunction( key))
			{
				triggered = true;
				tempOutput = key.ToString();
				break;
			}
		}

		if (triggered)
		{
			if (Output != null && Output.Length > 0)
			{
				tempOutput = Output;
			}

			if (SendGameObjectName)
			{
				tempOutput = name;
			}

			var ds = DSM.UserIntent;
			if (dataSack) ds = dataSack;

			ds.Value = tempOutput;
		}
	}
}
