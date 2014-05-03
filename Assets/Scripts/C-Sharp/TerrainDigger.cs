using UnityEngine;
using System.Collections;

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
