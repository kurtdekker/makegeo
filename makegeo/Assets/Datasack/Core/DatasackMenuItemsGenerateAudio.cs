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

#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public static partial class DatasackMenuItems
{
	// To use: select all the AudioClip(s) you want first, then hit this
	[MenuItem("Assets/GenerateAudioDatasacks")]
	static void GenerateAudioDatasacks()
	{
		foreach (var asset in Selection.objects)
		{
			var clip = asset as AudioClip;

			// open the AudioClip asset
			if (clip)
			{
				// get its name
				var nm = clip.name;

				Debug.Log("Processing asset named '" + nm + "'...");

				var path = AssetDatabase.GetAssetPath(clip);

				Debug.Log( "path = '" + path + "'");

				var directory = System.IO.Path.GetDirectoryName( path);

				// make a Datasack by that name
				var ds = ScriptableObject.CreateInstance<Datasack>();
				ds.name = nm;
				var nmAsset = nm + ".asset";
				var dsPath = System.IO.Path.Combine( directory, nmAsset);
				AssetDatabase.CreateAsset( ds, dsPath);

				// make a GameObject with:
				//	- AudioSource
				//	- hook up the AudioClip
				//	- add a DSAudioPlay
				//	- connect the Datasack
				var go = new GameObject( nm);
				var azz = go.AddComponent<AudioSource>();
				azz.clip = clip;
				azz.bypassListenerEffects = true;
				azz.playOnAwake = false;
				var play = go.AddComponent<DSAudioPlay>();
				play.dataSack = ds;

				// TODO:
				// dirty the scene
				// trigger a codegen directly!
			}
			else
			{
				Debug.LogError( "Warning: asset '" + asset.name + "' was NOT an AudioClip!");
			}
		}

		AssetDatabase.Refresh();
	}
}
#endif
