using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testmakeheightmap1 : MonoBehaviour
{
	public Texture2D Heightmap;

	public Vector3 Dimensions;

	public Material mtl;

	void Start ()
	{
		var go = MakeHeightmap1.Create( Heightmap, Dimensions);
		go.GetComponent<Renderer>().material = mtl;
		go.AddComponent<SpinMeY>().RateOfSpin = 20;
	}

	void OnGUI()
	{
		// display the heightmap texture as a little corner "chit"
		{
			float sz = Mathf.Min( Screen.width, Screen.height);
			sz *= 0.25f;
			Rect r = new Rect( sz * 0.05f, sz * 0.05f, sz, sz);
			GUI.DrawTexture( r, Heightmap);
		}
	}
}
