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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class UVTranslators
{
	// Purpose: Remapping UV coordinates from one area to another.
	//
	// Looks for UVs falling within the source rectangle, descales them,
	// moves them to the dest rectangle, and rescales them.

	public static void RemapRectangular(
		Mesh mesh,
		Rect source,
		Rect dest
	)
	{
		var original = mesh.uv;

		var remapped = new Vector2[original.Length];

		for (int i = 0; i < original.Length; i++)
		{
			var uv = original[i];

			// is this UV in the area of interest?
			if (source.Contains(uv))
			{
				// translate to around zero
				uv.x -= source.center.x;
				uv.y -= source.center.y;

				// descale it
				if (source.width > 0)
				{
					uv.x /= source.width;
				}
				if (source.height > 0)
				{
					uv.y /= source.height;
				}

				// rescale it
				uv.x *= dest.width;
				uv.y *= dest.height;

				// put it back where you want it
				uv.x += dest.center.x;
				uv.y += dest.center.y;
			}

			remapped[i] = uv;
		}

		mesh.uv = remapped;
	}
}
