/*
	The following license supersedes all notices in the source code.

	Copyright (c) 2020 Kurt Dekker/PLBM Games All rights reserved.

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
using UnityEngine.EventSystems;

[RequireComponent( typeof( InputField))]
public class DSUserIntentInputField : MonoBehaviour
{
	[Header("The DataSack to signal onEndEdit. Default is UserIntent.")]
	public Datasack DSUI;

	public	bool	SignalPayloadInsteadOfName;

	[Header("The actual text as it is edited, bidirectional connection.")]
	public Datasack dataSackPayload;

	private	InputField inputField;

	// Data flowing from InputField to the Datasack(s)
	void	OnValueChanged( string Value)
	{
		dataSackPayload.Value = Value;
	}
	void	OnEndEdit( string Value)
	{
		dataSackPayload.Value = Value;

		string signalledOutput = gameObject.name;
		if (SignalPayloadInsteadOfName) signalledOutput = Value;

		DSUI.Value = signalledOutput;
	}

	// Data flowing from Datasack to InputField
	void	OnDatasackChanged( Datasack ds)
	{
		inputField.text = ds.Value;
	}

	void	OnEnable()
	{
		if (DSUI == null) DSUI = DSM.UserIntent;

		inputField = GetComponent<InputField> ();

		// to connect data flows from InputField to Datasack(s):
		inputField.onValueChanged.AddListener (OnValueChanged);
		inputField.onEndEdit.AddListener( OnEndEdit);

		// to connect data flows from Datasack to InputField:
		dataSackPayload.OnChanged += OnDatasackChanged;
		inputField.text = dataSackPayload.Value;
	}
	void	OnDisable()
	{
		inputField.onValueChanged.RemoveListener (OnValueChanged);

		inputField.onEndEdit.RemoveListener( OnEndEdit);

		dataSackPayload.OnChanged -= OnDatasackChanged;
	}
}
