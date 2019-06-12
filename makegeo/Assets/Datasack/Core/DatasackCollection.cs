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

	[Header("This holds ALL datasacks in your project.", order = 100)]

	[Header("Do not modify it yourself!", order = 200)]

	[Header("Instead use CODEGEN from any", order = 300)]
	[Header("Datasack object to regenerate.", order = 310)]

	public	DatasackMapping[] Mappings;
}
