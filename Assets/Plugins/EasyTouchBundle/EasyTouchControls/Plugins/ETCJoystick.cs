/***********************************************
				EasyTouch Controls
	Copyright © 2014-2015 The Hedgehog Team
  http://www.blitz3dfr.com/teamtalk/index.php
		
	  The.Hedgehog.Team@gmail.com
		
**********************************************/
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

[System.Serializable]
public class ETCJoystick : ETCBase,IPointerEnterHandler,IDragHandler, IBeginDragHandler, IPointerDownHandler, IPointerUpHandler {
		
	#region Unity Events
	[System.Serializable] public class OnMoveStartHandler : UnityEvent{}
	[System.Serializable] public class OnMoveSpeedHandler : UnityEvent<Vector2> { }
	[System.Serializable] public class OnMoveHandler : UnityEvent<Vector2> { }
	[System.Serializable] public class OnMoveEndHandler : UnityEvent{ }

	[System.Serializable] public class OnTouchStartHandler : UnityEvent{}
	[System.Serializable] public class OnTouchUpHandler : UnityEvent{ }
    [System.Serializable]public class OnTouchUpHandler2 : UnityEvent<PointerEventData> { }

	[System.Serializable] public class OnDownUpHandler : UnityEvent{ }
	[System.Serializable] public class OnDownDownHandler : UnityEvent{ }
	[System.Serializable] public class OnDownLeftHandler : UnityEvent{ }
	[System.Serializable] public class OnDownRightHandler : UnityEvent{ }

	[System.Serializable] public class OnPressUpHandler : UnityEvent{ }
	[System.Serializable] public class OnPressDownHandler : UnityEvent{ }
	[System.Serializable] public class OnPressLeftHandler : UnityEvent{ }
	[System.Serializable] public class OnPressRightHandler : UnityEvent{ }


	[SerializeField] public OnMoveStartHandler onMoveStart;
	[SerializeField] public OnMoveHandler onMove;
	[SerializeField] public OnMoveSpeedHandler onMoveSpeed;
	[SerializeField] public OnMoveEndHandler onMoveEnd;

	[SerializeField] public OnTouchStartHandler onTouchStart;
	[SerializeField] public OnTouchUpHandler onTouchUp;
    [SerializeField]public OnTouchUpHandler2 onTouchUp2;

	[SerializeField] public OnDownUpHandler OnDownUp;
	[SerializeField] public OnDownDownHandler OnDownDown;
	[SerializeField] public OnDownLeftHandler OnDownLeft;
	[SerializeField] public OnDownRightHandler OnDownRight;

	[SerializeField] public OnDownUpHandler OnPressUp;
	[SerializeField] public OnDownDownHandler OnPressDown;
	[SerializeField] public OnDownLeftHandler OnPressLeft;
	[SerializeField] public OnDownRightHandler OnPressRight;
	#endregion

	#region Enumeration
	public enum JoystickArea { UserDefined,FullScreen, Left,Right,Top, Bottom, TopLeft, TopRight, BottomLeft, BottomRight};
	public enum JoystickType {Dynamic, Static};
	public enum RadiusBase {Width, Height};
	#endregion

	#region Members

	#region Public members
	public JoystickType joystickType;
	public bool allowJoystickOverTouchPad;
	public RadiusBase radiusBase;
	public ETCAxis axisX;
	public ETCAxis axisY;
	public RectTransform thumb;
	
	public JoystickArea joystickArea;
	public RectTransform userArea;
	#endregion
		
	#region Private members
	private Vector2 thumbPosition;
	private bool isDynamicActif;
	private Vector2 tmpAxis;
	private Vector2 OldTmpAxis;
	private bool isOnTouch;
	#endregion

	#region Joystick behavior option
	[SerializeField]
	private bool isNoReturnThumb;
	public bool IsNoReturnThumb {
		get {
			return isNoReturnThumb;
		}
		set {
			isNoReturnThumb = value;
		}
	}	

