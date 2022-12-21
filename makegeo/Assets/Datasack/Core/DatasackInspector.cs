/*
	The following license supersedes all notices in the source code.

	Copyright (c) 2019 Kurt Dekker/PLBM Games All rights reserved.

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

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public partial class Datasack
{
	public override string ToString ()
	{
		return string.Format ("[Datasack: name={0}, FullName={1}, Value={2}, iValue={3}, fValue={4}, bValue={5}]",
			name, FullName, Value, iValue, fValue, bValue);
	}

	bool DebugBreak;
	bool DebugLogging;

#if UNITY_EDITOR

	string DisplayDebugOutput()
	{
		string part1 = "Datasack " + FullName + " is currently: '" + Value + "'";
		string part2 = " <not parseable as float>";
		try
		{
			part2 = "--> as float value = " + fValue;
		}
		catch { } 		// gotta catch 'em all: fairly harmless in a small context
		return part1 + part2;
	}

	[CustomEditor(typeof(Datasack)), CanEditMultipleObjects]
	public class DatasackEditor : Editor
	{
		string IdentifierSafeString( string s)
		{
			s = s.Replace( "_", "_");
			s = s.Replace( "/", "_");
			s = s.Replace( "\\", "_");

			s = s.Replace( " ", "_");

			s = s.Replace( "-", "_");
			s = s.Replace( "+", "_");

			s = s.Replace( "(", "_");
			s = s.Replace( ")", "_");

			s = s.Replace( ";", "_");
			s = s.Replace( "&", "_");
			s = s.Replace( "$", "_");

			return s;
		}

		void CreateStaticGetterExpression( ref string s, string indentation, Datasack ds, string variableName)
		{
			bool InsertExtraLineAfterwards = false;
			if (!string.IsNullOrEmpty( ds.Comments))
			{
				InsertExtraLineAfterwards = true;

				s += "\n";
				s += indentation + "\t// .Comments field from Datasack:\n";

				foreach( var comment in ds.Comments.Split(
					new string[] { System.Environment.NewLine},
					StringSplitOptions.RemoveEmptyEntries))
				{
					s += indentation + "\t//\t" + comment + "\n";
				}
			}

			if (ds.Save)
			{
				InsertExtraLineAfterwards = true;

				s += "\n";
				s += indentation + "\t// Persistent (has .Save field checked in Datasack):\n";
			}

			s += indentation + "\tpublic static Datasack " + IdentifierSafeString( ds.name) +
				" { get { return DSM.I.Get( \"" +
				variableName  + "\"); } }\n";

			if (InsertExtraLineAfterwards)
			{
				s += "\n";
			}
		}

		void GenerateCode()
		{
			Debug.Log( "CODEGEN!");

			string s = "//\n//\n//\n" + 
				"// MACHINE-GENERATED CODE - DO NOT MODIFY BY HAND!\n" +
				"//\n" +
				"// NOTE: You definitely SHOULD commit this file to source control!!!\n" +
				"//\n//\n" +
				"// To regenerate this file, select any Datasack object, look\n" +
				"// in the custom Inspector window and press the CODEGEN button.\n" +
				"//\n//\n//\n";

			s += "public partial class DSM\n{\n";

			// This now scans the entire project for Datasacks,
			// not just underneath the Resources directories.

			string[] datasackGUIDs = AssetDatabase.FindAssets( "t:Datasack");
			int numDatasacks = datasackGUIDs.Length;

			Dictionary<string,List<Datasack>> SplitByDirectory = new Dictionary<string, List<Datasack>>();

			string[] datasackPaths = new string[ numDatasacks];
			for (int i = 0; i < numDatasacks; i++)
			{
				datasackPaths[i] = AssetDatabase.GUIDToAssetPath( datasackGUIDs[i]);
			}

			System.Array.Sort( datasackPaths);

			Datasack[] sacks = new Datasack[ numDatasacks];

			for (int i = 0; i < datasackGUIDs.Length; i++)
			{
				string assetPath = datasackPaths[i];

				sacks[i] = AssetDatabase.LoadAssetAtPath<Datasack>( assetPath);

				var ds = sacks[i];

				string dirName = System.IO.Path.GetDirectoryName( assetPath);
				if (!SplitByDirectory.ContainsKey( dirName))
				{
					SplitByDirectory[dirName] = new List<Datasack>();
				}
				SplitByDirectory[dirName].Add( ds);
			}

			// When producing the nested class name that will represent each subfolder,
			// we search upwards and stop when we hit a folder named any of these values.
			//
			// This means datasacks at any of these folders will be presented as
			// "flat namespace," i.e., just DSM.MyName rather than DSM.Folder.MyName.
			List<string> SpecialDirectoriesThatBeginDatasackNamespacing = new List<string>
			{
				"Resources",
				"Datasacks",
				"Datasack",
				"Assets",
			};

			foreach( var dirName in SplitByDirectory.Keys)
			{
				string pathPrefix = "";
				string indentation = "";

				string[] directoryParts = dirName.Split(
					new char[] {
						System.IO.Path.DirectorySeparatorChar,
						System.IO.Path.AltDirectorySeparatorChar,
					},
					StringSplitOptions.RemoveEmptyEntries );

				string classDefinitionOpen = "";
				string classDefinitionClose = "";

				int partsToInclude = 0;
				for (int i = directoryParts.Length - 1; i >= 0; i--)
				{
					if (SpecialDirectoriesThatBeginDatasackNamespacing.Contains( directoryParts[i]))
					{
						break;
					}
					partsToInclude++;
				}

				for (int i = 0; i < partsToInclude; i++)
				{
					int partNumber = (directoryParts.Length - partsToInclude) + i;
					string part = directoryParts[partNumber];

					pathPrefix += part + "/";

					indentation = indentation + "\t";

					// append
					classDefinitionOpen += indentation + "public static partial class " + IdentifierSafeString( part) + "\n";
					classDefinitionOpen += indentation + "{\n";

					// prepend!!
					classDefinitionClose = indentation + "}\n" + classDefinitionClose;
				}

				s += "\n";
				s += "// Datasacks from directory '" + dirName + "'\n";

				s += classDefinitionOpen;

				foreach( var ds in SplitByDirectory[dirName])
				{
					string variableName = pathPrefix + ds.name;
					CreateStaticGetterExpression( ref s, indentation, ds, variableName);
					ds.FullName = variableName;
				}

				s += classDefinitionClose;
			}

			s += "}\n";

			s += "\n// Total of " + numDatasacks + " datasacks found and processed.\n";

			Debug.Log( s);

			{
				System.IO.Directory.CreateDirectory( DSM.s_AllDatasacksDirectory);

				string outfile = DSM.s_AllDatasacksDirectory + "DSMCodegen.cs";
				using( System.IO.StreamWriter sw =
					new System.IO.StreamWriter(outfile, false))
				{
					sw.Write(s);
				}
			}

			{
				bool create = false;

				string assetPath = DSM.s_AllDatasacksResources;

				System.IO.Directory.CreateDirectory( assetPath);

				assetPath = assetPath + DSM.s_AllDatasacksAsset + ".asset";

				var dsc = AssetDatabase.LoadAssetAtPath<DatasackCollection>( assetPath);
				if (!dsc)
				{
					create = true;
					dsc = ScriptableObject.CreateInstance<DatasackCollection>();
				}

				dsc.Mappings = new DatasackCollection.DatasackMapping[ sacks.Length];

				for (int i = 0; i < sacks.Length; i++)
				{
					dsc.Mappings[i] = new DatasackCollection.DatasackMapping();
					dsc.Mappings[i].Fullname = sacks[i].FullName;
					dsc.Mappings[i].Datasack = sacks[i];
				}

				if (create)
				{
					AssetDatabase.CreateAsset( dsc, assetPath);
				}

				EditorUtility.SetDirty(dsc);

				AssetDatabase.SaveAssets();
			}

			AssetDatabase.Refresh();
		}

		string PlayerPrefsKey()
		{
			Datasack ds = (Datasack)target;
			return DSM.s_PlayerPrefsPrefix + ds.FullName;
		}

		public override void OnInspectorGUI()
		{
			Datasack ds = (Datasack)target;

			DrawDefaultInspector();

			EditorGUILayout.BeginVertical();

			if (GUILayout.Button( "CODEGEN"))
			{
				GenerateCode();
			}

			GUILayout.Space(20);

			if (GUILayout.Button( "RUNTIME POKE"))
			{
				ds.Poke();
			}

			GUILayout.Space(20);

			if (GUILayout.Button( "RUNTIME TOGGLE"))
			{
				ds.bValue = !ds.bValue;
			}

			GUILayout.Space(20);

			if (GUILayout.Button( "RUNTIME INCREMENT"))
			{
				ds.iValue++;
			}

			GUILayout.Space(20);

			GUI.color = Color.green;
			if (GUILayout.Button( "RESET TO INITIAL VALUE"))
			{
				ds.Value = ds.InitialValue;
			}

			GUILayout.Space(20);

			GUI.color = Color.cyan;
			if (GUILayout.Button( "OUTPUT CURRENT VALUE"))
			{
				Debug.Log( ds.DisplayDebugOutput());
			}

			GUILayout.Space(20);

			GUI.color = Color.yellow;
			if (GUILayout.Button( "DELETE SAVED VALUE"))
			{
				if (PlayerPrefs.HasKey(PlayerPrefsKey()))
				{
					PlayerPrefs.DeleteKey( PlayerPrefsKey());
					PlayerPrefs.Save();
				}
			}

			GUILayout.Space(20);

			GUI.color = Color.red;
			if (GUILayout.Button( "DELETE ALL PLAYER PREFS"))
			{
				if (EditorUtility.DisplayDialog( "CONFIRM!",
					"Confirm DELETE all PlayerPrefs stored settings?\n\n" +
					"(THIS ACTION CANNOT BE UNDONE!)", "DELETE ALL PREFS", "Cancel"))
				{
					DSM.ResetDictionaryIfRunning();
					PlayerPrefs.DeleteAll();
					PlayerPrefs.Save();
				}
			}

			GUILayout.Space(20);

			GUI.color = ds.DebugBreak ? Color.white : Color.gray;

			GUILayout.BeginHorizontal();
			GUILayout.Label("Debug Break", GUILayout.Width(160));
			ds.DebugBreak = GUILayout.Toggle( ds.DebugBreak, "BREAK");
			GUILayout.EndHorizontal();

			GUI.color = ds.DebugLogging ? Color.white : Color.gray;

			GUILayout.BeginHorizontal();
			GUILayout.Label("Debug Logging", GUILayout.Width(160));
			ds.DebugLogging = GUILayout.Toggle( ds.DebugLogging, "LOG");
			GUILayout.EndHorizontal();

			EditorGUILayout.EndVertical();
		}
	}
#endif
}
