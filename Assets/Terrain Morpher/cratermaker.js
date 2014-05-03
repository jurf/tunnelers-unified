private var tData : TerrainData;
private var saved : float[,];
var cratertex : Texture2D;
var xRes;
var yRes;
var craterData;

function Start () {
tData = Terrain.activeTerrain.terrainData;
xRes = tData.heightmapWidth;
yRes = tData.heightmapHeight;
saved = tData.GetHeights(0,0,xRes,yRes);
craterData = cratertex.GetPixels();
}
function OnApplicationQuit () {
tData.SetHeights(0,0,saved);
}
function Update () 
{
if (Input.GetMouseButtonDown(0))
	{
	var x : int = Mathf.Lerp(0, xRes, Mathf.InverseLerp(0, tData.size.x, mouse.mousepos.x));
	var z : int = Mathf.Lerp(0, yRes, Mathf.InverseLerp(0, tData.size.z, mouse.mousepos.z));
	x = Mathf.Clamp(x, cratertex.width/2, xRes-cratertex.width/2);
	z = Mathf.Clamp(z, cratertex.height/2, yRes-cratertex.height/2);
	var areaT = tData.GetHeights(x-cratertex.width/2, z-cratertex.height/2, cratertex.width, cratertex.height);
		for (i = 0; i < cratertex.height; i++) {
		for (j = 0; j < cratertex.width; j++) {
			areaT [i,j] = areaT [i,j] - craterData[i*cratertex.width+j].a*0.01;
		}
		}		
	tData.SetHeights(x-cratertex.width/2, z-cratertex.height/2, areaT);	
	}
}


