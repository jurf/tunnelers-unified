// vim:set ts=4 sw=4 sts=4 noet:
//
//  Laser.cs is part of Tunnelers: Unified
//  <https://github.com/VacuumGames/tunnelers-unified/>.
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

[AddComponentMenu ("Bullet/Laser")]

public class Laser: MonoBehaviour, IBullet {

	LineRenderer line;
	Transform spiral;
	Material spiralMat;

	// To know where the barrel is
	Transform start;
	public Transform Start {
		set {
			start = value;
		}
	}

	[Range (0, 600)]
	public float speed = 400f;

	[Range (0, 50)]
	public float rotationSpeed = 30f;

	[Range (0, 20)]
	public float range = 10f;

	// Width of the laser
	[Range (0, 1)]
	public float width = 0.5f;

	[Range (0, 100)]
	public float damage = 25f;

	// Density of the spiral
	[Range (0, 50)]
	public float densityToSpeed = 15f;

	[Range (0, 2)]
	public float fadeOutSpeed = 1f;

	float lineLength;
	bool stop;

	void Awake () {

		// Initialize references between objects
		spiral = transform.Find ("Laser Spiral");
		spiralMat = spiral.GetComponent <Renderer> ().material;

		// Since we need to get the width we might as well set it from
		// here since LineRenderer doesn't provide a way to get it
		line = GetComponent <LineRenderer> ();
		line.SetWidth (width, width);
	}

	void Update () {

		// We're a laser, we can't break off
		// Keep in mind the tank could have been destroyed
		if (start) {
			transform.position = start.position;
			transform.rotation = start.rotation;
		}

		// Fork if we're in the fade phase
		if (stop) {
			FadeOut ();
			return;
		}

		// Calculate the new length, but do not exceed the range
		var newLineLength = lineLength + speed * Time.deltaTime;
		if (newLineLength > range) {
			newLineLength = range;
			stop = true;
		}

		// Run the new length through a raycast
		lineLength = Ray (newLineLength);

		// Stretch the eye candy accordingly
		Stretch (lineLength);
		spiralMat.mainTextureOffset += new Vector2 (rotationSpeed * Time.deltaTime, 0);
	}

	float Ray (float length) {

		// Run a spherecast of the size of the laser
		// If we hit something apply the damage,
		// set the length to it and exit out
		RaycastHit hit;
		if (Physics.SphereCast (transform.position, width / 2, transform.forward, out hit, length)) {

			stop = true;
			if (hit.collider.tag == "Tank")
				DamagePlayer (hit.collider.gameObject, damage);
			return hit.distance;
		}

		return length;
	}

	void Stretch (float length) {

		// First the actual laser
		line.SetPosition (1, new Vector3 (0, 0, length));

		// Then the spiral
		Vector3 spiralScale = spiral.localScale;
		spiralScale.z = length / 2;
		spiral.localScale = spiralScale;

		// Rotate the spiral to get a lightning-like effect
		spiralMat.mainTextureScale = new Vector2 (length / densityToSpeed, 1);
	}

	float alpha = 1f;
	void FadeOut () {

		// Slow down over time
		alpha -= Time.deltaTime * fadeOutSpeed * 5;
		width -= Time.deltaTime * fadeOutSpeed;

		// If either has reached zero we're invisible
		if (alpha < 0 || width < 0)
			Destroy (gameObject);

		// Slow down the rotation too
		spiralMat.mainTextureOffset += new Vector2 (rotationSpeed * alpha * Time.deltaTime, 0);

		// Fade the spiral
		Color spiralColor = spiralMat.GetColor ("_TintColor");
		// The colour is already at half transparency
		spiralColor.a = alpha / 2;
		spiralMat.SetColor ("_TintColor", spiralColor);

		// Fade and shrink the laser
		Color lineColour = Color.white;
		lineColour.a = alpha;

		line.SetColors (lineColour, lineColour);
		line.SetWidth (width, width);
	}

	void DamagePlayer (GameObject player, float damage) {

		ILife shotPlayer = player.GetComponent <ILife> ();
		bool killed;
		shotPlayer.Damage (damage, out killed);
	}

	// Old GUI
	//	void OnGUI () {
	//
	//		float coolTime = (Time.time - time - coolDown) * -1f;
	//
	//		if (!Cooled) {
	//			GUI.Label (new Rect (Input.mousePosition.x + 20, -Input.mousePosition.y + Screen.height, 20, 20),
	//				"<color=#ff0000>" + coolTime + "</color>");
	//		}
	//
	//	}
}
