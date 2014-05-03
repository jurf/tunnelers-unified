import System.Collections.Generic;

var tanks : GameObject[];
var trollRockets : GameObject[];
var trollRocketTouchRadius : float;
var trollRocketExplodeRadius : float;
var minus : Vector3;
var digDistance : float;
var shake : boolean;
var shakeX : float;
var shakeY : float;
var shakeZ : float;
var updateMeshCollider : boolean;

var loadMesh : boolean;
var saveMesh : boolean;
var vertex : Vector3[];

var megaSectors = new List.<MegaSector> ();
var from : Vector3;
var to : Vector3;
var byX : float;
var byZ : float;
var addTo : float;
var amount : int;
var levels : int;
var radius : float;
var megaSectorsNum : int = 1;
var go : GameObject;

var test1 : Transform;
var test2 : Transform;

var indinces2 = new List.<int> ();

class MegaSector {

	var sectors = new List.<Sector> ();
	
	function MegaSector (a : List.<Sector>) {
	
		sectors = a;
		
	}
	
}

class Sector {
	
	var pos : Vector3;
	var vertex = new List.<int> ();
	
	function Sector (p : Vector3, v : List.<int>) {
		
		pos = p;
		vertex = v;
	
	}
	
}

class Indinces {
	
	var i = new List.<Diggers> ();
	
	function Indinces (things : GameObject[], startIn : List.<int>, explode : boolean) {
		
		if (things.Length > 0) {

			for (thing in things) {
				var thingVar = Diggers (thing);
				i.Add (thingVar);
			}
			
			for (t = 0; t < i.Count; t++) {
				for (sv in startIn) {
					i[t].v.Add (sv);
					if (explode)
						i[t].e.Add (sv);
				}
				
				//Debug.Log ("i.v = " + i[0].v);
				
			}
		}
	}
	
	function Indinces (thing : GameObject, startIn : List.<int>) {

		var thingVar = Diggers (thing);
		i.Add (thingVar);
			
		for (sv in startIn) {
			i[0].v.Add (sv);
		}	
	}

}
			
			
class Diggers {

	var o : GameObject;
	var v = new List.<int> ();
	var e = new List.<int> ();
	
	function Diggers (oo : GameObject) {
		
		o = oo;
	}

}
            
