#if UNITY_EDITOR
using UnityEditor;
using System.IO;
#endif
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace MTAssets.EasyMinimapSystem
{
    /*
     This class is responsible for the functioning of the "Minimap Scannner" component, and all its functions.
    */
    /*
     * The Easy Minimap System was developed by Marcos Tomaz in 2019.
     * Need help? Contact me (mtassets@windsoft.xyz)
    */

    [AddComponentMenu("MT Assets/Easy Minimap System/Minimap Scanner")] //Add this component in a category of addComponent menu
    public class MinimapScanner : MonoBehaviour
    {
        //Private constants
        private const float BASE_HEIGHT_IN_3D_WORLD = 99000; //Minimap Fog and Minimap Scanner uses the same BASE_HEIGHT_IN_3D_WORLD (The smaller BASE_HEIGHT_IN_3D_WORLD of all components of tool) but differents ORDERS IN LAYERS
        private const float MAX_SCAN_HEIGHT = 5000;
        private const int DEFAULT_ORDER_IN_LAYER = -10;

        //Private variables
        private GameObject minimapDataHolderObj;
        private MinimapDataHolder minimapDataHolder;
        private Transform minimapScansHolder;
        private GameObject tempSpriteObj;
        private SpriteRenderer tempSprite;

        //Cache variables
        private bool updateTextureResultOfScanOnMinimapNow = false;
        private ScanArea lastScanArea = ScanArea.Units2;

        //Editor Cache variables
        [HideInInspector]
        [SerializeField]
        private int currentTab = 0;

        //Enums of script
        public enum ScanArea
        {
            Units2,
            Units4,
            Units16,
            Units20,
            Units32,
            Units40,
            Units60,
            Units64,
            Units80,
            Units100,
            Units128,
            Units150,
            Units200,
            Units250,
            Units256,
            Units300,
            Units350,
            Units400,
            Units450,
            Units500,
            Units512,
        }
        public enum ScanResolution
        {
            Pixels128x128,
            Pixels256x256,
            Pixels512x512,
            Pixels1024x1024,
            Pixels2048x2048,
            Pixels4096x4096,
            Pixels8192x8192
        }
        public enum CreateScanInSide
        {
            Forward,
            Back,
            Left,
            Right
        }

        //Public variables
        [HideInInspector]
        [SerializeField]
        private Texture2D _textureResultOfScan = null;
        [HideInInspector]
        public float scanHeight = 100.0f;
        [HideInInspector]
        public ScanArea scanArea = ScanArea.Units40;
        [HideInInspector]
        public ScanResolution scanResolution = ScanResolution.Pixels512x512;
        [HideInInspector]
        public Color colorOfScanInGame = new Color(1, 1, 1, 1);
        [HideInInspector]
        public List<GameObject> gameObjectsToIgnore = new List<GameObject>();

        //Getters to verify and validate needed variables
        public Texture2D textureResultOfScan
        {
            get
            {
                return _textureResultOfScan;
            }
            set
            {
                _textureResultOfScan = value;
                updateTextureResultOfScanOnMinimapNow = true;
            }
        }

#if UNITY_EDITOR
        private bool clearUndoHistory = false;
        private float timeToDisableClearUndoHistory = 0;

        //The UI of this component
        #region INTERFACE_CODE
        [UnityEditor.CustomEditor(typeof(MinimapScanner))]
        public class CustomInspector : UnityEditor.Editor
        {
            Vector2 gameObjectsToIgnore_ScrollPos;

            public override void OnInspectorGUI()
            {
                //Start the undo event support, draw default inspector and monitor of changes
                MinimapScanner script = (MinimapScanner)target;
                EditorGUI.BeginChangeCheck();
                Undo.RecordObject(target, "Undo Event");

                //Clears undo history if desired. After a time, disable the clear undo
                if (script.clearUndoHistory == true)
                {
                    Undo.ClearAll();

                    script.timeToDisableClearUndoHistory += Time.deltaTime;
                    if (script.timeToDisableClearUndoHistory > 1f)
                    {
                        script.clearUndoHistory = false;
                        script.timeToDisableClearUndoHistory = 0;
                    }
                }

                //Support reminder
                GUILayout.Space(10);
                EditorGUILayout.HelpBox("Remember to read the Easy Minimap System documentation to understand how to use it.\nGet support at: mtassets@windsoft.xyz", MessageType.None);

                //Now that all components are validated, execution continues. -----------

                GUILayout.Space(10);

                //Set height in 0
                script.gameObject.transform.position = new Vector3(script.gameObject.transform.position.x, 0, script.gameObject.transform.position.z);
                script.gameObject.transform.rotation = Quaternion.Euler(0, 0, 0);
                script.gameObject.transform.localScale = new Vector3(1, 1, 1);

                //Show the toolbar
                script.currentTab = GUILayout.Toolbar(script.currentTab, new string[] { "Scanning", "Preview" });

                //Draw the content of toolbar selected
                switch (script.currentTab)
                {
                    case 0:
                        //---------------------------------------- Start of "Scanning" tab
                        //Start of settings
                        GUILayout.Space(10);
                        EditorGUILayout.LabelField("Settings For Minimap Scanner", EditorStyles.boldLabel);
                        GUILayout.Space(10);

                        script.textureResultOfScan = (Texture2D)EditorGUILayout.ObjectField(new GUIContent("Texture Result Of Scan",
                                        "This is the texture that is currently registered to be rendered and will cover the entire Scan Area of this Minimap Scanner, when rendered in the Minimap.\n\nYou can click on the button below to perform a scan and then this field will be automatically filled with a texture generated from your scenario (which you can modify later), or you can provide your own texture here."),
                                        script.textureResultOfScan, typeof(Texture2D), true, GUILayout.Height(16));
                        if (script.textureResultOfScan != null)
                            if (script.textureResultOfScan.width != script.GetSelectedScanResolution() || script.textureResultOfScan.height != script.GetSelectedScanResolution())
                                EditorGUILayout.HelpBox("You must provide a square texture that matches the same size selected under \"scanResolution\", to set a new texture in this Minimap Scanner. If you have already done the scanning, try clicking the button below to update the scan.", MessageType.Error);

                        script.scanHeight = EditorGUILayout.Slider(new GUIContent("Scan Height",
                                        "This is the time at which the scan will be performed. If you need to scan a cave that is below the 0 coordinate for example, you can lower this height until you are at a comfortable height to scan that cave, if you need to scan the whole scenario in a superior way, leave a height higher than the your scenario. 1 Unit = 1 Meter of Height."),
                                        script.scanHeight, (MAX_SCAN_HEIGHT * -1), MAX_SCAN_HEIGHT);

                        script.scanArea = (ScanArea)EditorGUILayout.EnumPopup(new GUIContent("Scan Area",
                                        "Size of area to be scanned. 1 Unity = 1 Meter. The larger the scanned area, the larger the area displayed in the resulting image."),
                                        script.scanArea);

                        script.scanResolution = (ScanResolution)EditorGUILayout.EnumPopup(new GUIContent("Scan Resolution",
                                        "Image resolution resulting from scanning. The higher the resolution, the more definition one will have after scanning. High definition images can be useful if you need to scan a world map that supports zooming for example."),
                                        script.scanResolution);

                        GUILayout.Space(10);

                        EditorGUILayout.LabelField("In Game Effects", EditorStyles.boldLabel);

                        script.colorOfScanInGame = EditorGUILayout.ColorField(new GUIContent("Color of Scan In Game",
                                        "The color of the scan will appear when rendered by the Minimap Camera."),
                                        script.colorOfScanInGame);

                        GUILayout.Space(10);
                        EditorGUILayout.LabelField("Ignore In Scan", EditorStyles.boldLabel);
                        GUILayout.Space(10);

                        Texture2D removeItemIcon = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/MT Assets/Easy Minimap System/Editor/Images/Remove.png", typeof(Texture2D));
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("GameObjects To Ignore", GUILayout.Width(145));
                        GUILayout.Space(MTAssetsEditorUi.GetInspectorWindowSize().x - 145);
                        EditorGUILayout.LabelField("Size", GUILayout.Width(30));
                        EditorGUILayout.IntField(script.gameObjectsToIgnore.Count, GUILayout.Width(50));
                        EditorGUILayout.EndHorizontal();
                        GUILayout.BeginVertical("box");
                        gameObjectsToIgnore_ScrollPos = EditorGUILayout.BeginScrollView(gameObjectsToIgnore_ScrollPos, GUIStyle.none, GUI.skin.verticalScrollbar, GUILayout.Width(MTAssetsEditorUi.GetInspectorWindowSize().x), GUILayout.Height(100));
                        if (script.gameObjectsToIgnore.Count == 0)
                            EditorGUILayout.HelpBox("Oops! No GameObjects was registered to be ignored! If you want to subscribe any, click the button below!", MessageType.Info);
                        if (script.gameObjectsToIgnore.Count > 0)
                            for (int i = 0; i < script.gameObjectsToIgnore.Count; i++)
                            {
                                GUILayout.BeginHorizontal();
                                if (GUILayout.Button(removeItemIcon, GUILayout.Width(25), GUILayout.Height(16)))
                                    script.gameObjectsToIgnore.RemoveAt(i);
                                script.gameObjectsToIgnore[i] = (GameObject)EditorGUILayout.ObjectField(new GUIContent("GameObject " + i.ToString(), "This GameObject will be ignored in the Scan. Click the button to the left if you want to remove this GameObject from the list."), script.gameObjectsToIgnore[i], typeof(GameObject), true, GUILayout.Height(16));
                                GUILayout.EndHorizontal();
                            }
                        EditorGUILayout.EndScrollView();
                        GUILayout.EndVertical();
                        GUILayout.BeginHorizontal();
                        if (GUILayout.Button("Add New Slot"))
                        {
                            script.gameObjectsToIgnore.Add(null);
                            gameObjectsToIgnore_ScrollPos.y += 999999;
                        }
                        if (script.gameObjectsToIgnore.Count > 0)
                            if (GUILayout.Button("Remove Empty Slots", GUILayout.Width(Screen.width * 0.48f)))
                                for (int i = script.gameObjectsToIgnore.Count - 1; i >= 0; i--)
                                    if (script.gameObjectsToIgnore[i] == null)
                                        script.gameObjectsToIgnore.RemoveAt(i);
                        GUILayout.EndHorizontal();

                        GUILayout.Space(20);

                        if (Application.isPlaying == false)
                        {
                            if (script.textureResultOfScan == null)
                                if (GUILayout.Button("Start Scan", GUILayout.Height(40)))
                                    script.DoScanInThisAreaOfComponentAndShowOnMinimap();
                            if (script.textureResultOfScan != null)
                            {
                                EditorGUILayout.BeginHorizontal();

                                if (GUILayout.Button("Delete Scan", GUILayout.Height(40)))
                                {
                                    //Delete the current scan if exists
                                    if (AssetDatabase.LoadAssetAtPath(AssetDatabase.GetAssetPath(script.textureResultOfScan), typeof(object)) != null)
                                        AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(script.textureResultOfScan));
                                    //Clear texture
                                    script.textureResultOfScan = null;
                                    //Update assets
                                    AssetDatabase.Refresh();
                                }
                                if (GUILayout.Button("Update Scan", GUILayout.Height(40)))
                                    script.DoScanInThisAreaOfComponentAndShowOnMinimap();

                                EditorGUILayout.EndHorizontal();
                            }
                        }
                        if (Application.isPlaying == true)
                        {
                            EditorGUILayout.HelpBox("Scanning manipulation not available while the game runs. To manipulate the scan while the application is running, use the C# API.", MessageType.Warning);
                        }

                        //---------------------------------------- End of "Scanning" tab
                        break;
                    case 1:
                        //---------------------------------------- Start of "Preview" tab
                        GUILayout.Space(10);

                        if (script.textureResultOfScan != null)
                        {
                            EditorGUILayout.HelpBox("This is the current scan this component has performed. If you want to scan again, just start a new scan. This scan will be overwritten.", MessageType.Info);
                            GUIStyle style = new GUIStyle();
                            style.normal.textColor = Color.black;
                            style.alignment = TextAnchor.MiddleCenter;
                            style.fontStyle = FontStyle.Bold;
                            GUIStyle estiloIcone = new GUIStyle();
                            estiloIcone.border = new RectOffset(0, 0, 0, 0);
                            estiloIcone.margin = new RectOffset(4, 0, 4, 0);
                            estiloIcone.alignment = TextAnchor.MiddleCenter;
                            EditorGUILayout.LabelField("Resolution of " + script.textureResultOfScan.width + "x" + script.textureResultOfScan.height, style);
                            GUILayout.Box(script.textureResultOfScan, estiloIcone, GUILayout.Width(Screen.width - 36), GUILayout.Height(400));
                        }
                        if (script.textureResultOfScan == null)
                        {
                            EditorGUILayout.HelpBox("No scan has been performed yet! To view a scan, please perform a scan first.", MessageType.Info);
                        }
                        //---------------------------------------- End of "Preview" tab
                        break;
                }

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
                //Draw the controls of this Scan
                MinimapScanner script = (MinimapScanner)target;

                //If application is running, not render the arrows
                if (Application.isPlaying == true)
                    return;

                //Set the color of controls
                Handles.color = Color.yellow;

                //Get the current position
                Vector3 position = script.gameObject.transform.position;

                //Distance after border of scan
                float distanceAfterEdge = 4;

                //Calculate the 4 points of area
                Vector3 left = new Vector3(position.x - distanceAfterEdge, 0, position.z + script.GetSelectedScanArea() / 2);
                Vector3 right = new Vector3(position.x + script.GetSelectedScanArea() + distanceAfterEdge, 0, position.z + script.GetSelectedScanArea() / 2);
                Vector3 top = new Vector3(position.x + script.GetSelectedScanArea() / 2, 0, position.z + script.GetSelectedScanArea() + distanceAfterEdge);
                Vector3 bottom = new Vector3(position.x + script.GetSelectedScanArea() / 2, 0, position.z - distanceAfterEdge);

                //Calculate the size of button, taking account the current camera distance
                float size = Vector3.Distance(Camera.current.transform.transform.position, new Vector3(position.x + script.GetSelectedScanArea() / 2, 0, position.z + script.GetSelectedScanArea() / 2)) * 0.08f;

                //The top buttom
                if (Handles.Button(top, Quaternion.Euler(0, 0, 0), size, size, Handles.ArrowHandleCap) == true)
                    script.CreateNewMinimapScannerBeside(CreateScanInSide.Forward);

                //The right buttom
                if (Handles.Button(right, Quaternion.Euler(0, 90, 0), size, size, Handles.ArrowHandleCap) == true)
                    script.CreateNewMinimapScannerBeside(CreateScanInSide.Right);

                //The left buttom
                if (Handles.Button(left, Quaternion.Euler(0, 270, 0), size, size, Handles.ArrowHandleCap) == true)
                    script.CreateNewMinimapScannerBeside(CreateScanInSide.Left);

                //The bottom buttom
                if (Handles.Button(bottom, Quaternion.Euler(0, 180, 0), size, size, Handles.ArrowHandleCap) == true)
                    script.CreateNewMinimapScannerBeside(CreateScanInSide.Back);
            }
        }

        public void OnDrawGizmosSelected()
        {
            //Set color of gizmos if not exists scan
            if (textureResultOfScan == null)
            {
                Gizmos.color = Color.red;
                Handles.color = Color.red;
            }
            //Set color of gizmos if exists scan
            if (textureResultOfScan != null)
            {
                Gizmos.color = Color.blue;
                Handles.color = Color.blue;
            }

            //Get the current position
            Vector3 position = this.gameObject.transform.position;

            //Calculate the 4 points of area
            Vector3 bottomLeft = new Vector3(position.x, 0, position.z);
            Vector3 topLeft = new Vector3(position.x, 0, position.z + GetSelectedScanArea());
            Vector3 bottomRight = new Vector3(position.x + GetSelectedScanArea(), 0, position.z);
            Vector3 topRight = new Vector3(position.x + GetSelectedScanArea(), 0, position.z + GetSelectedScanArea());
            Vector3 center = new Vector3(position.x + GetSelectedScanArea() / 2, 0, position.z + GetSelectedScanArea() / 2);

            //Show the 4 points
            Gizmos.DrawSphere(bottomLeft, 0.5f);
            Gizmos.DrawSphere(topLeft, 0.5f);
            Gizmos.DrawSphere(bottomRight, 0.5f);
            Gizmos.DrawSphere(topRight, 0.5f);
            Gizmos.DrawWireSphere(center, 0.5f);

            //Show the lines
            Handles.DrawAAPolyLine(5f, new Vector3[] { bottomLeft, topLeft, topRight, bottomRight, bottomLeft });
            Vector3 scanHeightVector3 = new Vector3(0, scanHeight, 0);
            Color colorBefore = Handles.color;
            Handles.color = Color.white;
            Color solidColor = colorBefore;
            solidColor.a = 0.10f;
            Handles.DrawSolidRectangleWithOutline(new Vector3[] { bottomLeft + scanHeightVector3, topLeft + scanHeightVector3, topRight + scanHeightVector3, bottomRight + scanHeightVector3 },
                                                  solidColor, colorBefore);
            Handles.color = colorBefore;
            Color linesColor = colorBefore;
            linesColor.a = 0.30f;
            Handles.color = linesColor;
            Handles.DrawLine(bottomLeft + scanHeightVector3, bottomLeft + (Vector3.down * MAX_SCAN_HEIGHT));
            Handles.DrawLine(topLeft + scanHeightVector3, topLeft + (Vector3.down * MAX_SCAN_HEIGHT));
            Handles.DrawLine(bottomRight + scanHeightVector3, bottomRight + (Vector3.down * MAX_SCAN_HEIGHT));
            Handles.DrawLine(topRight + scanHeightVector3, topRight + (Vector3.down * MAX_SCAN_HEIGHT));
            Handles.DrawSolidRectangleWithOutline(new Vector3[] { bottomLeft + (Vector3.down * MAX_SCAN_HEIGHT), topLeft + (Vector3.down * MAX_SCAN_HEIGHT), topRight + (Vector3.down * MAX_SCAN_HEIGHT), bottomRight + (Vector3.down * MAX_SCAN_HEIGHT) },
                                              Color.clear, colorBefore);

            //Draw meters indicator
            Handles.Label(topLeft, "(" + GetSelectedScanArea().ToString() + " Units)");
            Handles.Label(bottomRight, "(" + GetSelectedScanArea().ToString() + " Units)");
            GUIStyle style = new GUIStyle();
            style.normal.textColor = Color.white;
            style.alignment = TextAnchor.MiddleCenter;
            style.fontStyle = FontStyle.Bold;
            style.contentOffset = new Vector2(-20, 0);
            Handles.Label(center + scanHeightVector3, "Scan Height\n(" + scanHeight.ToString("F1") + " Units)", style);
        }
        #endregion
#endif

        //Core methods

        private int GetSelectedScanResolution()
        {
            //Return the value of selected scan resolution in enum
            switch (scanResolution)
            {
                case ScanResolution.Pixels128x128:
                    return 128;
                case ScanResolution.Pixels256x256:
                    return 256;
                case ScanResolution.Pixels512x512:
                    return 512;
                case ScanResolution.Pixels1024x1024:
                    return 1024;
                case ScanResolution.Pixels2048x2048:
                    return 2048;
                case ScanResolution.Pixels4096x4096:
                    return 4096;
                case ScanResolution.Pixels8192x8192:
                    return 8192;
            }
            return 0;
        }

        private float GetSelectedScanArea()
        {
            //Return the value of selected scan area, in enum
            switch (scanArea)
            {
                case ScanArea.Units2:
                    return 2;
                case ScanArea.Units4:
                    return 4;
                case ScanArea.Units16:
                    return 16;
                case ScanArea.Units20:
                    return 20;
                case ScanArea.Units32:
                    return 32;
                case ScanArea.Units40:
                    return 40;
                case ScanArea.Units60:
                    return 60;
                case ScanArea.Units64:
                    return 64;
                case ScanArea.Units80:
                    return 80;
                case ScanArea.Units100:
                    return 100;
                case ScanArea.Units128:
                    return 128;
                case ScanArea.Units150:
                    return 150;
                case ScanArea.Units200:
                    return 200;
                case ScanArea.Units250:
                    return 250;
                case ScanArea.Units256:
                    return 256;
                case ScanArea.Units300:
                    return 300;
                case ScanArea.Units350:
                    return 350;
                case ScanArea.Units400:
                    return 400;
                case ScanArea.Units450:
                    return 450;
                case ScanArea.Units500:
                    return 500;
                case ScanArea.Units512:
                    return 512;
            }
            return 0;
        }

        public void Awake()
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
            minimapScansHolder = minimapDataHolderObj.transform.Find("Minimap Scans Holder");
            if (minimapScansHolder == null)
            {
                GameObject obj = new GameObject("Minimap Scans Holder");
                minimapScansHolder = obj.transform;
                minimapScansHolder.SetParent(minimapDataHolderObj.transform);
                minimapScansHolder.localPosition = Vector3.zero;
                minimapScansHolder.localEulerAngles = Vector3.zero;
            }
            if (minimapDataHolder.instancesOfMinimapScannerInThisScene.Contains(this) == false)
                minimapDataHolder.instancesOfMinimapScannerInThisScene.Add(this);

            //Create the scan
            tempSpriteObj = new GameObject("Minimap Scan (" + this.gameObject.name + ")");
            tempSpriteObj.transform.SetParent(minimapScansHolder);
            tempSpriteObj.transform.position = new Vector3(this.gameObject.transform.position.x, BASE_HEIGHT_IN_3D_WORLD, this.gameObject.transform.position.z);
            tempSprite = tempSpriteObj.AddComponent<SpriteRenderer>();
            tempSprite.sortingOrder = DEFAULT_ORDER_IN_LAYER;
            tempSpriteObj.layer = LayerMask.NameToLayer("UI");

            //Add the activity monitor to the camera
            ActivityMonitor activityMonitor = tempSpriteObj.AddComponent<ActivityMonitor>();
            activityMonitor.responsibleScriptComponentForThis = this;

            //Inform that a new scan has performed, if already have a scan done by the editor
            if (textureResultOfScan != null)
                updateTextureResultOfScanOnMinimapNow = true;
        }

        public void Update()
        {
            //If the Minimap Scan created by this component is disabled, enable it
            if (tempSpriteObj.activeSelf == false)
                tempSpriteObj.SetActive(true);

            //If the current render texture result of scan, was changed, update then on sprite renderer
            if (textureResultOfScan != null)
                if (updateTextureResultOfScanOnMinimapNow == true || scanArea != lastScanArea)
                {
                    //Validate the texture of scan (check if texture is square and matches with resolution selected in GetSelectedScanResolution())
                    if (textureResultOfScan.width != GetSelectedScanResolution() || textureResultOfScan.height != GetSelectedScanResolution())
                    {
                        Debug.LogError("You must provide a square texture that matches the same size selected under \"scanResolution\", to set a new texture in this Minimap Scanner.");
                        return;
                    }

                    //Force the minimap scanner to display current texture 2d in variable textureResultOfScan on minimap
                    tempSprite.sprite = Sprite.Create(textureResultOfScan, new Rect(0.0f, 0.0f, textureResultOfScan.width, textureResultOfScan.height), new Vector2(0.5f, 0.5f), 100.0f);

                    //Calculates the size of scan, taking the pixels per unit into account
                    float areaResolutionAspect = (float)GetSelectedScanArea() / (float)GetSelectedScanResolution();
                    float finalSize = areaResolutionAspect * tempSprite.sprite.pixelsPerUnit;

                    //Set the size of scan in minimap
                    tempSpriteObj.transform.localScale = new Vector3(finalSize, finalSize, 1);

                    //Save the cache
                    updateTextureResultOfScanOnMinimapNow = false;
                    lastScanArea = scanArea;
                }

            //Updates a value, if changes
            if (colorOfScanInGame != tempSprite.color)
                tempSprite.color = colorOfScanInGame;

            //If scan height is minor or major than permitted value
            if (scanHeight > MAX_SCAN_HEIGHT)
                scanHeight = MAX_SCAN_HEIGHT;
            if (scanHeight < (MAX_SCAN_HEIGHT * -1))
                scanHeight = (MAX_SCAN_HEIGHT * -1);

            //Set height and rotation in zero
            this.transform.position = new Vector3(this.transform.position.x, 0, this.transform.position.z);
            this.transform.rotation = Quaternion.Euler(0, 0, 0);
            this.transform.localScale = new Vector3(1, 1, 1);

            //Move the scan to follow this gameobject
            tempSpriteObj.transform.position = new Vector3(this.gameObject.transform.position.x + GetSelectedScanArea() / 2, BASE_HEIGHT_IN_3D_WORLD, this.gameObject.transform.position.z + GetSelectedScanArea() / 2);
            //Rotate the scan
            tempSpriteObj.transform.rotation = Quaternion.Euler(90, 0, 0);
        }

        //Public methods

        public MinimapScanner CreateNewMinimapScannerBeside(CreateScanInSide sideToCreateNewScanner)
        {
            //Get the new position
            Vector3 positionForNewScanner = new Vector3(this.gameObject.transform.position.x, 0, this.gameObject.transform.position.z);
            switch (sideToCreateNewScanner)
            {
                case CreateScanInSide.Forward:
                    positionForNewScanner.z += GetSelectedScanArea();
                    break;
                case CreateScanInSide.Right:
                    positionForNewScanner.x += GetSelectedScanArea();
                    break;
                case CreateScanInSide.Left:
                    positionForNewScanner.x -= GetSelectedScanArea();
                    break;
                case CreateScanInSide.Back:
                    positionForNewScanner.z -= GetSelectedScanArea();
                    break;
            }

            //Create new GameObject, select then, and add Minimap Scanner
            GameObject newScan = new GameObject(this.gameObject.transform.name);
            newScan.transform.SetParent(this.gameObject.transform.parent);
            MinimapScanner mScanner = newScan.AddComponent<MinimapScanner>();
            mScanner.scanHeight = this.scanHeight;
            mScanner.scanArea = this.scanArea;
            mScanner.scanResolution = this.scanResolution;
            mScanner.colorOfScanInGame = this.colorOfScanInGame;
            mScanner.gameObjectsToIgnore.AddRange(this.gameObjectsToIgnore.ToArray());

            //Set the position
            newScan.transform.position = positionForNewScanner;

#if UNITY_EDITOR
            //Register the gameobject created for undo
            Undo.RegisterCreatedObjectUndo(newScan, "Scan Created");

            //Select the new Scanner
            if (Application.isPlaying == false)
                Selection.activeGameObject = newScan;
#endif

            //Return the created Minimap Scanner
            return mScanner;
        }

        public void DoScanInThisAreaOfComponentAndShowOnMinimap()
        {
            //Start scan
#if UNITY_EDITOR
            if (Application.isPlaying == false)
            {
                //Update or show progressbar
                EditorUtility.DisplayProgressBar("Scanning", "Wait a moment...", 0.0f);

                //Delete the current scan if exists
                if (AssetDatabase.LoadAssetAtPath(AssetDatabase.GetAssetPath(textureResultOfScan), typeof(object)) != null)
                    AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(textureResultOfScan));
            }
