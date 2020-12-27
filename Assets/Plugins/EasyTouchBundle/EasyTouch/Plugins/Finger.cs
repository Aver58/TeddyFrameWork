/***********************************************
				EasyTouch IV
	Copyright © 2014-2015 The Hedgehog Team
  http://www.blitz3dfr.com/teamtalk/index.php
		
	  The.Hedgehog.Team@gmail.com
		
**********************************************/
using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

// Represents informations on Finger for touch
// Internal use only, DO NOT USE IT
public class Finger : BaseFinger{

	public float startTimeAction;
	public Vector2 oldPosition;			
	public int tapCount;				// Number of taps.
	public TouchPhase phase;			// Describes the phase of the touch.
	public EasyTouch.GestureType gesture;		
	public EasyTouch.SwipeDirection oldSwipeType;

}

	



