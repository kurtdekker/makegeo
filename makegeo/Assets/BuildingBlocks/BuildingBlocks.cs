using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingBlocks : MonoBehaviour
{
	public Transform Constructed;
	public Transform DestructedArea;

	bool Assembling;

	void Start ()
	{
		// inventory everything we're gonna move.
		foreach( Transform child in Constructed)
		{
			OriginalPositionAndRotation.Attach( child);
		}

		Assembling = false;
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

		float VerticalExaggeration = Random.Range( 0.5f, 2.0f);

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

			opar.transform.position = finalPosition;

			yield return null;
		}

		// now add an RB and fling it!
		var rb = opar.transform.gameObject.AddComponent<Rigidbody>();

		Vector3 flingVelocity = lastMotion / Time.deltaTime;

		rb.velocity = flingVelocity;

		// only wait so long before we kill its physics
		float WaitToKillPhysics = 1.0f;

		for (float t = 0; t < WaitToKillPhysics; t += Time.deltaTime)
		{
			if (rb.velocity.magnitude < 0.01f)
			{
				break;
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

	float pieceYetTimer;
	void Update ()
	{
		if (pieceYetTimer > 0)
		{
			pieceYetTimer -= Time.deltaTime;
		}

		if (Input.GetKeyDown( KeyCode.Space))
		{
			Assembling = !Assembling;
		}

		if (pieceYetTimer <= 0)
		{
			pieceYetTimer += Random.Range( 0.1f, 0.25f);

			pieceYetTimer += Random.Range( 0.5f, 1.0f);

			if (Assembling)
			{
			}
			else
			{
				DoFlingPiece( Constructed, DestructedArea);
			}
		}
	}
}
