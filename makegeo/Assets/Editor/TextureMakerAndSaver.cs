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

// This stub is just to make Texture2D assets (not real textures) and save
// them to disk so you can grab them and use them in other projects.

#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;

class TextureMakerAndSaver
{
	[MenuItem("Assets/Create Texture Asset")]
	static void Create () 
	{
		string saveFilePath = 
			EditorUtility.SaveFilePanelInProject("Save Procedural Texture2D", "Procedural Texture2D", "asset", "");

		if (saveFilePath == "") return;

		int w = 8;
		int h = 8;

		var t2d = new Texture2D( w, h, TextureFormat.RGBA32, false);

		for (int j = 0; j < h; j++)
		{
			for (int i = 0; i < w; i++)
			{
				t2d.SetPixel( i, j, Random.ColorHSV());
			}
		}
		t2d.Apply();

		AssetDatabase.CreateAsset( t2d, saveFilePath);
	}
}

#endif
