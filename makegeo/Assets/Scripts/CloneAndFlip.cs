using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// @kurtdekker - ultra-simple clone-and-flip script
//
// At Start() this will:
//	- load the Mesh from a MeshFilter,
//	- clone all verts / faces and flip the normals
//	- recompute Normals (for lighting)
//	- put the new mesh back into the MeshFilter
//
// Limitations:
//	- it presumes triangles
//	- it doesn't handle multiple materials (submeshes)
//	- it just lets Unity recompute normals; you may wish to keep yours and flip them.
//	- doesn't handle vertex colors
//

public class CloneAndFlip : MonoBehaviour
{
	void Start ()
	{
		MeshFilter filter = GetComponent<MeshFilter>();

		if (!filter)
		{
			Debug.LogWarning( GetType() + ".Start(): expects to be on a MeshFilter-equipped GameObject!");
			return;
		}

		// make a working copy of the Mesh
		Mesh mesh = Instantiate<Mesh>( filter.sharedMesh);

		// terminology warning "tri" is used when it's really
		// an int that is one of the three triplets...

		// extract all the data from the native side
		Vector3[] originalVerts = mesh.vertices;
		Vector2[] originalUVs = mesh.uv;
		int[] originalTris = mesh.triangles;

		int origVertCount = originalVerts.Length;
		int origTriCount = originalTris.Length;

		// we must dupe the verts/uvs because normals are stored with verts
		Vector3[] verts = new Vector3[ origVertCount * 2];
		Vector2[] uvs = new Vector2[ origVertCount * 2];
		int[] tris = new int[ origTriCount * 2];

		// original verts
		System.Array.Copy( originalVerts, verts, origVertCount);
		// new copies
		System.Array.Copy( originalVerts, 0, verts, origVertCount, origVertCount);

		// original UVs
		System.Array.Copy( originalUVs, uvs, origVertCount);
		// new copies
		System.Array.Copy( originalUVs, 0, uvs, origVertCount, origVertCount);

		// original triangle indices
		System.Array.Copy( originalTris, tris, origTriCount);
		// new copies (unflipped)
		System.Array.Copy( originalTris, 0, tris, origTriCount, origTriCount);

		// point all second-half tris into our new verts
		for(int i = 0; i < origTriCount; i++)
		{
			int i2 = i + origTriCount;
			tris[i2] = tris[i2] + origVertCount;
		}

		// and now flip the tris around by swapping 2nd and 3rd indices
		for (int tri = 0; tri < originalTris.Length / 3; tri++)
		{
			// we elect to leave the 0th index unchanged and to swap the 1th and 2nd
			int i1 = tri * 3 + 1;

			// offset so we manipulate the second half of the target array
			i1 += originalTris.Length;

			// and this is the 2th (3rd) index
			int i2 = i1 + 1;

			// swap!
			int t = tris[i1];
			tris[i1] = tris[i2];
			tris[i2] = t;
		}

		mesh.vertices = verts;
		mesh.uv = uvs;
		mesh.triangles = tris;

		mesh.RecalculateNormals();

		filter.mesh = mesh;
	}
}
