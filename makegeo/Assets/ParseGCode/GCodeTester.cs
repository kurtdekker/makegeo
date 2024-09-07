using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GCodeTester : MonoBehaviour
{
	[Header( "Drag your GCode text file in here:")]
	public TextAsset GFile;

	GCodeReader gcr;

	IEnumerator Start ()
	{
		string[] lines = GFile.text.Split(
			new char[] { '\n'},
			System.StringSplitOptions.RemoveEmptyEntries);

		gcr = new GCodeReader(lines);

		while( true)
		{
			yield return gcr.RunOneCommand();
			yield return null;
		}
	}
}
