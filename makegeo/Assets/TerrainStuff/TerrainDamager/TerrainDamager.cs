/*
	The following license supersedes all notices in the source code.

	Copyright (c) 2021 Kurt Dekker/PLBM Games All rights reserved.

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

// TODO: support for adjusting splatmap to show damage texture

public class TerrainDamager : MonoBehaviour
{
	float[,] originalHeightmap;

	// world coordinates
	public float MaxDeformation = 1.0f;

	float[,] heightmap;
	TerrainData terrainData;
	float[,,] splatMaps;

	float TerrainVerticalScale
	{
		get
		{
			return terrainData.size.y;
		}
	}

	void Start()
	{
		var terrain = GetComponent<Terrain>();

		// Warning: this does NOT do a complete terrain clone!
//		terrainData = Instantiate<TerrainData>( terrain.terrainData);
		terrainData = TerrainDataCloner.Clone( terrain.terrainData);

		terrain.terrainData = terrainData;

		heightmap = terrainData.GetHeights( 0, 0, terrainData.heightmapResolution, terrainData.heightmapResolution);

		originalHeightmap = heightmap.Clone() as float[,];

		splatMaps = terrainData.GetAlphamaps( 0, 0, terrainData.alphamapWidth, terrainData.alphamapHeight);

		TerrainCollider terrainCollider = GetComponent<TerrainCollider>();
		terrainCollider.terrainData = terrainData;
	}

	// ignores the Y coordinate of position
	public void ApplyDamage( Vector3 position, TerrainDamageConfig config, float severity = 1.0f)
	{
		Vector3 terrainCell = position;

		terrainCell -= transform.position;

		terrainCell.x = (terrainCell.x * terrainData.heightmapResolution) / terrainData.size.x;
		terrainCell.z = (terrainCell.z * terrainData.heightmapResolution) / terrainData.size.z;

//		int iCenter = (int)terrainCell.x;
//		int jCenter = (int)terrainCell.z;

		// choose what this particular hole is going to look like
		float holeDepth = Random.Range( config.MinDepth, config.MaxDepth);
		float holeRadius = Random.Range( config.MinRadius, config.MaxRadius);

		holeDepth *= severity;
		holeRadius *= severity;

		if (config.RemoveEarth) holeDepth = -holeDepth;

		float baseAdjustment = holeDepth / TerrainVerticalScale;

		float maxHeightmapAdjustment = MaxDeformation / TerrainVerticalScale;

		int xMin = (int)(terrainCell.x - holeRadius);
		int xMax = (int)(terrainCell.x + holeRadius);
		int zMin = (int)(terrainCell.z - holeRadius);
		int zMax = (int)(terrainCell.z + holeRadius);

		int iHoleRadius = (int)holeRadius;
		if (iHoleRadius < 1) iHoleRadius = 1;

		int iHoleRadiusSquaredDivider = iHoleRadius * iHoleRadius * 2;

		// <WIP> future optimization: pull out just the sub-region that
		// gets modified and only update those heights rather than all.

		int dz = -iHoleRadius;
		for (int z = zMin; z <= zMax; z++, dz++)
		{
			int dx = -iHoleRadius;
			for (int x = xMin; x < xMax; x++, dx++)
			{
				if (z >= 0 && z < heightmap.GetLength(0))
				{
					if (x >= 0 && x < heightmap.GetLength(1))
					{
						float fraction = 1.0f;

						switch( config.HoleShape)
						{
						default :
						case TerrainDamageConfig.ProceduralHoleShape.RECTANGULAR :
							break;
						case TerrainDamageConfig.ProceduralHoleShape.INVERTEDCONE :
							{
								int offCenter = dx * dx + dz * dz;
								if (offCenter >= iHoleRadiusSquaredDivider) offCenter = iHoleRadiusSquaredDivider;
								fraction = (iHoleRadiusSquaredDivider - offCenter) / (float)iHoleRadiusSquaredDivider;
							}
							break;
						case TerrainDamageConfig.ProceduralHoleShape.CIRCULAR :
							{
								int offCenter = dx * dx + dz * dz;
								fraction = (offCenter <= iHoleRadiusSquaredDivider / 2) ? 1.0f : 0.0f;
							}
							break;
						}

						float adjustment = baseAdjustment * fraction;

						var heightSample = heightmap[z,x] + adjustment;

						if (heightSample < originalHeightmap[z,x] - maxHeightmapAdjustment)
						{
							heightSample = originalHeightmap[z,x] - maxHeightmapAdjustment;
						}
						if (heightSample > originalHeightmap[z,x] + maxHeightmapAdjustment)
						{
							heightSample = originalHeightmap[z,x] + maxHeightmapAdjustment;
						}

						heightmap[z,x] = heightSample;

						if (z < splatMaps.GetLength(0))
						{
							if (x < splatMaps.GetLength(1))
							{
								for (int k = 0; k < splatMaps.GetLength(2); k++)
								{
									if (fraction > 0.5f)
									{
										splatMaps[z,x,k] = (k == config.ColorForDamage) ? 1.0f : 0.0f;
									}
								}
							}
						}
					}
				}
			}
		}

		terrainData.SetHeights( 0, 0, heightmap);
		terrainData.SetAlphamaps( 0, 0, splatMaps);
	}
}
