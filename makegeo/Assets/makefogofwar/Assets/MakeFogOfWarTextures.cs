#if UNITY_EDITOR

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

// @kurtdekker
//
// To fabricate textures for use in FogOfWar Geometry
//

public static class MakeFogOfWarTextures
{
	const string s_OutputDirectory = "makefogofwar/Assets/";

	[MenuItem("Assets/MakeGeo/Create Fog Of War Textures")]
	static void MakeFOWTextures()
	{
		const int size = 256;

		// this much is either solid (on the left) or clear (on the right)
		const int border = 8;

		// use an alpha / transparent Material for this one:
		Texture2D t2dGradient = new Texture2D(size, size, TextureFormat.RGBA32, mipChain: true);

		// use a solid Material for this one:
		Texture2D t2dSolid = new Texture2D(size, size, TextureFormat.RGBA32, mipChain: true);

		// keep these two in synchrony!
		Color[] Colors = new Color[]
		{
			Color.white,
			Color.black,
		};
		string[] ColorNames = new string[]
		{
			"white",
			"black",
		};

		for (int colorIndex = 0; colorIndex < Colors.Length; colorIndex++)
		{
			for (int j = 0; j < size; j++)
			{
				for (int i = 0; i < size; i++)
				{
					// expand gradient to inner width, excluding borders
					float alphaWidth = size - border * 2;
					float alphaX = (i - border) / alphaWidth;
					alphaX = Mathf.Clamp01(alphaX);

					Color c = Colors[colorIndex];

					t2dSolid.SetPixel(i, j, c);

					// solid on left, clear on right
					float alpha = 1.0f - alphaX;
					c.a = alpha;

					t2dGradient.SetPixel(i, j, c);
				}
			}

			t2dSolid.Apply();
			t2dGradient.Apply();

			string filename = ColorNames[colorIndex];
			string filenameSolid = filename + "_solid" + ".png";
			string filenameGradient = filename + "_gradient" + ".png";
			filenameSolid = System.IO.Path.Combine(Application.dataPath, s_OutputDirectory, filenameSolid);
			filenameGradient = System.IO.Path.Combine(Application.dataPath, s_OutputDirectory, filenameGradient);

			Debug.Log($"filenameSolid = '{filenameSolid}'");
			Debug.Log($"filenameGradient = '{filenameGradient}'");

			// write!
			System.IO.File.WriteAllBytes(filenameSolid, t2dSolid.EncodeToPNG());
			System.IO.File.WriteAllBytes(filenameGradient, t2dGradient.EncodeToPNG());
		}

		// read
		AssetDatabase.Refresh();
	}
}
#endif
