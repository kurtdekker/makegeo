using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// @kurtdekker - the only purpose of this is to live at the
// top of each room chunk and tell you what the exit is.

public class ChainableRoom : MonoBehaviour
{
	[Header("Drag the exit of your room chunk into here.")]
	public Transform Exit;
}
