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
using UnityEngine.UI;

// MASTER CAUTION: this uses Vector3 objects and winds a plane facing Y+
// It interleaves Vector2 objects for UV purposes, and Vector3 objects for
// positional spaces. Be careful with your Y vs Z selections!!!

public static class MakeQuadPlane
{
    public static GameObject Create( QuadPlaneDef qpd)
    {
        GameObject go = new GameObject("MakeQuadPlane:" + qpd.ToString());

        Mesh mesh = new Mesh();

        using (var vh = new VertexHelper())
        {
            for (int j = 0; j < qpd.zCells; j++)
            {
                for (int i = 0; i < qpd.xCells; i++)
                {
                    Vector2 pos = new Vector2(
                        (i * qpd.WorldSize.x) / (qpd.xCells - 1),
                        (j * qpd.WorldSize.z) / (qpd.zCells - 1));

                    float height = 0.0f;
                    if (qpd.HeightFunction != null)
                    {
                        height = qpd.HeightFunction(pos);
                    }

                    height *= qpd.WorldSize.y;

                    UIVertex vtx = new UIVertex();

                    vtx.position = new Vector3(pos.x, height, pos.y);

                    vtx.uv0 = new Vector2(
                        (i / (qpd.xCells - 1.0f)) * qpd.UVScale.x,
                        (j / (qpd.zCells - 1.0f)) * qpd.UVScale.y);

                    vh.AddVert(vtx);
                }
            }

            for (int j = 0; j < qpd.zCells - 1; j++)
            {
                for (int i = 0; i < qpd.xCells - 1; i++)
                {
                    int n = j * qpd.xCells + i;

                    vh.AddTriangle(n, n + qpd.xCells, n + 1);
                    vh.AddTriangle(n + 1, n + qpd.xCells, n + qpd.xCells + 1);
                }
            }

            vh.FillMesh(mesh);
        }

        mesh.RecalculateBounds();
        mesh.RecalculateNormals();

        MeshFilter mf = go.AddComponent<MeshFilter>();
        mf.mesh = mesh;

        MeshRenderer mr = go.AddComponent<MeshRenderer>();

        return go;
    }
}
