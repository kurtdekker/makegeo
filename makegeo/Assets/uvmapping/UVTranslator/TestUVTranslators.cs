/*
	The following license supersedes all notices in the source code.

	Copyright (c) 2022 Kurt Dekker/PLBM Games All rights reserved.

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

public class TestUVTranslators : MonoBehaviour
{
	// randomly clones objects, remapping their colors via UV position.

	// Remember: zero is always the lower left of UV

	public GameObject Prefab;
	public Rect SourceRect
		// source green I chose in example ImphenziaPalette01-256-Gradient texture
		= new Rect(7.0f / 8.0f, 6.0f / 8.0f, 1.0f / 8.0f, 1.0f / 8.0f)
		;
	public Rect DestRect;

	public Transform LowerLeftCorner;
	public Transform UpperRightCorner;

	// greens in example ImphenziaPalette01-256-Gradient texture
	int[] KnownShadesOfGreenXY = new int[]
	{
		// more yellowy
		5,6,
		6,6,
		7,6,

		// intense ones
		5,1,
		6,1,
		7,1,

		// an occasional reddish fall tree
		1, 5,
		2, 5,
	};

	// this does the actual UV remaps above
	void RandomizeColor( GameObject copy)
	{
		// we'll pick a random shade of green (AND-ing with ~1 enforces only even chosen)
		int n = Random.Range( 0, KnownShadesOfGreenXY.Length) & (~1);

		DestRect = new Rect(
			KnownShadesOfGreenXY[n + 0] / 8.0f,
			KnownShadesOfGreenXY[n + 1] / 8.0f,
			1.0f / 8.0f,
			1.0f / 8.0f);

		var rends = copy.GetComponentsInChildren<Renderer>();
		foreach (var rend in rends)
		{
			var filter = rend.GetComponent<MeshFilter>();

			var mesh = filter.mesh;

			UVTranslators.RemapRectangular(mesh: mesh, source: SourceRect, dest: DestRect);
		}
	}

	// Irregular one-dimensional grid (strafe across):
	// The idea is that you traverse one axis smoothly (in this case the X axis)
	// while choosing positions randomly in the other axis. This leads to perfect
	// coverage in one axis, and yet still random coverage in the other.
	IEnumerator RandomTreeSpeckling( Transform parent)
	{
		int TreeCount = 250;

		for (int i = 0; i < TreeCount; i++)
		{
			float fx = Mathf.Lerp( LowerLeftCorner.position.x, UpperRightCorner.position.x, (i / (float)(TreeCount - 1)));
				
			float fz = Random.Range( LowerLeftCorner.position.z, UpperRightCorner.position.z);
				
			var copy = Instantiate<GameObject>(Prefab, parent);
			copy.transform.position = new Vector3( fx, 0, fz);

			// rotate the placed tree
			float orientation = Random.Range(0.0f, 360.0f);
			copy.transform.rotation = Quaternion.Euler(0, orientation, 0) * copy.transform.rotation;

			RandomizeColor( copy);

			yield return null;
		}
	}

	// perturbed 2-dimensional grid, with noise func
	IEnumerator PerturbedGridWithNoise( Transform parent, System.Func<int,int,bool> NoiseCheck)
	{
		float spacing = 1.0f;

		for (int j = (int)LowerLeftCorner.position.z; j <= UpperRightCorner.position.z; j++)
		{
			for (int i = (int)LowerLeftCorner.position.x; i <= UpperRightCorner.position.x; i++)
			{
				if (NoiseCheck( i, j))
				{
					var copy = Instantiate<GameObject>(Prefab, parent);

					float perturbX = Random.Range( -spacing * 0.35f, spacing * 0.35f);
					float perturbZ = Random.Range( -spacing * 0.35f, spacing * 0.35f);

					copy.transform.position = new Vector3(
						i * spacing + perturbX,
						0,
						j * spacing + perturbZ);

					// rotate the placed tree
					float orientation = Random.Range(0.0f, 360.0f);
					copy.transform.rotation = Quaternion.Euler(0, orientation, 0) * copy.transform.rotation;

					RandomizeColor(copy);

					yield return null;
				}
			}
		}
	}

	// we'll choose these once per run, to make the noise change
	float x1Scale;
	float y1Scale;
	float x1Offset;
	float y1Offset;
	float MinimumDensity;
	float MaximumDensity;
	bool DensityFunction1( int x, int y)
	{
		float x1 = x * x1Scale + x1Offset;
		float y1 = y * y1Scale + y1Offset;

		var v = Random.value;
		v *= (MaximumDensity - MinimumDensity);
		v += MinimumDensity;

		return v > Mathf.PerlinNoise( x1, y1);
	}

	void ChooseNoiseParams()
	{
		x1Scale = Random.Range( 10.0f, 20.0f);
		y1Scale = x1Scale + Random.Range( -3.0f, 4.0f);

		x1Scale = 1.0f / x1Scale;
		y1Scale = 1.0f / y1Scale;

		x1Offset = Random.Range( 0.0f, 1000.0f);
		y1Offset = Random.Range( 0.0f, 1000.0f);

		MinimumDensity = Random.Range( 0.0f, 0.2f);
		MaximumDensity = Random.Range( 0.5f, 1.0f);
	}

	GameObject Forest1;
	GameObject Forest2;

	void Start()
	{
		ChooseNoiseParams();

		Forest1 = new GameObject( "Forest1-RandomTreeSpeckling");
		StartCoroutine( RandomTreeSpeckling(Forest1.transform));

		Forest2 = new GameObject( "Forest2-PerturbedGridWithNoise");
		StartCoroutine( PerturbedGridWithNoise(Forest2.transform, DensityFunction1));
	}

	void Update()
	{
		if (Input.GetKeyDown( KeyCode.Alpha1))
		{
			Forest1.SetActive( !Forest1.activeSelf);
		}
		if (Input.GetKeyDown( KeyCode.Alpha2))
		{
			Forest2.SetActive( !Forest2.activeSelf);
		}
		if (Input.GetKeyDown( KeyCode.R))
		{
			UnityEngine.SceneManagement.SceneManager.LoadScene(
				UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
		}
	}
}
