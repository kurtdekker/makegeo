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

// This stub is just to make sprite assets and save them
// to disk so you can grab them and use them in other projects.

#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;

class SpriteMakerAndSaver
{
	[MenuItem("Assets/Create Sprite Asset")]
	static void Create () 
	{
		string saveFilePath = 
			EditorUtility.SaveFilePanelInProject("Save Procedural Sprite", "Procedural Sprite", "asset", "");

		if (saveFilePath == "") return;

		// TODO replace this with your own way of getting the texture in here...
		var t2d = Resources.Load<Texture2D>( "abcd_transparent");

		Rect rectPixels = new Rect(0,0,t2d.width,t2d.height);

		Vector2 pivotUV = Vector2.one / 2;

		Sprite sprite = Sprite.Create( t2d, rectPixels, pivotUV);

		AssetDatabase.CreateAsset( sprite, saveFilePath);
	}
}

#endif
