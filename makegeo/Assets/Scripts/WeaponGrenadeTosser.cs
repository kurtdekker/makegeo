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

// Put this on your camera gimbal so it faces a reasonable direction when you toss

public class WeaponGrenadeTosser : MonoBehaviour
{
	public GameObject ProjectilePrefab;
	public TerrainDamageConfig DamageConfig;

	public GameObject ExplosionPrefab;

	public Vector3 RelativeMotion;
	public Vector3 GravityForThisWeapon;

	System.Func<Vector3> GetTosserMotion;

	public static WeaponGrenadeTosser Attach( GameObject go, System.Func<Vector3> GetTosserMotion = null)
	{
		var wgt = go.AddComponent<WeaponGrenadeTosser>();
		wgt.GetTosserMotion = GetTosserMotion;
		wgt.RelativeMotion = new Vector3( 0, 5.0f, 15.0f);
		wgt.GravityForThisWeapon = Vector3.up * -20.0f;
		return wgt;
	}

	// This is a quick hack that only handles one damager in the scene.
	// You might need some kind of multiplex/dispatch if you have many.
	TerrainDamager damager;
	void Start()
	{
		damager = FindObjectOfType<TerrainDamager>();
	}

	void Update ()
	{
		if (Input.GetKeyDown( KeyCode.Space))
		{
			var go = new GameObject( "Grenade Out!");
			go.transform.position = transform.position + transform.right * 0.5f + transform.up * 0.5f;

			var visible = Instantiate<GameObject>( ProjectilePrefab, go.transform);
			foreach( var collider in new List<Collider>( visible.GetComponentsInChildren<Collider>()))
			{
				Destroy( collider);
			}

			// initial toss motion
			Vector3 tossVelocity = transform.forward * RelativeMotion.z + transform.up * RelativeMotion.y;
			// maybe add in user motion
			if (GetTosserMotion != null)
			{
				tossVelocity += GetTosserMotion();
			}
			// parabolic motion
			Ballistic3D.Attach( go, tossVelocity, GravityForThisWeapon);

			// this gets called when the RaycastSensor notices you hit something
			System.Action<Vector3> OnHit = (pos) => {
				Instantiate<GameObject>( ExplosionPrefab, pos, Quaternion.identity);

				damager.ApplyDamage( pos, DamageConfig);

				Destroy( go);
			};
			RaycastSensor.Attach( go, OnHit);

			// death sentence
			TTL.Attach( go, 10.0f);

			// make it tumble
			var spinner = go.AddComponent<SpinMeAllAxes>();
			spinner.RateOfSpin = Random.onUnitSphere * Random.Range( 1000, 2000);
		}	
	}
}
