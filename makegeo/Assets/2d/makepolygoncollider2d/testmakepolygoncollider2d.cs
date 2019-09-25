using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testmakepolygoncollider2d : MonoBehaviour
{
	void Start ()
	{
		Vector2[] v1 = new Vector2[] {
			new Vector2( -1, 0),
			new Vector2( 0, 1),
			new Vector2( 1, 0),
			new Vector2( 0, -1),
		};

		MakeCollider2D.Create( v1);	
	}
}