function Awake () {

	if (Network.isClient && !Network.isServer) { enabled = false; }
	
	if (shake) {
		var mesh : Mesh = GetComponent(MeshFilter).mesh;
		var vertices : Vector3[] = mesh.vertices;
	 
		for (i = 0; i < vertices.Length; i++) {
			vertices[i] = vertices[i] + Vector3 (Random.Range (-shakeX, shakeX), Random.Range (-shakeY, shakeY), Random.Range (-shakeZ, shakeZ));
		}
	
		mesh.vertices = vertices;
		mesh.RecalculateBounds();
	}
	
	if (loadMesh) {
		var meshe : Mesh = GetComponent(MeshFilter).mesh;
		meshe.vertices = vertex;
		meshe.RecalculateBounds();
	}
	
	for (mm = 0; mm < megaSectorsNum; mm++) {
		var _byX = byX * (mm + 1);
		var _byZ = byZ * (mm + 1);
		var _addTo = addTo * (mm + 1);
		megaSectors.Add (MegaSector(new List.<Sector> ()));
		
		for (ll = -5; ll < levels; ll++) {
			for (aa = -5; aa < amount; aa++) {
				if (ll % 2 == 0) {
					if (_byX * aa > from.x && _byZ * ll > from.z && _byX * aa < to.x && _byZ * ll < to.z) {
						megaSectors[mm].sectors.Add (Sector(Vector3 (_byX * aa, 2, _byZ * ll), new List.<int> () ));
						//var sph = Instantiate (go, Vector3 (_byX * aa, 2, _byZ * ll), Quaternion.Euler (Vector3.zero));
						//sph.transform.localScale = Vector3 (1,1,1) * (mm + 1);
					}
				} else {
					if (_byX * aa + addTo > from.x && _byZ * ll > from.z && _byX * aa + addTo < to.x && _byZ * ll < to.z) {
						megaSectors[mm].sectors.Add (Sector(Vector3 (_byX * aa + _addTo, 2, _byZ * ll), new List.<int> () ));
						//var _sph = Instantiate (go, Vector3 (_byX * aa + _addTo, 2, _byZ * ll), Quaternion.Euler (Vector3.zero));
						//_sph.transform.localScale = Vector3 (1,1,1) * (mm + 1);
					}
				}
			}
		}
	}
		
	var _vertices : Vector3[] = GetComponent (MeshFilter).mesh.vertices;
	
	for (s in megaSectors[0].sectors) {
		for (v = 0; v < _vertices.Length; v++) {
			if (Vector3.Distance (s.pos, transform.TransformPoint (_vertices[v])) < radius) {
				s.vertex.Add (v);
			}
		}
	}
	
	for (ii = 1; ii < megaSectors.Count; ii++) {
		for (ss = 0; ss < megaSectors[ii].sectors.Count; ss++) {
			for (sss = 0; sss < megaSectors[0].sectors.Count; sss++) {
				//Debug.Log (Vector3.Distance (megaSectors[ii].sectors[ss].pos, megaSectors[0].sectors[sss].pos));
				//Debug.Log (radius * (ii + 1));
				if (Vector3.Distance (megaSectors[ii].sectors[ss].pos, megaSectors[0].sectors[sss].pos) < radius * (ii + 1)) {
					 megaSectors[ii].sectors[ss].vertex.Add (sss);
				}
			}
		}
	}
		
	
//	Debug.Log (megaSectors[0].sectors.length);
//	Debug.Log (megaSectors[0].sectors [50].vertex);
//	Debug.Log (megaSectors[0].sectors [50].vertex.length);
//	Debug.Log (megaSectors[1].sectors.length);
//	Debug.Log (megaSectors[1].sectors [15].vertex);
//	Debug.Log (megaSectors[1].sectors [15].vertex.length);
	
//	for (t in megaSectors[1].sectors) {
//		Debug.Log (t.vertex.length);
//	}
//		
}		


