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
public class ETCButton : ETCBase, IPointerEnterHandler, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler { 

	#region Unity Events
	[System.Serializable] public class OnDownHandler : UnityEvent{}
	[System.Serializable] public class OnPressedHandler : UnityEvent{}
	[System.Serializable] public class OnPressedValueandler : UnityEvent<float>{}
	[System.Serializable] public class OnUPHandler : UnityEvent{}

	[SerializeField] public OnDownHandler onDown;
	[SerializeField] public OnPressedHandler onPressed;
	[SerializeField] public OnPressedValueandler onPressedValue;
	[SerializeField] public OnUPHandler onUp;
	#endregion

	#region Members

	#region Public members
	public ETCAxis axis;

	public Sprite normalSprite;
	public Color normalColor;

	public Sprite pressedSprite;
	public Color pressedColor;	
	#endregion

	#region Private members
	private Image cachedImage; 
	private bool isOnPress;
	private GameObject previousDargObject;
	private bool isOnTouch;
	#endregion

	#endregion

	#region Constructor
	public ETCButton(){

		axis = new ETCAxis( "Button");
		_visible = true;
		_activated = true;
		isOnTouch = false;

		enableKeySimulation = true;
		#if !UNITY_EDITOR
			enableKeySimulation = false;
		#endif

		axis.positivekey = KeyCode.Space;

		showPSInspector = true; 
		showSpriteInspector = false;
		showBehaviourInspector = false;
		showEventInspector = false;
	}
	#endregion

	#region Monobehaviour Callback
	protected override void Awake (){
		base.Awake ();

		cachedImage = GetComponent<UnityEngine.UI.Image>();

	}

	void Start(){
		isOnPress = false;
	}


	protected override void UpdateControlState ()
	{
		UpdateButton();
	}
	#endregion

	#region UI Callback
	public void OnPointerEnter(PointerEventData eventData){

		if (isSwipeIn && !isOnTouch){

			if (eventData.pointerDrag != null){
				if (eventData.pointerDrag.GetComponent<ETCBase>() && eventData.pointerDrag!= gameObject){
					previousDargObject=eventData.pointerDrag;
					//ExecuteEvents.Execute<IPointerUpHandler> (previousDargObject, eventData, ExecuteEvents.pointerUpHandler);
				}
			}

			eventData.pointerDrag = gameObject;
			eventData.pointerPress = gameObject;
			OnPointerDown( eventData);
		}
	}

	public void OnPointerDown(PointerEventData eventData){

		if (_activated && !isOnTouch){
			pointId = eventData.pointerId;

			axis.ResetAxis();
			axis.axisState = ETCAxis.AxisState.Down;

			isOnPress = false;
			isOnTouch = true;

			onDown.Invoke();
			ApllyState();
		}
	}

	public void OnPointerUp(PointerEventData eventData){
		if (pointId == eventData.pointerId){
			isOnPress = false;
			isOnTouch = false;
			axis.axisState = ETCAxis.AxisState.Up;
			axis.axisValue = 0;
			onUp.Invoke();
			ApllyState();

			if (previousDargObject){
				ExecuteEvents.Execute<IPointerUpHandler> (previousDargObject, eventData, ExecuteEvents.pointerUpHandler);
				previousDargObject = null;
			}
		}
	}

	public void OnPointerExit(PointerEventData eventData){
		if (pointId == eventData.pointerId){
			if (axis.axisState == ETCAxis.AxisState.Press && !isSwipeOut){
				OnPointerUp(eventData);
			}
		}
	}
	#endregion

	#region Button Update
	private void UpdateButton(){

		if (axis.axisState == ETCAxis.AxisState.Down){
			isOnPress = true;
			axis.axisState = ETCAxis.AxisState.Press;
		}

		if (isOnPress){
			axis.UpdateButton();
			onPressed.Invoke();
			onPressedValue.Invoke( axis.axisValue);

		}

		if (axis.axisState == ETCAxis.AxisState.Up){
			isOnPress = false;
			axis.axisState = ETCAxis.AxisState.None;
		}


		if (enableKeySimulation && _activated && _visible && !isOnTouch){
			
			
			if (Input.GetKey( axis.positivekey) && axis.axisState ==ETCAxis.AxisState.None  ){
				axis.axisState = ETCAxis.AxisState.Down;
			}
			
			if (!Input.GetKey(axis.positivekey) && axis.axisState == ETCAxis.AxisState.Press){
				axis.axisState = ETCAxis.AxisState.Up;
				onUp.Invoke();
			}
		}

	}	
	#endregion

	#region Private Method
	protected override void SetVisible (){
		GetComponent<Image>().enabled = _visible;
	}

	private void ApllyState(){

		switch (axis.axisState){
		case ETCAxis.AxisState.Down:
			case ETCAxis.AxisState.Press:
				cachedImage.sprite = pressedSprite;
				cachedImage.color = pressedColor;
				break;
			default:
				cachedImage.sprite = normalSprite;
				cachedImage.color = normalColor;
				break;
		}


	
	}
	#endregion
}
