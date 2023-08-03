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

public class ProximityButtonForUI : MonoBehaviour
{
	[Tooltip("Populate this with RectTransforms with the things you want considered as touchable points. " +
		"These do NOT have to be buttons and don't even have to be raycastable at this stage.")]
	public RectTransform[] ButtonSubRectangles;

	[Tooltip("Popoulate this with a separate RectTransform that defines the area you want considered close enough " +
		"for the individual RectTransforms inside. Also used to find the root canvas RectTransform.")]
	public RectTransform ContainingRectangle;

	RectTransform canvasRT;
	CanvasScaler scaler;

	void Awake()
	{
		var canvas = ContainingRectangle.GetComponentInParent<Canvas>();
		if (canvas.renderMode != RenderMode.ScreenSpaceOverlay)
		{
			Debug.LogError( GetType() + ".Awake(): Unsupported Canvas.RenderMode: " + canvas.renderMode.ToString());
		}
		canvasRT = canvas.GetComponent<RectTransform>();
		scaler = canvasRT.GetComponent<CanvasScaler>();
	}

	public Vector3 ScreenPosToUIPos( Vector3 pos)
	{
		switch( scaler.uiScaleMode)
		{
		case CanvasScaler.ScaleMode.ConstantPixelSize :
			pos = (pos * canvasRT.rect.height) / Screen.height;
			break;

		case CanvasScaler.ScaleMode.ScaleWithScreenSize :
			break;

//		case CanvasScaler.ScaleMode.ConstantPhysicalSize :
//			break;

		default :
			Debug.LogError( GetType() +
				".GetButtonTouchedNames(): unhandled CanvasScaler ScaleMode: " +
				scaler.uiScaleMode.ToString());
			break;
		}
		return pos;
	}

	// Returns a string array of the same length as the
	// number of subrectangle areas in the enclosing box.
	// Returns null if you aren't touching it, or the GameObject name if you are
	public string[] GetButtonTouchedNames()
	{
		MicroTouch[] allTouches = MicroTouch.GatherMicroTouches();

		List<MicroTouch> withins = new List<MicroTouch>();

		// Transform the screen touch coordinates into canvas UI coordinates
		// If they are within the super rect, add them to our checked touches
		for (int i = 0; i < allTouches.Length; i++)
		{
			var p = ScreenPosToUIPos( allTouches[i].position);
			if (p.x >= ContainingRectangle.rect.x + ContainingRectangle.position.x)
			{
				if (p.x < ContainingRectangle.rect.x + ContainingRectangle.rect.width + ContainingRectangle.position.x)
				{
					if (p.y >= ContainingRectangle.rect.y + ContainingRectangle.position.y)
					{
						if (p.y < ContainingRectangle.rect.y + ContainingRectangle.rect.height + ContainingRectangle.position.y)
						{
							allTouches[i].position = p;
							withins.Add( allTouches[i]);
						}
					}
				}
			}
		}

		string[] results = new string[ ButtonSubRectangles.Length];

		for (int t = 0; t < withins.Count; t++)
		{
			float distance = 0;
			int closestb = 0;

			for (int b = 0; b < ButtonSubRectangles.Length; b++)
			{
				float d = Vector3.Distance( withins[t].position, ButtonSubRectangles[b].position);
				if ( b == 0 || d < distance)
				{
					distance = d;
					closestb = b;
				}
			}

			results[closestb] = ButtonSubRectangles[closestb].name;
		}

		return results;
	}
}
