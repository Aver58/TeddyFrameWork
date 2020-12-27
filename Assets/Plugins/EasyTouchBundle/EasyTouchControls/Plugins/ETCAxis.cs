/***********************************************
				EasyTouch Controls
	Copyright © 2014-2015 The Hedgehog Team
  http://www.blitz3dfr.com/teamtalk/index.php
		
	  The.Hedgehog.Team@gmail.com
		
**********************************************/

using UnityEngine;
using System.Collections;

[System.Serializable]
public class ETCAxis {

	#region Enumeration
	public enum DirectAction {Rotate, RotateLocal,Translate, TranslateLocal, Scale, Force,RelativeForce, Torque,RelativeTorque};
	public enum AxisInfluenced{X,Y,Z};
	public enum AxisRange {Classical, Positif};
	public enum AxisState {None,Down,Press,Up, DownUp,DownDown,DownLeft,DownRight, PressUp, PressDown, PressLeft, PressRight};
	public enum ActionOn {Down,Press};
	#endregion

	#region Members
	public string name;

	public bool enable;
	public AxisRange range;
	public bool invertedAxis;
	public float speed;
	public float deadValue;

	public bool isEnertia;
	public float inertia;
	public float inertiaThreshold;

	// Auto stabilization
	public bool isAutoStab;
	public float autoStabThreshold;
	public float autoStabSpeed;
	private float startAngle;

	// Clamp rotation
	public bool isClampRotation;
	public float maxAngle;
	public float minAngle;

	// time push
	public bool isValueOverTime;
	public float overTimeStep;
	public float maxOverTimeValue;

	// AvisValue
	public float axisValue;
	public float axisSpeedValue;
	public float axisThreshold;

	public AxisState axisState;

	[SerializeField]
	private Transform _directTransform;
	public Transform directTransform {
		get {
			return _directTransform;
		}

		set {
			_directTransform = value;
			if (_directTransform!=null){
				directCharacterController = _directTransform.GetComponent<CharacterController>();
				directRigidBody = _directTransform.GetComponent<Rigidbody>();
			}
			else{
				directCharacterController=null;	
			}

		}
	}
	
	public DirectAction directAction;
	public AxisInfluenced axisInfluenced;
	public ActionOn actionOn;

	public CharacterController directCharacterController;
	public Rigidbody directRigidBody;
	public float gravity;

	public KeyCode positivekey;
	public KeyCode negativeKey;
	#endregion

	#region Constructeur
	public ETCAxis(string axisName){
		name = axisName;
		enable = true;
		range = AxisRange.Classical;
		speed = 15;
		invertedAxis = false;
		isEnertia = false;
		inertia = 0;
		inertiaThreshold = 0.08f;
		axisValue = 0;
		axisSpeedValue = 0;
		gravity = 0;
		isAutoStab = false;
		autoStabThreshold = 0.01f;
		autoStabSpeed = 10;
		maxAngle = 90;
		minAngle = 90;
		axisState = AxisState.None;
		maxOverTimeValue = 1;
		overTimeStep = 1;
		isValueOverTime = false;
		axisThreshold = 0.5f;
		deadValue = 0.1f;
		actionOn = ActionOn.Press;
	}
	#endregion

	#region Public method
	public void InitAxis(){
		startAngle = GetAngle();
	}

	public void UpdateAxis(float realValue, bool isOnDrag, ETCBase.ControlType type,bool deltaTime=true){


		// Auto stabilization
		if (isAutoStab && axisValue ==0){
			DoAutoStabilisation();
		}

		if (invertedAxis){realValue *= -1;}

		// Time push
		if (isValueOverTime && realValue!=0){

			axisValue += overTimeStep * Mathf.Sign(realValue ) * Time.deltaTime;

			if (Mathf.Sign(axisValue )>0){
				axisValue = Mathf.Clamp( axisValue,0,maxOverTimeValue);
			}
			else{
				axisValue = Mathf.Clamp( axisValue,-maxOverTimeValue,0);
			}
		}

		// Axis value
		ComputAxisValue(realValue, type,isOnDrag,deltaTime );

	}

