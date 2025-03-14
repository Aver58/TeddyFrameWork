#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MTAssets.EasyMinimapSystem
{
    /*
     This class is responsible for the functioning of the "Minimap Line" component, and all its functions.
    */
    /*
     * The Easy Minimap System was developed by Marcos Tomaz in 2019.
     * Need help? Contact me (mtassets@windsoft.xyz)
    */

    [AddComponentMenu("MT Assets/Easy Minimap System/Minimap Line")] //Add this component in a category of addComponent menu
    public class MinimapLine : MonoBehaviour
    {
        //Private constants
        private const float BASE_HEIGHT_IN_3D_WORLD = 99003;

        //Private variables
        private GameObject minimapDataHolderObj;
        private MinimapDataHolder minimapDataHolder;
        private Transform minimapLinesHolder;
        private GameObject tempLineObj;
        private LineRenderer tempLine;

        //Private cache variables
        private bool alreadySettedDefaultMaterialOnLineRenderer = false;
        private bool updateLinesPositionsRenderNow = true;

        //Public variables
        [HideInInspector]
        public Color lineRenderColor = Color.white;
        [HideInInspector]
        public float lineWidthSize = 1.0f;
        [HideInInspector]
        public bool roundLineCorners = false;
        [HideInInspector]
        [SerializeField]
        private Vector3[] _listOfLinePositions = new Vector3[0];
        [HideInInspector]
        public float movementsSmoothing = 14;

        //The UI of this component
#if UNITY_EDITOR
        #region INTERFACE_CODE
        [UnityEditor.CustomEditor(typeof(MinimapLine))]
        public class CustomInspector : UnityEditor.Editor
        {
            Vector2 listOfPositions_ScrollPos;

            public override void OnInspectorGUI()
            {
                //Start the undo event support, draw default inspector and monitor of changes
                MinimapLine script = (MinimapLine)target;
                EditorGUI.BeginChangeCheck();
                Undo.RecordObject(target, "Undo Event");

                //If not have positions, create two default positions
                if (script._listOfLinePositions.Length == 0)
                {
                    Vector3[] tempArray = new Vector3[2];
                    tempArray[0] = script.gameObject.transform.position;
                    tempArray[1] = script.gameObject.transform.position;
                    tempArray[1] = new Vector3(tempArray[1].x + 10.0f, tempArray[1].y, tempArray[1].z + 10.0f);
                    script._listOfLinePositions = tempArray;
                }

                //Support reminder
                GUILayout.Space(10);
                EditorGUILayout.HelpBox("Remember to read the Easy Minimap System documentation to understand how to use it.\nGet support at: mtassets@windsoft.xyz", MessageType.None);

                //Now that all components are validated, execution continues. ----------]
                GUILayout.Space(10);

                //Start of settings
                EditorGUILayout.LabelField("Settings For Minimap Line", EditorStyles.boldLabel);
                GUILayout.Space(10);

                script.lineRenderColor = EditorGUILayout.ColorField(new GUIContent("Line Render Color",
                        "The color in which the line will be rendered."),
                        script.lineRenderColor);

                script.lineWidthSize = EditorGUILayout.Slider(new GUIContent("Line Width Size",
                        "The width over which the line will be rendered on the map."),
                        script.lineWidthSize, 0.5f, 100.0f);

                script.roundLineCorners = EditorGUILayout.Toggle(new GUIContent("Round Line Corners",
                        "Check this box if you want the line to have rounded corners."),
                        script.roundLineCorners);

                GUILayout.Space(10);
                EditorGUILayout.LabelField("Line World Positions", EditorStyles.boldLabel);
                GUILayout.Space(10);

                if (Application.isPlaying == false)
                {
                    Texture2D removeItemIcon = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/MT Assets/Easy Minimap System/Editor/Images/Remove.png", typeof(Texture2D));
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("List Of Line Positions", GUILayout.Width(145));
                    GUILayout.Space(MTAssetsEditorUi.GetInspectorWindowSize().x - 145);
                    EditorGUILayout.LabelField("Size", GUILayout.Width(30));
                    EditorGUILayout.IntField(script._listOfLinePositions.Length, GUILayout.Width(50));
                    EditorGUILayout.EndHorizontal();
                    GUILayout.BeginVertical("box");
                    listOfPositions_ScrollPos = EditorGUILayout.BeginScrollView(listOfPositions_ScrollPos, GUIStyle.none, GUI.skin.verticalScrollbar, GUILayout.Width(MTAssetsEditorUi.GetInspectorWindowSize().x), GUILayout.Height(100));
                    if (script._listOfLinePositions.Length == 0)
                        EditorGUILayout.HelpBox("Oops! No Positions was registered! If you want to subscribe any, click the button below!", MessageType.Info);
                    if (script._listOfLinePositions.Length > 0)
                        for (int i = 0; i < script._listOfLinePositions.Length; i++)
                        {
                            GUILayout.BeginHorizontal();
                            if (GUILayout.Button(removeItemIcon, GUILayout.Width(25), GUILayout.Height(16)))
                            {
                                List<Vector3> tempList = new List<Vector3>();
                                for (int x = 0; x < script._listOfLinePositions.Length; x++)
                                    tempList.Add(script._listOfLinePositions[x]);
                                tempList.RemoveAt(i);
                                script._listOfLinePositions = tempList.ToArray();
                            }
                            script._listOfLinePositions[i] = EditorGUILayout.Vector3Field(new GUIContent("Vector " + i.ToString(), "This Vector3 represents a position that the line will pass through. Click the button to the left if you want to remove this Vector3 from the list."), script._listOfLinePositions[i]);
                            GUILayout.EndHorizontal();
                        }
                    EditorGUILayout.EndScrollView();
                    GUILayout.EndVertical();
                    GUILayout.BeginHorizontal();
                    if (GUILayout.Button("Add New Slot"))
                    {
                        Vector3 lastPosition = script._listOfLinePositions[script._listOfLinePositions.Length - 1];
                        List<Vector3> tempList = new List<Vector3>();
                        for (int x = 0; x < script._listOfLinePositions.Length; x++)
                            tempList.Add(script._listOfLinePositions[x]);
                        tempList.Add(new Vector3(lastPosition.x + 1.0f, lastPosition.y, lastPosition.z + 1.0f));
                        script._listOfLinePositions = tempList.ToArray();
                        listOfPositions_ScrollPos.y += 999999;
                    }
                    if (script._listOfLinePositions.Length > 0)
                        if (GUILayout.Button("Remove Zero Slots", GUILayout.Width(Screen.width * 0.48f)))
                            for (int i = script._listOfLinePositions.Length - 1; i >= 0; i--)
                                if (script._listOfLinePositions[i] == Vector3.zero)
                                {
                                    List<Vector3> tempList = new List<Vector3>();
                                    for (int x = 0; x < script._listOfLinePositions.Length; x++)
                                        tempList.Add(script._listOfLinePositions[x]);
                                    tempList.RemoveAt(i);
                                    script._listOfLinePositions = tempList.ToArray();
                                }
                    GUILayout.EndHorizontal();
                }
                if (Application.isPlaying == true)
                    EditorGUILayout.HelpBox("Editing Positions of the World, along these Line, can only be done through the C# API while the application is running.", MessageType.Info);

                GUILayout.Space(10);
                EditorGUILayout.LabelField("Settings For Movement", EditorStyles.boldLabel);
                GUILayout.Space(10);

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
        }

        public void OnDrawGizmosSelected()
        {
            //Set color of gizmos
            Gizmos.color = lineRenderColor;
            Handles.color = lineRenderColor;

            //Get the current position
            Vector3 position = this.gameObject.transform.position;

            //If not have a transform to connect, cancel
            if (_listOfLinePositions.Length == 0)
                return;

            //Draw the line
            Handles.DrawAAPolyLine(8f, _listOfLinePositions);
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
            minimapLinesHolder = minimapDataHolderObj.transform.Find("Minimap Lines Holder");
            if (minimapLinesHolder == null)
            {
                GameObject obj = new GameObject("Minimap Lines Holder");
                minimapLinesHolder = obj.transform;
                minimapLinesHolder.SetParent(minimapDataHolderObj.transform);
                minimapLinesHolder.localPosition = Vector3.zero;
                minimapLinesHolder.localEulerAngles = Vector3.zero;
            }
            if (minimapDataHolder.instancesOfMinimapLineInThisScene.Contains(this) == false)
                minimapDataHolder.instancesOfMinimapLineInThisScene.Add(this);

            //Create the minimap text
            tempLineObj = new GameObject("Minimap Line (" + this.gameObject.transform.name + ")");
            tempLineObj.transform.SetParent(minimapLinesHolder);
            tempLineObj.transform.position = new Vector3(this.gameObject.transform.position.x, BASE_HEIGHT_IN_3D_WORLD, this.gameObject.transform.position.z);
            tempLine = tempLineObj.AddComponent<LineRenderer>();
            tempLineObj.layer = LayerMask.NameToLayer("UI");
            tempLine.SetPositions(new Vector3[] { new Vector3(0, 0, 0), new Vector3(0, 0, 0) });
            tempLine.alignment = LineAlignment.TransformZ;
            tempLine.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            tempLine.receiveShadows = false;
            tempLine.lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.Off;
            tempLine.reflectionProbeUsage = UnityEngine.Rendering.ReflectionProbeUsage.Off;

            //Add the activity monitor to the camera
            ActivityMonitor activeMonitor = tempLineObj.AddComponent<ActivityMonitor>();
            activeMonitor.responsibleScriptComponentForThis = this;
        }

        void Update()
        {
            //If the Minimap Line created by this component is disabled, enable it
            if (tempLineObj.activeSelf == false)
                tempLineObj.SetActive(true);

            //Define the default material on line renderer if not defined yet
            if (tempLine.materials[0] != minimapDataHolder.defaultMaterialForMinimapItems && alreadySettedDefaultMaterialOnLineRenderer == false)
            {
                tempLine.materials = new Material[] { minimapDataHolder.defaultMaterialForMinimapItems };
                alreadySettedDefaultMaterialOnLineRenderer = true;
            }

            //Change the color of line renderer if change
            if (tempLine.startColor != lineRenderColor || tempLine.endColor != lineRenderColor)
            {
                tempLine.startColor = lineRenderColor;
                tempLine.endColor = lineRenderColor;
            }

            //Define the line width of line renderer
            tempLine.widthMultiplier = lineWidthSize * MinimapDataGlobal.GetMinimapItemsSizeGlobalMultiplier();

            //Define the corners of line
            if (roundLineCorners == false)
                tempLine.numCapVertices = 0;
            if (roundLineCorners == true)
                tempLine.numCapVertices = 90;

            //If the list of world positions for Line was changed, update then in the Line Renderer too
            if (updateLinesPositionsRenderNow == true)
            {
                tempLine.positionCount = _listOfLinePositions.Length;
                tempLine.SetPositions(new Vector3[_listOfLinePositions.Length]);
                for (int i = 0; i < _listOfLinePositions.Length; i++)
                {
                    Vector3 worldPosition = _listOfLinePositions[i];
                    worldPosition.y = BASE_HEIGHT_IN_3D_WORLD;
                    tempLine.SetPosition(i, worldPosition);
                }
                updateLinesPositionsRenderNow = false;
            }

            //If line size is minor or major than normal, reset
            if (lineWidthSize < 0.5f)
                lineWidthSize = 0.5f;
            if (lineWidthSize > 100.0f)
                lineWidthSize = 100.0f;

            //Move the camera to follow this gameobject
            tempLineObj.transform.position = Vector3.Lerp(tempLineObj.transform.position, new Vector3(this.gameObject.transform.position.x, BASE_HEIGHT_IN_3D_WORLD, this.gameObject.transform.position.z), movementsSmoothing * Time.deltaTime);
            //Rotate the camera
            tempLineObj.transform.rotation = Quaternion.Lerp(tempLineObj.transform.rotation, Quaternion.Euler(90, 0, 0), movementsSmoothing * Time.deltaTime);
        }

        //Public methods

        public void SetNewWorldPositions(Vector3[] worldPositions)
        {
            //This method will define new World positions for this line
            _listOfLinePositions = new Vector3[worldPositions.Length];
            for (int i = 0; i < _listOfLinePositions.Length; i++)
                _listOfLinePositions = worldPositions;
            updateLinesPositionsRenderNow = true;
        }

        public Vector3[] GetDefinedWorldPositions()
        {
            //This method will return array of existants positions for this line
            Vector3[] worldPos = new Vector3[_listOfLinePositions.Length];
            for (int i = 0; i < worldPos.Length; i++)
                worldPos = _listOfLinePositions;
            return worldPos;
        }

        public bool isThisMinimapLineBeingVisibleByAnyMinimapCamera()
        {
            //This method will return true if this Minimap Line is being visualized by any minimap camera
            bool isVisible = false;
            if (tempLine.isVisible == true)
                isVisible = true;
            return isVisible;
        }

        public LineRenderer GetGeneratedLineRendererAtRunTime()
        {
            //Return the line renderer generated at runtime by this component
            return tempLine;
        }

        public MinimapLine[] GetListOfAllMinimapLinesInThisScene()
        {
            //If is not playing, cancel
            if (Application.isPlaying == false)
            {
                Debug.LogError("It is only possible to obtain the list of Minimap Lines in this scene, if the application is being executed.");
                return null;
            }

            //Return a list that contains reference to all of this component in this scene
            return minimapDataHolder.instancesOfMinimapLineInThisScene.ToArray();
        }
    }
}