	private Vector2 noReturnPosition;
	private Vector2 noReturnOffset;

	[SerializeField]
	private bool isNoOffsetThumb;
	public bool IsNoOffsetThumb {
		get {
			return isNoOffsetThumb;
		}
		set {
			isNoOffsetThumb = value;
		}
	}
	#endregion

	#region Inspector


	#endregion

	#endregion
	
	#region Constructor
	public ETCJoystick(){
		joystickType = JoystickType.Static;
		allowJoystickOverTouchPad = false;
		radiusBase = RadiusBase.Width;

		axisX = new ETCAxis("Horizontal");
		axisY = new ETCAxis("Vertical");
	
		_visible = true;
		_activated = true;

		joystickArea = JoystickArea.FullScreen;

		isDynamicActif = false;
		isOnDrag = false;
		isOnTouch = false;

		axisX.positivekey = KeyCode.RightArrow;
		axisX.negativeKey = KeyCode.LeftArrow;

		axisY.positivekey = KeyCode.UpArrow;
		axisY.negativeKey = KeyCode.DownArrow;

		enableKeySimulation = true;

		isNoReturnThumb = false;

		showPSInspector = true;
		showAxesInspector = false;
		showEventInspector = false;
		showSpriteInspector = false;
	}
	#endregion

	#region Monobehaviours Callback
	protected override void Awake (){
		base.Awake ();

		if (joystickType == JoystickType.Dynamic){
			this.rectTransform().anchorMin = new Vector2(0.5f,0.5f);
			this.rectTransform().anchorMax = new Vector2(0.5f,0.5f);
			this.rectTransform().SetAsLastSibling();
			visible = false;
		}
	}

	void Start(){
		tmpAxis = Vector2.zero;
		OldTmpAxis = Vector2.zero;

		axisX.InitAxis();
		axisY.InitAxis();
		noReturnPosition = thumb.position;
	}

    void OnEnable()
    {
        EasyTouch.On_TouchUp += On_TouchUp;
    }

    void OnDisable()
    {
        EasyTouch.On_TouchUp -= On_TouchUp;
    }

    void OnDestroy()
    {
        EasyTouch.On_TouchUp -= On_TouchUp;
    }

    // Touch end
    void On_TouchUp(Gesture gesture)
    {
        OnPointerUp(null);
    }


	protected override void UpdateControlState ()
	{
		UpdateJoystick();
	}

	#endregion
	
	#region UI Callback
	public void OnPointerEnter(PointerEventData eventData){

		if (joystickType == JoystickType.Dynamic && !isDynamicActif && _activated){
			eventData.pointerDrag = gameObject;
			eventData.pointerPress = gameObject;

			isDynamicActif = true;
		}

		if (joystickType == JoystickType.Dynamic &&  !eventData.eligibleForClick){
			OnPointerUp( eventData );
		}

	}

	public void OnPointerDown(PointerEventData eventData){
		onTouchStart.Invoke();

		if (isNoOffsetThumb){
			OnDrag( eventData);
		}
	}

	public void OnBeginDrag(PointerEventData eventData){


	}
	
	public void OnDrag(PointerEventData eventData){

		isOnDrag = true;
		isOnTouch = true;

		float radius =  GetRadius();


		if (!isNoReturnThumb){
			thumbPosition =  (eventData.position - eventData.pressPosition) / cachedRootCanvas.rectTransform().localScale.x;
		}
		else{
			thumbPosition =((eventData.position - noReturnPosition) /cachedRootCanvas.rectTransform().localScale.x) + noReturnOffset;
		}

		if (isNoOffsetThumb){
		    thumbPosition =  (eventData.position - (Vector2)cachedRectTransform.position) / cachedRootCanvas.rectTransform().localScale.x;
		}

		thumbPosition.x = Mathf.FloorToInt( thumbPosition.x);
		thumbPosition.y = Mathf.FloorToInt( thumbPosition.y);


		if (!axisX.enable){
			thumbPosition.x=0;
		}

		if (!axisY.enable){
			thumbPosition.y=0;
		}

        thumb.gameObject.SetActive(true);
		if (thumbPosition.magnitude > radius)
        {
            if (!isNoReturnThumb)
            {
                thumbPosition = thumbPosition.normalized * radius;
            }
            else
            {
                thumbPosition = thumbPosition.normalized * radius;
            }
		}

		thumb.anchoredPosition =  thumbPosition; 

	}

