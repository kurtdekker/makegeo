/*
	The following license supersedes all notices in the source code.

	Copyright (c) 2021 Kurt Dekker/PLBM Games All rights reserved.

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

public class TestUVTranslators : MonoBehaviour
{
	// randomly clones objects, remapping their colors via UV position.

	public GameObject Prefab;
	public Rect SourceRect
		// green in example ImphenziaPalette01-256-Gradient texture
		= new Rect(7.0f / 8.0f, 6.0f / 8.0f, 1.0f / 8.0f, 1.0f / 8.0f)
		;
	public Rect DestRect;

	IEnumerator Start()
	{
		for (int i = 0; i < 100; i++)
		{
			var copy = Instantiate<GameObject>(Prefab, transform);
			copy.transform.position = new Vector3(
				Random.Range(-15.0f, 15.0f),
				0,
				Random.Range(-15.0f, 5.0f));

			// rotate the placed tree
			float orientation = Random.Range(0.0f, 360.0f);
			copy.transform.rotation = Quaternion.Euler(0, orientation, 0) * copy.transform.rotation;

			// we'll pick a random rectangle
			int x = Random.Range(0, 8);
			int y = Random.Range(0, 8);
			DestRect = new Rect(x / 8.0f, y / 8.0f, 1.0f / 8.0f, 1.0f / 8.0f);

			var rends = copy.GetComponentsInChildren<Renderer>();
			foreach (var rend in rends)
			{
				var filter = rend.GetComponent<MeshFilter>();

				var mesh = filter.mesh;

				UVTranslators.RemapRectangular(mesh: mesh, source: SourceRect, dest: DestRect);
			}

			yield return new WaitForSeconds(0.1f);
		}
	}
}
