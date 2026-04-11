
// @kurtdekker

// This stub is just to make a combination Texture2D and sprite
// asset and save theat to disk.

#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;

class TextureAndSpriteMakerAndSaver
{
	[MenuItem("Assets/MakeGeo/Create Texture+Sprite Asset")]
	static void Create ()
	{
		string saveFilePath = 
			EditorUtility.SaveFilePanelInProject("Save Procedural TextureSprite", "Procedural TextureSprite", "asset", "");

		if (saveFilePath == "") return;

		// first make the texture

		int w = 8;
		int h = 8;

		var t2d = new Texture2D( w, h, TextureFormat.RGBA32, false);

		t2d.filterMode = FilterMode.Point;

		for (int j = 0; j < h; j++)
		{
			for (int i = 0; i < w; i++)
			{
				t2d.SetPixel( i, j, Random.ColorHSV());
			}
		}
		t2d.Apply();

		// now make the sprite

		Rect rectPixels = new Rect(0,0,t2d.width,t2d.height);

		Vector2 pivotUV = Vector2.one / 2;

		Sprite sprite = Sprite.Create( t2d, rectPixels, pivotUV);

		// make the texture permanent
		AssetDatabase.CreateAsset( t2d, saveFilePath);

		// add the sprite to the texture
		AssetDatabase.AddObjectToAsset( sprite, t2d);

		AssetDatabase.SaveAssets();
	}
}

#endif
