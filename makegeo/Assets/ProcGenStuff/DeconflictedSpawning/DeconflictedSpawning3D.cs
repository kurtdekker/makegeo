using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// @kurtdekker

public class DeconflictedSpawning3D : MonoBehaviour
{
	[Header( "Make sure it has a collider so we can overlap!")]
	public GameObject PrefabToSpawn;

	[Header( "Make this bigger to deconflict more.")]
	public float CheckRadius = 0.55f;

	Vector3 ChooseRandomPosition()
	{
		float x = Random.Range( -8.0f, -1.0f);
		float y = Random.Range( -6.0f, +6.0f);
		float z = Random.Range( -3.0f, +3.0f);
		return new Vector3(x,y,z);
	}

	void Update()
	{
		Vector3 position = ChooseRandomPosition();

		Collider[] results = Physics.OverlapSphere( position, CheckRadius);

		if (results == null || results.Length == 0)
		{
			Instantiate<GameObject>( PrefabToSpawn, position, Quaternion.identity);
		}
	}
}
