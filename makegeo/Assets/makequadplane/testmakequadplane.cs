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

public class testmakequadplane : MonoBehaviour
{
    public Material mtl;

    void Start()
    {
        // some sample quad planes of various sizes and descriptions

        {
            var mqp = new QuadPlaneDef(2, 2);
            GameObject go = MakeQuadPlane.Create(mqp);
            go.transform.position = new Vector3(-3, 0, 0);
            go.GetComponent<MeshRenderer>().material = mtl;
        }

        {
            var mqp = new QuadPlaneDef(4, 10, new Vector3( 3.0f, 1.0f, 4.0f));

            // we'll back this one's texture coordinates a bit tighter
            mqp.UVScale = Vector2.one * 3.0f;

            GameObject go = MakeQuadPlane.Create(mqp);
            go.transform.position = new Vector3( 0, 0, 0);
            go.GetComponent<MeshRenderer>().material = mtl;
        }

        {
            var mqp = new QuadPlaneDef(40, 80, new Vector3(3.2f, 0.15f, 6.2f));

            // we'll make this one "interesting" with intermingling sine waves
            mqp.HeightFunction = (v2) =>
            {
                // these constants control how fast the
                // sine wave repeats in each axis
                return Mathf.Sin( v2.x * 5.0f) + Mathf.Sin( v2.x * 2.0f + v2.y * 15.0f);
            };

            GameObject go = MakeQuadPlane.Create(mqp);
            go.transform.position = new Vector3(4, 0, 0);
            go.GetComponent<MeshRenderer>().material = mtl;
        }
    }
}
