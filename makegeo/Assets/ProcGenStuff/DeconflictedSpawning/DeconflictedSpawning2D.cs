using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// @kurtdekker

public class DeconflictedSpawning2D : MonoBehaviour
{
	[Header( "Make sure it has a Collider2D so we can overlap!")]
	public GameObject PrefabToSpawn;

	[Header( "Make this bigger to deconflict more.")]
	public float CheckRadius = 0.55f;

	Vector2 ChooseRandomPosition()
	{
		float x = Random.Range( +1.0f, +8.0f);
		float y = Random.Range( -6.0f, +6.0f);
		return new Vector2(x,y);
	}

	void Update()
	{
		Vector2 position = ChooseRandomPosition();

		Collider2D result = Physics2D.OverlapCircle( position, CheckRadius);

		if (!result)
		{
			Instantiate<GameObject>( PrefabToSpawn, position, Quaternion.identity);
		}
	}
}
