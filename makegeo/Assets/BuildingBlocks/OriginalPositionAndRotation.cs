using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OriginalPositionAndRotation : MonoBehaviour
{
	public static OriginalPositionAndRotation Attach( Transform tr)
	{
		var opar = tr.gameObject.AddComponent<OriginalPositionAndRotation>();

		opar.Sequence = tr.GetSiblingIndex();

		opar.Position = tr.position;
		opar.Rotation = tr.rotation;

		return opar;
	}

	public int Sequence { get; private set; }

	public Vector3 Position { get; private set; }

	public Quaternion Rotation { get; private set; }
}
