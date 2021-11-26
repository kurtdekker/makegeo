using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// @kurtdekker
// This simple demo is purely to show the logic behind
// deconflicting items going into a regular 2D grid.

public class SpawningInAGrid : MonoBehaviour
{
	[Header( "Grid dimensions.")]
	public int Across;
	public int Down;

	[Header( "WARNING: this must fit in the above grid!")]
	public int Count;

	[Header("You must provide what to spawn here:")]
	public GameObject PrefabToSpawn;

	void Reset()
	{
		Across = 10;
		Down = 5;

		Count = 10;
	}

	Vector3 CellToWorld( int x, int y)
	{
		// TODO: you replace this with whatever spacing or orientation you want
		Vector3 position = new Vector3( x, y, 0);

		// center it around (0,0,0)
		position -= new Vector3( Across, Down) / 2;

		return position;
	}

	// this will be filled out and will contain all created GameObjects
	GameObject[,] TheBoard;

	void FillBoard()
	{
		TheBoard = new GameObject[ Across, Down];

		// we'll parent all created objects here to keep things tidy
		GameObject parent = new GameObject("BoardHierarchy");

		for (int i = 0; i < Count; i++)
		{
			int x = 0;
			int y = 0;

			// WARNING: this will loop choosing spots until it finds one
			// It is ON YOU to make sure you don't ask for more items
			// than will fit on the given board dimensions. If you fail
			// to meet this condition, this code WILL LOCK UP!

			do
			{
				x = Random.Range( 0, Across);
				y = Random.Range( 0, Down);
			}
			while( TheBoard[x,y]);

			// we know where we can put it, let's instantiate it
			GameObject spawnedEntity = Instantiate<GameObject>( PrefabToSpawn);

			// parent it...
			spawnedEntity.transform.SetParent( parent.transform);

			// position it...
			spawnedEntity.transform.position = CellToWorld( x, y);

			// TODO: do any other stuff you want (rotate, etc.) here

			// record the new item in our grid.
			TheBoard[x,y] = spawnedEntity;
		}
	}

	void Start ()
	{
		FillBoard();
	}
}
