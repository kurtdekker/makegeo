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

	[Header("Spacings on X and Y")]
	public Vector2 Spacing;

	[Header("Provide a player, if you want to move.")]
	public GameObject PlayerToSpawn;

	public bool AllowDiagonalMovement;

	void Reset()
	{
		Across = 7;
		Down = 5;

		Count = 10;

		Spacing = Vector2.one;
	}

	Vector3 CellToWorld( int x, int y)
	{
		// TODO: you replace this with whatever spacing or orientation you want
		Vector3 position = new Vector3( x, y, 0);

		// center it around (0,0,0)
		position -= new Vector3( Across - 1, Down - 1) / 2;

		position.x *= Spacing.x;
		position.y *= Spacing.y;

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

	IEnumerator Start ()
	{
		FillBoard();

		if (PlayerToSpawn)
		{
			// find a cell to start the player
			int playerX = 0;
			int playerY = 0;
			do
			{
				playerX = Random.Range( 0, Across);
				playerY = Random.Range( 0, Down);
			}
			while( TheBoard[playerX,playerY]);

			GameObject PlayerInstance = Instantiate<GameObject>( PlayerToSpawn);
			PlayerInstance.transform.position = CellToWorld( playerX, playerY);

			// this is the main input-and-move loop
			while( true)
			{
				// presume no movement
				int xm = 0;
				int ym = 0;

				float h = Input.GetAxisRaw( "Horizontal");
				float v = Input.GetAxisRaw( "Vertical");

				const float threshhold = 0.2f;

				const float Speed = 5.0f;

				if (h < -threshhold) xm = -1;
				if (h > +threshhold) xm = +1;
				if (v < -threshhold) ym = -1;
				if (v > +threshhold) ym = +1;

				if (xm != 0 || ym != 0)
				{
					if (!AllowDiagonalMovement)
					{
						if (xm != 0) ym = 0;
						if (ym != 0) xm = 0;
					}

					int newX = playerX + xm;
					int newY = playerY + ym;

					// validate the move
					bool validMove = true;

					if (newX < 0) validMove = false;
					if (newX >= Across) validMove = false;
					if (newY < 0) validMove = false;
					if (newY >= Down) validMove = false;

					if (validMove)
					{
						if (TheBoard[newX, newY])
						{
							validMove = false;
						}
					}

					if (validMove)
					{
						playerX = newX;
						playerY = newY;

						Vector3 newPosition = CellToWorld( newX, newY);

						while( Vector3.Distance( newPosition, PlayerInstance.transform.position) > 0.01f)
						{
							Vector3 interimPosition = Vector3.MoveTowards( PlayerInstance.transform.position, newPosition, Speed * Time.deltaTime);

							PlayerInstance.transform.position = interimPosition;

							yield return null;
						}

						PlayerInstance.transform.position = newPosition;
					}
				}

				yield return null;
			}
		}
	}
}
