using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainByPerlinNoise : MonoBehaviour
{
	Terrain terrain;
	TerrainData originalTerrainData;
	TerrainData terrainData;

	void Start ()
	{
		terrain = FindObjectOfType<Terrain>();

		originalTerrainData = terrain.terrainData;

		// this makes a fresh runtime copy so that we leave the
		// on-disk asset unmolested until we want to change it
		terrainData = Instantiate<TerrainData>( originalTerrainData);
	}

	void Regenerate()
	{
		// <WIP> stack of noise inserts here...

		// we'll shuffle up position
		DSM.Perlin.XOffset.fValue = Random.Range( -1000.0f, 1000.0f);
		DSM.Perlin.YOffset.fValue = Random.Range( -1000.0f, 1000.0f);

		float[,] heightmap = terrainData.GetHeights( 0, 0, terrainData.heightmapResolution, terrainData.heightmapResolution);

		for (int j = 0; j < heightmap.GetLength(1); j++)
		{
			for (int i = 0; i < heightmap.GetLength(0); i++)
			{
				// choose the frequency of the noise by scaling these
				float x = DSM.Perlin.XOffset.fValue + i * DSM.Perlin.XFrequency.fValue;
				float y = DSM.Perlin.YOffset.fValue + j * DSM.Perlin.YFrequency.fValue;

				float height = Mathf.PerlinNoise( x, y) * DSM.Perlin.VerticalScale.fValue;

				heightmap[i,j] += height;
			}
		}

		terrainData.SetHeights( 0, 0, heightmap);

		terrain.terrainData = terrainData;
	}

	void OnUserIntent( Datasack ds)
	{
		switch(ds.Value)
		{
		case "ButtonSaveTerrainData":
			// <WIP> this is not presently saving...
			originalTerrainData = terrainData;
			break;
		case "ButtonRegenerate":
			Regenerate();
			break;
		}
	}
	void OnEnable()
	{
		DSM.UserIntent.OnChanged += OnUserIntent;
	}
	void OnDisable()
	{
		DSM.UserIntent.OnChanged -= OnUserIntent;
	}
}
