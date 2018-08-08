using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO: This class is to replace the use of the following classes:
//
//	DSAudioPlay - oneshotter (poke to play)
//	DSAudioVolume - volume control (offered by an optional Datasack)
//	DSAudioPitch - pitch control (offered by optional Datasack)
//
// Additional controls:
//
//	Control Looping on/off
//	Continuous volume control using tweening to different levels?
//

public class DSAudioControl : MonoBehaviour
{
	[Header("WARNING - NOT IMPLEMENTED YET!")]

	public	Datasack	DSControlVolume;

	public	Datasack	DSControlPitch;

	public	Datasack	DSControlLooping;

	public	Datasack	DSVolumeChangeRate;

	// <WIP> follow the AudioSource[] array methodology of
	// DSAudioPlay, including the various play strategies.

	// <WIP> non-supplied datavars should not
	// modify the underlying AudioSource parameter.

	void Awake()
	{
		throw new System.NotImplementedException( "DSAudioControl.Awake();");
	}
}
