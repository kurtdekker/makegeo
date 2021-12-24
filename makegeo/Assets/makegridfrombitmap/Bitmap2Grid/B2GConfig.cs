using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class B2GConfig : ScriptableObject
{
	[System.Serializable]
	public class ColorToPrefab
	{
		public Color color;
		public GameObject prefab;
	}

	[Header( "Fill this out with color,prefab combos.")]
	public ColorToPrefab[] Mappings;

	// replace with other color distance heuristics if you prefer, or weight the channels unevently.
	float ColorDistance (Color c1, Color c2)
	{
		float dr = c1.r - c2.r;
		float dg = c1.g - c2.g;
		float db = c1.b - c2.b;
		float da = c1.a - c2.a;

		return Mathf.Abs( dr) + Mathf.Abs( dg) + Mathf.Abs( db) + Mathf.Abs( da);
	}

	public GameObject LookupColor( Color c)
	{
		// this will return whatever prefab is slotted into
		// the color that is closest, which might be null.

		float bestDistance = 0;
		int bestIndex = -1;

		for (int i = 0; i < Mappings.Length; i++)
		{
			var mapping = Mappings[i];

			var distance = ColorDistance( mapping.color, c);

			if (i == 0 || distance < bestDistance)
			{
				bestIndex = i;
				bestDistance = distance;
			}
		}

		if (bestIndex >= 0)
		{
			// this might be null still, by design, for open space for instance
			return Mappings[bestIndex].prefab;
		}

		return null;
	}
}
