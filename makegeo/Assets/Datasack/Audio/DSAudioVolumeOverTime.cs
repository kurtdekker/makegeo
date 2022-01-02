/*
	The following license supersedes all notices in the source code.

	Copyright (c) 2021 Kurt Dekker/PLBM Games All rights reserved.

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

using UnityEngine;

// Purpose: for setting volumes (instantly from your perspective)
// but having them traverse up/down gradually over time (fading)

public class DSAudioVolumeOverTime : MonoBehaviour
{
	public	Datasack	dataSack;

	[Header("How rapidly it slews 100% span (0 == instant)")]
	public float FadeUpRate;
	public float FadeDownRate;

	public bool DisregardInitialVolume;

	private AudioSource[] azzs;
	private float[] initialVolumes;

	// this gets moved slowly towards the datasack
	float currentVolume;
	// set by the datasack
	float desiredVolume;

	private void Reset()
	{
		// instantaneous
		FadeUpRate = 0.0f;
		// will slew full range in 1/2 a second
		FadeDownRate = 2.0f;
	}

	void Start ()
	{
		OnChanged (dataSack);

		currentVolume = desiredVolume;

		SendCurrentVolumeToAudioSources();
	}

	void	SendCurrentVolumeToAudioSources()
	{
		var volume = currentVolume;
		for (int i = 0; i < azzs.Length; i++)
		{
			var az = azzs[i];
			az.volume = DisregardInitialVolume ? volume : volume * initialVolumes[i];
		}
	}

	private void Update()
	{
		bool changed = false;

		if (currentVolume < desiredVolume)
		{
			if (FadeUpRate <= 0) currentVolume = desiredVolume;

			currentVolume = Mathf.MoveTowards(currentVolume, desiredVolume, FadeUpRate * Time.deltaTime);
			changed = true;
		}
		if (currentVolume > desiredVolume)
		{
			if (FadeDownRate <= 0) currentVolume = desiredVolume;

			currentVolume = Mathf.MoveTowards(currentVolume, desiredVolume, FadeDownRate * Time.deltaTime);
			changed = true;
		}

		if (changed)
		{
			SendCurrentVolumeToAudioSources();
		}
	}

	void OnChanged(Datasack ds)
	{
		desiredVolume = ds.fValue;
	}

	void	OnEnable()
	{
		azzs = GetComponentsInChildren<AudioSource>();

		initialVolumes = new float[azzs.Length];
		for (int i = 0; i < azzs.Length; i++)
		{
			initialVolumes[i] = azzs[i].volume;
		}

		dataSack.OnChanged += OnChanged;	
	}
	void	OnDisable()
	{
		dataSack.OnChanged -= OnChanged;	
	}
}
