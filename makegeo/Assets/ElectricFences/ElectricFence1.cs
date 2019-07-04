using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElectricFence1 : MonoBehaviour
{
	public Material ZappyMaterial;

	public float Thickness;

	public float LinearJitter;

	public int Count;

	void Reset()
	{
		Thickness = 0.5f;
		LinearJitter = 0.5f;
		Count = 3;
	}

	// one line renderer per vertical fence "band"
	LineRenderer[] LRs;
	float[] UVOffsets;

	void Start()
	{
		LRs = new LineRenderer[ Count];
		UVOffsets = new float[ Count];

		int limit = transform.childCount;

		for (int bandNumber = 0; bandNumber < Count; bandNumber++)
		{
			var go = new GameObject( "ZappyLine");
			go.transform.SetParent( transform);
			go.transform.localPosition = Vector3.zero;

			var LR = go.AddComponent<LineRenderer>();

			LR.positionCount = limit;

			float distance = 0;

			Vector3 prevPosition = Vector3.zero;

			for (int postNumber = 0; postNumber < limit; postNumber++)
			{
				Transform tr = transform.GetChild (postNumber);

				// top and bottom of post
				Transform p1 = tr.GetChild(0);
				Transform p2 = tr.GetChild(1);

				Vector3 position = tr.position;

				// unless you provide two children per post, it won't offset the points
				if (p1 && p2)
				{
					float bandFraction = 0;
					if (Count > 0) bandFraction = (float)bandNumber / (Count - 1);

					var Offset = Vector3.Lerp( p1.position, p2.position, bandFraction) - tr.position;

					position += Offset;
				}

				LR.SetPosition( postNumber, position);

				if (postNumber > 0)
				{
					distance += Vector3.Distance( position, prevPosition);
				}

				prevPosition = tr.position;
			}

			// adjust material UVs to match length
			Material mtl = new Material( ZappyMaterial);
			Vector2 temp = mtl.mainTextureScale;
			temp.x *= distance;
			mtl.mainTextureScale = temp;

			LR.material = mtl;

			LRs[bandNumber] = LR;
		}
	}

	void Update()
	{
		if (LinearJitter > 0)
		{
			for (int i = 0; i < LRs.Length; i++)
			{
				var LR = LRs[i];

				// jitter back and forth
				UVOffsets[i] += Random.Range( 0, LinearJitter) * (Random.Range( 0, 2) * 2 - 1);

				UVOffsets[i] %= 1.0f;

				LR.material.mainTextureOffset += Vector2.right * UVOffsets[i];
			}
		}
	}

	void OnDrawGizmos()
	{
		for (int i = 0; i < transform.childCount; i++)
		{
			Transform tr = transform.GetChild (i);

			Gizmos.DrawWireSphere (tr.position, 0.5f);

			if (tr.childCount == 2)
			{
				Transform a = tr.GetChild (0);
				Transform b = tr.GetChild (1);

				Gizmos.DrawLine (a.position, b.position);
			}
		}
	}
}
