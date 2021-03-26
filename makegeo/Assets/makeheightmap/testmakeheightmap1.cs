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
	}
}
