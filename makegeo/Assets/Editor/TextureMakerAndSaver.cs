
// @kurtdekker

// This stub is just to make Texture2D assets (not real textures) and save
// them to disk so you can grab them and use them in other projects.

#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;

class TextureMakerAndSaver
{
	[MenuItem("Assets/MakeGeo/Create Texture Asset")]
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
