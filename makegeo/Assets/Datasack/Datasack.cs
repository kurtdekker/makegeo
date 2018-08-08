/*
    The following license supersedes all notices in the source code.
*/

/*
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

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu]
public partial class Datasack : ScriptableObject
{
	public	string	InitialValue;

	public	bool	Save;

	public	delegate	void	OnValueChangedDelegate( Datasack ds);

	[NonSerialized]
	public	OnValueChangedDelegate	OnChanged;

	[NonSerialized]
	public	OnValueChangedDelegate	OnChangedOnceOnly;

	void OnEnable()
	{
		bool holdBreak = DebugBreak;
		bool holdLogging = DebugLogging;

		DebugBreak = false;
		DebugLogging = false;

		Value = InitialValue;

		if (Save)
		{
			Value = PlayerPrefs.GetString (
				DSM.s_PlayerPrefsPrefix + name.ToLower(), Value);
		}

		DebugBreak = holdBreak;
		DebugLogging = holdLogging;
	}

	[NonSerialized] private	string	TheData;

	public	void	Poke()
	{
		if (OnChanged != null)
		{
			OnChanged.Invoke (this);
		}

		var call = OnChangedOnceOnly;
		if (call != null)
		{
			OnChangedOnceOnly = null;
			call(this);
		}
	}

	public	string	Value
	{
		get
		{
			return TheData;
		}
		set
		{
			if (DebugLogging)
			{
				Debug.Log( "Datasack " + name + " changed: '" + TheData + "' to '" + value + "'");
			}

			if (DebugBreak)
			{
				Debug.LogWarning( "Datasack " + name + ": set to DebugBreak");
				Debug.Break();
			}

			TheData = value;

			Poke();

			if (Save)
			{
				#if UNITY_EDITOR
				if (EditorApplication.isPlaying)
				#endif
					DSM.I.SetDirty();
			}
		}
	}
}
