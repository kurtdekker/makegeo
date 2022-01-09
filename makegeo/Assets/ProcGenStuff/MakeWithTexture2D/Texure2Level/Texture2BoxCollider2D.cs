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

#if UNITY_EDITOR
using UnityEditor;
#endif

// @kurtdekker
//
// Typical workflow for this script:
//
//	Edit your texture at the pixel level
//	Import texture:
//		- import as sprite
//		- no mipmaps
//		- Filter Mode Point
//		- Read Write Enabled
//		- NO compression
//	Emplace empty GameObject with this script on it
//		- move it to where you want lower left pixel to be
//		- make sure your Z == 0
//	Put this script on a parent GameObject
//	Check the colors; default is black for collider, red for trigger
//	Check the cell size; default is (1,1)
//
// Press the Regenerate button
//
// WARNING: do not change the scale of this object
// WARNING: Regenerate wipes out all children under 'Generated'
//	If you want to keep some, move them to another child GameObject

public class Texture2BoxCollider2D : MonoBehaviour
{
	[Header( "Texture (mark as read/write, NO compression!)")]
	public Texture2D Texture;

	[Header( "Colors to make colliders")]
	public Color[] ColliderColors;

	[Header( "Colors to make triggers")]
	public Color[] TriggerColors;

	[Header( "Cell size (World units)")]
	public Vector2 CellSize;

	[Header( "Zero offset (World units)")]
	public Vector2 ZeroOffset;

	[Header( "How close does the color have to be?")]
	public float ColorEpsilon;

	[Header( "How to tint the results in editor.")]
	public Material ColliderMaterial;
	public Material TriggerMaterial;

	const string s_GeneratedGameObjectName = "Generated";

	void Reset()
	{
		ColliderColors = new Color[] {
			Color.black,
		};

		TriggerColors = new Color[] {
			Color.red,
		};

		CellSize = new Vector2( 1, 1);

		ZeroOffset = new Vector2( 0, 0);

		ColorEpsilon = 0.1f;
	}

#if UNITY_EDITOR
	public enum RunningState
	{
		OFF,
		COLLIDER,
		TRIGGER,
	}

	bool IsColor( Color c1, Color c2)
	{
		float distance = Mathf.Abs( c1.r - c2.r) +
			Mathf.Abs( c1.g - c2.g) +
			Mathf.Abs( c1.b - c2.b);

		return distance < ColorEpsilon;
	}

	RunningState EvaluateColor( Color c1)
	{
		foreach( var c2 in ColliderColors)
		{
			if (IsColor( c1, c2)) return RunningState.COLLIDER;
		}

		foreach( var c2 in TriggerColors)
		{
			if (IsColor( c1, c2)) return RunningState.TRIGGER;
		}

		return RunningState.OFF;
	}

	void Regenerate()
	{
		// delete all children under "Generated"
		for (int i = 0; i < transform.childCount; i++)
		{
			var tr = transform.GetChild(i);

			if (tr.name == s_GeneratedGameObjectName)
			{
				DestroyImmediate( tr.gameObject);
			}
		}

		var parent = new GameObject( s_GeneratedGameObjectName).transform;
		parent.SetParent( transform);
		parent.localPosition = Vector3.zero;

		for (int j = 0; j < Texture.height; j++)
		{
			// run-length encode the colliders going across on each line
			RunningState state = RunningState.OFF;
			int count = 0;
			Color lastColor = Color.magenta;
			for (int i = 0; i < Texture.width; i++)
			{
				var c1 = Texture.GetPixel( i, j);

				var rs = EvaluateColor(c1);

				if (rs == state)
				{
					count++;
				}
				else
				{
					// change of state
					CheckProduceCollider( lastColor, i, j, count, state, parent);

					state = rs;
					count = 1;
				}

				lastColor = c1;
			}
			// flush any pending run
			if (count > 0)
			{
				CheckProduceCollider( lastColor, Texture.width, j, count, state, parent);
			}
		}
	}

	void CheckProduceCollider( Color c1, int i, int j, int count, RunningState state, Transform parent)
	{
		switch( state)
		{
		case RunningState.COLLIDER :
		case RunningState.TRIGGER :
			if (count > 0)
			{
				var quad = GameObject.CreatePrimitive( PrimitiveType.Quad);

				DestroyImmediate( quad.GetComponent<Collider>());

				var bc = quad.AddComponent<BoxCollider2D>();

				if (state == RunningState.TRIGGER)
				{
					bc.isTrigger = true;
				}

				quad.transform.SetParent( parent);

				float lateralOffset = ((float)count / 2) + 1;
				float lateralScale = count;

				Vector3 position = Vector3.left * lateralOffset;

				position += new Vector3( CellSize.x * i, CellSize.y * j);

				position += (Vector3)ZeroOffset;

				quad.transform.localPosition = position;

				Vector3 scale = new Vector3( CellSize.x * lateralScale, CellSize.y * 1, 1);

				quad.transform.localScale = scale;

				int x = i - (1 + count);
				int y = j;

				var nm = System.String.Format( "T2BC2D-{2}-({0},{1})-W:{3}", x, y, state, count);

				quad.name = nm;

				var rndrr = quad.GetComponent<Renderer>();

				if (state == RunningState.COLLIDER)
				{
					rndrr.material = ColliderMaterial;	
				}
				if (state == RunningState.TRIGGER)
				{
					rndrr.material = TriggerMaterial;	
				}
			}
			break;
		}
	}

	[CustomEditor(typeof(Texture2BoxCollider2D))]
	public class Texture2BoxCollider2DEditor : Editor
	{
		public override void OnInspectorGUI()
		{
			Texture2BoxCollider2D t2bc = (Texture2BoxCollider2D)target;

			DrawDefaultInspector();

			EditorGUILayout.BeginVertical();

			GUILayout.Space(20);

			if (GUILayout.Button( "REGENERATE"))
			{
				t2bc.Regenerate();
			}

			GUILayout.Space(20);

			GUILayout.Label( "WARNING: be sure to dirty your scene and save it!");

			EditorGUILayout.EndVertical();
		}
	}
#endif
}
