/*
	The following license supersedes all notices in the source code.

	Copyright (c) 2020 Kurt Dekker/PLBM Games All rights reserved.

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

public class TestRemappers : MonoBehaviour
{
	IEnumerator WaitForInput()
	{
		while( true)
		{
			if (Input.GetKeyDown(KeyCode.Space)) break;

			if (Input.GetKeyDown(KeyCode.Return)) break;

			if (Input.GetMouseButtonDown(0)) break;

			yield return null;
		}
		yield return null;
	}

	IEnumerator Start ()
	{
		List<GameObject> all = new List<GameObject>();

		{
			var go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
			go.transform.position = Vector3.right * -1.3f;
			all.Add(go);
			go = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
			go.transform.position = Vector3.right * 0.0f;
			all.Add(go);
			go = GameObject.CreatePrimitive(PrimitiveType.Cube);
			go.transform.position = Vector3.right * 1.3f;
			all.Add(go);
		}

		foreach( var go in all)
		{
			SpinMeY.Attach(go);
		}

		yield return WaitForInput();

		foreach( var go in all)
		{
			go.GetComponent<Renderer>().material = MaterialFactory.Instance.abcd;
		}

		yield return WaitForInput();

		foreach( var go in all)
		{
			UVRemappers.ApplyCylindrical(go,Vector3.zero);
		}

		yield return WaitForInput();
	}
}