#endif
            //Get current state of GameObjects to ignore
            bool[] stateOfGoToIgnote = new bool[gameObjectsToIgnore.Count];

            //Disable all GameObjects to ignore, and save original state of each one
            for (int i = 0; i < gameObjectsToIgnore.Count; i++)
            {
                stateOfGoToIgnote[i] = gameObjectsToIgnore[i].activeSelf;
                gameObjectsToIgnore[i].SetActive(false);
            }

            //Create the camera for take image
            GameObject cameraObj = new GameObject("Temp Camera");
            cameraObj.transform.SetParent(this.gameObject.transform);
            cameraObj.transform.localPosition = new Vector3(GetSelectedScanArea() / 2, scanHeight, GetSelectedScanArea() / 2);
            cameraObj.transform.localRotation = Quaternion.Euler(90, 0, 0);
            Camera camera = cameraObj.AddComponent<Camera>();
            camera.clearFlags = CameraClearFlags.Color;
            camera.backgroundColor = new Color(1, 1, 1, 0);
            camera.cullingMask = ~0;
            camera.orthographic = true;
            camera.orthographicSize = GetSelectedScanArea() / 2;
            camera.nearClipPlane = 0.1f;
            camera.farClipPlane = BASE_HEIGHT_IN_3D_WORLD * 2.0f;
            camera.useOcclusionCulling = false;
            camera.allowHDR = false;
            camera.allowMSAA = false;
            camera.allowDynamicResolution = false;

            //Pre alocate the needed variables
            RenderTexture renderTexture = null;
            Rect rectTexture = new Rect();
            Texture2D scanResultTexture = null;

            //Start the scan
            if (renderTexture == null)
            {
                //Create temp render texture
                rectTexture = new Rect(0, 0, GetSelectedScanResolution(), GetSelectedScanResolution());
                renderTexture = new RenderTexture(GetSelectedScanResolution(), GetSelectedScanResolution(), 24, RenderTextureFormat.ARGB32);
                scanResultTexture = new Texture2D(GetSelectedScanResolution(), GetSelectedScanResolution(), TextureFormat.RGBA32, false, true);
            }

            //Render the scene manually, in render texture
            camera.targetTexture = renderTexture;
            camera.Render();

            //Read pixels of render texture
            RenderTexture.active = renderTexture;
            scanResultTexture.ReadPixels(rectTexture, 0, 0);
            scanResultTexture.Apply();

            //Reset active components
            camera.targetTexture = null;
            RenderTexture.active = null;
