/***********************************************
				EasyTouch IV
	Copyright © 2014-2015 The Hedgehog Team
  http://www.blitz3dfr.com/teamtalk/index.php
		
	  The.Hedgehog.Team@gmail.com
		
**********************************************/
using UnityEngine;
using System.Collections;

[System.Serializable]
public class ECamera{

	public Camera camera;
	public bool guiCamera;
	
	public ECamera( Camera cam,bool gui){
		this.camera = cam;
		this.guiCamera = gui;
	}
	
}
