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

using UnityEngine;
using System.Collections;

public class VAButton : MonoBehaviour
{
	// You can provide manual overlay of the
	// standard Unity3D Input axes here.
	public string InputAxisOverlayX { private get; set; }
	public string InputAxisOverlayY { private get; set; }

	Texture2D t2d_button_ring;
	Rect r_button_ring;
	Rect r_button_finger;

	public bool doClamp;
	public bool doNormalize;

	public Rect r_label;
	public string label;
	public Color labelColor;
	public Color buttonColor;

	Rect _r_downable;

	// Primarily you set this, and it also sets r_label.
	// But then you can also set r_label independently.
	public Rect r_downable
	{
		get
		{
			return _r_downable;
		}
		set
		{
			_r_downable = value;
			r_label = value;
		}
	}

	public Vector3 outputRaw;	// pre-deadband
	public Vector3 output;
	public float minMagnitude;
	public bool detectedTap;
	public bool fingerDown;

	float beganTime;

	float deflectionRadius;

	public bool constrainFingerCenterWithinRing = true;

	Vector2 v2down;
	int fingerId;

	void Awake()
	{
		labelColor = new Color (0.7f, 0.7f, 0.7f);
		buttonColor = Color.white;
	}

	void Start ()
	{
		if (minMagnitude == 0)
		{
			minMagnitude = 0.2f;
		}

		t2d_button_ring = Resources.Load<Texture2D>(
			"Textures/uibuttons/20130926_button_ring");
		float sz = Screen.height * 0.2f;
		r_button_ring = new Rect( 0, 0, sz, sz);
		sz *= 0.7f;
		r_button_finger = new Rect( 0, 0, sz, sz);

		deflectionRadius = sz / 2;
	}
	
	void UpdatePosition( Vector2 pos)
	{
		if (!fingerDown) return;
		
		// axis assignments transform
		output.x = (pos.x - v2down.x) / deflectionRadius;
		output.y = (v2down.y - pos.y) / deflectionRadius;
		output.z = 0;

		if (doNormalize)
		{
			if (output.magnitude >= 1.0f)
			{
				output = output.normalized;
			}
		}
		if (doClamp)
		{
			output.x = Mathf.Clamp( output.x, -1.0f, 1.0f);
			output.y = Mathf.Clamp( output.y, -1.0f, 1.0f);
		}

		outputRaw = output;

		if (output.magnitude <= minMagnitude)
		{
			output = Vector3.zero;
			return;
		}
	}

	void OverlayStandardInputAxes()
	{
		if (InputAxisOverlayX != null)
		{
			float axisInput = Input.GetAxis ( InputAxisOverlayX);
			outputRaw.x += axisInput;
			output.x += axisInput;
		}
		if (InputAxisOverlayY != null)
		{
			float axisInput = Input.GetAxis ( InputAxisOverlayY);
			outputRaw.y += axisInput;
			output.y += axisInput;
		}
	}

	void CheckConstraints()
	{
		if (constrainFingerCenterWithinRing)
		{
			Vector2 center;
			center = r_button_finger.center;
			// simple constrain by rectangle, not by radius
			if (center.x < r_button_ring.x)
			{
				center.x = r_button_ring.x;
			}
			if (center.x > r_button_ring.x + r_button_ring.width)
			{
				center.x = r_button_ring.x + r_button_ring.width;
			}
			if (center.y < r_button_ring.y)
			{
				center.y = r_button_ring.y;
			}
			if (center.y > r_button_ring.y + r_button_ring.height)
			{
				center.y = r_button_ring.y + r_button_ring.height;
			}
			r_button_finger.center = center;
		}
	}

	void Update ()
	{
		output = Vector3.zero;
		outputRaw = Vector3.zero;

		if (Time.timeScale == 0)
		{
			return;
		}

		bool fingerDownNext = false;
		
		MicroTouch[] mts = MicroTouch.GatherMicroTouches();

		foreach (MicroTouch t in mts)
		{
			Vector2 pos = new Vector2( t.position.x, Screen.height - t.position.y);
			
			if (r_downable.Contains( pos))
			{
				if (!fingerDown)
				{
					if (t.phase == TouchPhase.Began)
					{
						beganTime = Time.time;
						fingerDown = true;
						fingerDownNext = true;
						fingerId = t.fingerId;
						v2down = pos;
						r_button_ring.x = v2down.x - r_button_ring.width / 2;
						r_button_ring.y = v2down.y - r_button_ring.height / 2;
						r_button_finger.x = pos.x - r_button_finger.width / 2;
						r_button_finger.y = pos.y - r_button_finger.height / 2;
						UpdatePosition( pos);
						CheckConstraints();
					}
				}
			}
			if (fingerDown)
			{
				if (t.fingerId == fingerId)
				{
					if ((t.phase == TouchPhase.Ended) ||
					    (t.phase == TouchPhase.Canceled))
					{
						if (Time.time - beganTime < 0.2f)
						{
							detectedTap = true;
						}
					}
					else
					{
						fingerDownNext = true;
						UpdatePosition( pos);
						r_button_finger.x = pos.x - r_button_finger.width / 2;
						r_button_finger.y = pos.y - r_button_finger.height / 2;
						CheckConstraints();
					}
				}
			}
		}
		
		OverlayStandardInputAxes ();
		
		fingerDown = fingerDownNext;
	}
	
	void OnGUI()
	{
		if (Time.timeScale == 0)
		{
			return;
		}

		if (fingerDown)
		{
			GUI.color = buttonColor;
			GUI.DrawTexture( r_button_ring, t2d_button_ring);
			GUI.DrawTexture( r_button_finger, t2d_button_ring);
		}
		else
		{
			if (label != null)
			{
				GUI.color = labelColor;
				GUI.Label ( r_label, label, OurStyles.LABELCJ(10));
			}
		}

// uncomment these lines to see the touchable rectangle
//		GUI.color = new Color (1, 1, 1, 0.2f);
//		GUI.DrawTexture (r_downable, LAZY.t2d_white32x32);
	}
}
