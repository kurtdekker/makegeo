/*
	The following license supersedes all notices in the source code.

	Copyright (c) 2022 Kurt Dekker/PLBM Games All rights reserved.

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
	[Multiline]
	public	string	InitialValue;

	public	bool	Save;

	public	delegate	void	OnValueChangedDelegate( Datasack ds);

	[NonSerialized]
	public	OnValueChangedDelegate	OnChanged;

	[NonSerialized]
	public	OnValueChangedDelegate	OnChangedOnceOnly;

	// This name is the name of the Datasack object itself (.name) but
	// also prefixed with the path location so it is unique. It must be
	// unique for retrieval from the master dictionary, and for
	// persistent saving/loading to PlayerPrefs.
	[NonSerialized]
	public	string		FullName;

	public void LoadPersistent()
	{
		TheData = InitialValue;
		if (Save)
		{
			string s_PrefsKey = DSM.s_PlayerPrefsPrefix + FullName;
			Value = PlayerPrefs.GetString (s_PrefsKey, Value);
		}
	}

	[NonSerialized] private	string	TheData;

	public	void	iPoke( int i)
	{
		Poke( i.ToString());
	}
	public	void	Poke( string message = null)
	{
		if (message != null)
		{
			TheData = message;
		}

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

	// Clears it to blank / zero / false
	public	void	Clear()
	{
		Value = "";
	}

	// Resets it to the specified InitialValue
	public	void	Reset()
	{
		Value = InitialValue;
	}

	public	string	Value
	{
		get
		{
			if (TheData == null)
			{
				TheData = "";
			}
			return TheData;
		}
		set
		{
			TheData = value;

#if UNITY_EDITOR
			if (DebugLogging)
			{
				Debug.Log( "Datasack " + FullName + " changed:\n" +
					DisplayDebugOutput());
			}

			if (DebugBreak)
			{
				Debug.LogWarning( "Datasack " + FullName + ": set to DebugBreak");
				Debug.Break();
			}
#endif

			Poke();

			if (Save)
			{
				#if UNITY_EDITOR
				if (EditorApplication.isPlaying)
				#endif
					DSM.SetDirty();
			}
		}
	}
}
