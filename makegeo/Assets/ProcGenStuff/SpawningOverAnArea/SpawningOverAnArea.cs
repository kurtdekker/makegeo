using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawningOverAnArea : MonoBehaviour
{
	[Header("Provide two Transforms to delimit AABB area to spawn.")]
	public Transform Corner1;
	public Transform Corner2;

	[Header( "What will be spawned.")]
	public GameObject PrefabToSpawn;

	[Header( "How close can we let these items get?")]
	public float MinimumSpacing;

	[Header( "WARNING: make sure they can ALL FIT!")]
	public int Count;

	void Reset()
	{
		MinimumSpacing = 2.0f;

		Count = 10;
	}

	void PopulateArea()
	{
		// keep things tidy
		GameObject parent = new GameObject("AreaSpawnHolder");

		List<GameObject> CreatedSoFar = new List<GameObject>();

		for (int i = 0; i < Count; i++)
		{
			bool acceptable = true;
			Vector3 position = Vector3.zero;

			// WARNING: this code WILL LOCK UP if you don't give
			// enough room to fit all. That is on you!
			do
			{
				// presume it will fit
				acceptable = true;

				// chooses purely randomly within the 3D volume that
				// is axis-bounded by Corner1 and Corner1 (AABB)
				float x = Random.value;
				float y = Random.value;
				float z = Random.value;

				// propose a position
				position = new Vector3(
					Mathf.Lerp( Corner1.position.x, Corner2.position.x, x),
					Mathf.Lerp( Corner1.position.y, Corner2.position.y, y),
					Mathf.Lerp( Corner1.position.z, Corner2.position.z, z));

				// check all previously-emplaced objects for closeness
				foreach( var existing in CreatedSoFar)
				{
					float distance = Vector3.Distance( existing.transform.position, position);

					if (distance < MinimumSpacing)
					{
						// nope, too close to somebody
						acceptable = false;

						// give up immediately, try again
						break;
					}
				}
			}
			while( !acceptable);

			// we know where we can put it, let's instantiate it
			GameObject spawnedEntity = Instantiate<GameObject>( PrefabToSpawn);

			// parent it...
			spawnedEntity.transform.SetParent( parent.transform);

			// position it...
			spawnedEntity.transform.position = position;

			// TODO: do any other stuff you want (rotate, etc.) here

			// record the new item in our grid.
			CreatedSoFar.Add(spawnedEntity);
		}
	}

	void Start ()
	{
		PopulateArea();
	}
}
