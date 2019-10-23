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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// WARNING! Internal class: other Datasack scripts will add this as needed.

// NOTE: Presently there are no uses for this within Datasacks module since
// the Datasacks do not support Texture2D or sprite storage.

public class DSSkinnableAbstraction : MonoBehaviour
{
	private Image	image;

	// <WIP> observe and interoperate with other types of skinnable objects

	public	static	DSSkinnableAbstraction	Attach( GameObject go)
	{
		DSSkinnableAbstraction sa = go.AddComponent<DSSkinnableAbstraction>();
		return sa;
	}
	public	static	DSSkinnableAbstraction	Attach( MonoBehaviour script)
	{
		return Attach( script.gameObject);
	}

	void	LazyFinder()
	{
		if (!image)
		{
			image = GetComponent<Image>();
		}
	}

	public void SetSprite( Sprite sprite)
	{
		if (!sprite)
		{
			SetEnabled(false);
			return;
		}

		LazyFinder();

		bool good = false;

		if (image)
		{
			image.sprite = sprite;
			good = true;
		}

		if (!good)
		{
			Debug.LogError(GetType() + ".SetSprite(): no suitable skinnable object found.");
		}
	}

	public void SetTexture2D( Texture2D t2d)
	{
		if (!t2d)
		{
			SetEnabled(false);
			return;
		}

		Rect r = new Rect( 0, 0, t2d.width, t2d.height);

		var sprite = Sprite.Create( t2d, r, Vector2.one * 0.5f);

		SetSprite( sprite);
	}

	public void SetEnabled( bool ena)
	{
		LazyFinder();

		if (image)
		{
			image.enabled = ena;
		}
	}
}
