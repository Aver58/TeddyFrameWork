#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MTAssets.EasyMinimapSystem
{
    /*
     This class is responsible for the functioning of the "Minimap Text" component, and all its functions.
    */
    /*
     * The Easy Minimap System was developed by Marcos Tomaz in 2019.
     * Need help? Contact me (mtassets@windsoft.xyz)
    */

    [AddComponentMenu("MT Assets/Easy Minimap System/Minimap Text")] //Add this component in a category of addComponent menu
    public class MinimapText : MonoBehaviour
    {
        //Private constants
        private const float BASE_HEIGHT_IN_3D_WORLD = 99001;

        //Private variables
        private GameObject minimapDataHolderObj;
        private MinimapDataHolder minimapDataHolder;
        private Transform minimapTextsHolder;
        private GameObject tempTextObj;
        private TextMesh tempText;
        private MeshRenderer tempTextRenderer;

        //Enums of script
        public enum FollowRotationOf
        {
            ThisGameObject,
            CustomGameObject
        }

        //Public variables
        [HideInInspector]
        public string textToRender = "Minimap Text";
        [HideInInspector]
        public int fontSize = 20;
        [HideInInspector]
        public Font font;
        [HideInInspector]
        public Color textColor = new Color(1, 1, 1, 1);
        [HideInInspector]
        public FollowRotationOf followRotationOf = FollowRotationOf.ThisGameObject;
        [HideInInspector]
        public Transform customGameObjectToFollowRotation = null;
        [HideInInspector]
        public float movementsSmoothing = 14;

        //The UI of this component
#if UNITY_EDITOR
        #region INTERFACE_CODE
        [UnityEditor.CustomEditor(typeof(MinimapText))]
        public class CustomInspector : UnityEditor.Editor
        {
            public override void OnInspectorGUI()
            {
                //Start the undo event support, draw default inspector and monitor of changes
                MinimapText script = (MinimapText)target;
                EditorGUI.BeginChangeCheck();
                Undo.RecordObject(target, "Undo Event");

                //Support reminder
                GUILayout.Space(10);
                EditorGUILayout.HelpBox("Remember to read the Easy Minimap System documentation to understand how to use it.\nGet support at: mtassets@windsoft.xyz", MessageType.None);

                //Now that all components are validated, execution continues. ----------]
                GUILayout.Space(10);

                //Start of settings
                EditorGUILayout.LabelField("Settings For Minimap Text", EditorStyles.boldLabel);
                GUILayout.Space(10);

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PrefixLabel(new GUIContent("Text To Render",
                        "The text you want to render on the minimap."));
                script.textToRender = EditorGUILayout.TextArea(script.textToRender, GUILayout.Height(40));
                EditorGUILayout.EndHorizontal();

                script.textColor = EditorGUILayout.ColorField(new GUIContent("Text Color",
                        "Text rendering color."),
                        script.textColor);

                script.font = (Font)EditorGUILayout.ObjectField(new GUIContent("Text Font",
                "Custom font for rendering."),
                script.font, typeof(Font), true, GUILayout.Height(16));

                script.fontSize = EditorGUILayout.IntField(new GUIContent("Font Size",
                "Size at which text will be displayed on the minimap. Adjust the size taking into account the size of the minimap camera's field of view. The larger the field of view, the smaller the text will look."),
                script.fontSize);

                GUILayout.Space(10);
                EditorGUILayout.LabelField("Settings For Movement", EditorStyles.boldLabel);
                GUILayout.Space(10);

                script.followRotationOf = (FollowRotationOf)EditorGUILayout.EnumPopup(new GUIContent("Follow Rotation Of",
                                   "Choose a GameObject for this Minimap Text to follow its rotation.\n\nThis GameObject - This minimap text will follow the rotation of this GameObject.\n\nCustom GameObject - This minimap text will follow the rotation of another GameObject of your choice."),
                                   script.followRotationOf);
                if (script.followRotationOf == FollowRotationOf.CustomGameObject)
                {
                    EditorGUI.indentLevel += 1;
                    script.customGameObjectToFollowRotation = (Transform)EditorGUILayout.ObjectField(new GUIContent("GameObject To Follow",
                        "This minimap text will follow the rotation of this GameObject."),
                        script.customGameObjectToFollowRotation, typeof(Transform), true, GUILayout.Height(16));
                    EditorGUI.indentLevel -= 1;
                }

                script.movementsSmoothing = EditorGUILayout.Slider(new GUIContent("Movement Smoothing",
                                "The speed at which this Minimap Item will follow GameObjects.\n\nThe higher the smoothing value, the faster this Item will rotate/move to the destination direction."), script.movementsSmoothing, 1f, 100f);

                //Final space
                GUILayout.Space(10);

                //Apply changes on script, case is not playing in editor
                if (GUI.changed == true && Application.isPlaying == false)
                {
                    EditorUtility.SetDirty(script);
                    UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(script.gameObject.scene);
                }
                if (EditorGUI.EndChangeCheck() == true)
                {

                }
            }

            protected virtual void OnSceneGUI()
            {
                //Get this script
                MinimapText script = (MinimapText)target;

                //Show the text
                GUIStyle style = new GUIStyle();
                style.normal.textColor = script.textColor;
                style.alignment = TextAnchor.MiddleCenter;
                style.fontStyle = FontStyle.Bold;
                style.fontSize = 25;
                style.contentOffset = new Vector2(-script.textToRender.Length * 4f, 0);
                Handles.Label(new Vector3(script.transform.position.x, script.transform.position.y, script.transform.position.z), script.textToRender, style);
            }
        }
        #endregion
#endif

        //Core methods

        void Awake()
        {
            //Create the holder, if not exists
            minimapDataHolderObj = GameObject.Find("Minimap Data Holder");
            if (minimapDataHolderObj == null)
            {
                minimapDataHolderObj = new GameObject("Minimap Data Holder");
                minimapDataHolder = minimapDataHolderObj.AddComponent<MinimapDataHolder>();
            }
            if (minimapDataHolderObj != null)
                minimapDataHolder = minimapDataHolderObj.GetComponent<MinimapDataHolder>();
            minimapTextsHolder = minimapDataHolderObj.transform.Find("Minimap Texts Holder");
            if (minimapTextsHolder == null)
            {
                GameObject obj = new GameObject("Minimap Texts Holder");
                minimapTextsHolder = obj.transform;
                minimapTextsHolder.SetParent(minimapDataHolderObj.transform);
                minimapTextsHolder.localPosition = Vector3.zero;
                minimapTextsHolder.localEulerAngles = Vector3.zero;
            }
            if (minimapDataHolder.instancesOfMinimapTextInThisScene.Contains(this) == false)
                minimapDataHolder.instancesOfMinimapTextInThisScene.Add(this);

            //Create the minimap text
            tempTextObj = new GameObject("Minimap Text (" + this.gameObject.transform.name + ")");
            tempTextObj.transform.SetParent(minimapTextsHolder);
            tempTextObj.transform.position = new Vector3(this.gameObject.transform.position.x, BASE_HEIGHT_IN_3D_WORLD, this.gameObject.transform.position.z);
            tempText = tempTextObj.AddComponent<TextMesh>();
            tempTextObj.layer = LayerMask.NameToLayer("UI");
            tempText.characterSize = 1;
            tempText.anchor = TextAnchor.MiddleCenter;
            tempText.alignment = TextAlignment.Center;
            tempText.fontStyle = FontStyle.Bold;
            tempTextRenderer = tempText.GetComponent<MeshRenderer>();

            //Add the activity monitor to the camera
            ActivityMonitor activeMonitor = tempTextObj.AddComponent<ActivityMonitor>();
            activeMonitor.responsibleScriptComponentForThis = this;
        }

        void Update()
        {
            //If the Minimap Item created by this component is disabled, enable it
            if (tempTextObj.activeSelf == false)
                tempTextObj.SetActive(true);

            //Change the text of text mesh, if text to render is different
            if (string.Equals(textToRender, tempText.text) == false)
                tempText.text = textToRender;

            //Update the font size, if is different
            if ((int)(fontSize * MinimapDataGlobal.GetMinimapItemsSizeGlobalMultiplier()) != tempText.fontSize)
                tempText.fontSize = (int)(fontSize * MinimapDataGlobal.GetMinimapItemsSizeGlobalMultiplier());

            //Update the font if changed
            if (font != tempText.font)
                tempText.font = font;

            //Update the text color if changed
            if (textColor != tempText.color)
                tempText.color = textColor;

            //Move the camera to follow this gameobject
            tempTextObj.transform.position = Vector3.Lerp(tempTextObj.transform.position, new Vector3(this.gameObject.transform.position.x, BASE_HEIGHT_IN_3D_WORLD, this.gameObject.transform.position.z), movementsSmoothing * Time.deltaTime);
            //Rotate the camera
            if (followRotationOf == FollowRotationOf.ThisGameObject)
                tempTextObj.transform.rotation = Quaternion.Lerp(tempTextObj.transform.rotation, Quaternion.Euler(90, this.gameObject.transform.rotation.eulerAngles.y, 0), movementsSmoothing * Time.deltaTime);
            if (followRotationOf == FollowRotationOf.CustomGameObject && customGameObjectToFollowRotation != null)
                tempTextObj.transform.rotation = Quaternion.Lerp(tempTextObj.transform.rotation, Quaternion.Euler(90, customGameObjectToFollowRotation.rotation.eulerAngles.y, 0), movementsSmoothing * Time.deltaTime);
        }

        //Public methods

        public bool isThisMinimapTextBeingVisibleByAnyMinimapCamera()
        {
            //This method will return true if this Minimap Text is being visualized by any minimap camera
            bool isVisible = false;
            if (tempTextRenderer.isVisible == true)
                isVisible = true;
            return isVisible;
        }

        public TextMesh GetGeneratedTextMeshAtRunTime()
        {
            //Return the text generated at runtime by this component
            return tempText;
        }

        public MinimapText[] GetListOfAllMinimapTextsInThisScene()
        {
            //If is not playing, cancel
            if (Application.isPlaying == false)
            {
                Debug.LogError("It is only possible to obtain the list of Minimap Texts in this scene, if the application is being executed.");
                return null;
            }

            //Return a list that contains reference to all of this component in this scene
            return minimapDataHolder.instancesOfMinimapTextInThisScene.ToArray();
        }
    }
}