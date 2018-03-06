using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElectricFence1 : MonoBehaviour
{
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