#if UNITY_EDITOR
            if (Application.isPlaying == false)
                DestroyImmediate(cameraObj);
#endif
            if (Application.isPlaying == true)
                Destroy(cameraObj);

#if UNITY_EDITOR
            string pathForFile = "";
            if (Application.isPlaying == false)
            {
                //Save the scan result
                byte[] bytesOfScan = scanResultTexture.EncodeToPNG();

                //Create base folders
                if (!AssetDatabase.IsValidFolder("Assets/MT Assets/_AssetsData"))
                    AssetDatabase.CreateFolder("Assets/MT Assets", "_AssetsData");
                if (!AssetDatabase.IsValidFolder("Assets/MT Assets/_AssetsData/Scans"))
                    AssetDatabase.CreateFolder("Assets/MT Assets/_AssetsData", "Scans");

                //Generate a name for the scan file
                DateTime date = DateTime.Now;
                pathForFile = "Assets/MT Assets/_AssetsData/Scans/Scan (" + date.Year + date.Month + date.Day + date.Hour + date.Minute + date.Second + date.Millisecond.ToString() + ").png";

                //Save image on disk
                System.IO.File.WriteAllBytes(pathForFile, bytesOfScan);

                //Update or show progressbar
                EditorUtility.DisplayProgressBar("Scanning", "Wait a moment...", 1.0f);

                //Updates assets
                AssetDatabase.ImportAsset(pathForFile);
                AssetDatabase.Refresh();
            }
