using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//
// @kurtdekker... a replacement for TrailRenderer
//
// to use:
//
//	- place this on a nearby GameObject
//		- don't rotate or scale it
//		- needs and will add a MeshRenderer
//		- don't forget the double-sided Material!
//	- drag at least two transforms into children field
//

[RequireComponent(typeof( MeshRenderer))]
public class CustomTrailRenderer : MonoBehaviour
{
	[Header("Drives the head of the ribbon:")]
	public Transform[] children;

	public float duration = 0.25f;

	const int FPS = 30;

	float _duration;
	int _trailLength;
	int _acrossCount;


	int trailLength;

	// workspaces
	Vector3[] vertices;
	int[] triangles;
	Vector2[] uvs;



	MeshFilter mf;
	Mesh mesh;


	private void Start()
	{
		mf = GetComponent<MeshFilter>();
		if (!mf)
		{
			mf = gameObject.AddComponent<MeshFilter>();
		}

		mesh = new Mesh();
	}


	float time;

	void LateUpdate()
	{
		bool ageThePoints = false;

		// synthesize our own steady frame update; you may not want this!

		float updateInterval = 1.0f / FPS;

		time += Time.deltaTime;
		if (time >= updateInterval)
		{
			time -= updateInterval;
			ageThePoints = true;
		}




		int acrossCount = children.Length;



		if (duration != _duration || acrossCount != _acrossCount)
		{
			_duration = duration;


			trailLength = 0;

			if (_duration > 0)
			{
				trailLength = 1 + (int)(FPS * duration);
			}

			if (trailLength < 2)
			{
				trailLength = 2;
			}
		}





		if (vertices == null || _trailLength != trailLength)
		{
			_trailLength = trailLength;


			int vertexCount = acrossCount * trailLength;

			vertices = new Vector3[ vertexCount];
			uvs = new Vector2[vertexCount];

			// quads made of pairs of triangles going across the front
			int triangleCount = (acrossCount - 1) * (trailLength - 1) * 2;

			triangles = new int[triangleCount * 3];



			// fill out the history with where we are now
			int trino = 0;
			for (int i = 0; i < acrossCount; i++)
			{
				Vector3 position = children[i].position;

				for (int j = 0; j < trailLength; j++)
				{
					int n = j * acrossCount + i;

					vertices[n] = position;

					if (i < acrossCount - 1 && j < trailLength - 1)
					{
						triangles[trino] = n;
						triangles[trino + 1] = n + 1;
						triangles[trino + 2] = n + 1 + children.Length;

						triangles[trino + 3] = n;
						triangles[trino + 4] = n + 1 + children.Length;
						triangles[trino + 5] = n + children.Length;

						trino += 6;
					}
				}

			}

		}



		// time to age and reload the vertices in stripes going back in time
		for (int j = 0; j < trailLength; j++)
		{
			int n = j * acrossCount;

			float u = (float)j / (trailLength - 1);


			for (int i = 0; i < acrossCount; i++)
			{
				if (j < trailLength - 1)
				{
					if (ageThePoints)
					{
						vertices[n] = vertices[n + acrossCount];
					}
				}
				else
				{
					Vector3 position = children[i].position;

					vertices[n] = position;
				}


				// TODO: make UVs aware of relative distances between points
				// NOTE: This is just terping by child count...
				float v = (float)i / (acrossCount - 1);
				uvs[n] = new Vector2(u, v);


				n++;
			}
		}


		mesh.vertices = vertices;
		mesh.uv = uvs;
		mesh.triangles = triangles;

		mesh.RecalculateBounds();
		mesh.RecalculateNormals();

		mf.sharedMesh = mesh;
	}
}
