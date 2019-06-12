#if UNITY_EDITOR

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

// This is just a handy script to let you bulk-create
// a large number of Datasacks, such as when you are
// replicating a particular namespaced structure.
//
// To use:
//	1. fill out the names in the array below
//	2. update the destination path folder
//	3. enable the [MenuItem...] decorator
//	4. goto your Unity editor menu 'Assets' and run it

public static class DatasackEditorUtils
{
	// 2. update the destination path folder (or just use this one)
	const string DestinationFolderPath = "Assets/Datasack/Resources/Datasacks/";

	// 3. uncomment this [MenuItem...] decorator
//	[MenuItem( "Assets/Create bulk datasacks")]
	static void CreateBulkDatasacks()
	{
		// 1. fill out these names (or load them from a file?)	
		string[] names = new string[] {
			"DatasackName1",
			"DatasackName2",
			"DatasackName3",
		};

		foreach( var nm in names)
		{
			var ds = ScriptableObject.CreateInstance<Datasack>();
			ds.name = nm;

			AssetDatabase.CreateAsset( ds, DestinationFolderPath + nm + ".asset");
		}
	}
}

#endif
