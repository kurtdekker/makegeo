using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// @kurtdekker - just doodling, this is an incomplete, ultra cheese ultra cheap
// G-Code parser, based entirely on what I read here:
//
// This only prints a since slice right now.
//
// NOTE: I am NOT affiliated with the following website in any way!
//
// https://howtomechatronics.com/tutorials/g-code-explained-list-of-most-important-g-code-commands/
//
// NOTE: this will create a LOT of GameObjects. It is in no way intended as a way
// to visualize anything but the cheapest and cheesiest 3D model slices.
//
// Somebody else, take it away.... GO!
//

public class GCodeReader
{
	string[] Lines;
	int Index;

	// we'll track planar Z later...
	Vector2 cursor;

	float minSphereSize = 0.02f;
	float maxSphereSize = 0.03f;

	float printheadLength = 0.2f;
	GameObject printHead;

	public GCodeReader( string[] lines)
	{
		Lines = lines;
		Index = 0;

		printHead = GameObject.CreatePrimitive( PrimitiveType.Cube);
		printHead.transform.localScale  = new Vector3( 0.05f, printheadLength, 0.05f);
	}

	Vector3 GCode2World( Vector2 gcode)
	{
		return new Vector3( gcode.x, 0, gcode.y);
	}

	void PrintheadChase()
	{
		Vector3 worldPosition = GCode2World( cursor);

		printHead.transform.position = worldPosition + Vector3.up * printheadLength / 2;
	}

	void SnapPrinthead( Vector3 position)
	{
		cursor = position;
		PrintheadChase();
	}

	void DepositMaterial()
	{
		GameObject blob = GameObject.CreatePrimitive( PrimitiveType.Cube);
		blob.transform.position = GCode2World(cursor);
		blob.transform.localScale = new Vector3(
			Random.Range( minSphereSize, maxSphereSize),
			Random.Range( minSphereSize, maxSphereSize),
			Random.Range( minSphereSize, maxSphereSize));
	}

	IEnumerable DragPrinthead( Vector2 targetPosition)
	{
		Vector3 currentCursor = cursor;

		float distance = Vector2.Distance( targetPosition, currentCursor);

		int count = 1 + (int)(distance / minSphereSize);

		for (int i = 0; i <= count; i++)
		{
			float fraction = (float)i / count;

			Vector2 position = Vector2.Lerp( currentCursor, targetPosition, fraction);

			cursor = position;

			DepositMaterial();

			PrintheadChase();

			yield return null;
		}

		cursor = targetPosition;
	}

	float Parse(string[]parts, string s)
	{
		char c = s[0];

		foreach( var part in parts)
		{
			if (part[0] == c)
			{
				return float.Parse( part.Substring(1));
			}
		}
		Debug.LogError( "cannot parse for '" + c + "' in these lines");
		for (int i = 0; i < parts.Length; i++)
		{
			Debug.LogError( "Part " + i + ": " + parts[i]);
		}
		throw new System.FormatException( "cannot parse for '" + c + "' in these lines");
	}

	IEnumerable RotateAround( Vector2 targetPosition, Vector2 center)
	{
		Vector2 currentCursor = cursor;

		// the two arms
		Vector2 dA = currentCursor - center;
		Vector2 dB = targetPosition - center;

		float radius = dA.magnitude;

		radius = dB.magnitude;

		float dotProduct = Vector2.Dot( dA, dB);

		dotProduct = dA.x * dB.x + dA.y * dB.y;

		float cos = dotProduct / (radius * radius);

		if (cos < -1 || cos > +1)
		{
			Debug.LogWarning( "acos domain exceedance.");
			yield break;
		}

		float theta = Mathf.Acos( cos);

		float arcLength = radius * theta;

		int count = 1 + (int)(arcLength / minSphereSize);

		for (int i = 0; i <= count; i++)
		{
			float fraction = (float)i / count;

			Vector2 position = center + (Vector2)Vector3.Slerp( dA, dB, fraction);

			cursor = position;

			DepositMaterial();

			PrintheadChase();

			yield return null;
		}

		cursor = targetPosition;
	}

	public IEnumerator RunOneCommand()
	{
		int operationsPerYield = 10;
		int operationCounter = 0;

		System.Func<bool> ShouldYield = () => {
			operationCounter++;
			return (operationCounter % operationsPerYield) == 0;
		};

		string line = Lines[Index];

		string[] parts = line.Split( ' ');

		string cmd = parts[0];

		Vector2 position = Vector3.zero;

		Vector2 center = Vector2.zero;

		System.Func<Vector2> ReadXY = () => {
			float x = Parse(parts, "X");
			float y = Parse(parts, "Y");
			return new Vector2( x, y);
		};

		System.Func<Vector2> ReadIJ = () => {
			float i = Parse(parts, "I");
			float j = Parse(parts, "J");
			return new Vector2( i, j);
		};

		switch(cmd)
		{
		case "%":
			break;
		case ";":
			break;

		case "G21":
		case "G17":
		case "G90":
			// silent fail for now
			break;

		case "M03":
			break;

		case "G28":
			break;

		case "G00": // fast reposition
			position = ReadXY();
			SnapPrinthead(position);
			break;

		case "G01": // linear transit
			position = ReadXY();
			foreach( var itt in DragPrinthead(position))
			{
				if (ShouldYield())
					yield return null;
			}
			break;

		case "G02": // clockwise arc
		case "G03": // counter-clockwise arc
			position = ReadXY();
			center = ReadIJ() + cursor;
			foreach( var itt in RotateAround(position, center))
			{
				if (ShouldYield())
					yield return null;
			}
			break;

		default :
			throw new System.NotImplementedException(cmd);
			break;
		}

		Index++;

		yield return null;
	}
}
