
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// @kurtdekker

public class testmakefogofwar : MonoBehaviour
{
	[Header("What goes around the inner ring:")]
	public Material mtlGradient;
	[Header("What fills the outer ring:")]
	public Material mtlSolid;

	void Start()
	{
		Camera cam = Camera.main;

		float camHeight = cam.orthographicSize;
		float camWidth = (Screen.width * camHeight) / Screen.height;

		GameObject go = MakeFogOfWar.Create(
			sectors: 32,
			innerRadius: 2,
			gradientWidth: 1,
			totalRadius: 20,
			new Rect(-camWidth, -camHeight, camWidth * 2, camHeight * 2)
		);
		go.GetComponent<MeshRenderer>().materials = new Material[] {
			mtlGradient,
			mtlSolid,
		};
		go.transform.position = new Vector3(0, 0, 0);
	}
}
