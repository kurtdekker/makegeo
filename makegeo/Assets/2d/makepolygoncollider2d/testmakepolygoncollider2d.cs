using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testmakepolygoncollider2d : MonoBehaviour
{
	public Material mtl1;

	void Start ()
	{
		Vector2[] v1 = new Vector2[] {
			new Vector2( -1, 0),
			new Vector2( 0, 1),
			new Vector2( 2, 0),
			new Vector2( 0, -1),
		};

		var go = MakeCollider2D.Create( v1);
		go.GetComponent<Renderer>().material = mtl1;

		go.AddComponent<Rigidbody2D>();
	}
}
