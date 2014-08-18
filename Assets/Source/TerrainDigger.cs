//
//  TerrainDigger.cs is part of Tunnelers: Unified
//  <https://github.com/VacuumGames/tunnelers-unified/>.
//
//  Copyright (c) 2014 Juraj Fiala<doctorjellyface@riseup.net>
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

public class TerrainDigger : MonoBehaviour {

	public TerrainData tData;
	public float [,] saved;
	public Texture2D cratertex;
	public int xRes;
	public int yRes;
	public Color [] craterData;
	public float opacity;
	
	public Transform tank;
	
	void Start () {
	
		tData = Terrain.activeTerrain.terrainData;
		xRes = tData.heightmapWidth;
		yRes = tData.heightmapHeight;
		saved = tData.GetHeights (0,0, xRes, yRes);
		craterData = cratertex.GetPixels ();
		
	}
		
	void OnApplicationQuit () {
	
		tData.SetHeights (0,0, saved);
		
	}
		
	void Update () {
		
		int x = (int) Mathf.Lerp (0, xRes, Mathf.InverseLerp (0, tData.size.x, tank.position.x));
		int z = (int) Mathf.Lerp (0, yRes, Mathf.InverseLerp (0, tData.size.z, tank.position.z));
		x = Mathf.Clamp (x, cratertex.width / 2, xRes - cratertex.width / 2);
		z = Mathf.Clamp (z, cratertex.height / 2, yRes - cratertex.height / 2);
		float [,] areaT = tData.GetHeights (x - cratertex.width / 2, z - cratertex.height / 2, cratertex.width, cratertex.height);
		
		for (int i = 0; i < cratertex.height; i++) {
			for (int j = 0; j < cratertex.width; j++) {
				areaT [i,j] = areaT [i,j] - craterData [i * cratertex.width + j].a * opacity;
			}
		}		
		
		tData.SetHeights (x - cratertex.width / 2, z - cratertex.height / 2, areaT);	
	}
}
