using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

// to use:
//	create blank GameObject in scnee
//	put this Bitmap2Grid script on it
//	create texture
//		- no compression
//		- non power of two : NONE
//		- mark read/write enabled
//		- slot texture into Bitmap2Grid script
//	create B2GConfig scriptable object
//		- select colors and prefabs
//		- slot B2GConfig instance into Bitmap2Grid script
//	choose dimensional spacing (defaults to 1,1)
//	go to inspector window for this script, hit GENERATE
//
// @kurtdekker
//

public class Bitmap2Grid : MonoBehaviour
{
	[Header( "Texture to use for layout.")]
	public Texture2D Bitmap;

	[Header( "B2GConfig to use.")]
	public B2GConfig Config;

	[Header( "Dimensional spacing.")]
	public Vector2 Spacing = Vector3.one;

	// do your own axis remapping here if you want X/Z for instance
	Vector3 ComputePosition( int i, int j)
	{
		float x = i * Spacing.x;
		float y = j * Spacing.y;

		// optionally center?
		x -= (Bitmap.width - 1) * Spacing.x / 2;
		y -= (Bitmap.height - 1) * Spacing.y / 2;

		return new Vector3( x, y, 0);
	}

	void RegenerateGrid()
	{
		// TODO: destroy the old one... I'll leave you to do this by hand!

		Transform parent = new GameObject( "Bitmap2Grid-Generated").transform;
		parent.SetParent( transform);
		parent.localPosition = Vector3.zero;
		parent.localRotation = Quaternion.identity;

		for (int j = 0; j < Bitmap.height; j++)
		{
			for (int i = 0; i < Bitmap.width; i++)
			{
				// if you get an error here, check your texture import settings to be read/write
				var c = Bitmap.GetPixel( i, j);

				var prefab = Config.LookupColor( c);

				if (prefab)
				{
					var copy = Instantiate<GameObject>( prefab, parent);
					copy.transform.localPosition = ComputePosition( i, j);

					// demonstration of putting integer cell coorinates
					GridCoordinates coordinates = copy.AddComponent<GridCoordinates>();
					coordinates.CellX = i;
					coordinates.CellY = j;
				}
			}
		}
	}

#if UNITY_EDITOR
	[CustomEditor( typeof( Bitmap2Grid))]
	public class Bitmap2GridEditor : Editor
	{
		public override void OnInspectorGUI()
		{
			Bitmap2Grid b2g = (Bitmap2Grid)target;

			DrawDefaultInspector();

			EditorGUILayout.BeginVertical();

			if (GUILayout.Button( "REGENERATE GRID"))
			{
				b2g.RegenerateGrid();

				EditorSceneManager.MarkSceneDirty(
					UnityEngine.SceneManagement.SceneManager.GetActiveScene());
			}

			EditorGUILayout.EndVertical();
		}
	}
#endif
}
