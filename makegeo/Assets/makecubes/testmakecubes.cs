using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testmakecubes : MonoBehaviour
{
	[Header( "One material for each face: back, right, front, left, top, bottom.")]
	public Material[] sixMonoMaterials;

	public Material[] sixColorMaterials;

	void Start ()
	{
		{
			var go = MakeCube.Create( Vector3.one, sixMonoMaterials);
			go.transform.position = Vector3.left * 1.5f;
		}

		{
			var go = MakeCube.Create( new Vector3( 1, 2, 3), sixColorMaterials);
			go.transform.position = Vector3.right * 1.5f;
		}
	}
}
