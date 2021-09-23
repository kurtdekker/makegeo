using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// @kurtdekker - breaking apart and assembling structures out of blocks.

public class BuildingBlocks : MonoBehaviour
{
	public Transform Constructed;
	public Transform DestructedArea;

	public PhysicMaterial BlockPhysicMaterial;

	bool Disassembling;

	void Start ()
	{
		// inventory everything we're gonna move.
		foreach( Transform child in Constructed)
		{
			OriginalPositionAndRotation.Attach( child);
		}

		Disassembling = false;
	}

	IEnumerator FlingPieceCoRo( OriginalPositionAndRotation opar, Transform dest)
	{
		float ArcInterval = Random.Range( 0.5f, 0.7f);

		float fraction = 0;
		float time = 0;

		// perhaps randomize this around a small area here?
		Vector3 destPosition = dest.position;

		Vector3 randomArea = Random.insideUnitSphere;
		randomArea.y *= 0.2f;
		destPosition += randomArea;

		Vector3 startPosition = opar.Position;

		Vector3 lastPosition = startPosition;
		Vector3 lastMotion = Vector3.zero;

		float distance = Vector3.Distance( opar.Position, destPosition);

		float VerticalExaggeration = Random.Range( 0.4f, 1.0f);

		Transform piece = opar.transform;

		while( fraction < 1)
		{
			fraction = time / ArcInterval;
			time += Time.deltaTime;

			float angle = fraction * Mathf.PI;

			float cosineTween = (-Mathf.Cos( angle) + 1) / 2;

			Vector3 lateralPosition = Vector3.Lerp( startPosition, destPosition, cosineTween);

			float sineTween = Mathf.Sin( angle);

			Vector3 finalPosition = lateralPosition + Vector3.up * (sineTween * distance * VerticalExaggeration);

			lastMotion = finalPosition - lastPosition;

			lastPosition = finalPosition;

			piece.position = finalPosition;

			yield return null;
		}

		// if you fail to provide colliders, we need to add one because
		// otherwise the Rigidbody won't have an inertial tensor or CG.
		var col = piece.GetComponent<Collider>();
		if (!col)
		{
			col = opar.transform.gameObject.AddComponent<BoxCollider>();
		}
		col.material = BlockPhysicMaterial;

		// now add an RB and fling it!
		var rb = piece.gameObject.AddComponent<Rigidbody>();

		rb.collisionDetectionMode = CollisionDetectionMode.Continuous;

		Vector3 flingVelocity = lastMotion / Time.deltaTime;

		rb.velocity = flingVelocity;

		rb.angularVelocity = Random.onUnitSphere * Random.Range( 0.5f, 4.0f);

		// only wait so long before we kill its physics, even if it is midair
		float WaitToKillPhysics = 3.0f;

		for (float t = 0; t < WaitToKillPhysics; t += Time.deltaTime)
		{
			// accelerates demise when stopped
			if (rb.velocity.magnitude < 0.01f)
			{
				t += Time.deltaTime * 3.0f;
			}
			yield return null;
		}

		Destroy( rb);

		opar.transform.SetParent( dest);
	}

	void DoFlingPiece( Transform source, Transform dest)
	{
		if (source.childCount > 0)
		{
			var piece = source.GetChild( source.childCount - 1);

			piece.SetParent( null);

			var opar = piece.GetComponent<OriginalPositionAndRotation>();

			StartCoroutine( FlingPieceCoRo( opar, dest));
		}
	}

	IEnumerator RestoreToRightfulPlace( OriginalPositionAndRotation opar, Transform dest)
	{
		float ArcInterval = Random.Range( 0.5f, 0.7f);

		float fraction = 0;
		float time = 0;

		Transform piece = opar.transform;

		// our destroyed disassembled "found" position, as we lie
		Vector3 startPosition = piece.position;
		Quaternion startRotation = piece.rotation;

		float distance = Vector3.Distance( startPosition, opar.Position);

		float VerticalExaggeration = Random.Range( 0.1f, 0.2f);

		while( fraction < 1)
		{
			fraction = time / ArcInterval;
			time += Time.deltaTime;

			float angle = fraction * Mathf.PI;

			float cosineTween = (-Mathf.Cos( angle) + 1) / 2;

			Vector3 lateralPosition = Vector3.Lerp( startPosition, opar.Position, cosineTween);

			float sineTween = Mathf.Sin( angle);

			Vector3 finalPosition = lateralPosition + Vector3.up * (sineTween * distance * VerticalExaggeration);

			Quaternion rotation = Quaternion.Slerp( startRotation, opar.Rotation, cosineTween);

			piece.position = finalPosition;
			piece.rotation = rotation;

			yield return null;
		}

		piece.position = opar.Position;
		piece.rotation = opar.Rotation;

		piece.SetParent( dest);
	}

	void AssemblePiece( Transform source, Transform dest)
	{
		OriginalPositionAndRotation lowestOpar = null;

		// find piece in source stack with lowest sequence
		foreach( Transform piece in source)
		{
			var opar = piece.GetComponent<OriginalPositionAndRotation>();
			if (opar)
			{
				if (lowestOpar == null || opar.Sequence < lowestOpar.Sequence)
				{
					lowestOpar = opar;
				}
			}
		}

		if (lowestOpar)
		{
			Transform piece = lowestOpar.transform;

			piece.SetParent( null);

			StartCoroutine( RestoreToRightfulPlace( lowestOpar, dest));
		}
	}

	float pieceYetTimer;
	void Update ()
	{
		if (pieceYetTimer > 0)
		{
			pieceYetTimer -= Time.deltaTime;
		}

		if (Input.GetKeyDown( KeyCode.Space))
		{
			Disassembling = !Disassembling;
		}

		if (pieceYetTimer <= 0)
		{
			pieceYetTimer += Random.Range( 0.1f, 0.25f);

			if (Disassembling)
			{
				DoFlingPiece( Constructed, DestructedArea);
			}
			else
			{
				AssemblePiece( DestructedArea, Constructed);
			}
		}
	}
}
