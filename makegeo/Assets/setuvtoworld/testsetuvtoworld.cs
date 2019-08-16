using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testsetuvtoworld : MonoBehaviour
{
	void	OnChanged( Datasack ds)
	{
		switch( ds.Value)
		{
		case "ButtonAddSetUVToWorldScript" :
			var uvsetter = gameObject.AddComponent<SetUVToWorld>();
			uvsetter.DriveToAllChildren = true;
			break;
		}
	}

	void	OnEnable()
	{
		DSM.UserIntent.OnChanged += OnChanged;	
	}
	void	OnDisable()
	{
		DSM.UserIntent.OnChanged -= OnChanged;	
	}
}
