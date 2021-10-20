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

// quick example of how to track materials per renderers
// handles varying numbers of materials per renderer

public class TrackRenderersAndMaterials : MonoBehaviour
{
	// jagged array of material arrays
	Material[][] CreateJaggedMaterialArray( Renderer[] Renderers)
	{
		Material[][] result = new Material[ Renderers.Length][];

		// traverse each renderer...
		for (int i = 0; i < Renderers.Length; i++)
		{
			var rend = Renderers[i];

			// get the materials array out
			var materials = rend.materials;

			// I'm not 100% sure this copy process is required, given how
			// Unity clones materials, but this a safer approach.
			Material[] copy = new Material[ materials.Length];
			System.Array.Copy( materials, copy, materials.Length);

			result[i] = copy;
		}

		return result;
	}

	void Start ()
	{
		// you can obtain this list however you like
		Renderer[] AllRenderers = FindObjectsOfType<Renderer>();

		// note: this jaggedMaterials array is in the same order as the above
		var jaggedMaterials = CreateJaggedMaterialArray( AllRenderers);

		// Display what we found
		for (int i = 0; i < jaggedMaterials.Length; i++)
		{
			string output = "Item : " + AllRenderers[i] + ":";

			output += " has " + jaggedMaterials[i].Length + " materials.\n";

			for (int j = 0; j < jaggedMaterials[i].Length; j++)
			{
				output += "Material " + j + " is " + jaggedMaterials[i][j].name + ", ";
			}

			Debug.Log( output);
		}

		// as long as you don't change the ordering of AllRenderers, it will
		// one-to-one correspond to what is in jaggedMaterials.
	}
}
