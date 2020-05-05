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

public class TestTerrainDamager : MonoBehaviour
{
	public TerrainDamager Damager;

	public GameObject GrenadePrefab;
	public TerrainDamageConfig GrenadeDamageConfig;

	public GameObject CherryBombPrefab;
	public TerrainDamageConfig CherryBombDamageConfig;

	public GameObject ExplosionPrefab;

	WeaponGrenadeTosser wgt;

	void Start()
	{
		var camToUse = Camera.main;

		var mover = CheeseWASDMover.Create( camToUse, new Vector3( 10, 0, 10), 50, CheeseWASDMover.CheeseWASDViewType.FirstPerson);

		wgt = WeaponGrenadeTosser.Attach( camToUse.gameObject,
			() => {
				return mover.LastVelocity * 0.9f;
			}
		);
		wgt.ExplosionPrefab = ExplosionPrefab;
	}

	void Update ()
	{
		switch( DSM.SelectedWeapon.iValue)
		{
		default :
		case 0 :
			wgt.DamageConfig = GrenadeDamageConfig;
			wgt.ProjectilePrefab = GrenadePrefab;
			wgt.RelativeMotion = new Vector3( 0, 5.0f, 15.0f);
			wgt.GravityForThisWeapon = Vector3.up * -20.0f;
			break;
		case 1 :
			wgt.DamageConfig = CherryBombDamageConfig;
			wgt.ProjectilePrefab = CherryBombPrefab;
			wgt.RelativeMotion = new Vector3( 0, 10.0f, 25.0f);
			wgt.GravityForThisWeapon = Vector3.up * -10.0f;
			break;
		}

		if (Input.GetKeyDown( KeyCode.BackQuote))
		{
			Vector3 position = new Vector3( Random.Range( 10, 20), 0, Random.Range( 10, 200));

			Damager.ApplyDamage( position, GrenadeDamageConfig);
		}	
	}
}
