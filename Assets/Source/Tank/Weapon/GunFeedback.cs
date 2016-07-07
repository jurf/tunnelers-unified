// vim:set ts=4 sw=4 sts=4 noet:
//
//  GunFeedback.cs is part of Tunnelers: Unified
//  <https://github.com/VacuumGames/tunnelers-unified/>.
//
//  Copyright (c) 2016 Juraj Fiala <doctorjellyface@riseup.net>
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

public class GunFeedback: MonoBehaviour, IShootable {

	Light light;
	LensFlare flare;

	[Range (0, 20)]
	public float feedback = 10f;
	[Range (0, 0.5f)]
	public float maxFeedback = 0.2f;
	[Range (0, 0.3f)]
	public float smoothTime = 0.1F;

	[Range (0, 3)]
	public float barrelIntensity = 1.5f;
	[Range (0, 10)]
	public float lightIntensity = 8f;
	[Range (0, 3)]
	public float flareIntensity = 2f;

	float x;
	float follow;
	float velocity;
	float offset;

	void Awake () {

		// Initialize references between objects
		Transform flareGo = transform.Find ("Flare");
		light = flareGo.GetComponent <Light> ();
		flare = flareGo.GetComponent <LensFlare> ();
		offset = transform.localPosition.z;
	}

	void Update () {

		// Calculate the new x moving to 0, but don't exceed max feedback
		x = Mathf.Clamp (Mathf.SmoothDamp (x, 0, ref velocity, smoothTime), 0, maxFeedback); 

		// The fire jag happens very suddenly, let's smooth it out
		follow = Mathf.Lerp (follow, x, 0.5f);

		// Apply x to all the effects
		light.intensity = follow * lightIntensity;
		flare.brightness = follow * flareIntensity;

		Vector3 pos = transform.localPosition;
		pos.z = offset - follow * barrelIntensity;
		transform.localPosition = pos;
	}

	public void Shoot () {

		// Add to the velocity SmoothDamp uses, this causes a negative effect
		velocity += feedback;
	}
}
