using UnityEngine;
using UnityEditor;
using System.Collections;

public class ETCAxisInspector{

	public static void AxisInspector(ETCAxis axis, string label, ETCBase.ControlType type){

		EditorGUILayout.BeginHorizontal();
		//GUI.color = color;
		axis.enable = ETCGuiTools.Toggle(label +  " axis : ",axis.enable,true,125,true);
		//GUI.color = Color.white;
		axis.name =  EditorGUILayout.TextField("",axis.name,GUILayout.MinWidth(50));
		EditorGUILayout.EndHorizontal();

		if (axis.enable){

			EditorGUI.indentLevel++;

			axis.invertedAxis = ETCGuiTools.Toggle("Inverted axis",axis.invertedAxis,true);

			if (type == ETCBase.ControlType.Joystick ){
				axis.deadValue = EditorGUILayout.Slider("Dead lenght",axis.deadValue,0f,1f);
			}
		
			if (type == ETCBase.ControlType.Button || type == ETCBase.ControlType.DPad){
				axis.isValueOverTime = ETCGuiTools.Toggle("Value over the time",axis.isValueOverTime,true);
				if (axis.isValueOverTime){
					//EditorGUI.indentLevel--;
					ETCGuiTools.BeginGroup(5);{
						axis.overTimeStep = EditorGUILayout.FloatField("Step",axis.overTimeStep);
						axis.maxOverTimeValue = EditorGUILayout.FloatField("Max value",axis.maxOverTimeValue);
					}ETCGuiTools.EndGroup();
					//EditorGUI.indentLevel++;
				}
			}
	
			if (type == ETCBase.ControlType.Joystick ){
				axis.axisThreshold = EditorGUILayout.Slider("On/Off Threshold",axis.axisThreshold,0f,1f);
			}

			string labelspeed = "Speed";
			if (type== ETCBase.ControlType.TouchPad){
				labelspeed ="Sensitivity";
			}
			axis.speed = EditorGUILayout.FloatField(labelspeed,axis.speed);
			
			
			// Direct properties
			EditorGUILayout.Space();

				axis.actionOn = (ETCAxis.ActionOn)EditorGUILayout.EnumPopup("Action on",axis.actionOn );
				axis.directTransform = (Transform)EditorGUILayout.ObjectField("Direct action to",axis.directTransform,typeof(Transform),true);
				
				axis.directAction = (ETCAxis.DirectAction ) EditorGUILayout.EnumPopup( "Action",axis.directAction);
				axis.axisInfluenced = (ETCAxis.AxisInfluenced) EditorGUILayout.EnumPopup("Affected axis",axis.axisInfluenced);
				
				EditorGUILayout.Space();

			
			if ( axis.directCharacterController!=null){
				axis.gravity = EditorGUILayout.FloatField("Gravity",axis.gravity);
			}

			//if (type != ETCBase.ControlType.TouchPad){
			// Inertia
			axis.isEnertia = ETCGuiTools.Toggle("Enable inertia", axis.isEnertia,true);
				if (axis.isEnertia){
					EditorGUI.indentLevel--;
					ETCGuiTools.BeginGroup(20);{
						axis.inertia = EditorGUILayout.Slider("Inertia",axis.inertia,1f,500f);
						axis.inertiaThreshold = EditorGUILayout.FloatField("Threshold",axis.inertiaThreshold);
					}ETCGuiTools.EndGroup();
					EditorGUI.indentLevel++;
				}
				
				// AutoStab & clamp rotation
				if (axis.directAction == ETCAxis.DirectAction.RotateLocal ){
					//AutoStab
					axis.isAutoStab = ETCGuiTools.Toggle("Automatic stabilization",axis.isAutoStab,true);
					if (axis.isAutoStab){
						EditorGUI.indentLevel--;
						ETCGuiTools.BeginGroup(20);{
							axis.autoStabSpeed = EditorGUILayout.FloatField("Speed",axis.autoStabSpeed);
							axis.autoStabThreshold = EditorGUILayout.FloatField("Threshold ", axis.autoStabThreshold);
						}ETCGuiTools.EndGroup();
						EditorGUI.indentLevel++;
					}
					
					// clamp rotation
					axis.isClampRotation = ETCGuiTools.Toggle("Clamp rotation",axis.isClampRotation,true);
					if (axis.isClampRotation){
						EditorGUI.indentLevel--;
						ETCGuiTools.BeginGroup(20);{
							axis.maxAngle = EditorGUILayout.FloatField("Max angle",axis.maxAngle);	
							axis.minAngle = EditorGUILayout.FloatField("Min angle",axis.minAngle);
						}ETCGuiTools.EndGroup();
						EditorGUI.indentLevel++;
					}
					
				}
			//}

			EditorGUILayout.Space();
			axis.positivekey = (UnityEngine.KeyCode)EditorGUILayout.EnumPopup("Positive key",axis.positivekey);
			axis.negativeKey = (UnityEngine.KeyCode)EditorGUILayout.EnumPopup("Negative key",axis.negativeKey);
			EditorGUI.indentLevel--;
		}
	}
}
