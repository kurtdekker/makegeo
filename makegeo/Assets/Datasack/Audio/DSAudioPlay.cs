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
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class DSAudioPlay : MonoBehaviour
{
	public	Datasack	dataSack;

	private AudioSource[] azzs;

	public enum PlayStrategy
	{
		RANDOM,
		SEQUENCE,
		ALLATONCE,
		SHUFFLE,
	}
	public PlayStrategy Strategy;

	private int lastPlayed;

	void	OnChanged( Datasack ds)
	{
		// NOTE: does nothing with ds!!

		if (Strategy == PlayStrategy.ALLATONCE)
		{
			foreach( var az in azzs)
			{
				az.Play();
			}
			return;
		}

		if (Strategy == PlayStrategy.RANDOM)
		{
			lastPlayed = Random.Range( 0, azzs.Length);
		}

		azzs[lastPlayed].Play();

		// done after the .Play() so we get 0 played first
		if ((Strategy == PlayStrategy.SEQUENCE) ||
			(Strategy == PlayStrategy.SHUFFLE))
		{
			lastPlayed++;
			if (lastPlayed >= azzs.Length)
			{
				lastPlayed = 0;
				if (Strategy == PlayStrategy.SHUFFLE)
				{
					Shuffle();
				}
			}
		}
	}

	void	Shuffle()
	{
		for (int i = 0; i < azzs.Length; i++)
		{
			int j = Random.Range( i, azzs.Length);
			if (i != j)
			{
				var t = azzs[i];
				azzs[i] = azzs[j];
				azzs[j] = t;
			}
		}
	}

	void	OnEnable()
	{
		azzs = GetComponentsInChildren<AudioSource>();
		dataSack.OnChanged += OnChanged;

		if (Strategy == PlayStrategy.SHUFFLE)
		{
			Shuffle();
		}
	}
	void	OnDisable()
	{
		dataSack.OnChanged -= OnChanged;	
	}

#if UNITY_EDITOR
	[CustomEditor( typeof( DSAudioPlay))]
	public class DSAudioPlayEditor : Editor
	{
		public override void OnInspectorGUI()
		{
			var play = (DSAudioPlay)target;

			DrawDefaultInspector();

			EditorGUILayout.BeginVertical();

			if (GUILayout.Button( " PLAY AUDIO "))
			{
				play.OnChanged(null);
			}

			EditorGUILayout.EndVertical();
		}
	}
#endif
}
