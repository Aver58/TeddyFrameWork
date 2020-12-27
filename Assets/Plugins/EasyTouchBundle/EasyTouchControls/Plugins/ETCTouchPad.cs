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

[System.Serializable]
public class ETCTouchPad : ETCBase,IBeginDragHandler, IDragHandler,IPointerEnterHandler,  IPointerDownHandler, IPointerUpHandler, IPointerExitHandler {

	#region Unity Events
	[System.Serializable] public class OnMoveStartHandler : UnityEvent{}
	[System.Serializable] public class OnMoveHandler : UnityEvent<Vector2> { }
	[System.Serializable] public class OnMoveSpeedHandler : UnityEvent<Vector2> { }
	[System.Serializable] public class OnMoveEndHandler : UnityEvent{ }
	
	[System.Serializable] public class OnTouchStartHandler : UnityEvent{}
	[System.Serializable] public class OnTouchUPHandler : UnityEvent{}
	
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
	[SerializeField] public OnTouchUPHandler onTouchUp;
	
	
	[SerializeField] public OnDownUpHandler OnDownUp;
	[SerializeField] public OnDownDownHandler OnDownDown;
	[SerializeField] public OnDownLeftHandler OnDownLeft;
	[SerializeField] public OnDownRightHandler OnDownRight;
	
	[SerializeField] public OnDownUpHandler OnPressUp;
	[SerializeField] public OnDownDownHandler OnPressDown;
	[SerializeField] public OnDownLeftHandler OnPressLeft;
	[SerializeField] public OnDownRightHandler OnPressRight;
	#endregion

	#region Public members
	public ETCAxis axisX;
	public ETCAxis axisY;
	public bool isDPI;
	#endregion

	#region Private members
	private Image cachedImage; 

	private Vector2 tmpAxis;
	private Vector2 OldTmpAxis;
	
	private GameObject previousDargObject;

	private bool isOut;
	private bool isOnTouch;
	#endregion

	#region Constructor
	public ETCTouchPad(){

		axisX = new ETCAxis("Horizontal");
		axisX.speed = 1;

		axisY = new ETCAxis("Vertical");
		axisY.speed = 1;

		_visible = true;
		_activated = true;

		showPSInspector = true; 
		showSpriteInspector = false;
		showBehaviourInspector = false;
		showEventInspector = false;

		tmpAxis = Vector2.zero;
		isOnDrag = false;
		isOnTouch = false;
		
		axisX.positivekey = KeyCode.RightArrow;
		axisX.negativeKey = KeyCode.LeftArrow;
		
		axisY.positivekey = KeyCode.UpArrow;
		axisY.negativeKey = KeyCode.DownArrow;
		
		enableKeySimulation = true;
		#if !UNITY_EDITOR
		enableKeySimulation = false;
		#endif

		isOut = false;

		axisX.axisState = ETCAxis.AxisState.None;

		useFixedUpdate = false;

		isDPI = false;
	}
	#endregion

	#region Monobehaviour Callback
	protected override void Awake (){
		base.Awake ();
		
		cachedImage = GetComponent<Image>();

		if (!_visible){
			cachedImage.color = new Color(0,0,0,0);
		}
		
	}

	void Start(){
		tmpAxis = Vector2.zero;
		OldTmpAxis = Vector2.zero;
		axisX.InitAxis();
		axisY.InitAxis();
	}

	/*
	void Update(){
		
		if (!useFixedUpdate){
			UpdateTouchPad();
		}
	}
	
	void FixedUpdate(){
		if (useFixedUpdate){
			UpdateTouchPad();
		}
	}*/

	protected override void UpdateControlState ()
	{
		UpdateTouchPad();
	}
	#endregion

	#region UI Callback
	public void OnPointerEnter(PointerEventData eventData){

		if (isSwipeIn &&  axisX.axisState == ETCAxis.AxisState.None && _activated && !isOnTouch){

			if (eventData.pointerDrag != null && eventData.pointerDrag!= gameObject){
				previousDargObject=eventData.pointerDrag;
			}
			else if (eventData.pointerPress!= null && eventData.pointerPress!= gameObject){
				previousDargObject=eventData.pointerPress;
			}

			eventData.pointerDrag = gameObject;
			eventData.pointerPress = gameObject;
			OnPointerDown( eventData);

		}
	}

