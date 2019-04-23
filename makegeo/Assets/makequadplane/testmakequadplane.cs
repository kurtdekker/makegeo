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