function Update () {

	//if (Network.isClient && !Network.isServer) { return; }
	
	tanks = GameObject.FindGameObjectsWithTag ("DigPosition");
	trollRockets = GameObject.FindGameObjectsWithTag ("RocketTouch");
	
	var mesh : Mesh = GetComponent(MeshFilter).mesh;
	var vertices : Vector3[] = mesh.vertices;
	var bigMegaSector : int = megaSectors.Count - 1;
	//Debug.Log (bigMegaSector);
	
	var indinces = new List.<int> ();
	
	for (sector = 0; sector < megaSectors[bigMegaSector].sectors.Count; sector++) {
		indinces.Add (sector);
	}
	
	indinces2 = indinces; 
	
	var iTanks = new List.<Indinces> ();
	//var iTanks = Indinces (tanks, indinces);
	iTanks.Add (Indinces (tanks, indinces, false));
	
	//Debug.Log (iTanks[0].i[0].o);
	//Debug.Log (iTanks[0].i[0].v);
	
	var iRockets = new List.<Indinces> ();
	iRockets.Add (Indinces (trollRockets, indinces, true));
	
//	for (ii in indinces) {
//		 if (ii.GetType () != int) {
//		 	Debug.Log (ii.GetType () + " " + ii);
//		 }
//	}
	
	//Debug.Log (indinces);
	
	var indincesInt : int = 0;
	
	for (i = bigMegaSector; i > -1; i--) {
		//Debug.Log (i);
		//Debug.Log ("Before = : " + iTanks[0].i[0].v.Count);
		
		var iTanks2 = iTanks[indincesInt];
		//var iTanks2 = CloneList (iTanks);
		//itanks2 = iTanks.
//		iTanks2.i.AddRange (iTanks.i);
		//var iTanks2 = iTanks;
		
//		for (tankThing2 = 0; tankThing2 < iTanks2.Count; tankThing2++) {
//			iTanks2[tankThing2].v = new iTanks.i[tankThing2].v;
//		}
		
		//Debug.Log ("Before clear: " + iTanks2.i[0].v.Count);
		
//		for (tankClear in iTanks.i) {	
//			tankClear.v.Clear ();
//		}
		
		//Debug.Log (iTanks2.i[0].o);	
		//Debug.Log ("After clear: " + iTanks2.i[0].v.Count);
		
		//var iRockets2 : Diggers[] = iRockets.i.ToBuiltin (Diggers) as Diggers[];
//		var iRockets2 = CloneList (iRockets);
//		iRockets2.i.AddRange (iRockets.i);

		var iRockets2 = iRockets[indincesInt];
		
//		for (rocketClear in iRockets.i) {	
//			rocketClear.v.Clear ();
//		}

		iTanks.Add (Indinces (tanks, new List.<int> (), false));
		iRockets.Add (Indinces (trollRockets, new List.<int> (), true));
		
		indincesInt++;

		if (i != 0) {
			
//			for (tank in iTanks2) {
			for (tankInt = 0; tankInt < iTanks2.i.Count; tankInt++) {
				var tank = iTanks2.i[tankInt];
				//Debug.Log(tank.v.Count);
				for (iTank in tank.v) {
					//Debug.Log (tank.o);
					//Debug.Log (Vector3.Distance (megaSectors[i].sectors[iTank].pos, tank.o.transform.position) + "  " + ((radius + digDistance) * (i + 1)) + "  " + iTank);
					
					if (Vector3.Distance (megaSectors[i].sectors[iTank].pos, tank.o.transform.position) < (radius + digDistance) * (i + 1)) {
						//Debug.Log ("tankInt = " + tankInt + " and iTanks.i[tankInt].v = " + iTanks[0].i[tankInt].v);
						//iTanks.i[tankInt].v = iTanks.i[tankInt].v.Concat (iTanks.i[tankInt].v, megaSectors[i].sectors[iTank].vertex);
						iTanks[indincesInt].i[tankInt].v.AddRange (megaSectors[i].sectors[iTank].vertex);
						//Debug.Log (iTanks[0].i[tankInt].v);
					}
				}
			}
			
//			for (trollRocket in iRockets2) {
			for (rocketInt = 0; rocketInt < iRockets2.i.Count; rocketInt++) {
				var trollRocket = iRockets2.i[rocketInt];
				for (iRocket in trollRocket.v) {
					if (Vector3.Distance (megaSectors[i].sectors[iRocket].pos, trollRocket.o.transform.position) < (radius + trollRocketTouchRadius) * (i + 1)) {
						//iRockets.i[rocketInt].v = iRockets.i[rocketInt].v.Concat (iRockets.i[rocketInt].v, megaSectors[i].sectors[iRocket].vertex);
						iRockets[indincesInt].i[rocketInt].v.AddRange (megaSectors[i].sectors[iRocket].vertex);
					}
				}
				
				for (eRocket in trollRocket.e) {
					if (Vector3.Distance (megaSectors[i].sectors[eRocket].pos, trollRocket.o.transform.position) < (radius + trollRocketTouchRadius) * (i + 1)) {
						//iRockets.i[rocketInt].v = iRockets.i[rocketInt].v.Concat (iRockets.i[rocketInt].v, megaSectors[i].sectors[iRocket].vertex);
						iRockets[indincesInt].i[rocketInt].e.AddRange (megaSectors[i].sectors[eRocket].vertex);
					}
				}
			}
			
			//Debug.Log ("------END OF FIRST LOOP----------");
		
		} else {
			
			for (tt in iTanks2.i) {
				//Debug.Log ("tt.v = " + tt.v);
				for (ti in tt.v) {
					for (tVertex in megaSectors[0].sectors[ti].vertex) {
					//Debug.Log ("ti = " + ti);
					//Debug.Log ("Vertex: " + transform.TransformPoint (vertices[tVertex]) + ", tank: " + tt.o.transform.position + ", and the Vector3.Distance: " + Vector3.Distance (transform.TransformPoint(vertices[ti]), tt.o.transform.position) + ";;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;");
					if (Vector3.Distance (transform.TransformPoint(vertices[tVertex]), tt.o.transform.position) < digDistance) {
						//Debug.Log ("Should have dug!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
						if (vertices[tVertex].z != minus.z) {
							tt.o.SendMessageUpwards("YouDug");
							vertices[tVertex] = Vector3 (vertices[tVertex].x, vertices[tVertex].y, minus.z);
						}
					}
					}
				}
			}
			
			//for (rrInt = 0; rrInt < iRockets2.i.Count; rrInt++) {
			//	var rr = iRockets2.i[rrInt];
			for (rr in iRockets2.i) {
				var isTriggered = false;
				
				for (ri in rr.v) {
					for (rVertex in megaSectors[0].sectors[ri].vertex) {
						if (!isTriggered) {
							if (Vector3.Distance (transform.TransformPoint(vertices[rVertex]), rr.o.transform.position) < trollRocketTouchRadius && rr.o.transform.parent.tag != "ExplodeRocket") {
								//Debug.Log (vertices[rVertex].z);
								if (!Mathf.Approximately (vertices[rVertex].z, minus.z)) {
									//Debug.Log ("Should explode");
									//Debug.Log (rr.GetType ());
									for (explode in rr.e) {
										for (eVertex in megaSectors[0].sectors[explode].vertex) {
											if (Vector3.Distance (transform.TransformPoint(vertices[eVertex]), rr.o.transform.position) < trollRocketExplodeRadius) {
												if (vertices[eVertex].z != minus.z) {
													vertices[eVertex] = Vector3 (vertices[eVertex].x, vertices[eVertex].y, minus.z);
												}
											}
										}
									}
								
								//SimulateExplode (rr);
								rr.o.transform.parent.GetComponent (RocketIdentifier).from.GetComponent (RocketShooter).Explode (rr.o.transform.parent.transform);
								rr.o.tag = "Untagged";
								rr.o.transform.parent.tag = "Untagged";
								isTriggered = true;
								
								}
							} else if (rr.o.transform.parent.tag == "ExplodeRocket") {
								for (explode in rr.e) {
									for (eVertex in megaSectors[0].sectors[explode].vertex) {
										if (Vector3.Distance (transform.TransformPoint(vertices[eVertex]), rr.o.transform.position) < trollRocketExplodeRadius) {
											if (vertices[eVertex].z != minus.z) {
												vertices[eVertex] = Vector3 (vertices[eVertex].x, vertices[eVertex].y, minus.z);
											}
										}
									}
								}

								rr.o.transform.parent.GetComponent (RocketIdentifier).from.GetComponent (RocketShooter).Explode (rr.o.transform.parent.transform);
								rr.o.tag = "Untagged";
								rr.o.transform.parent.tag = "Untagged";
								isTriggered = true;
							}
								 
						}
					}
				}
			}
//			
//			for (ss in indinces2) {
//				for (v in megaSectors[0].sectors[ss].vertex) {
//					//Debug.Log (v);
//					if (Vector3.Distance (transform.TransformPoint(vertices[v]), tank.position) < digDistance) {
//						if (vertices[v].z != minus.z) {
//							tank.SendMessageUpwards("YouDug");
//							vertices[v] = Vector3 (vertices[v].x, vertices[v].y, minus.z);
//						}
//					}
//					
//					for (trollRocket in trollRockets) {
//						if (Vector3.Distance (transform.TransformPoint(vertices[v]), trollRocket.transform.position) < trollRocketExplodeRadius) {
//							if (vertices[v].z != minus.z) {
//								trollRocket.GetComponent (RocketIdentifier).from.GetComponent (RocketShooter).Explode (trollRocket.transform);
//								trollRocket.tag = "Untagged";
//								vertices[v] = Vector3 (vertices[v].x, vertices[v].y, minus.z);
//							}
//						}
//					}
//					
//				}
//			}
		}
	}

//	
//	for (i = 0; i < vertices.Length; i++) {
//		if (Vector3.Distance (transform.TransformPoint(vertices[i]), tank.position) < digDistance) {
//			if (vertices[i].z != minus.z) {
//				tank.SendMessageUpwards("YouDug");
//			}
//			vertices[i] = Vector3 (vertices[i].x, vertices[i].y, minus.z);
//			//tank.GetComponent(Digger).timeSinceLastDig = Time.realtimeSinceStartup;
//		}
//	}
//	
	mesh.vertices = vertices;
	mesh.RecalculateBounds();
	
	if (saveMesh) {
		vertex = vertices;
	}
	
	if (updateMeshCollider) {
		GetComponent(MeshCollider).sharedMesh = null;
		GetComponent(MeshCollider).sharedMesh = GetComponent(MeshFilter).mesh;
    }
}

