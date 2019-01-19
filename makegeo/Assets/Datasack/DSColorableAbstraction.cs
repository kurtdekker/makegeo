/*
	The following license supersedes all notices in the source code.

	Copyright (c) 2018 Kurt Dekker/PLBM Games All rights reserved.

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
using UnityEngine.UI;

// WARNING! Internal class: other Datasack scripts will add this as needed.

public class DSColorableAbstraction : MonoBehaviour
{
	private	Text	text;
	private Image	image;

	// <WIP> observe and interoperate with other colorable objects

	public	static	DSColorableAbstraction	Attach( MonoBehaviour script)
	{
		DSColorableAbstraction ca = script.gameObject.AddComponent<DSColorableAbstraction>();
		return ca;
	}

	void	LazyFinder()
	{
		if (!text)
		{
			text = GetComponent<Text>();
		}
		if (!image)
		{
			image = GetComponent<Image>();
		}
	}

	public	void	SetColor( Color c)
	{
		LazyFinder();

		bool good = false;

		if (text)
		{
			text.color = c;
			good = true;
		}

		if (image)
		{
			image.color = c;
			good = true;
		}

		if (!good)
		{
			Debug.LogError( GetType() + ".SetColor(): no suitable colorable object found.");
		}
	}
}
