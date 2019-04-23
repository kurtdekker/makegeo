using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testmakequadplane : MonoBehaviour
{
    public Material mtl;

    void Start()
    {
        {
            var mqp = new QuadPlaneDef(2, 2);
            GameObject go = MakeQuadPlane.Create(mqp);
            go.transform.position = new Vector3(-3, 0, 0);
            go.GetComponent<MeshRenderer>().material = mtl;
        }

        {
            var mqp = new QuadPlaneDef(4, 10, new Vector3( 2.5f, 1.0f, 3.5f));
            GameObject go = MakeQuadPlane.Create(mqp);
            go.transform.position = new Vector3( 0, 0, 0);
            go.GetComponent<MeshRenderer>().material = mtl;
        }

        {
            var mqp = new QuadPlaneDef(40, 40, new Vector3(3.2f, 0.25f, 3.2f));

            // we'll make this one "interesting" with intermingling sine waves
            mqp.HeightFunction = (v2) =>
            {
                return Mathf.Sin( v2.x / 10.0f) + Mathf.Sin( v2.y / 5.0f);
            };

            GameObject go = MakeQuadPlane.Create(mqp);
            go.transform.position = new Vector3(4, 0, 0);
            go.GetComponent<MeshRenderer>().material = mtl;
        }
    }
}
