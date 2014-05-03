var saved : float[,,];
private var tData : TerrainData;
var cratertex : Texture2D;
var craterData;
var xRes: int;
var yRes: int;
var layers: int;

function Start () {
tData = Terrain.activeTerrain.terrainData;
yRes = tData.alphamapHeight;
xRes = tData.alphamapWidth;
layers = tData.alphamapLayers;
craterData = cratertex.GetPixels();
saved = tData.GetAlphamaps (0, 0, xRes, yRes);
}

function OnApplicationQuit () {
tData.SetAlphamaps (0,0,saved);
}
function Update () {
if (Input.GetMouseButtonDown(0)){
var g : int = Mathf.Lerp(0, xRes, Mathf.InverseLerp(0, tData.size.x, mouse.mousepos.x));
var b : int = Mathf.Lerp(0, yRes, Mathf.InverseLerp(0, tData.size.z, mouse.mousepos.z));
g = Mathf.Clamp(g, cratertex.width/2, xRes-cratertex.width/2);
b = Mathf.Clamp(b, cratertex.height/2, yRes-cratertex.height/2);
var area = tData.GetAlphamaps(g-cratertex.width/2, b-cratertex.height/2, cratertex.width, cratertex.height);
for (x = 0; x < cratertex.height; x++) {
	for (y = 0; y < cratertex.width; y++) {
	for (z = 0; z < layers; z++){	
	if (z == 1){
	area [x,y,z] += craterData[x*cratertex.width + y].a;
	}
	else{	
	area [x,y,z] -= craterData[x*cratertex.width + y].a;	
	}	
		}}}
tData.SetAlphamaps (g-cratertex.width/2,b-cratertex.height/2,area);
}
}