	public void UpdateButton(){

		if (isValueOverTime){
			axisValue += overTimeStep * Time.deltaTime;
			axisValue = Mathf.Clamp( axisValue,0,maxOverTimeValue);
		}
		else{
			if (axisState == AxisState.Press || axisState == AxisState.Down){
				axisValue = 1;
			}
			else{
				axisValue = 0;
			}
		}
		axisSpeedValue = axisValue * speed * Time.deltaTime;

		switch (actionOn){
			case ActionOn.Down:
				if (axisState == AxisState.Down){
					DoDirectAction();
				}
				break;
			case ActionOn.Press:
				if (axisState == AxisState.Press){
					DoDirectAction();
				}
				break;
			}
	}

	public void ResetAxis(){
		if (!isEnertia || (isEnertia && Mathf.Abs(axisValue)<inertiaThreshold)  ){
			axisValue =0;
			axisSpeedValue =0;
		}
	}
	
	public void DoDirectAction(){
	
		if (directTransform){
			Vector3 localAxis = GetInfluencedAxis();

			switch (  directAction){
			case ETCAxis.DirectAction.Rotate:
				directTransform.Rotate( localAxis *  axisSpeedValue, Space.World);
				break;
				
			case ETCAxis.DirectAction.RotateLocal:
				directTransform.Rotate( localAxis *  axisSpeedValue,Space.Self);
				break;
				
				
			case ETCAxis.DirectAction.Translate:
				if ( directCharacterController==null){
					directTransform.Translate(localAxis *  axisSpeedValue,Space.World);
				}
				else{
					Vector3 direction = localAxis *  axisSpeedValue;
					directCharacterController.Move( direction  );
				}
				break;
				
				
			case ETCAxis.DirectAction.TranslateLocal:
				if ( directCharacterController==null){
					directTransform.Translate(localAxis *  axisSpeedValue,Space.Self);
				}
				else{
					Vector3 direction =  directCharacterController.transform.TransformDirection(localAxis) *  axisSpeedValue;
					directCharacterController.Move( direction );
				}
				break;	
				
			case ETCAxis.DirectAction.Scale:
				directTransform.localScale +=  localAxis *  axisSpeedValue;
				break;

			case ETCAxis.DirectAction.Force:
				if (directRigidBody!=null){
					directRigidBody.AddForce( localAxis * axisValue * speed);
				}
				else{
					Debug.LogWarning("ETCAxis : "+ name + " No rigidbody on gameobject : "+ _directTransform.name); 
				}
				break;

			case ETCAxis.DirectAction.RelativeForce:
				if (directRigidBody!=null){
					directRigidBody.AddRelativeForce( localAxis * axisValue * speed);
				}
				else{
					Debug.LogWarning("ETCAxis : "+ name + " No rigidbody on gameobject : "+ _directTransform.name); 
				}
				break;

			case ETCAxis.DirectAction.Torque:
				if (directRigidBody!=null){
					directRigidBody.AddTorque(localAxis * axisValue * speed);
				}
				else{
					Debug.LogWarning("ETCAxis : "+ name + " No rigidbody on gameobject : "+ _directTransform.name); 
				}
				break;

			case ETCAxis.DirectAction.RelativeTorque:
				if (directRigidBody!=null){
					directRigidBody.AddRelativeTorque(localAxis * axisValue * speed);
				}
				else{
					Debug.LogWarning("ETCAxis : "+ name + " No rigidbody on gameobject : "+ _directTransform.name); 
				}
				break;
			}
		}

		if (isClampRotation &&  directAction == DirectAction.RotateLocal){
			DoAngleLimitation();
		}

	}

	public void DoGravity(){
		if (directCharacterController != null && gravity!=0){
			directCharacterController.Move( Vector3.down*gravity*Time.deltaTime);	
		}
	}


	#endregion

	#region Private methods

