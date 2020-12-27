/***********************************************
				EasyTouch Controls
	Copyright © 2014-2015 The Hedgehog Team
  http://www.blitz3dfr.com/teamtalk/index.php
		
	  The.Hedgehog.Team@gmail.com
		
**********************************************/
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEditor;


[CustomEditor(typeof(ETCJoystick))]
public class ETCJoystickInspector:Editor  {

	public override void OnInspectorGUI(){
		
		ETCJoystick t = (ETCJoystick)target;

			
		EditorGUILayout.Space();

		t.gameObject.name = EditorGUILayout.TextField("Joystick name",t.gameObject.name);
		t.activated = ETCGuiTools.Toggle("Activated",t.activated,true);
		t.visible = ETCGuiTools.Toggle("Visible",t.visible,true);
		t.useFixedUpdate = ETCGuiTools.Toggle("Use Fixed Update",t.useFixedUpdate,true);

		EditorGUILayout.Space();

		#region Type & Size
		t.showPSInspector = ETCGuiTools.BeginFoldOut( "Position & Size",t.showPSInspector);
		if (t.showPSInspector){
			ETCGuiTools.BeginGroup();{

				// Type
				t.joystickType = (ETCJoystick.JoystickType)EditorGUILayout.EnumPopup("Type",t.joystickType);

				if (t.joystickType == ETCJoystick.JoystickType.Static){
					t.anchor = (ETCBase.RectAnchor)EditorGUILayout.EnumPopup( "Anchor",t.anchor);
					if (t.anchor != ETCBase.RectAnchor.UserDefined){
						t.anchorOffet = EditorGUILayout.Vector2Field("Offset",t.anchorOffet);
					}

					t.IsNoOffsetThumb = ETCGuiTools.Toggle("No offset thumb",t.IsNoOffsetThumb,true);

				//	if (t.isNoOffsetThumb) t.isNoReturn = false;
					t.IsNoReturnThumb = ETCGuiTools.Toggle("No return of the thumb",t.IsNoReturnThumb,true);
				}
				else if( t.joystickType == ETCJoystick.JoystickType.Dynamic){
					t.anchor = ETCBase.RectAnchor.UserDefined;
					t.allowJoystickOverTouchPad = ETCGuiTools.Toggle("Allow over touchpad",t.allowJoystickOverTouchPad,true);
					t.joystickArea = (ETCJoystick.JoystickArea)EditorGUILayout.EnumPopup( "Joystick area",t.joystickArea);
					if (t.joystickArea == ETCJoystick.JoystickArea.UserDefined){
						t.userArea = (RectTransform)EditorGUILayout.ObjectField("User area",t.userArea,typeof(RectTransform),true);
					}
				}

				EditorGUILayout.Space();

				// Area sprite ratio
				Rect rect =  t.GetComponent<Image>().sprite.rect;
				float ratio = rect.width / rect.height;
				
				// Area Size
				if (ratio>=1){
					float s = EditorGUILayout.FloatField("Background size", t.rectTransform().rect.width);
					t.rectTransform().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal,s);
					t.rectTransform().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical,s/ratio);
				}
				else{
					float s = EditorGUILayout.FloatField("Background size", t.rectTransform().rect.height);
					t.rectTransform().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical,s);
					t.rectTransform().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal,s*ratio);
				}
				
				// Thumb sprite ratio
				rect = t.thumb.GetComponent<Image>().sprite.rect;
				ratio = rect.width / rect.height;
				
				// Thumb size
				if (ratio>=1){
					float s = EditorGUILayout.FloatField("Thumb size", t.thumb.rectTransform().rect.width);
					t.thumb.rectTransform().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal,s);
					t.thumb.rectTransform().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical,s/ratio);
				}
				else{
					float s = EditorGUILayout.FloatField("Thumb size", t.thumb.rectTransform().rect.height);
					t.thumb.rectTransform().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical,s);
					t.thumb.rectTransform().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal,s*ratio);
				}
				
				EditorGUILayout.Space();

				t.radiusBase = (ETCJoystick.RadiusBase)EditorGUILayout.EnumPopup("Radius based on",t.radiusBase );

				EditorGUILayout.Space();

	

			}ETCGuiTools.EndGroup();

		}
		#endregion

		#region Axes properties
		t.showAxesInspector = ETCGuiTools.BeginFoldOut( "Axes properties",t.showAxesInspector);
		if (t.showAxesInspector){
			ETCGuiTools.BeginGroup();{

				EditorGUILayout.Space();

				t.enableKeySimulation = ETCGuiTools.Toggle("Enable key simulation",t.enableKeySimulation,true);
				if (t.enableKeySimulation){
					t.allowSimulationStandalone = ETCGuiTools.Toggle("Allow simulation on standalone",t.allowSimulationStandalone,true);
				}
				EditorGUILayout.Space();

				ETCGuiTools.BeginGroup(5);{
					ETCAxisInspector.AxisInspector( t.axisX,"Horizontal",ETCBase.ControlType.Joystick);
				}ETCGuiTools.EndGroup();

				ETCGuiTools.BeginGroup(5);{
					ETCAxisInspector.AxisInspector( t.axisY,"Vertical" ,ETCBase.ControlType.Joystick);
				}ETCGuiTools.EndGroup();

			}ETCGuiTools.EndGroup();
		}
		#endregion

		#region sprites
		t.showSpriteInspector = ETCGuiTools.BeginFoldOut( "Sprites",t.showSpriteInspector);
		if (t.showSpriteInspector){
			ETCGuiTools.BeginGroup();{
				#region Background
				Sprite areaSprite = t.GetComponent<Image>().sprite;
				
				EditorGUILayout.BeginHorizontal();
				t.GetComponent<Image>().sprite = (Sprite)EditorGUILayout.ObjectField("Background",t.GetComponent<Image>().sprite,typeof(Sprite),true,GUILayout.MinWidth(100));
				t.GetComponent<Image>().color = EditorGUILayout.ColorField("",t.GetComponent<Image>().color,GUILayout.Width(50));
				EditorGUILayout.EndHorizontal();
				EditorGUILayout.Space();
				Rect spriteRect = new Rect( areaSprite.rect.x/ areaSprite.texture.width,
				                           areaSprite.rect.y/ areaSprite.texture.height,
				                           areaSprite.rect.width/ areaSprite.texture.width,
				                           areaSprite.rect.height/ areaSprite.texture.height);
				GUILayout.Space(8);
				Rect lastRect = GUILayoutUtility.GetLastRect();
				lastRect.x = 20;
				lastRect.width = 100;
				lastRect.height = 100;
				
				GUILayout.Space(100);
				
				ETCGuiTools.DrawTextureRectPreview( lastRect,spriteRect,t.GetComponent<Image>().sprite.texture,Color.white);
				#endregion
				EditorGUILayout.Space();
				#region thumb
				Sprite thumbSprite = t.thumb.GetComponent<Image>().sprite;
				
				EditorGUILayout.BeginHorizontal();
				t.thumb.GetComponent<Image>().sprite = (Sprite)EditorGUILayout.ObjectField("Thumb",t.thumb.GetComponent<Image>().sprite,typeof(Sprite),true,GUILayout.MinWidth(100));
				t.thumb.GetComponent<Image>().color = EditorGUILayout.ColorField("",t.thumb.GetComponent<Image>().color,GUILayout.Width(50));
				EditorGUILayout.EndHorizontal();
				
				spriteRect = new Rect( thumbSprite.rect.x/ thumbSprite.texture.width,
				                      thumbSprite.rect.y/ thumbSprite.texture.height,
				                      thumbSprite.rect.width/ thumbSprite.texture.width,
				                      thumbSprite.rect.height/ thumbSprite.texture.height);
				
				GUILayout.Space(8);
				lastRect = GUILayoutUtility.GetLastRect();
				lastRect.x = 20;
				lastRect.width = 100;
				lastRect.height = 100;
				
				GUILayout.Space(100);
				
				ETCGuiTools.DrawTextureRectPreview( lastRect,spriteRect,t.thumb.GetComponent<Image>().sprite.texture,Color.white);
				
				#endregion
			}ETCGuiTools.EndGroup();
		}
		#endregion

		#region Events
		t.showEventInspector = ETCGuiTools.BeginFoldOut( "Move Events",t.showEventInspector);
		if (t.showEventInspector){
			ETCGuiTools.BeginGroup();{

				serializedObject.Update();
				SerializedProperty moveStartEvent = serializedObject.FindProperty("onMoveStart");
				EditorGUILayout.PropertyField(moveStartEvent, true, null);
				serializedObject.ApplyModifiedProperties();

				serializedObject.Update();
				SerializedProperty moveEvent = serializedObject.FindProperty("onMove");
				EditorGUILayout.PropertyField(moveEvent, true, null);
				serializedObject.ApplyModifiedProperties();

				serializedObject.Update();
				SerializedProperty moveSpeedEvent = serializedObject.FindProperty("onMoveSpeed");
				EditorGUILayout.PropertyField(moveSpeedEvent, true, null);
				serializedObject.ApplyModifiedProperties();

				serializedObject.Update();
				SerializedProperty moveEndEvent = serializedObject.FindProperty("onMoveEnd");
				EditorGUILayout.PropertyField(moveEndEvent, true, null);
				serializedObject.ApplyModifiedProperties();

			}ETCGuiTools.EndGroup();
		}

		t.showTouchEventInspector = ETCGuiTools.BeginFoldOut( "Touch Events",t.showTouchEventInspector);
		if (t.showTouchEventInspector){
			ETCGuiTools.BeginGroup();{

				serializedObject.Update();
				SerializedProperty touchStartEvent = serializedObject.FindProperty("onTouchStart");
				EditorGUILayout.PropertyField(touchStartEvent, true, null);
				serializedObject.ApplyModifiedProperties();

				serializedObject.Update();
				SerializedProperty touchUpEvent = serializedObject.FindProperty("onTouchUp");
				EditorGUILayout.PropertyField(touchUpEvent, true, null);
				serializedObject.ApplyModifiedProperties();
			}ETCGuiTools.EndGroup();
		}

		t.showDownEventInspector = ETCGuiTools.BeginFoldOut( "Down Events",t.showDownEventInspector);
		if (t.showDownEventInspector){
			ETCGuiTools.BeginGroup();{

				serializedObject.Update();
				SerializedProperty downUpEvent = serializedObject.FindProperty("OnDownUp");
				EditorGUILayout.PropertyField(downUpEvent, true, null);
				serializedObject.ApplyModifiedProperties();

				serializedObject.Update();
				SerializedProperty downRightEvent = serializedObject.FindProperty("OnDownRight");
				EditorGUILayout.PropertyField(downRightEvent, true, null);
				serializedObject.ApplyModifiedProperties();

				serializedObject.Update();
				SerializedProperty downDownEvent = serializedObject.FindProperty("OnDownDown");
				EditorGUILayout.PropertyField(downDownEvent, true, null);
				serializedObject.ApplyModifiedProperties();

				serializedObject.Update();
				SerializedProperty downLeftEvent = serializedObject.FindProperty("OnDownLeft");
				EditorGUILayout.PropertyField(downLeftEvent, true, null);
				serializedObject.ApplyModifiedProperties();

			}ETCGuiTools.EndGroup();
		}

		t.showPressEventInspector = ETCGuiTools.BeginFoldOut( "Press Events",t.showPressEventInspector);
		if (t.showPressEventInspector){
			ETCGuiTools.BeginGroup();{

				serializedObject.Update();
				SerializedProperty pressUpEvent = serializedObject.FindProperty("OnPressUp");
				EditorGUILayout.PropertyField(pressUpEvent, true, null);
				serializedObject.ApplyModifiedProperties();

				serializedObject.Update();
				SerializedProperty pressRightEvent = serializedObject.FindProperty("OnPressRight");
				EditorGUILayout.PropertyField(pressRightEvent, true, null);
				serializedObject.ApplyModifiedProperties();

				serializedObject.Update();
				SerializedProperty pressDownEvent = serializedObject.FindProperty("OnPressDown");
				EditorGUILayout.PropertyField(pressDownEvent, true, null);
				serializedObject.ApplyModifiedProperties();

				serializedObject.Update();
				SerializedProperty pressLeftEvent = serializedObject.FindProperty("OnPressLeft");
				EditorGUILayout.PropertyField(pressLeftEvent, true, null);
				serializedObject.ApplyModifiedProperties();

			}ETCGuiTools.EndGroup();
		}

		#endregion

		if (GUI.changed){
			EditorUtility.SetDirty(t);
		}

		if (t.anchor != ETCBase.RectAnchor.UserDefined && t.joystickType == ETCJoystick.JoystickType.Static){
			t.SetAnchorPosition();
		}
	}
		
}

