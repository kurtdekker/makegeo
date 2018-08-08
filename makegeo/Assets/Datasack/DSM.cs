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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class DSM : MonoBehaviour
{
	public static bool shuttingDown { get; private set; }

	static	DSM	_I;

	public	const	string	s_PlayerPrefsPrefix = "DataBag_";

	public	static	DSM I
	{
		get
		{
			if (!_I)
			{
				_I = new GameObject ("DatasackManager(DSM.I)").AddComponent<DSM> ();

				DontDestroyOnLoad (_I.gameObject);

				_I.AllSacks = new Dictionary<string, Datasack> ();

				Datasack[] sacks = Resources.LoadAll<Datasack>( "");

				foreach (var sack in sacks)
				{
					_I.AllSacks [sack.name.ToLower()] = sack;
				}
			}
			return _I;
		}
	}

	void	WriteToPlayerPrefs()
	{
		if (AllSacks != null)
		{
			foreach( var kvp in AllSacks)
			{
				if (kvp.Value.Save)
				{
					PlayerPrefs.SetString (s_PlayerPrefsPrefix + kvp.Key.ToLower(), kvp.Value.Value);
				}
			}
			PlayerPrefs.Save();
		}
	}

	void	OnDisable()
	{
		shuttingDown = true;

		WriteToPlayerPrefs();
	}

	void	Awake()
	{
		if (_I)
		{
			Destroy (this);
		}
	}

	public	void	SetDirty()
	{
		dirty = true;
	}

	bool	dirty;
	void	Update()
	{
		if (dirty)
		{
			dirty = false;
			WriteToPlayerPrefs();
		}
	}

	Dictionary<string,Datasack> AllSacks;

	public	Datasack	Get( string sackname, bool AutoAdd = false)
	{
		sackname = sackname.ToLower();

		// creating Datasack on the fly
		if (!AllSacks.ContainsKey( sackname))
		{
			if (AutoAdd)
			{
				AllSacks [sackname] = ScriptableObject.CreateInstance<Datasack> ();
			}
			else
			{
				Debug.LogError( GetType()+".Get(): Datasack '" + sackname + "' does not exist. Set AutoAdd = true to add at runtime.");
			}
		}
		return AllSacks [sackname];
	}
}