	private void ComputAxisValue(float realValue, ETCBase.ControlType type, bool isOnDrag, bool deltaTime){

		if (enable){

			if (type == ETCBase.ControlType.Joystick){
				float dist = Mathf.Max(Mathf.Abs(realValue),0.001f);
				float dead = Mathf.Max(dist - deadValue, 0)/(1f - deadValue)/dist;
				realValue *= dead;
			}

			if (isEnertia){
				realValue = (realValue-axisValue);
				realValue /= inertia;

				axisValue += realValue;

				if (Mathf.Abs(axisValue)< inertiaThreshold && !isOnDrag ) {
					axisValue = 0;
				}
			}
			else if (!isValueOverTime || (isValueOverTime && realValue ==0)){
				axisValue = realValue;
			}

			if (deltaTime){
				axisSpeedValue = axisValue * speed * Time.deltaTime;
			}
			else{
				axisSpeedValue = axisValue * speed;
			}
		}
		else{
			axisValue = 0;
			axisSpeedValue =0;
		}
	}
		
	private Vector3 GetInfluencedAxis(){
		
		Vector3 axis = Vector3.zero;
		
		switch(axisInfluenced){
		case ETCAxis.AxisInfluenced.X:
			axis = Vector3.right;
			break;
		case ETCAxis.AxisInfluenced.Y:
			axis = Vector3.up;
			break;
		case ETCAxis.AxisInfluenced.Z:
			axis = Vector3.forward;
			break;
		}	
		
		return axis;
	}

	private float GetAngle(){
		
		float angle=0;
		
		if (_directTransform!=null){
			switch(axisInfluenced){
				case AxisInfluenced.X:
					angle = _directTransform.localRotation.eulerAngles.x;
					break;
				case AxisInfluenced.Y:
					angle = _directTransform.localRotation.eulerAngles.y;
					break;
				case AxisInfluenced.Z:
					angle = _directTransform.localRotation.eulerAngles.z;
					break;			
			}	
			
			if (angle<=360 && angle>=180){
				angle = angle -360;	
			}
		}

		return angle;
	}

	private void DoAutoStabilisation(){
		
		float angle= GetAngle();

		if (angle<=360 && angle>=180){
			angle = angle -360;	
		}		

		if (angle > startAngle - autoStabThreshold || angle < startAngle + autoStabThreshold){
			
			float axis=0;
			Vector3 stabAngle = Vector3.zero;
			
			if (angle > startAngle - autoStabThreshold){
				axis = angle + autoStabSpeed/100f*Mathf.Abs (angle-startAngle) * Time.deltaTime*-1;
			}
			
			
			if (angle < startAngle + autoStabThreshold){
				axis = angle + autoStabSpeed/100f*Mathf.Abs (angle-startAngle) * Time.deltaTime;
			}
			
			switch(axisInfluenced){
			case AxisInfluenced.X:	
				stabAngle = new Vector3(axis,_directTransform.localRotation.eulerAngles.y,_directTransform.localRotation.eulerAngles.z);
				break;
			case AxisInfluenced.Y:	
				stabAngle = new Vector3(_directTransform.localRotation.eulerAngles.x,axis,_directTransform.localRotation.eulerAngles.z);
				break;
			case AxisInfluenced.Z:	
				stabAngle = new Vector3(_directTransform.localRotation.eulerAngles.x,_directTransform.localRotation.eulerAngles.y,axis);
				break;
			}
			
			_directTransform.localRotation  = Quaternion.Euler( stabAngle);	
		}
	}

	private void DoAngleLimitation(){
		
		float angle= GetAngle();

		angle = Mathf.Clamp (angle, -minAngle, maxAngle);

		switch(axisInfluenced){
		case AxisInfluenced.X:
			_directTransform.localEulerAngles = new Vector3( angle,_directTransform.localEulerAngles.y, _directTransform.localEulerAngles.z);
			break;	
		case AxisInfluenced.Y:
			_directTransform.localEulerAngles = new Vector3( _directTransform.localEulerAngles.x,angle, _directTransform.localEulerAngles.z);
			break;
		case AxisInfluenced.Z:
			_directTransform.localEulerAngles = new Vector3( _directTransform.localEulerAngles.x,_directTransform.localEulerAngles.y,angle);
			break;	
			
		}
		
	}
	#endregion
}
