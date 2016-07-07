//
//  Weapon.cs is part of Tunnelers: Unified
//  <https://github.com/VacuumGames/tunnelers-unified/>
//
//  Copyright (c) 2015-2016 Juraj Fiala <doctorjellyface@riseup.net>
//
//  Tunnelers: Unified is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//
//  Tunnelers: Unified is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
//
//  You should have received a copy of the GNU General Public License
//  along with Tunnelers: Unified.  If not, see <http://www.gnu.org/licenses/>.
//

using UnityEngine;

public class Weapon: MonoBehaviour, IShootable {

	ILife life;
	Transform bulletSpawn;

	public GameObject bulletPrefab;

	[Range (0, 100)]
	public float energyConsumption = 10f;
	[Range (0, 10)]
	public float coolDown = 5f;

	float time;
	bool Cooled {
		get {
			// Compare if the delta between the current time
			// and the timestamp is bigger than the cooldown
			return Time.time - time > coolDown;
		}
	}

	void Awake () {

		// Initialize references between objects
		life = GetComponent <ILife> ();
		bulletSpawn = transform.Find ("Turret").Find ("Barrel");
	}

	public void Shoot () {

		// If we don't have energy or aren't cooled yet, don't shoot
		if (!life.HaveEnergy || !Cooled)
			return;

		// If everything is alright, mark the time
		time = Time.time;

		// Shooting costs us energy
		life.Energy -= energyConsumption;

		// Create the bullet
		var bullet = (GameObject) Instantiate (bulletPrefab, bulletSpawn.position, bulletSpawn.rotation);
		bullet.GetComponent <IBullet> ().Start = bulletSpawn;
		bulletSpawn.GetComponent <IShootable> ().Shoot ();
	}
}
