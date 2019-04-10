using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DatasackCollection : ScriptableObject
{
	[System.Serializable]
	public class DatasackMapping
	{
		public string Fullname;
		public Datasack Datasack;
	}

	[Header("This holds ALL datasacks in your project.")]

	[Header("Do not modify it yourself!")]

	[Header("Instead use CODEGEN from any")]
	[Header("Datasack object to regenerate.")]

	public	DatasackMapping[] Mappings;
}
