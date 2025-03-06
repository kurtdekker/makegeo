using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemoChainOfRooms : MonoBehaviour
{
	[Header("Drag all possible room prefabs in here:")]
	public ChainableRoom[] PossibleRoomPrefabs;

	private void Start()
	{
		Transform PreviousRoomExit = null;

		int numRooms = 5;

		for (int i = 0; i < numRooms; i++)
		{
			// choose a room, any room
			int whichRoom = Random.Range( 0, PossibleRoomPrefabs.Length);
			ChainableRoom roomPrefab = PossibleRoomPrefabs[whichRoom];

			// default position / rotation to start from
			Vector3 spawnPosition = Vector3.zero;
			Quaternion spawnRotation = Quaternion.identity;

			// did we have a previous room?
			if (PreviousRoomExit)
			{
				spawnPosition = PreviousRoomExit.position;
				spawnRotation = PreviousRoomExit.rotation;
			}

			// make the room, parented to ourselves, at the position / rotation
			ChainableRoom roomInstance = Instantiate<ChainableRoom>(
				roomPrefab, spawnPosition, spawnRotation, transform);

			// remember our previous room's exit
			PreviousRoomExit = roomInstance.Exit;

			// if you fail to set the exit in a particular room, we're done
			if (!PreviousRoomExit)
			{
				Debug.Log($"Exit in room {roomPrefab.name} was null... quitting...");
				break;
			}
		}
	}
}
