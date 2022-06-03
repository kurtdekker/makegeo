/*
	The following license supersedes all notices in the source code.

	Copyright (c) 2022 Kurt Dekker/PLBM Games All rights reserved.

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

// @kurtdekker
//	Cheesy UV-mooshing squishing stuff
//	Expects to find a MeshFilter

public class UVMooshing : MonoBehaviour
{
	public	float	UVDistance;
	public	float	RateFactor;

	public void Reset()
	{
		UVDistance = 0.01f;
		RateFactor = 0.02f;
	}

	MeshFilter mf;

	int count;

	Vector2[] OriginalUVs;

	Vector2[] WorkUVs;

	float[] angles;
	float[] rates;
	float[] radii;

	void Start ()
	{
		mf = GetComponent<MeshFilter>();

		List<Vector2> uvs = new List<Vector2>();
		mf.mesh.GetUVs( 0, uvs);

		OriginalUVs = uvs.ToArray();

		count = OriginalUVs.Length;

		WorkUVs = new Vector2[count];
		angles = new float[count];
		rates = new float[count];
		radii = new float[count];

		for (int i = 0; i < count; i++)
		{
			angles[i] = Random.Range( 0.0f, 360.0f);	
			rates[i] = Random.Range( 0.5f, 1.0f) * (Random.Range( 0, 2) * 2 - 1);
			radii[i] = Random.Range( 0.0f, 1.0f) * UVDistance;
		}
	}
	
	void Update ()
	{
		for (int i = 0; i < count; i++)
		{
			angles[i] += rates[i] * RateFactor;

			WorkUVs[i] = OriginalUVs[i] + new Vector2(
				Mathf.Sin( angles[i]) * radii[i],
				Mathf.Cos( angles[i]) * radii[i]);
		}

		mf.mesh.uv = WorkUVs;
	}
}
