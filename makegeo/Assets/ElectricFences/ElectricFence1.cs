/*
	The following license supersedes all notices in the source code.

	Copyright (c) 2019 Kurt Dekker/PLBM Games All rights reserved.

	http://www.twitter.com/kurtdekker

	Redistribution and use in source and binary forms, with or without
	modification, are permitted provided that the following conditions are
	met:

	Redistributions of source code must retain the above copyright notice,
	this list of conditions and the following disclaimer.

	Redistributions in binary form must reproduce the above copyright
	notice, this list of conditions and the following disclaimer in the
	documentation and/or other materials provided with the distribution.

	Neither the name of the Kurt Dekker/PLBM Games nor the names of its
	contributors may be used to endorse or promote products derived from
	this software without specific prior written permission.

	THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS
	IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED
	TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A
	PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT
	HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL,
	SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED
	TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR
	PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF
	LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING
	NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
	SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/

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
