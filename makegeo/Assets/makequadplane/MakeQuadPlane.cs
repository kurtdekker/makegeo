using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
                        i / (qpd.xCells - 1.0f),
                        j / (qpd.zCells - 1.0f));

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
