using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PixelTerrain1 : MonoBehaviour
{
	[Header( "The output renderer: we'll clone its material.")]
	[Header( "Put a collider on it if you want to click on it.")]
	public Renderer Target;

	[Header( "Power of 2 best, may even be required!")]
	public int Size = 64;

	Material mtl;

	Texture2D workTexture;
	Color[] workPixels;

	Color ColorDirt = new Color( 0.7f, 0.4f, 0.2f);	// brownish
	Color ColorNothing = Color.black;

	void Start ()
	{
		// backup find
		if (!Target)
		{
			Target = GetComponent<Renderer>();
		}

		// complain
		if (!Target)
		{
			Debug.LogError( "Did not provide Target renderer.");
			Debug.Break();
			return;
		}

		// get
		mtl = Target.material;

		// copy
		mtl = new Material(mtl);

		// work output
		workTexture = new Texture2D( Size, Size);
		workTexture.filterMode = FilterMode.Point;
		workTexture.wrapMode = TextureWrapMode.Clamp;

		// pixels
		workPixels = workTexture.GetPixels();

		// clear
		for (int i = 0; i < workPixels.Length; i++)
		{
			workPixels[i] = ColorNothing;
		}

		// display
		mtl.mainTexture = workTexture;
		Target.material = mtl;
	}

	void UpdateClicking()
	{
		Vector3 mousePosition = Input.mousePosition;

		Camera cam = Camera.main;

		Ray ray = cam.ScreenPointToRay( mousePosition);
		RaycastHit hit;

		if (Physics.Raycast( ray: ray, hitInfo: out hit))
		{
			Vector2 texel = hit.textureCoord;

			int x = (int)(texel.x * Size);
			int y = (int)(texel.y * Size);

			int offset = x + y * Size;

			// add dirt?
			if (Input.GetMouseButton(0))
			{
				workPixels[offset] = ColorDirt;
			}

			// blow hole?
			if (Input.GetMouseButtonDown(1))
			{
				for (int j = -2; j <= 2; j++)
				{
					for (int i = -2; i <= 2; i++)
					{
						int m = x + i;
						int n = y + j;

						if (m >= 0 && m < Size)
						{
							if (n >= 0 && n < Size)
							{
								offset = m + n * Size;
								workPixels[offset] = ColorNothing;
							}
						}
					}
				}
			}
		}
	}

	void UpdateFallingSand()
	{
		// traverse from bottom up, moving pixels down-ish
		for (int y = 1; y < Size; y++)
		{
			for (int x = 0; x < Size; x++)
			{
				// random stagger
				if (Random.value > 0.2f) continue;

				int offset = x + y * Size;

				Color c = workPixels[ offset];

				if (c == ColorDirt)
				{
					// fall straight down?
					if (workPixels[offset - Size] != ColorDirt)
					{
						workPixels[offset] = ColorNothing;
						workPixels[offset - Size] = c;
						continue;
					}
					// fall left diagonal
					if (x > 0)
					{
						if (workPixels[offset - Size - 1] != ColorDirt)
						{
							workPixels[offset] = ColorNothing;
							workPixels[offset - Size - 1] = c;
							continue;
						}
					}
					// fall right diagonal
					if (x < Size - 1)
					{
						if (workPixels[offset - Size + 1] != ColorDirt)
						{
							workPixels[offset] = ColorNothing;
							workPixels[offset - Size + 1] = c;
							continue;
						}
					}
				}
			}
		}
	}

	void UpdatePixelsToTexture()
	{
		workTexture.SetPixels( workPixels);
		workTexture.Apply();
	}

	void Update ()
	{
		UpdateClicking();
		UpdateFallingSand();
		UpdatePixelsToTexture();
	}
}
