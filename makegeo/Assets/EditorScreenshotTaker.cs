using UnityEngine;
using System.Collections;

public class EditorScreenshotTaker : MonoBehaviour
{
	static bool tried;

	string prefix = "~/Desktop/";
//	string prefix = "/Users/kurt/Desktop/";

	string appName = "KurtGame";

	public static void PossiblyAttach( string appName)
	{
		if (tried) return;
		tried = true;
		if (Application.isEditor)
		{
			EditorScreenshotTaker est = new GameObject(
				"EditorScreenshotTaker().PossiblyAttach();").
				AddComponent<EditorScreenshotTaker>();
			est.appName = appName;
			DontDestroyOnLoad( est.gameObject);
		}
	}

	void Update ()
	{
		if (Input.GetKeyDown( KeyCode.BackQuote))
		{
			string filename = System.String.Format(
				"{0}Screenshot-{1}-{2}.png",
					prefix, appName, System.DateTime.Now.ToFileTime());
			Debug.Log ( filename);
			Application.CaptureScreenshot( filename);
		}
	}
}
