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

public class DSGameObjectControl : MonoBehaviour
{
	[Header( "Controlling datasack:")]
	[Tooltip( "This Datasack controls objects below.")]
	public	Datasack	dataSack;

	[Header( "For simple boolean on/off control:")]
	[Tooltip( "GameObjects to ENABLE when Datasack is poked TRUE (false poke will DISABLE!).")]
	public	GameObject[]	ToEnable;

	[Tooltip( "GameObjects to DISABLE when Datasack is poked TRUE (false poke will ENABLE!).")]
	public	GameObject[]	ToDisable;

	[Header( "For .iValue control of lists, or .Value control of GameObjects by their name")]
	[Tooltip( "GameObject array to map to iValue of Datasack (zero-based, can have gaps)")]
	public	GameObject[]	IndexArray;

	[Tooltip( "Auto-populates above array based on transform children.")]
	public	bool			OperateOnChildren;

	[Tooltip( "Selection strategy for list of items.")]
	public	SelectionStrategy	selectionStrategy;

	public enum SelectionStrategy
	{
		SELECT_EXACTLY_ONE,
		SELECT_LESS_THAN,
		SELECT_LESS_THAN_OR_EQUAL,
		SELECT_BY_GAMEOBJECT_NAME,
	}

	void OnChanged( Datasack ds)
	{
		bool pokedTrue = ds.bValue;

		// boolean handling:
		foreach( var go in ToEnable)
		{
			if (go)
			{
				go.SetActive( pokedTrue);
			}
		}
		foreach( var go in ToDisable)
		{
			if (go)
			{
				go.SetActive( !pokedTrue);
			}
		}

		if (OperateOnChildren)
		{
			if (transform.childCount != IndexArray.Length)
			{
				IndexArray = new GameObject[ transform.childCount];
			}
			for (int i = 0; i < transform.childCount; i++)
			{
				IndexArray[i] = transform.GetChild(i).gameObject;
			}
		}

		// iValue integer list selections
		for (int i = 0; i < IndexArray.Length; i++)
		{
			var currentGameObject = IndexArray[i];
			if (currentGameObject)
			{
				// presume selection SelectionStrategy.SELECT_EXACTLY_ONE:
				bool onoff = i == ds.iValue;

				switch( selectionStrategy)
				{
				case SelectionStrategy.SELECT_LESS_THAN :
					onoff = i < ds.iValue;
					break;
				case SelectionStrategy.SELECT_LESS_THAN_OR_EQUAL :
					onoff = i <= ds.iValue;
					break;
				case SelectionStrategy.SELECT_BY_GAMEOBJECT_NAME :
					onoff = (ds.Value == currentGameObject.name);
					break;
				}

				currentGameObject.SetActive( onoff);
			}
		}
	}

	void OnEnable()
	{
		dataSack.OnChanged += OnChanged;

		dataSack.Poke();
	}

	void OnDisable()
	{
		dataSack.OnChanged -= OnChanged;
	}
}