#endif

            //Restore state of all GameObjects to ignore
            for (int i = 0; i < gameObjectsToIgnore.Count; i++)
            {
                gameObjectsToIgnore[i].SetActive(stateOfGoToIgnote[i]);
            }

#if UNITY_EDITOR
            if (Application.isPlaying == false)
            {
                //Clear the progress bar
                EditorUtility.ClearProgressBar();

                //Show the preview
                currentTab = 1;
                Debug.Log("The minimap scan in \"" + this.gameObject.name + "\" is complete! You can view it on the \"Preview\" tab.");

                //Clear the undo history, to prevent incorrect undo after scan
                clearUndoHistory = true;
            }
#endif

#if UNITY_EDITOR
            //If is in editor, load texture from files
            if (Application.isPlaying == false)
                textureResultOfScan = (Texture2D)AssetDatabase.LoadAssetAtPath(pathForFile, typeof(Texture2D));
#endif
            if (Application.isPlaying == true)
                //Set the current texture result of scan, in this component
                textureResultOfScan = scanResultTexture;

            //Inform that a new scan was performed, to Update() methods update the sprite renderer with the new texture result of scan in-game
            if (Application.isPlaying == true)
                updateTextureResultOfScanOnMinimapNow = true;
        }

        public bool isThisMinimapScanBeingVisibleByAnyMinimapCamera()
        {
            //This method will return true if this Minimap Scan is being visualized by any minimap camera
            bool isVisible = false;
            if (tempSprite.isVisible == true)
                isVisible = true;
            return isVisible;
        }

        public SpriteRenderer GetGeneratedSpriteAtRunTime()
        {
            //Return the sprite generated at runtime by this component
            return tempSprite;
        }

        public MinimapScanner[] GetListOfAllMinimapScannersInThisScene()
        {
            //If is not playing, cancel
            if (Application.isPlaying == false)
            {
                Debug.LogError("It is only possible to obtain the list of Minimap Scanners in this scene, if the application is being executed.");
                return null;
            }

            //Return a list that contains reference to all of this component in this scene
            return minimapDataHolder.instancesOfMinimapScannerInThisScene.ToArray();
        }
    }
}