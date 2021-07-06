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

public class OurStyles
{
	public static float NotionalPointWidth
	{
		get
		{
			return (Screen.width > Screen.height) ? 480.0f : 320.0f;
		}
	}

	private static Dictionary<int,GUIStyle> _LABELLJ;
	public static GUIStyle LABELLJ( int fontsize)
	{
		if (_LABELLJ == null)
		{
			_LABELLJ = new Dictionary<int, GUIStyle>();
		}
		if (!_LABELLJ.ContainsKey(fontsize))
		{
			GUIStyle gst = new GUIStyle();
			gst.alignment = TextAnchor.MiddleLeft;
			gst.fontSize = (int)((fontsize * Screen.width) / NotionalPointWidth);
			gst.normal.textColor = Color.white;
			_LABELLJ[fontsize] = gst;
		}
		return _LABELLJ[fontsize];
	}

	private static Dictionary<int,GUIStyle> _LABELULX;
	public static GUIStyle LABELULX( int fontsize)
	{
		if (_LABELULX == null)
		{
			_LABELULX = new Dictionary<int, GUIStyle>();
		}
		if (!_LABELULX.ContainsKey(fontsize))
		{
			GUIStyle gst = new GUIStyle();
			gst.alignment = TextAnchor.UpperLeft;
			gst.wordWrap = true;
			gst.fontSize = (int)((fontsize * Screen.width) / NotionalPointWidth);
			gst.normal.textColor = Color.white;
			_LABELULX[fontsize] = gst;
		}
		return _LABELULX[fontsize];
	}

	private static Dictionary<int,GUIStyle> _LABELURX;
	public static GUIStyle LABELURX( int fontsize)
	{
		if (_LABELURX == null)
		{
			_LABELURX = new Dictionary<int, GUIStyle>();
		}
		if (!_LABELURX.ContainsKey(fontsize))
		{
			GUIStyle gst = new GUIStyle();
			gst.alignment = TextAnchor.UpperRight;
			gst.wordWrap = true;
			gst.fontSize = (int)((fontsize * Screen.width) / NotionalPointWidth);
			gst.normal.textColor = Color.white;
			_LABELURX[fontsize] = gst;
		}
		return _LABELURX[fontsize];
	}

    private static Dictionary<int, GUIStyle> _LABELLRX;
    public static GUIStyle LABELLRX(int fontsize)
    {
        if (_LABELLRX == null)
        {
            _LABELLRX = new Dictionary<int, GUIStyle>();
        }
        if (!_LABELLRX.ContainsKey(fontsize))
        {
            GUIStyle gst = new GUIStyle();
            gst.alignment = TextAnchor.LowerRight;
            gst.wordWrap = true;
            gst.fontSize = (int)((fontsize * Screen.width) / NotionalPointWidth);
            gst.normal.textColor = Color.white;
            _LABELLRX[fontsize] = gst;
        }
        return _LABELLRX[fontsize];
    }

    private static Dictionary<int,GUIStyle> _LABELRJ;
	public static GUIStyle LABELRJ( int fontsize)
	{
		if (_LABELRJ == null)
		{
			_LABELRJ = new Dictionary<int, GUIStyle>();
		}
		if (!_LABELRJ.ContainsKey(fontsize))
		{
			GUIStyle gst = new GUIStyle();
			gst.alignment = TextAnchor.MiddleRight;
			gst.fontSize = (int)((fontsize * Screen.width) / NotionalPointWidth);
			gst.normal.textColor = Color.white;
			_LABELRJ[fontsize] = gst;
		}
		return _LABELRJ[fontsize];
	}

	private static Dictionary<int,GUIStyle> _LABELCJ;
	public static GUIStyle LABELCJ( int fontsize)
	{
		if (_LABELCJ == null)
		{
			_LABELCJ = new Dictionary<int, GUIStyle>();
		}
		if (!_LABELCJ.ContainsKey(fontsize))
		{
			GUIStyle gst = new GUIStyle();
			gst.alignment = TextAnchor.MiddleCenter;
			gst.fontSize = (int)((fontsize * Screen.width) / NotionalPointWidth);
			gst.normal.textColor = Color.white;
			_LABELCJ[fontsize] = gst;
		}
		return _LABELCJ[fontsize];
	}
	
	private static GUISkin _SKINBASE;
	private static GUISkin SKINBASE
	{
		get
		{
			if (_SKINBASE == null)
			{
				_SKINBASE = Resources.Load( "GUISkins/SKINBASE") as GUISkin;
			}
			return _SKINBASE;
		}
	}
	private static Dictionary<int,GUIStyle> _BUTTONBASE;
	public static GUIStyle BUTTONBASE( int fontsize)
	{
		if (_BUTTONBASE == null)
		{
			_BUTTONBASE = new Dictionary<int, GUIStyle>();
		}
		if (!_BUTTONBASE.ContainsKey(fontsize))
		{
			GUIStyle gst = new GUIStyle( SKINBASE.button);
			gst.alignment = TextAnchor.MiddleCenter;
			gst.fontSize = (int)((fontsize * Screen.width) / NotionalPointWidth);
			gst.normal.textColor = Color.white;
			_BUTTONBASE[fontsize] = gst;
		}
		return _BUTTONBASE[fontsize];
	}
	private static Dictionary<int,GUIStyle> _BOXBASE;
	public static GUIStyle BOXBASE( int fontsize)
	{
		if (_BOXBASE == null)
		{
			_BOXBASE = new Dictionary<int, GUIStyle>();
		}
		if (!_BOXBASE.ContainsKey(fontsize))
		{
			GUIStyle gst = new GUIStyle( SKINBASE.box);
			gst.alignment = TextAnchor.UpperCenter;
			gst.fontSize = (int)((fontsize * Screen.width) / NotionalPointWidth);
			gst.normal.textColor = Color.white;
			_BOXBASE[fontsize] = gst;
		}
		return _BOXBASE[fontsize];
	}
}