	public void OnBeginDrag(PointerEventData eventData){
		if (pointId == eventData.pointerId){
			onMoveStart.Invoke();
		}

	}

	public void OnDrag(PointerEventData eventData){

		if (activated && !isOut && pointId == eventData.pointerId){
			isOnTouch = true;
			isOnDrag = true;
			if (isDPI){
				tmpAxis = new Vector2(eventData.delta.x / Screen.width * 1000 , eventData.delta.y / Screen.height *1000);
			}
			else{
				tmpAxis = new Vector2(eventData.delta.x , eventData.delta.y );
			}

			if (!axisX.enable){
				tmpAxis.x=0;
			}

			if (!axisY.enable){
				tmpAxis.y=0;
			}
		}
	}
	
	public void OnPointerDown(PointerEventData eventData){

		if (_activated && !isOnTouch){
			axisX.axisState = ETCAxis.AxisState.Down;
			tmpAxis = eventData.delta;
			isOut = false;
			isOnTouch = true;
			pointId = eventData.pointerId;

			onTouchStart.Invoke();
		}
	}


	public void OnPointerUp(PointerEventData eventData){

		if (pointId == eventData.pointerId){
			isOnDrag = false;
			isOnTouch = false;
			tmpAxis = Vector2.zero;
			OldTmpAxis = Vector2.zero;
			
			axisX.axisState = ETCAxis.AxisState.None;
			axisY.axisState = ETCAxis.AxisState.None;
			
			if (!axisX.isEnertia && !axisY.isEnertia){
				axisX.ResetAxis();
				axisY.ResetAxis();
				onMoveEnd.Invoke();
			}
			
			onTouchUp.Invoke();


			if (previousDargObject){
				ExecuteEvents.Execute<IPointerUpHandler> (previousDargObject, eventData, ExecuteEvents.pointerUpHandler);
				previousDargObject = null;
			}

			pointId = -1;
		}

	}
	
	public void OnPointerExit(PointerEventData eventData){
		if (pointId == eventData.pointerId){
			if (!isSwipeOut){
				isOut = true;
				OnPointerUp( eventData);
			}
		}
	}
	#endregion

	#region Update TouchPad
	private void UpdateTouchPad(){

		#region Key simulation
		
		if (enableKeySimulation && !isOnTouch && _activated && _visible){
			isOnDrag = false;
			tmpAxis = Vector2.zero;
			
			if (Input.GetKey( axisX.positivekey)){
				isOnDrag = true;
				tmpAxis = new Vector2(1,tmpAxis.y);
			}
			else if (Input.GetKey( axisX.negativeKey)){
				isOnDrag = true;
				tmpAxis = new Vector2(-1,tmpAxis.y);
			}
			
			if (Input.GetKey( axisY.positivekey)){
				isOnDrag = true;
				tmpAxis = new Vector2(tmpAxis.x,1);
				
			}
			else if (Input.GetKey( axisY.negativeKey)){
				isOnDrag = true;
				tmpAxis = new Vector2(tmpAxis.x,-1);
			}
		}
		#endregion

		OldTmpAxis.x = axisX.axisValue;
		OldTmpAxis.y = axisY.axisValue;
		
		axisX.UpdateAxis( tmpAxis.x,isOnDrag,ETCBase.ControlType.DPad);
		axisY.UpdateAxis( tmpAxis.y,isOnDrag, ETCBase.ControlType.DPad);
		
		axisX.DoGravity();
		axisY.DoGravity();
		
		#region Move event
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
		if (OldTmpAxis.x == 0 && Mathf.Abs(axisX.axisValue)>0){
			
			
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
		if (OldTmpAxis.y==0 && Mathf.Abs(axisY.axisValue)>0 ){
			
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
		tmpAxis = Vector2.zero;
	}
	#endregion

	#region Private Method
	protected override void SetVisible (){
		if (Application.isPlaying){
			if (!_visible){
				cachedImage.color = new Color(0,0,0,0);
			}
			else{
				cachedImage.color = new Color(1,1,1,1);
			}
		}
	
	}
	#endregion
}