	public void OnPointerUp (PointerEventData eventData){

        thumb.gameObject.SetActive(false);
		isOnDrag = false;
		isOnTouch = false;

		if (isNoReturnThumb){
			noReturnPosition = thumb.position;
			noReturnOffset = thumbPosition;
		}

		if (!isNoReturnThumb){
			thumbPosition =  Vector2.zero;
			thumb.anchoredPosition = Vector2.zero;

			axisX.axisState = ETCAxis.AxisState.None;
			axisY.axisState = ETCAxis.AxisState.None;
		}

		if (!axisX.isEnertia && !axisY.isEnertia){
//			if (!isNoReturn){
				axisX.ResetAxis();
				axisY.ResetAxis();
				tmpAxis = Vector2.zero;
				OldTmpAxis = Vector2.zero;
				onMoveEnd.Invoke();
		//	}
		}

		if (joystickType == JoystickType.Dynamic){
			visible = false;
			isDynamicActif = false;
		}

		onTouchUp.Invoke();
        if (eventData != null)
        {
            onTouchUp2.Invoke(eventData);
        }
	}

	#endregion

	#region Joystick Update
	private void UpdateJoystick(){

		#region dynamic joystick
		if (joystickType == JoystickType.Dynamic && !_visible && _activated){
			Vector2 localPosition = Vector2.zero;
			Vector2 screenPosition = Vector2.zero;

			if (isTouchOverJoystickArea(ref localPosition, ref screenPosition)){

				GameObject overGO = GetFirstUIElement( screenPosition);

				if (overGO == null || (allowJoystickOverTouchPad && overGO.GetComponent<ETCTouchPad>()) || (overGO != null && overGO.GetComponent<ETCArea>() ) ) {
					cachedRectTransform.anchoredPosition = localPosition;
					visible = true;
				}
			}
		}
		#endregion

		#region Key simulation
		if (enableKeySimulation && !isOnTouch && _activated && _visible){

			if (!isNoReturnThumb){
				thumb.localPosition = Vector2.zero;
			}

			isOnDrag = false;

			if (Input.GetKey( axisX.positivekey)){
				isOnDrag = true;
				thumb.localPosition = new Vector2(GetRadius(), thumb.localPosition.y);
			}
			else if (Input.GetKey( axisX.negativeKey)){
				isOnDrag = true;
				thumb.localPosition = new Vector2(-GetRadius(), thumb.localPosition.y);
			}

			if (Input.GetKey( axisY.positivekey)){
				isOnDrag = true;
				thumb.localPosition = new Vector2(thumb.localPosition.x,GetRadius() );
			}
			else if (Input.GetKey( axisY.negativeKey)){
				isOnDrag = true;
				thumb.localPosition = new Vector2(thumb.localPosition.x,-GetRadius());
			}

			thumbPosition = thumb.localPosition;
		}
		#endregion

		// Computejoystick value
		OldTmpAxis.x = axisX.axisValue;
		OldTmpAxis.y = axisY.axisValue;

		tmpAxis = thumbPosition / GetRadius();

		axisX.UpdateAxis( tmpAxis.x,isOnDrag, ETCBase.ControlType.Joystick,true);
		axisY.UpdateAxis( tmpAxis.y,isOnDrag, ETCBase.ControlType.Joystick,true);

		axisX.DoGravity();
		axisY.DoGravity();

		#region Move event
		if ((axisX.axisValue!=0 ||  axisY.axisValue!=0 ) && OldTmpAxis == Vector2.zero){
			onMoveStart.Invoke();
		}
		if (axisX.axisValue!=0 ||  axisY.axisValue!=0 ){

			// X axis
			if( axisX.actionOn == ETCAxis.ActionOn.Down && (axisX.axisState == ETCAxis.AxisState.DownLeft || axisX.axisState == ETCAxis.AxisState.DownRight)){
				axisX.DoDirectAction();
			}
			else if (axisX.actionOn == ETCAxis.ActionOn.Press){
				axisX.DoDirectAction();
			}

			// Y axis
			if( axisY.actionOn == ETCAxis.ActionOn.Down && (axisY.axisState == ETCAxis.AxisState.DownUp || axisY.axisState == ETCAxis.AxisState.DownDown)){
				axisY.DoDirectAction();
			}
			else if (axisY.actionOn == ETCAxis.ActionOn.Press){
				axisY.DoDirectAction();
			}
			onMove.Invoke( new Vector2(axisX.axisValue,axisY.axisValue));
			onMoveSpeed.Invoke( new Vector2(axisX.axisSpeedValue,axisY.axisSpeedValue));
		}
		else if (axisX.axisValue==0 &&  axisY.axisValue==0  && OldTmpAxis!=Vector2.zero) {
			onMoveEnd.Invoke();
		}
		#endregion

		#region Down & press event
		float coef =1;
		if (axisX.invertedAxis) coef = -1;
		if (Mathf.Abs(OldTmpAxis.x)< axisX.axisThreshold && Mathf.Abs(axisX.axisValue)>=axisX.axisThreshold){

			if (axisX.axisValue*coef >0){
				axisX.axisState = ETCAxis.AxisState.DownRight;
				OnDownRight.Invoke();
			}
			else if (axisX.axisValue*coef<0){
				axisX.axisState = ETCAxis.AxisState.DownLeft;
				OnDownLeft.Invoke();
			}
			else{
				axisX.axisState = ETCAxis.AxisState.None;
			}
		}
		else if (axisX.axisState!= ETCAxis.AxisState.None) {
			if (axisX.axisValue*coef>0){
				axisX.axisState = ETCAxis.AxisState.PressRight;
				OnPressRight.Invoke();
			}
			else if (axisX.axisValue*coef<0){
				axisX.axisState = ETCAxis.AxisState.PressLeft;
				OnPressLeft.Invoke();
			}
			else{
				axisX.axisState = ETCAxis.AxisState.None;
			}
		}

		coef =1;
		if (axisY.invertedAxis) coef = -1;
		if (Mathf.Abs(OldTmpAxis.y)< axisY.axisThreshold && Mathf.Abs(axisY.axisValue)>=axisY.axisThreshold  ){
			
			if (axisY.axisValue*coef>0){
				axisY.axisState = ETCAxis.AxisState.DownUp;
				OnDownUp.Invoke();
			}
			else if (axisY.axisValue*coef<0){
				axisY.axisState = ETCAxis.AxisState.DownDown;
				OnDownDown.Invoke();
			}
			else{
				axisY.axisState = ETCAxis.AxisState.None;
			}
		}
		else if (axisY.axisState!= ETCAxis.AxisState.None) {
			if (axisY.axisValue*coef>0){
				axisY.axisState = ETCAxis.AxisState.PressUp;
				OnPressUp.Invoke();
			}
			else if (axisY.axisValue*coef<0){
				axisY.axisState = ETCAxis.AxisState.PressDown;
				OnPressDown.Invoke();
			}
			else{
				axisY.axisState = ETCAxis.AxisState.None;
			}
		}
		#endregion

	}		
	#endregion

