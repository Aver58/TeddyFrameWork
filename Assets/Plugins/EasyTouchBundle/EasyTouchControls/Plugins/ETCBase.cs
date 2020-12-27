/***********************************************
				EasyTouch Controls
	Copyright © 2014-2015 The Hedgehog Team
  http://www.blitz3dfr.com/teamtalk/index.php
		
	  The.Hedgehog.Team@gmail.com
		
**********************************************/
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public abstract class ETCBase : MonoBehaviour {

	public enum ControlType {Joystick, TouchPad, DPad, Button};
	public enum RectAnchor { UserDefined,BottomLeft,BottomCenter,BottonRight,CenterLeft,Center,CenterRight,TopLeft,TopCenter, TopRight};
	public enum DPadAxis{ Two_Axis, Four_Axis };

	protected RectTransform cachedRectTransform;	
	protected Canvas cachedRootCanvas;

	[SerializeField]
	protected RectAnchor _anchor;
	public RectAnchor anchor {
		get {
			return _anchor;
		}
		set {
			if (value != _anchor){
				_anchor = value;
				SetAnchorPosition();
			}
		}
	}
	
	[SerializeField]
	protected Vector2 _anchorOffet;
	public Vector2 anchorOffet {
		get {
			return _anchorOffet;
		}
		set {
			if (value != _anchorOffet){
				_anchorOffet = value;
				SetAnchorPosition();
			}
		}
	}
	
	[SerializeField]
	protected bool _visible;
	public bool visible {
		get {
			return _visible;
		}
		set {
			if (value != _visible){
				_visible = value;
				SetVisible();
			}
		}
	}
	
	[SerializeField]
	protected bool _activated;
	public bool activated {
		get {
			return _activated;
		}
		set {
			if (value != _activated){
				_activated = value;
				SetActivated();
			}
		}
	}

	public int pointId=-1;

	public bool enableKeySimulation;
	public bool allowSimulationStandalone;

	public DPadAxis dPadAxisCount;
	public bool useFixedUpdate;

	private bool isShuttingDown;

	private List<RaycastResult> uiRaycastResultCache= new List<RaycastResult>();
	private PointerEventData uiPointerEventData;
	private EventSystem uiEventSystem;

	public bool isOnDrag;
	public bool isSwipeIn;
	public bool isSwipeOut;

	public bool showPSInspector;
	public bool showSpriteInspector;
	public bool showEventInspector;
	public bool showBehaviourInspector;
	public bool showAxesInspector;
	public bool showTouchEventInspector;
	public bool showDownEventInspector;
	public bool showPressEventInspector;



	protected virtual void Awake(){
		isShuttingDown = false;
		cachedRectTransform = transform as RectTransform;
		cachedRootCanvas = transform.parent.GetComponent<Canvas>();

		if (!isShuttingDown){
			ETCInput.instance.RegisterControl( this);
		}

		#if (!UNITY_EDITOR) 
		if (!allowSimulationStandalone){
			enableKeySimulation = false;
		}
		#endif
	}
	
	void OnDestroy(){

#pragma warning disable 618
		if (!isShuttingDown && !Application.isLoadingLevel){
#pragma warning restore 618
			if (ETCInput.instance)
				ETCInput.instance.UnRegisterControl( this );
		}
	}

	void OnApplicationQuit(){
		isShuttingDown = true;
	}

	public virtual void Update(){

		if (!useFixedUpdate){
			StartCoroutine ("UpdateVirtualControl");
		}
	}
	
	public virtual void FixedUpdate(){
		if (useFixedUpdate){
			StartCoroutine ("UpdateVirtualControl");
		}
	}

	IEnumerator UpdateVirtualControl() {
		yield return new WaitForEndOfFrame();
		UpdateControlState();
	}

	protected virtual void UpdateControlState(){

	}

	protected virtual void SetVisible(){

	}

	protected virtual void SetActivated(){

	}

	public void SetAnchorPosition(){
		
		switch (_anchor){
		case RectAnchor.TopLeft:
			this.rectTransform().anchorMin = new Vector2(0,1);
			this.rectTransform().anchorMax = new Vector2(0,1);
			this.rectTransform().anchoredPosition = new Vector2( this.rectTransform().sizeDelta.x/2f + _anchorOffet.x, -this.rectTransform().sizeDelta.y/2f - _anchorOffet.y);
			break;
		case RectAnchor.TopCenter:
			this.rectTransform().anchorMin = new Vector2(0.5f,1);
			this.rectTransform().anchorMax = new Vector2(0.5f,1);
			this.rectTransform().anchoredPosition = new Vector2(  _anchorOffet.x, -this.rectTransform().sizeDelta.y/2f - _anchorOffet.y);
			break;
		case RectAnchor.TopRight:
			this.rectTransform().anchorMin = new Vector2(1,1);
			this.rectTransform().anchorMax = new Vector2(1,1);
			this.rectTransform().anchoredPosition = new Vector2( -this.rectTransform().sizeDelta.x/2f - _anchorOffet.x, -this.rectTransform().sizeDelta.y/2f - _anchorOffet.y);
			break;
			
		case RectAnchor.CenterLeft:
			this.rectTransform().anchorMin = new Vector2(0,0.5f);
			this.rectTransform().anchorMax = new Vector2(0,0.5f);
			this.rectTransform().anchoredPosition = new Vector2( this.rectTransform().sizeDelta.x/2f + _anchorOffet.x, _anchorOffet.y);
			break;
			
		case RectAnchor.Center:
			this.rectTransform().anchorMin = new Vector2(0.5f,0.5f);
			this.rectTransform().anchorMax = new Vector2(0.5f,0.5f);
			this.rectTransform().anchoredPosition = new Vector2(  _anchorOffet.x, _anchorOffet.y);
			break;
			
		case RectAnchor.CenterRight:
			this.rectTransform().anchorMin = new Vector2(1,0.5f);
			this.rectTransform().anchorMax = new Vector2(1,0.5f);
			this.rectTransform().anchoredPosition = new Vector2( -this.rectTransform().sizeDelta.x/2f -  _anchorOffet.x, _anchorOffet.y);
			break; 
			
		case RectAnchor.BottomLeft:
			this.rectTransform().anchorMin = new Vector2(0,0);
			this.rectTransform().anchorMax = new Vector2(0,0);
			this.rectTransform().anchoredPosition = new Vector2( this.rectTransform().sizeDelta.x/2f + _anchorOffet.x, this.rectTransform().sizeDelta.y/2f + _anchorOffet.y);
			break;
		case RectAnchor.BottomCenter:
			this.rectTransform().anchorMin = new Vector2(0.5f,0);
			this.rectTransform().anchorMax = new Vector2(0.5f,0);
			this.rectTransform().anchoredPosition = new Vector2(  _anchorOffet.x, this.rectTransform().sizeDelta.y/2f + _anchorOffet.y);
			break;
		case RectAnchor.BottonRight:
			this.rectTransform().anchorMin = new Vector2(1,0);
			this.rectTransform().anchorMax = new Vector2(1,0);
			this.rectTransform().anchoredPosition = new Vector2( -this.rectTransform().sizeDelta.x/2f - _anchorOffet.x, this.rectTransform().sizeDelta.y/2f + _anchorOffet.y);
			break;
		}
		
	}

	protected GameObject GetFirstUIElement( Vector2 position){
		
		uiEventSystem = EventSystem.current;
		if (uiEventSystem != null){
			
			uiPointerEventData = new PointerEventData( uiEventSystem);
			uiPointerEventData.position = position;
			
			uiEventSystem.RaycastAll( uiPointerEventData, uiRaycastResultCache);
			if (uiRaycastResultCache.Count>0){
				return uiRaycastResultCache[0].gameObject;
			}
			else{
				return null;
			}
		}
		else{
			return null;
		}
	}

}