function SimulateExplode (rocket : Diggers) {
	
	Debug.Log ("Was Called");
	
	var mesh : Mesh = GetComponent(MeshFilter).mesh;
	var vertices : Vector3[] = mesh.vertices;
	var bigMegaSector : int = megaSectors.Count - 1;
	
	var indinces = new List.<int> ();
	
	for (sector = 0; sector < megaSectors[bigMegaSector].sectors.Count; sector++) {
		indinces.Add (sector);
	}
	
	var iRockets = new List.<Indinces> ();
	iRockets.Add (Indinces (rocket.o, indinces));
	
	var indincesInt : int = 0;
	
	for (i = bigMegaSector; i > -1; i--) {

		var iRockets2 = iRockets[indincesInt];
		
		iRockets.Add (Indinces (trollRockets, new List.<int> (), false));
		
		indincesInt++;
	
		
		if (i != 0) {
		
			for (iRocket in iRockets2.i[0].v) {
				if (Vector3.Distance (megaSectors[i].sectors[iRocket].pos, rocket.o.transform.position) < (radius + trollRocketTouchRadius) * (i + 1)) {
					iRockets[indincesInt].i[0].v.AddRange (megaSectors[i].sectors[iRocket].vertex);
				}
			}
			
		} else {
			
//			for (ri in iRockets2.i[0].v) {
//				if (Vector3.Distance (megaSectors[i].sectors[ri].pos, rocket.o.transform.position) < trollRocketExplodeRadius) {
//					if (vertices[ri].z != minus.z) {
//						vertices[ri] = Vector3 (vertices[ri].x, vertices[ri].y, minus.z);
//					}
//				}
//			}
			
			for (ri in iRockets2.i[0].v) {
				for (rVertex in megaSectors[0].sectors[ri].vertex) {
					//Debug.Log (Vector3.Distance (transform.TransformPoint(vertices[rVertex]), iRockets2.i[0].o.transform.position));
					if (Vector3.Distance (transform.TransformPoint(vertices[rVertex]), iRockets2.i[0].o.transform.position) < trollRocketExplodeRadius) {
						Debug.Log ("Shorter");
						if (vertices[rVertex].z != minus.z) {
							vertices[rVertex] = Vector3 (vertices[rVertex].x, vertices[rVertex].y, minus.z);
						}
					}
				}
			}
			
		}
	}
	
	mesh.vertices = vertices;
	mesh.RecalculateBounds();
	
}
	
		
	