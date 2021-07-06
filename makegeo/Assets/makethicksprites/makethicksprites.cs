using UnityEngine;
using System.Collections;

public class makethicksprites : MonoBehaviour
{
	public Texture2D tex;

	public Material unlit;

	int[,] depths;

	int[] xytable = new int[] { 0, 1, 0, -1, 0};

	float aspect = 0.8f;

	float sz = 0.1f;

	IEnumerator Start ()
	{
		int w = tex.width;
		int h = tex.height;

		depths = new int[w,h];

		int HH = tex.height - 1;

		GameObject thing = new GameObject ("Thing");

		for (int j = 0; j < h; j++)
		{
			for (int i = 0; i < w; i++)
			{
				Color c0 = tex.GetPixel( i, HH - j);
				if (c0.a > 0.5f)
				{
					int lowestdepth = -1;
					for (int d4 = 0; d4 < 4; d4++)
					{
						int dx = xytable[d4];
						int dy = xytable[d4 + 1];
						int x = i + dx;
						int y = j + dy;
						int depth = 1;
						while(x >= 0 && x < w && y >= 0 && y < h)
						{
							x += dx;
							y += dy;
							Color c = tex.GetPixel( x, HH - y);
							if (c.a > 0.5f)
							{
								depth++;
							}
							else
							{
								break;
							}
						}
						if ((lowestdepth < 0) || (depth < lowestdepth))
						{
							lowestdepth = depth;
						}
					}
					depths[i,j] = lowestdepth;

					GameObject cube = GameObject.CreatePrimitive ( PrimitiveType.Cube);
					cube.transform.SetParent( thing.transform);
					cube.transform.position = new Vector3(
						(i - 17) * sz, (HH - j) * sz, 0);
					cube.transform.localScale =
						new Vector3( sz, sz, sz * Mathf.Sqrt( lowestdepth) * 4);
					c0.a = 1.0f;
					Material mtl = new Material(unlit);
					mtl.color = c0;
					cube.GetComponent<Renderer>().material = mtl;
				}
			}
		}

		thing.transform.position += Vector3.down * 0.5f;

		while(true)
		{
			thing.transform.rotation = Quaternion.Euler( 0, 100 * Time.time, 0);
			yield return null;
		}
	}

	void Update()
	{
		if (Input.GetKeyDown( KeyCode.H))
		{
			showHeights = !showHeights;
		}
	}

	bool showHeights;

	void OnGUI()
	{
		if (showHeights)
		{
			int across = depths.GetLength (0);
			int down = depths.GetLength (1);

			int W = Screen.width;
			int H = Screen.height;

			int W2 = (int)(H * aspect);
			int X = (Screen.width - W2) / 2;

			X = 0;
			W2 = W;

			GUI.DrawTexture ( new Rect( X, 0, W2, H), tex);

			GUI.color = ((Time.time % 1.0f) < 0.5f) ? Color.black : Color.white;

			for (int j = 0; j < down; j++)
			{
				for (int i = 0; i < across; i++)
				{
					int x = (W2 * i) / across;
					int y = (H * j) / down;
					int w = W / across;
					int h = W / down;

					GUI.Label (
						new Rect( X + x, y, w, h),
						depths[i,j].ToString (),
						OurStyles.LABELLJ (10));
				}
			}
		}
		else
		{
			GUI.Label( MR.SR( 0,0,0.5f, 0.1f), "Press H to show heights", OurStyles.LABELULX( 14));
		}
	}
}
