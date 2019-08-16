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
