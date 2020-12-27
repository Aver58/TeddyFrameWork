/***********************************************
				EasyTouch IV
	Copyright © 2014-2015 The Hedgehog Team
  http://www.blitz3dfr.com/teamtalk/index.php
		
	  The.Hedgehog.Team@gmail.com
		
**********************************************/
using UnityEngine;
using System.Collections;

public class TwoFingerGesture{

	public EasyTouch.GestureType currentGesture = EasyTouch.GestureType.None;
	public EasyTouch.GestureType oldGesture= EasyTouch.GestureType.None;
	public int finger0;											
	public int finger1;	

	public float startTimeAction;	
	public float timeSinceStartAction;
	public Vector2 startPosition;
	public Vector2 position;
	public Vector2 deltaPosition;
	public Vector2 oldStartPosition;

	public float fingerDistance;
	public float oldFingerDistance;

	// Game Object
	public GameObject pickedObject;
	public GameObject oldPickedObject;
	public Vector3 hitPoint;
	public Camera pickedCamera;
	public bool isGuiCamera;

	// UI
	public bool isOverGui;
	public GameObject pickedUIElement;
	
	public bool dragStart=false;
	public bool swipeStart=false;

	public bool inSingleDoubleTaps = false;
	public float tapCurentTime = 0;

	public void ClearPickedObjectData(){
		pickedObject = null;
		oldPickedObject = null;
		pickedCamera = null;
		isGuiCamera = false;
	}

	public void ClearPickedUIData(){
		isOverGui = false;
		pickedUIElement = null;
	}
}


