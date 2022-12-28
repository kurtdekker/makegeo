using UnityEngine;

[ExecuteInEditMode]
public class makeringlinerenderer : MonoBehaviour
{
	public LineRenderer lineRenderer;

	public float Radius = 2.5f;
	public int Segments = 20;

	void OnValidate()
	{
		if (lineRenderer)
		{
			LineRendererUtility.MakeRing( lineRenderer: lineRenderer, radius: Radius, segments: Segments);
		}
	}
}
