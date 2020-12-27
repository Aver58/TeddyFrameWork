using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(ETCButton))]
public class ETCButtonInspector : Editor {

	public override void OnInspectorGUI(){
		
		ETCButton t = (ETCButton)target;
		
		
		EditorGUILayout.Space();
		

		t.gameObject.name = EditorGUILayout.TextField("Button name",t.gameObject.name);
		t.axis.name = t.gameObject.name;

		t.activated = ETCGuiTools.Toggle("Activated",t.activated,true);
		t.visible = ETCGuiTools.Toggle("Visible",t.visible,true);
		t.useFixedUpdate = ETCGuiTools.Toggle("Use Fixed Updae",t.useFixedUpdate,true);

		#region Position & Size
		t.showPSInspector = ETCGuiTools.BeginFoldOut( "Position & Size",t.showPSInspector);
		if (t.showPSInspector){
			ETCGuiTools.BeginGroup();{
				// Anchor
				t.anchor = (ETCBase.RectAnchor)EditorGUILayout.EnumPopup( "Anchor",t.anchor);
				if (t.anchor != ETCBase.RectAnchor.UserDefined){
					t.anchorOffet = EditorGUILayout.Vector2Field("Offset",t.anchorOffet);
				}

				EditorGUILayout.Space();
				
				// Area sprite ratio
				if (t.GetComponent<Image>().sprite != null){
					Rect rect =  t.GetComponent<Image>().sprite.rect;
					float ratio = rect.width / rect.height;
					
					// Area Size
					if (ratio>=1){
						float s = EditorGUILayout.FloatField("Size", t.rectTransform().rect.width);
						t.rectTransform().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal,s);
						t.rectTransform().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical,s/ratio);
					}
					else{
						float s = EditorGUILayout.FloatField("Size", t.rectTransform().rect.height);
						t.rectTransform().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical,s);
						t.rectTransform().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal,s*ratio);
					}
				}
			}ETCGuiTools.EndGroup();
		}
		#endregion

		#region Behaviour
		t.showBehaviourInspector = ETCGuiTools.BeginFoldOut( "Behaviour",t.showBehaviourInspector);
		if (t.showBehaviourInspector){
			ETCGuiTools.BeginGroup();{

				EditorGUILayout.Space();
				t.enableKeySimulation = ETCGuiTools.Toggle("Enable key simulation",t.enableKeySimulation,true);
				if (t.enableKeySimulation){
					t.allowSimulationStandalone = ETCGuiTools.Toggle("Allow simulation on standalone",t.allowSimulationStandalone,true);
				}
				EditorGUILayout.Space();

				t.isSwipeIn = ETCGuiTools.Toggle("Swipe in",t.isSwipeIn,true);
				t.isSwipeOut = ETCGuiTools.Toggle("Swipe out",t.isSwipeOut,true);

				t.axis.isValueOverTime = ETCGuiTools.Toggle("Value over the time",t.axis.isValueOverTime,true);
				if (t.axis.isValueOverTime){
					//EditorGUI.indentLevel--;
					ETCGuiTools.BeginGroup(5);{
						t.axis.overTimeStep = EditorGUILayout.FloatField("Step",t.axis.overTimeStep);
						t.axis.maxOverTimeValue = EditorGUILayout.FloatField("Max value",t.axis.maxOverTimeValue);
					}ETCGuiTools.EndGroup();
					//EditorGUI.indentLevel++;
				}

				EditorGUILayout.Space();

				t.axis.speed = EditorGUILayout.FloatField("Speed",t.axis.speed);

				EditorGUILayout.Space();

				t.axis.actionOn = (ETCAxis.ActionOn)EditorGUILayout.EnumPopup("Action on",t.axis.actionOn);
				t.axis.directTransform = (Transform)EditorGUILayout.ObjectField("Direct action to",t.axis.directTransform,typeof(Transform),true);
				
				t.axis.directAction = (ETCAxis.DirectAction ) EditorGUILayout.EnumPopup( "Action",t.axis.directAction);
				t.axis.axisInfluenced = (ETCAxis.AxisInfluenced) EditorGUILayout.EnumPopup("Affected axis",t.axis.axisInfluenced);

				EditorGUILayout.Space();
				t.axis.positivekey = (UnityEngine.KeyCode)EditorGUILayout.EnumPopup("Positive key",t.axis.positivekey);

			}ETCGuiTools.EndGroup();


		}
		#endregion

		#region Sprite
		t.showSpriteInspector = ETCGuiTools.BeginFoldOut( "Sprites",t.showSpriteInspector);
		if (t.showSpriteInspector){
			ETCGuiTools.BeginGroup();{

				// Normal state				
				EditorGUILayout.BeginHorizontal();
				EditorGUI.BeginChangeCheck ();
				t.normalSprite = (Sprite)EditorGUILayout.ObjectField("Normal",t.normalSprite,typeof(Sprite),true,GUILayout.MinWidth(100));
				t.normalColor = EditorGUILayout.ColorField("",t.normalColor,GUILayout.Width(50));
				if (EditorGUI.EndChangeCheck ()) {
					t.GetComponent<Image>().sprite = t.normalSprite;
					t.GetComponent<Image>().color = t.normalColor;
				}

				EditorGUILayout.EndHorizontal();
				EditorGUILayout.Space();

				if ( t.normalSprite){
					Rect spriteRect = new Rect( t.normalSprite.rect.x/ t.normalSprite.texture.width,
					                           t.normalSprite.rect.y/ t.normalSprite.texture.height,
					                           t.normalSprite.rect.width/ t.normalSprite.texture.width,
					                           t.normalSprite.rect.height/ t.normalSprite.texture.height);
					GUILayout.Space(8);
					Rect lastRect = GUILayoutUtility.GetLastRect();
					lastRect.x = 20;
					lastRect.width = 100;
					lastRect.height = 100;
					
					GUILayout.Space(100);
					
					ETCGuiTools.DrawTextureRectPreview( lastRect,spriteRect,t.normalSprite.texture,Color.white);
				}		

				// Press state
				EditorGUILayout.BeginHorizontal();
				t.pressedSprite = (Sprite)EditorGUILayout.ObjectField("Pressed",t.pressedSprite,typeof(Sprite),true,GUILayout.MinWidth(100));
				t.pressedColor = EditorGUILayout.ColorField("",t.pressedColor,GUILayout.Width(50));
				EditorGUILayout.EndHorizontal();
				EditorGUILayout.Space();

				if (t.pressedSprite){
					Rect spriteRect = new Rect( t.pressedSprite.rect.x/ t.pressedSprite.texture.width,
					                      t.pressedSprite.rect.y/ t.pressedSprite.texture.height,
					                      t.pressedSprite.rect.width/ t.pressedSprite.texture.width,
					                      t.pressedSprite.rect.height/ t.pressedSprite.texture.height);
					GUILayout.Space(8);
					Rect lastRect = GUILayoutUtility.GetLastRect();
					lastRect.x = 20;
					lastRect.width = 100;
					lastRect.height = 100;
					
					GUILayout.Space(100);
					
					ETCGuiTools.DrawTextureRectPreview( lastRect,spriteRect,t.pressedSprite.texture,Color.white);
				}

			}ETCGuiTools.EndGroup();
		}
		#endregion

		#region Events
		t.showEventInspector = ETCGuiTools.BeginFoldOut( "Events",t.showEventInspector);
		if (t.showEventInspector){
			ETCGuiTools.BeginGroup();{
				
				serializedObject.Update();
				SerializedProperty down = serializedObject.FindProperty("onDown");
				EditorGUILayout.PropertyField(down, true, null);
				serializedObject.ApplyModifiedProperties();
				
				serializedObject.Update();
				SerializedProperty press = serializedObject.FindProperty("onPressed");
				EditorGUILayout.PropertyField(press, true, null);
				serializedObject.ApplyModifiedProperties();
				
				serializedObject.Update();
				SerializedProperty pressTime = serializedObject.FindProperty("onPressedValue");
				EditorGUILayout.PropertyField(pressTime, true, null);
				serializedObject.ApplyModifiedProperties();

				serializedObject.Update();
				SerializedProperty up = serializedObject.FindProperty("onUp");
				EditorGUILayout.PropertyField(up, true, null);
				serializedObject.ApplyModifiedProperties();

			}ETCGuiTools.EndGroup();
		}
		#endregion

		if (GUI.changed){
			EditorUtility.SetDirty(t);
		}

		if (t.anchor != ETCBase.RectAnchor.UserDefined){
			t.SetAnchorPosition();
		}
	}
}
