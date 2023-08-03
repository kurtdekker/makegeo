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
using System.Collections;
using System.Collections.Generic;

public class ProximityButtonSet : MonoBehaviour
{
	public class ProximityButton
	{
		// static text label
		public string label;
		// provide this in lieu of a static label
		public System.Func<string> GetLabel;

		public Vector2 labelSize;
		public Vector2 position;
		public bool fingerDown;
		public bool fingerTouched;
		public bool prevFingerDown;
		public bool enabled;

		public Color labelColor = new Color( 0.7f, 0.7f, 0.7f, 0.7f);

		public Texture2D texture;
		public float textureAngle;
		public Vector2 textureSize;
		public Color textureColor = Color.white;

		// Warning: if you use this property, you should also
		// enable manual updating and call it in a consistently
		// orderly fashion, or else you may miss trigger events.
		public bool trigger
		{
			get
			{
				return fingerDown && !prevFingerDown;
			}
		}

		public void SetPosition( Vector2 newPosition)
		{
			position = newPosition;
		}

		public ProximityButton( string label, Vector2 position)
		{
			this.enabled = true;
			this.position = position;
			this.label = label;
			this.labelSize = new Vector2(Screen.width * 0.10f, Screen.height * 0.10f);
		}
	}

	List<ProximityButton> pbses;

	public bool ManualUpdating;

	float diameter;

	public void SetDiameter( float newDiameter)
	{
		diameter = newDiameter;
	}

	public ProximityButton AddButton( string label, Vector2 position)
	{
		ProximityButton pb = new ProximityButton (label, position);
		pbses.Add (pb);
		return pb;
	}

	public static ProximityButtonSet Create( float diameter)
	{
		ProximityButtonSet pbs = new GameObject ("ProximityButtonSet.Create();").
			AddComponent<ProximityButtonSet> ();
		pbs.diameter = diameter;
		pbs.pbses = new List<ProximityButton> ();
		return pbs;
	}

	void Update()
	{
		if (!ManualUpdating)
		{
			MyUpdate ();
		}
	}
	
	public void MyUpdate ()
	{
		MicroTouch[] mts = MicroTouch.GatherMicroTouches();

		foreach( ProximityButton pb in pbses)
		{
			pb.prevFingerDown = pb.fingerDown;
			pb.fingerDown = false;
			pb.fingerTouched = false;
		}

		foreach (MicroTouch t in mts)
		{
			Vector2 pos = new Vector2( t.position.x, Screen.height - t.position.y);
		
			ProximityButton pbClosest = null;
			float distClosest = 0;
			foreach( ProximityButton pb in pbses)
			{
				float distance = Vector3.Distance( pb.position, pos);
				if (distance < diameter)
				{
					if ((pbClosest == null) ||
						(distance < distClosest))
					{
						pbClosest = pb;
						distClosest = distance;
					}
				}
			}
			if (pbClosest != null)
			{
				if (pbClosest.enabled)
				{
					pbClosest.fingerDown = true;

					if (t.phase == TouchPhase.Began)
					{
						pbClosest.fingerTouched = true;
					}
				}
			}
		}
	}
	
	void OnGUI()
	{
		foreach( ProximityButton pb in pbses)
		{
			if (pb.enabled)
			{
				string label = pb.label;

				if (pb.GetLabel != null)
				{
					label = pb.GetLabel();
				}

				if (label != null)
				{
					GUI.color = pb.labelColor;
					Rect r = new Rect(0, 0, Screen.width * 0.10f, Screen.height * 0.05f);
					r.center = pb.position;
					GUI.Label(r, label, OurStyles.LABELCJ(10));
				}
				if (pb.texture != null)
				{
					GUI.color = pb.textureColor;
					GUI.matrix = Matrix4x4.identity;
					GUIUtility.RotateAroundPivot(pb.textureAngle, pb.position);
					Rect r = new Rect();
					r.size = pb.textureSize;
					r.center = pb.position;
					GUI.DrawTexture(r, pb.texture);
					GUI.matrix = Matrix4x4.identity;
				}
			}
		}
	}
}
