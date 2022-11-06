/*
	The following license supersedes all notices in the source code.

	Copyright (c) 2022 Kurt Dekker/PLBM Games All rights reserved.

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

public class MakeRoadsFromChildTransforms : MonoBehaviour
{
	public RoadConfiguration Config;

	// maxLinearVertexSpacing: max allowable spacing in the direction of road travel
	IEnumerable<PositionAndHeading> PointFeeder( float maxLinearVertexSpacing)
	{
		var children = new Transform[ transform.childCount];

		for (int i = 0; i < transform.childCount; i++)
		{
			children[i] = transform.GetChild(i);
		}

		int point = 0;

		while( point < children.Length)
		{
			if (point > 0)
			{
				var t1 = children[point - 1];
				var t2 = children[point];

				float distance = Vector3.Distance( t2.position, t1.position);

				float heading1 = t1.rotation.eulerAngles.y;
				float heading2 = t2.rotation.eulerAngles.y;

				// Compute steps required so we don't exceed maxLinearVertexSpacing
				//
				// TODO: one day this should be dynamic according to how sharply the local
				// terrain slope is changing, eg, second derivative of dHeight / dDistance.
				//
				// This allow reduced geometry overall while maintaing ground
				// contour fidelity.
				int steps = 1 + (int)(distance / maxLinearVertexSpacing);

				int limit = steps;
				if (point == children.Length - 1) limit++;

				for (int i = 0; i < limit; i++)
				{
					float fraction = (float)i / steps;

					Vector3 position = Vector3.Lerp( t1.position, t2.position, fraction);
					float heading = Mathf.Lerp( heading1, heading2, fraction);

					yield return new PositionAndHeading( position, heading);
				}
			}

			point++;
		}
	}

	void Start()
	{
		MakeRoads.Create( Config, PointFeeder( maxLinearVertexSpacing: 2.0f));
	}
}