	#region Touch manager
	private bool isTouchOverJoystickArea(ref Vector2 localPosition, ref Vector2 screenPosition){
		
		bool touchOverArea = false;
		bool doTest = false;
		screenPosition = Vector2.zero;
		
		int count = GetTouchCount();
		
		int i=0;
		while (i<count && !touchOverArea){
			#if ((UNITY_ANDROID || UNITY_IPHONE || UNITY_WINRT || UNITY_BLACKBERRY) && !UNITY_EDITOR) 
			if (Input.GetTouch(i).phase == TouchPhase.Began){
				screenPosition = Input.GetTouch(i).position;
				doTest = true;
			}
			#else
			if (Input.GetMouseButton(0)){
				screenPosition = Input.mousePosition;
				doTest = true;
			}
			#endif
			
			if (doTest && isScreenPointOverArea(screenPosition, ref localPosition) ){
				touchOverArea = true;
			}
			
			i++;
		}
		
		return touchOverArea;
	}
	
	private bool isScreenPointOverArea(Vector2 screenPosition, ref Vector2 localPosition){
		
		bool returnValue = false;
		
		if (joystickArea != JoystickArea.UserDefined){
			if (RectTransformUtility.ScreenPointToLocalPointInRectangle( cachedRootCanvas.rectTransform(),screenPosition,null,out localPosition)){
				
				switch (joystickArea){
				case JoystickArea.Left:
					if (localPosition.x<0){
						returnValue=true;
					}
					break;
					
				case JoystickArea.Right:
					if (localPosition.x>0){
						returnValue = true;
					}
					break;
					
				case JoystickArea.FullScreen:
					returnValue = true;
					break;
					
				case JoystickArea.TopLeft:
					if (localPosition.y>0 && localPosition.x<0){
						returnValue = true;
					}
					break;
				case JoystickArea.Top:
					if (localPosition.y>0){
						returnValue = true;
					}
					break;
					
				case JoystickArea.TopRight:
					if (localPosition.y>0 && localPosition.x>0){
						returnValue=true;
					}
					break;
					
				case JoystickArea.BottomLeft:
					if (localPosition.y<0 && localPosition.x<0){
						returnValue = true;
					}
					break;
					
				case JoystickArea.Bottom:
					if (localPosition.y<0){
						returnValue = true;
					}
					break;
					
				case JoystickArea.BottomRight:
					if (localPosition.y<0 && localPosition.x>0){
						returnValue = true;
					}
					break;
				}
			}
		}
		else{
			if (RectTransformUtility.RectangleContainsScreenPoint( userArea, screenPosition, cachedRootCanvas.worldCamera )){
				RectTransformUtility.ScreenPointToLocalPointInRectangle( cachedRootCanvas.rectTransform(),screenPosition,cachedRootCanvas.worldCamera,out localPosition);
				returnValue = true;
			}
		}
		
		return returnValue;
		
	}
	
	private int GetTouchCount(){
		#if ((UNITY_ANDROID || UNITY_IPHONE || UNITY_WINRT || UNITY_BLACKBERRY) && !UNITY_EDITOR) 
		return Input.touchCount;
		#else
		if (Input.GetMouseButton(0) || Input.GetMouseButtonUp(0)){
			return 1;
		}
		else{
			return 0;
		}
		#endif
	}
	#endregion

	#region Other private method
	private float GetRadius(){
		
		float radius =0;
		
		switch (radiusBase){
		case RadiusBase.Width:
			radius = cachedRectTransform.sizeDelta.x * 0.5f;
			break;
		case RadiusBase.Height:
			radius = cachedRectTransform.sizeDelta.y * 0.5f;
			break;
		}
		
		return radius;
	}

	protected override void SetActivated (){
	
		GetComponent<CanvasGroup>().blocksRaycasts = _activated;
	}

	protected override void SetVisible (){

		GetComponent<Image>().enabled = _visible;
		thumb.GetComponent<Image>().enabled = _visible;
		GetComponent<CanvasGroup>().blocksRaycasts = _activated;
	}
	#endregion
	
}

