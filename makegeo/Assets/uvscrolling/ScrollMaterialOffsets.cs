using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// @kurtdekker - changes material UV offset in a given direction
// Expects to find a Renderer

public class ScrollMaterialOffsets : MonoBehaviour
{
	public Vector2 UVPosition;

	public Vector2 UVDirection;

	public float UVSpeed;

	public Renderer Rendrr;

	Material mtl;

	void Reset()
	{
		UVPosition = new Vector2( Random.value, Random.value);

		float angle = Random.Range( 0.0f, 306.0f);
		Quaternion rotation = Quaternion.Euler( 0, 0, angle);
		UVDirection = rotation * Vector2.up;

		UVSpeed = 1.0f;
	}

	void Start()
	{
		// tries to get if you don't supply
		if (!Rendrr)
		{
			Rendrr = GetComponent<Renderer>();
		}

		mtl = new Material(Rendrr.material);
		Rendrr.material = mtl;
	}

	void Update ()
	{
		UVPosition += UVDirection * UVSpeed * Time.deltaTime;

		mtl.mainTextureOffset = UVPosition;
	}
}
