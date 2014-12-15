#pragma strict

public var t:GUITexture;
 
function Awake()
{
    var size = 0.13 * Screen.width;
   
    t.pixelInset.width = size;
    t.pixelInset.height = size;
    t.pixelInset.x = -size/2;
    t.pixelInset.y = -size/3;
}

function Start () {

}

function Update () {

}