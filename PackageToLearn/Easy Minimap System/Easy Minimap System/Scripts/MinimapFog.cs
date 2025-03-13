#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Events;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace MTAssets.EasyMinimapSystem
{
    /*
     This class is responsible for the functioning of the "Minimap Fog" component, and all its functions.
    */
    /*
     * The Easy Minimap System was developed by Marcos Tomaz in 2019.
     * Need help? Contact me (mtassets@windsoft.xyz)
    */

    [AddComponentMenu("MT Assets/Easy Minimap System/Minimap Fog")] //Add this component in a category of addComponent menu
    public class MinimapFog : MonoBehaviour
    {
        //Private constants
        private const float BASE_HEIGHT_IN_3D_WORLD = 99000; //Minimap Fog and Minimap Scanner uses the same BASE_HEIGHT_IN_3D_WORLD (The smaller BASE_HEIGHT_IN_3D_WORLD of all components of tool) but differents ORDERS IN LAYERS
        private const float CHUNK_SIDES_SIZE_ON_UNITS = 200.0f;
        private const float CHUNK_RAY_SIZE_ON_UNITS = 100.0f;
        private const int DEFAULT_ORDER_IN_LAYER_FOR_SPRITE = -8;
        private const int MAX_QUANTITY_OF_DYNAMICS_FOG_REVEALERS_IN_POOL = 100;
        private const float DELAY_BETWEEN_EACH_STATIC_FOG_ACTIVATION_OR_DEACTIVATION = 0.10f;

        //Private variables
        private GameObject minimapDataHolderObj;
        private MinimapDataHolder minimapDataHolder;
        private Transform minimapFogsHolder;
        private GameObject tempBaseGameObj;

        //Private cache variables (for PathsTraveledByTargets)
        private bool isTargetsPathsRevealerRoutineRunning = false;
        private List<GameObject> staticsFogsToBeEnabled_byRevealAllFogsCloserAtThisGlobalPosition = new List<GameObject>();
        private List<GameObject> staticsFogsToBeDisabled_byUnRevealAllFogsCloserAtThisGlobalPosition = new List<GameObject>();
        private List<GameObject> staticsFogsToBeEnabled_byLoadStateOfFogOfWar = new List<GameObject>();
        //Private cache variables (for OnlyTargetsCurrentPosition)
        private Dictionary<SpriteMask, Transform> dynamicFogsRevealersPoolAndAssociatedTransforms = new Dictionary<SpriteMask, Transform>();
        private bool isDynamicFogsRevealersUpdatedsInSize = false;
        private FieldOfViewForFogs lastViewDistanceForFogs = FieldOfViewForFogs.Units30;

        //Enums of Script
        private enum ArrowToBeDrawedDirection
        {
            Forward,
            Back,
            Right,
            Left
        }
        private enum FogRemovalMethod
        {
            PathsTraveledByTargets,
            OnlyTargetsCurrentPosition
        }
        private enum FieldOfViewForChunks
        {
            Units250,
            Units400
        }
        private enum QuantityOfFogsPerChunk
        {
            Fogs25,
            Fogs36,
            Fogs64,
            Fogs100,
            Fogs121,
            Fogs144
        }
        private enum FogsUpdateFrequency
        {
            x60PerSecond,
            x45PerSecond,
            x30PerSecond,
            x15PerSecond,
            x10PerSecond,
            x5PerSecond,
            x4PerSecond,
            x3PerSecond,
            x2PerSecond,
            x1PerSecond,
        }
        public enum FieldOfViewForFogs
        {
            Units20,
            Units25,
            Units30,
            Units35,
            Units40,
            Units45,
            Units50,
            Units55,
            Units60,
            Units65,
            Units70,
            Units75,
            Units80,
            Units85,
            Units90,
            Units95,
            Units100,
            Units105,
            Units110,
            Units115,
            Units120
        }

        //Classes of script
        private class ArrowToBeDrawed
        {
            //This class stores a information about a Arrow to be drawed and information
            public ArrowToBeDrawedDirection directionOfArrow = ArrowToBeDrawedDirection.Forward;
            public FogChunkItem fogChunkItemOfArrow;

            //The constructor
            public ArrowToBeDrawed(ArrowToBeDrawedDirection directionOfArrow, FogChunkItem fogChunkItem)
            {
                this.directionOfArrow = directionOfArrow;
                this.fogChunkItemOfArrow = fogChunkItem;
            }
        }
        private class FogsParametersForGeneration
        {
            //This class stores temporary data as parameters for fogs generation to fill chunks
            public int quantityInX = 0;
            public int quantityInY = 0;
            public int spaceBetweenEachFog = 0;
            public int sizeRadiusOfEachFog = 0;
        }
        [System.Serializable]
        public class FogChunkItem
        {
            //This class stores data of a Fog Chunk that exists in the World and is saved in scene

            //---- Variables for persistent storage of this Fog Chunk data in Editor/Runtime

            //Fog Chunk storage data (this values will be filled on editor when creating Fog Chunks)
            public int thisFogChunkId = -1;
            public float chunkSizeOfSides = 0;
            public float chunkSizeRay = 0;
            public Vector3 globalPositionOfPivot = Vector3.zero;
            public Vector3 globalPositionOfBottomLeftPoint = Vector3.zero;
            public Vector3 globalPositionOfBottomRightPoint = Vector3.zero;
            public Vector3 globalPositionOfTopLeftPoint = Vector3.zero;
            public Vector3 globalPositionOfTopRightPoint = Vector3.zero;

            //---- Variables to be used in runtime for calcs and etc (will be filled in runtime)

            //Fog Chunk data
            public GameObject fogChunkGameObject;
            public List<GameObject> fogsOfThisChunk;
        }
        [System.Serializable]
        public class SaveStateDataOfFogOfWar
        {
            //This class is responsible for save the state data of Fog Of War (USED BY BINNARY FORMATTER)
            public float[] allStaticFogsRevealeds_xPos;
            public float[] allStaticFogsRevealeds_yPos;
            public float[] allStaticFogsRevealeds_zPos;
        }

        //Custom events of script
        [System.Serializable]
        public class OnRevealFog : UnityEvent<Vector3> { }
        [System.Serializable]
        public class OnUnrevealFog : UnityEvent<Vector3> { }

        //Current existing fog chunks (database)
        ///<summary>[WARNING] Do not change the value of this variable. This is a variable used for internal tool operations.</summary> 
        [HideInInspector]
        public List<FogChunkItem> currentExistingFogChunks = new List<FogChunkItem>();

        //Public variables (Only available in Inspecotr)
        [HideInInspector]
        [SerializeField]
        private FogRemovalMethod fogRemovalMethod = FogRemovalMethod.PathsTraveledByTargets;
        [HideInInspector]
        [SerializeField]
        private FieldOfViewForChunks viewDistanceForChunks = FieldOfViewForChunks.Units250;
        [HideInInspector]
        [SerializeField]
        private QuantityOfFogsPerChunk fogsQuantityPerChunk = QuantityOfFogsPerChunk.Fogs64;
        [HideInInspector]
        [SerializeField]
        private FogsUpdateFrequency fogsUpdateFrequency = FogsUpdateFrequency.x3PerSecond;
        [HideInInspector]
        [SerializeField]
        private Sprite fogsBaseSprite = null;
        [HideInInspector]
        [SerializeField]
        private Color fogBaseColor = new Color(19.0f / 255.0f, 19.0f / 255.0f, 19.0f / 255.0f, 1.0f);
        [HideInInspector]
        [SerializeField]
        private Sprite revealerBaseSprite = null;

        //Public variables
        [HideInInspector]
        public FieldOfViewForFogs viewDistanceForFogs = FieldOfViewForFogs.Units30;
        [HideInInspector]
        public string fogsRevealedSaveId = "";
        [HideInInspector]
        public List<Transform> targetsThatCanRemoveFog = new List<Transform>();

        //Input Events and Parameters
        [Space(10)]
        public OnRevealFog onRevealFog = new OnRevealFog();
        public OnUnrevealFog onUnrevealFog = new OnUnrevealFog();
        public UnityEvent onStartLoadState = new UnityEvent();
        public UnityEvent onDoneLoadState = new UnityEvent();

#if UNITY_EDITOR
        //Public variables of interface
        [HideInInspector]
        [SerializeField]
        private bool showMinimapFogEventsOptions = false;

        //The UI of this component
        #region INTERFACE_CODE
        [UnityEditor.CustomEditor(typeof(MinimapFog))]
        public class CustomInspector : UnityEditor.Editor
        {
            Vector2 targetsThatCanRemoveFog_ScrollPos;

            public override void OnInspectorGUI()
            {
                //Start the undo event support, draw default inspector and monitor of changes
                MinimapFog script = (MinimapFog)target;
                EditorGUI.BeginChangeCheck();
                Undo.RecordObject(target, "Undo Event");

                //Support reminder
                GUILayout.Space(10);
                EditorGUILayout.HelpBox("Remember to read the Easy Minimap System documentation to understand how to use it.\nGet support at: mtassets@windsoft.xyz", MessageType.None);
                GUILayout.Space(10);

                //If not have no one Chunk in World, create the first
                if (script.currentExistingFogChunks.Count == 0)
                    script.CreateNewFogChunkOnPosition(new Vector3(0, 0, 0));

                GUIStyle titulo = new GUIStyle();
                titulo.fontSize = 16;
                titulo.normal.textColor = new Color(0, 79.0f / 250.0f, 3.0f / 250.0f);
                titulo.alignment = TextAnchor.MiddleCenter;
                EditorGUILayout.LabelField("There are a total of " + script.currentExistingFogChunks.Count + " Chunks of Fogs!", titulo);
                GUIStyle subTitulo = new GUIStyle();
                subTitulo.fontSize = 10;
                subTitulo.normal.textColor = new Color(69.0f / 250.0f, 69.0f / 250.0f, 69.0f / 250.0f);
                subTitulo.alignment = TextAnchor.MiddleCenter;
                if (script.fogRemovalMethod == FogRemovalMethod.PathsTraveledByTargets)
                    EditorGUILayout.LabelField("There are a total of " + (script.GetSelectedFogsQuantityPerChunk() * script.currentExistingFogChunks.Count).ToString() + " Fogs", subTitulo);
                if (script.fogRemovalMethod == FogRemovalMethod.OnlyTargetsCurrentPosition)
                    EditorGUILayout.LabelField("There are a total of 1 Fog", subTitulo);

                //Editor Settings
                GUILayout.Space(10);
                EditorGUILayout.LabelField("Settings For Fogs Cache", EditorStyles.boldLabel);
                GUILayout.Space(10);

                script.fogRemovalMethod = (FogRemovalMethod)EditorGUILayout.EnumPopup(new GUIContent("Fogs Removal Method",
                            "Here you can define the method in which the Targets will remove the fog along the way.\n\nPaths Traveled By Targets - It will remove Fog from all paths traveled by Targets and keep Fog removed.\n\nOnly Targets Current Position - It will remove Fog only from the positions that the Targets are currently in. Fog will return to the location if the Target leaves the location."),
                            script.fogRemovalMethod);
                if (script.fogRemovalMethod == FogRemovalMethod.PathsTraveledByTargets)
                {
                    EditorGUI.indentLevel += 1;
                    script.viewDistanceForChunks = (FieldOfViewForChunks)EditorGUILayout.EnumPopup(new GUIContent("View Dist. For Chunks",
                                                "This distance defines which Chunks will be considered to be closest to the Targets so that they can reveal what's on Fog. Before the Fogs disappear, it is checked which Chunks are closest and then make that Chunk's Fogs disappear. A greater viewing distance can reduce the performance of your game."),
                                                script.viewDistanceForChunks);

                    script.fogsQuantityPerChunk = (QuantityOfFogsPerChunk)EditorGUILayout.EnumPopup(new GUIContent("Fogs Quant. Per Chunk",
                                "Here it is possible to define the amount of Fogs that each Chunk will have. More Fogs will bring a smoother feeling, but it can cost performance and slightly more memory consumption."),
                                script.fogsQuantityPerChunk);

                    script.fogsUpdateFrequency = (FogsUpdateFrequency)EditorGUILayout.EnumPopup(new GUIContent("Fogs Updates Frequency",
                                "This variable defines the number of times per second that this component will update the rendering of the Fogs. A greater number of updates may have a higher performance cost, but greater accuracy."),
                                script.fogsUpdateFrequency);
                    EditorGUI.indentLevel -= 1;
                }

                script.fogsBaseSprite = (Sprite)EditorGUILayout.ObjectField(new GUIContent("Fogs Base Sprite",
                        "The Sprite that will be used as the Base to create the Fog. The Sprite will cover each created Chunk."),
                        script.fogsBaseSprite, typeof(Sprite), true, GUILayout.Height(16));
                if (script.fogsBaseSprite == null)
                    script.fogsBaseSprite = (Sprite)AssetDatabase.LoadAssetAtPath("Assets/MT Assets/Easy Minimap System/Sprites/FogOfWar.png", typeof(Sprite));

                script.fogBaseColor = EditorGUILayout.ColorField(new GUIContent("Fogs Base Color",
                        "The color in which the Fog should be rendered."),
                        script.fogBaseColor);

                EditorGUILayout.BeginHorizontal();
                script.revealerBaseSprite = (Sprite)EditorGUILayout.ObjectField(new GUIContent("Revealer Base Sprite",
                        "This will be the Sprite that will be used as a Fog Revealer. This Sprite will give the form as to the Fog will be revealed."),
                        script.revealerBaseSprite, typeof(Sprite), true, GUILayout.Height(16));
                if (script.revealerBaseSprite == null)
                    script.revealerBaseSprite = (Sprite)AssetDatabase.LoadAssetAtPath("Assets/MT Assets/Easy Minimap System/Sprites/FogOfWarCircle.png", typeof(Sprite));
                Texture2D circle = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/MT Assets/Easy Minimap System/Editor/Images/Circle.png", typeof(Texture2D));
                Texture2D octagon = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/MT Assets/Easy Minimap System/Editor/Images/Octagon.png", typeof(Texture2D));
                Texture2D square = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/MT Assets/Easy Minimap System/Editor/Images/Square.png", typeof(Texture2D));
                if (GUILayout.Button(new GUIContent(circle, "Click here to use the Fog reveal method, with the \"Circle\" format."), GUILayout.Height(16), GUILayout.Width(32)))
                    script.revealerBaseSprite = (Sprite)AssetDatabase.LoadAssetAtPath("Assets/MT Assets/Easy Minimap System/Sprites/FogOfWarCircle.png", typeof(Sprite));
                if (GUILayout.Button(new GUIContent(octagon, "Click here to use the Fog reveal method, with the \"Octagon\" format."), GUILayout.Height(16), GUILayout.Width(32)))
                    script.revealerBaseSprite = (Sprite)AssetDatabase.LoadAssetAtPath("Assets/MT Assets/Easy Minimap System/Sprites/FogOfWarOctagon.png", typeof(Sprite));
                if (GUILayout.Button(new GUIContent(square, "Click here to use the Fog reveal method, with the \"Square\" format."), GUILayout.Height(16), GUILayout.Width(32)))
                    script.revealerBaseSprite = (Sprite)AssetDatabase.LoadAssetAtPath("Assets/MT Assets/Easy Minimap System/Sprites/FogOfWarSquare.png", typeof(Sprite));
                EditorGUILayout.EndHorizontal();

                //Gameplay Settings
                GUILayout.Space(10);
                EditorGUILayout.LabelField("Settings For Minimap Fogs", EditorStyles.boldLabel);
                GUILayout.Space(10);

                script.viewDistanceForFogs = (FieldOfViewForFogs)EditorGUILayout.EnumPopup(new GUIContent("View Dist. For Fogs",
                            "Here you can change the distance at which Targets will be able to see or reveal the terrain under the fog. Greater viewing distance will allow a larger area to be revealed."),
                            script.viewDistanceForFogs);

                EditorGUILayout.BeginHorizontal();
                script.fogsRevealedSaveId = EditorGUILayout.TextField(new GUIContent("Fogs Revealed Save ID",
                            "The ID of the save slot and load of save data for this Minimap Fog. With this component it is possible for you to save the current state and all revealed locations to be loaded later. The data is still saved even if the game is closed. The save is also completely managed by this component, you only need to enter the ID to access the save Slot and this ID functions as a Key. You can define any name you want, this ID will be used as a reference to Load or Save the revealed Fog data.\n\n** Saving and loading Fogs that have already been revealed is only possible with \"Paths Traveled By Targets\" enabled. **"),
                            script.fogsRevealedSaveId);
                if (script.fogsRevealedSaveId == "")
                {
                    DateTime dateTime = DateTime.Now;
                    script.fogsRevealedSaveId = "save_id_" + (dateTime.Ticks).ToString();
                }
                if (File.Exists(Application.persistentDataPath + "/" + script.fogsRevealedSaveId + ".bin") == true)
                    if (GUILayout.Button("Reset Save State", GUILayout.Height(18), GUILayout.Width(110)))
                    {
                        script.DeleteFileOfSaveStateOfFogOfWar();
                        Debug.Log("The currently existing Save State file, which saves your Editor Fog Of War data, has been deleted. Start your game again to create a new Save State file.");
                    }
                EditorGUILayout.EndHorizontal();

                //Targets Settings
                GUILayout.Space(10);
                EditorGUILayout.LabelField("Settings For Fog Revealing", EditorStyles.boldLabel);
                GUILayout.Space(10);

                Texture2D removeItemIcon = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/MT Assets/Easy Minimap System/Editor/Images/Remove.png", typeof(Texture2D));
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Targets That Can Remove Fog", GUILayout.Width(300));
                GUILayout.Space(MTAssetsEditorUi.GetInspectorWindowSize().x - 300);
                EditorGUILayout.LabelField("Size", GUILayout.Width(30));
                EditorGUILayout.IntField(script.targetsThatCanRemoveFog.Count, GUILayout.Width(50));
                EditorGUILayout.EndHorizontal();
                GUILayout.BeginVertical("box");
                targetsThatCanRemoveFog_ScrollPos = EditorGUILayout.BeginScrollView(targetsThatCanRemoveFog_ScrollPos, GUIStyle.none, GUI.skin.verticalScrollbar, GUILayout.Width(MTAssetsEditorUi.GetInspectorWindowSize().x), GUILayout.Height(100));
                if (script.targetsThatCanRemoveFog.Count == 0)
                    EditorGUILayout.HelpBox("Oops! No Transforms was registered to reveal the Fog! If you want to subscribe any, click the button below!", MessageType.Info);
                if (script.targetsThatCanRemoveFog.Count > 0)
                    for (int i = 0; i < script.targetsThatCanRemoveFog.Count; i++)
                    {
                        GUILayout.BeginHorizontal();
                        if (GUILayout.Button(removeItemIcon, GUILayout.Width(25), GUILayout.Height(16)))
                            script.targetsThatCanRemoveFog.RemoveAt(i);
                        script.targetsThatCanRemoveFog[i] = (Transform)EditorGUILayout.ObjectField(new GUIContent("Transform " + i.ToString(), "This Transform will reveal the Fog. Click the button to the left if you want to remove this Transform from the list."), script.targetsThatCanRemoveFog[i], typeof(Transform), true, GUILayout.Height(16));
                        GUILayout.EndHorizontal();
                    }
                EditorGUILayout.EndScrollView();
                GUILayout.EndVertical();
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("Add New Slot"))
                {
                    script.targetsThatCanRemoveFog.Add(null);
                    targetsThatCanRemoveFog_ScrollPos.y += 999999;
                }
                if (script.targetsThatCanRemoveFog.Count > 0)
                    if (GUILayout.Button("Remove Empty Slots", GUILayout.Width(Screen.width * 0.48f)))
                        for (int i = script.targetsThatCanRemoveFog.Count - 1; i >= 0; i--)
                            if (script.targetsThatCanRemoveFog[i] == null)
                                script.targetsThatCanRemoveFog.RemoveAt(i);
                GUILayout.EndHorizontal();

                //Events Settings
                GUILayout.Space(10);
                EditorGUILayout.LabelField("Minimap Fog Events", EditorStyles.boldLabel);
                GUILayout.Space(10);
                script.showMinimapFogEventsOptions = EditorGUILayout.Foldout(script.showMinimapFogEventsOptions, (script.showMinimapFogEventsOptions == true ? "Hide Minimap Fog Events Parameters" : "Show Minimap Fog Events Parameters"));
                if (script.showMinimapFogEventsOptions == true)
                {
                    EditorGUILayout.HelpBox("Events \"OnRevealFog\", \"OnUnrevealFog\", \"OnStartLoadState\" and \"OnDoneLoadState\" only work if the Fog Removal Method is equal to \"PathsTraveledByTargets\".", MessageType.Info);
                    DrawDefaultInspector();
                }

                //Set height in 0
                script.gameObject.transform.position = new Vector3(0, 0, 0);
                script.gameObject.transform.rotation = Quaternion.Euler(0, 0, 0);
                script.gameObject.transform.localScale = new Vector3(1, 1, 1);

                //Show a Warning
                GUILayout.Space(10);
                EditorGUILayout.HelpBox("It is highly recommended that you maintain only one \"Minimap Fog\" component per scene. Only one Minimap Fog component is able to manage the entire Minimap Fog in your scene. Don't worry, this is just an optimization tip!\n\nIf you need Fogs for different levels of height in your scene, you can use the C# API of this component for that. For example, if the player enters a Cave that is below height 0, you can use the API to reset the Fog and Load the Fog Revealed Save of that Cave. When the player leaves the Cave, save the revealed Fogs using the API of this component and load the Fogs from the surface. This is the most optimized and recommended way to do this.", MessageType.Info);

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
                MinimapFog script = (MinimapFog)target;
                EditorGUI.BeginChangeCheck();
                Undo.RecordObject(target, "Undo Event");

                //If application is running, not render the arrows
                if (Application.isPlaying == true)
                    return;

                //If not have a first Chunk, cancel
                if (script.currentExistingFogChunks.Count == 0)
                    return;

                //Set the color of controls
                Handles.color = Color.yellow;

                //List of positions that already added to dictionary sometime
                List<Vector3> alreadyAddedToDictionarySometime = new List<Vector3>();
                //List of positions of arrows already drawed
                Dictionary<Vector3, ArrowToBeDrawed> dictionaryOfArrowsToBeDrawedAndIdOfEachFogChunkInDb = new Dictionary<Vector3, ArrowToBeDrawed>();
                //Run a loop to get all data of all arrows to be drawed
                foreach (FogChunkItem fogChunk in script.currentExistingFogChunks)
                {
                    //Calculate the 4 points of area
                    Vector3 left = Vector3.Lerp(fogChunk.globalPositionOfTopLeftPoint, fogChunk.globalPositionOfBottomLeftPoint, 0.5f);
                    Vector3 right = Vector3.Lerp(fogChunk.globalPositionOfTopRightPoint, fogChunk.globalPositionOfBottomRightPoint, 0.5f);
                    Vector3 top = Vector3.Lerp(fogChunk.globalPositionOfTopRightPoint, fogChunk.globalPositionOfTopLeftPoint, 0.5f);
                    Vector3 bottom = Vector3.Lerp(fogChunk.globalPositionOfBottomRightPoint, fogChunk.globalPositionOfBottomLeftPoint, 0.5f);

                    //Add arrow of Left that is already drawed
                    if (dictionaryOfArrowsToBeDrawedAndIdOfEachFogChunkInDb.ContainsKey(left) == true)
                        dictionaryOfArrowsToBeDrawedAndIdOfEachFogChunkInDb.Remove(left);
                    if (dictionaryOfArrowsToBeDrawedAndIdOfEachFogChunkInDb.ContainsKey(left) == false && alreadyAddedToDictionarySometime.Contains(left) == false)
                    {
                        dictionaryOfArrowsToBeDrawedAndIdOfEachFogChunkInDb.Add(left, new ArrowToBeDrawed(ArrowToBeDrawedDirection.Left, fogChunk));
                        alreadyAddedToDictionarySometime.Add(left);
                    }

                    //Add arrow of Right that is already drawed
                    if (dictionaryOfArrowsToBeDrawedAndIdOfEachFogChunkInDb.ContainsKey(right) == true)
                        dictionaryOfArrowsToBeDrawedAndIdOfEachFogChunkInDb.Remove(right);
                    if (dictionaryOfArrowsToBeDrawedAndIdOfEachFogChunkInDb.ContainsKey(right) == false && alreadyAddedToDictionarySometime.Contains(right) == false)
                    {
                        dictionaryOfArrowsToBeDrawedAndIdOfEachFogChunkInDb.Add(right, new ArrowToBeDrawed(ArrowToBeDrawedDirection.Right, fogChunk));
                        alreadyAddedToDictionarySometime.Add(right);
                    }

                    //Add arrow of Top that is already drawed
                    if (dictionaryOfArrowsToBeDrawedAndIdOfEachFogChunkInDb.ContainsKey(top) == true)
                        dictionaryOfArrowsToBeDrawedAndIdOfEachFogChunkInDb.Remove(top);
                    if (dictionaryOfArrowsToBeDrawedAndIdOfEachFogChunkInDb.ContainsKey(top) == false && alreadyAddedToDictionarySometime.Contains(top) == false)
                    {
                        dictionaryOfArrowsToBeDrawedAndIdOfEachFogChunkInDb.Add(top, new ArrowToBeDrawed(ArrowToBeDrawedDirection.Forward, fogChunk));
                        alreadyAddedToDictionarySometime.Add(top);
                    }

                    //Add arrow of Bottom that is already drawed
                    if (dictionaryOfArrowsToBeDrawedAndIdOfEachFogChunkInDb.ContainsKey(bottom) == true)
                        dictionaryOfArrowsToBeDrawedAndIdOfEachFogChunkInDb.Remove(bottom);
                    if (dictionaryOfArrowsToBeDrawedAndIdOfEachFogChunkInDb.ContainsKey(bottom) == false && alreadyAddedToDictionarySometime.Contains(bottom) == false)
                    {
                        dictionaryOfArrowsToBeDrawedAndIdOfEachFogChunkInDb.Add(bottom, new ArrowToBeDrawed(ArrowToBeDrawedDirection.Back, fogChunk));
                        alreadyAddedToDictionarySometime.Add(bottom);
                    }
                }

                //Render arrows to increase fog size, on borders of all Fog Chunks
                foreach (var item in dictionaryOfArrowsToBeDrawedAndIdOfEachFogChunkInDb)
                {
                    //Calculate the size of button, taking account the current camera distance
                    float size = Vector3.Distance(Camera.current.transform.transform.position, item.Value.fogChunkItemOfArrow.globalPositionOfPivot) * 0.08f;

                    //Prepare the direction of arrow
                    Quaternion eulerAnglesOfArrow = Quaternion.Euler(0, 0, 0);
                    if (item.Value.directionOfArrow == ArrowToBeDrawedDirection.Forward)
                        eulerAnglesOfArrow = Quaternion.Euler(0, 0, 0);
                    if (item.Value.directionOfArrow == ArrowToBeDrawedDirection.Back)
                        eulerAnglesOfArrow = Quaternion.Euler(0, 180, 0);
                    if (item.Value.directionOfArrow == ArrowToBeDrawedDirection.Left)
                        eulerAnglesOfArrow = Quaternion.Euler(0, 270, 0);
                    if (item.Value.directionOfArrow == ArrowToBeDrawedDirection.Right)
                        eulerAnglesOfArrow = Quaternion.Euler(0, 90, 0);

                    //Render this arrow
                    if (Handles.Button(item.Key, eulerAnglesOfArrow, size, size, Handles.ArrowHandleCap) == true)
                    {
                        //Calculate the position of new Fog Chunk
                        Vector3 newFogChunkPosition = item.Key;
                        if (item.Value.directionOfArrow == ArrowToBeDrawedDirection.Forward)
                            newFogChunkPosition.z += CHUNK_RAY_SIZE_ON_UNITS;
                        if (item.Value.directionOfArrow == ArrowToBeDrawedDirection.Back)
                            newFogChunkPosition.z -= CHUNK_RAY_SIZE_ON_UNITS;
                        if (item.Value.directionOfArrow == ArrowToBeDrawedDirection.Left)
                            newFogChunkPosition.x -= CHUNK_RAY_SIZE_ON_UNITS;
                        if (item.Value.directionOfArrow == ArrowToBeDrawedDirection.Right)
                            newFogChunkPosition.x += CHUNK_RAY_SIZE_ON_UNITS;

                        script.CreateNewFogChunkOnPosition(newFogChunkPosition);
                    }
                }

                //Apply changes on script, case is not playing in editor
                if (GUI.changed == true && Application.isPlaying == false)
                {
                    EditorUtility.SetDirty(script);
                    UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(script.gameObject.scene);
                }
                if (EditorGUI.EndChangeCheck() == true)
                {
                    //Apply the change, if moved the handle
                    //script.transform.position = teste;
                }
                Repaint();
            }
        }

        public void OnDrawGizmosSelected()
        {
            //If not have a first Chunk, cancel
            if (currentExistingFogChunks.Count == 0)
                return;

            //Set color of gizmos
            Gizmos.color = Color.blue;
            Handles.color = Color.blue;

            //Render all Fog Chunks
            foreach (FogChunkItem fogChunk in currentExistingFogChunks)
            {
                //Set default color
                Handles.color = Color.blue;
                //Render the perimeter of this Fog Chunk
                Handles.DrawAAPolyLine(4.0f, new Vector3[] {
                    fogChunk.globalPositionOfBottomLeftPoint,
                    fogChunk.globalPositionOfBottomRightPoint,
                    fogChunk.globalPositionOfTopRightPoint,
                    fogChunk.globalPositionOfTopLeftPoint,
                    fogChunk.globalPositionOfBottomLeftPoint
                });
                //Draw the pivot of Chunk
                Gizmos.DrawSphere(fogChunk.globalPositionOfPivot, 2.0f);
                //If Fogs Removal Method is Reveal traveled paths
                if (fogRemovalMethod == FogRemovalMethod.PathsTraveledByTargets)
                {
                    //Prepare the fogs inside data
                    FogsParametersForGeneration fogsParams = GetFogsParametersForGenerationForFillingChunks();
                    Vector3 startingPos = fogChunk.globalPositionOfBottomLeftPoint;

                    //Draw the fogs inside Chunk
                    for (int x = 0; x < fogsParams.quantityInX; x++)
                        for (int y = 0; y < fogsParams.quantityInY; y++)
                        {
                            //Calculate position of this fog
                            Vector3 thisFogPosition = new Vector3(startingPos.x + (fogsParams.spaceBetweenEachFog * 0.5f) + (fogsParams.spaceBetweenEachFog * x),
                                                                  0,
                                                                  startingPos.z + (fogsParams.spaceBetweenEachFog * 0.5f) + (fogsParams.spaceBetweenEachFog * y)
                                                                  );
                            //Set default color for each fog
                            Handles.color = Color.gray;
                            //If distance is equal or minor than the desired GetSelectedViewDistanceForFogs, the fog will be blue
                            foreach (Transform trs in targetsThatCanRemoveFog)
                                if (trs != null)
                                    if (Vector3.Distance(thisFogPosition, trs.position) <= GetSelectedViewDistanceForFogs())
                                        Handles.color = Color.blue;
                            //Render the fog
                            Handles.DrawWireDisc(thisFogPosition, Vector3.up, fogsParams.sizeRadiusOfEachFog);
                        }
                }
            }

            //Render the View Distance For Fogs
            foreach (Transform trs in targetsThatCanRemoveFog)
            {
                //If transform is null, skip
                if (trs == null)
                    continue;

                //Draw the view distance for chunks (if desired)
                if (fogRemovalMethod == FogRemovalMethod.PathsTraveledByTargets)
                {
                    Handles.color = Color.cyan;
                    Handles.DrawWireDisc(trs.position, Vector3.up, GetSelectedViewDistanceForChunks());
                    Handles.DrawAAPolyLine(1.0f, new Vector3[] {
                        new Vector3(trs.position.x - GetSelectedViewDistanceForChunks(), 0, trs.position.z),
                        new Vector3(trs.position.x + GetSelectedViewDistanceForChunks(), 0, trs.position.z)
                    });
                    Handles.DrawAAPolyLine(1.0f, new Vector3[] {
                        new Vector3(trs.position.x, 0, trs.position.z - GetSelectedViewDistanceForChunks()),
                        new Vector3(trs.position.x, 0, trs.position.z + GetSelectedViewDistanceForChunks())
                    });
                }
                //Draw the view distance for fogs
                Handles.color = Color.green;
                Handles.DrawWireDisc(trs.position, Vector3.up, GetSelectedViewDistanceForFogs());
            }
        }
        #endregion

        //Core method for Editor

        public int CreateNewFogChunkOnPosition(Vector3 globalPosition)
        {
            //This method will create a new Fog Chunk on a determined position and return id of this new fog chunk
            FogChunkItem newFogChunk = new FogChunkItem();
            int idForThisNewFogChunk = currentExistingFogChunks.Count;
            newFogChunk.thisFogChunkId = idForThisNewFogChunk;
            newFogChunk.chunkSizeOfSides = CHUNK_SIDES_SIZE_ON_UNITS;
            newFogChunk.chunkSizeRay = CHUNK_RAY_SIZE_ON_UNITS;
            newFogChunk.globalPositionOfPivot = new Vector3(globalPosition.x, 0, globalPosition.z);
            newFogChunk.globalPositionOfBottomLeftPoint = new Vector3(globalPosition.x - CHUNK_RAY_SIZE_ON_UNITS, 0, globalPosition.z - CHUNK_RAY_SIZE_ON_UNITS);
            newFogChunk.globalPositionOfBottomRightPoint = new Vector3(globalPosition.x + CHUNK_RAY_SIZE_ON_UNITS, 0, globalPosition.z - CHUNK_RAY_SIZE_ON_UNITS);
            newFogChunk.globalPositionOfTopLeftPoint = new Vector3(globalPosition.x - CHUNK_RAY_SIZE_ON_UNITS, 0, globalPosition.z + CHUNK_RAY_SIZE_ON_UNITS);
            newFogChunk.globalPositionOfTopRightPoint = new Vector3(globalPosition.x + CHUNK_RAY_SIZE_ON_UNITS, 0, globalPosition.z + CHUNK_RAY_SIZE_ON_UNITS);

            //Add the new fog chunk to database
            currentExistingFogChunks.Add(newFogChunk);
            //Return id of recent created fog chunk
            return idForThisNewFogChunk;
        }
#endif

        //Core methods

        private int GetSelectedViewDistanceForChunks()
        {
            //Return the value of selected View Distance For Chunks
            switch (viewDistanceForChunks)
            {
                case FieldOfViewForChunks.Units250:
                    return 250;
                case FieldOfViewForChunks.Units400:
                    return 400;
            }
            return 0;
        }

        private int GetSelectedFogsQuantityPerChunk()
        {
            //Return the value of selected Fogs Quantity per Chunk
            switch (fogsQuantityPerChunk)
            {
                case QuantityOfFogsPerChunk.Fogs25:
                    return 25;
                case QuantityOfFogsPerChunk.Fogs36:
                    return 36;
                case QuantityOfFogsPerChunk.Fogs64:
                    return 64;
                case QuantityOfFogsPerChunk.Fogs100:
                    return 100;
                case QuantityOfFogsPerChunk.Fogs121:
                    return 121;
                case QuantityOfFogsPerChunk.Fogs144:
                    return 144;
            }
            return 0;
        }

        private int GetSelectedViewDistanceForFogs()
        {
            //Return the value of selected View Distance For Fogs
            switch (viewDistanceForFogs)
            {
                case FieldOfViewForFogs.Units20:
                    return 20;
                case FieldOfViewForFogs.Units25:
                    return 25;
                case FieldOfViewForFogs.Units30:
                    return 30;
                case FieldOfViewForFogs.Units35:
                    return 35;
                case FieldOfViewForFogs.Units40:
                    return 40;
                case FieldOfViewForFogs.Units45:
                    return 45;
                case FieldOfViewForFogs.Units50:
                    return 50;
                case FieldOfViewForFogs.Units55:
                    return 55;
                case FieldOfViewForFogs.Units60:
                    return 60;
                case FieldOfViewForFogs.Units65:
                    return 65;
                case FieldOfViewForFogs.Units70:
                    return 70;
                case FieldOfViewForFogs.Units75:
                    return 75;
                case FieldOfViewForFogs.Units80:
                    return 80;
                case FieldOfViewForFogs.Units85:
                    return 85;
                case FieldOfViewForFogs.Units90:
                    return 90;
                case FieldOfViewForFogs.Units95:
                    return 95;
                case FieldOfViewForFogs.Units100:
                    return 100;
                case FieldOfViewForFogs.Units105:
                    return 105;
                case FieldOfViewForFogs.Units110:
                    return 110;
                case FieldOfViewForFogs.Units115:
                    return 115;
                case FieldOfViewForFogs.Units120:
                    return 120;
            }
            return 0;
        }

        private FogsParametersForGeneration GetFogsParametersForGenerationForFillingChunks()
        {
            //Allocate the data to return
            FogsParametersForGeneration fogsParams = new FogsParametersForGeneration();

            //Fill the data
            if (GetSelectedFogsQuantityPerChunk() == 25)
            {
                fogsParams.quantityInX = 5;
                fogsParams.quantityInY = 5;
                fogsParams.spaceBetweenEachFog = 40;
                fogsParams.sizeRadiusOfEachFog = 30;
            }
            if (GetSelectedFogsQuantityPerChunk() == 36)
            {
                fogsParams.quantityInX = 6;
                fogsParams.quantityInY = 6;
                fogsParams.spaceBetweenEachFog = 34;
                fogsParams.sizeRadiusOfEachFog = 30;
            }
            if (GetSelectedFogsQuantityPerChunk() == 64)
            {
                fogsParams.quantityInX = 8;
                fogsParams.quantityInY = 8;
                fogsParams.spaceBetweenEachFog = 25;
                fogsParams.sizeRadiusOfEachFog = 25;
            }
            if (GetSelectedFogsQuantityPerChunk() == 100)
            {
                fogsParams.quantityInX = 10;
                fogsParams.quantityInY = 10;
                fogsParams.spaceBetweenEachFog = 20;
                fogsParams.sizeRadiusOfEachFog = 20;
            }
            if (GetSelectedFogsQuantityPerChunk() == 121)
            {
                fogsParams.quantityInX = 11;
                fogsParams.quantityInY = 11;
                fogsParams.spaceBetweenEachFog = 18;
                fogsParams.sizeRadiusOfEachFog = 20;
            }
            if (GetSelectedFogsQuantityPerChunk() == 144)
            {
                fogsParams.quantityInX = 12;
                fogsParams.quantityInY = 12;
                fogsParams.spaceBetweenEachFog = 17;
                fogsParams.sizeRadiusOfEachFog = 20;
            }

            //Return the data
            return fogsParams;
        }

        public void Awake()
        {
            //If not have the starting config, and not have chunks, cancel
            if (currentExistingFogChunks.Count == 0)
                return;

            //Create the holder, if not exists
            minimapDataHolderObj = GameObject.Find("Minimap Data Holder");
            if (minimapDataHolderObj == null)
            {
                minimapDataHolderObj = new GameObject("Minimap Data Holder");
                minimapDataHolder = minimapDataHolderObj.AddComponent<MinimapDataHolder>();
            }
            if (minimapDataHolderObj != null)
                minimapDataHolder = minimapDataHolderObj.GetComponent<MinimapDataHolder>();
            minimapFogsHolder = minimapDataHolderObj.transform.Find("Minimap Fogs Holder");
            if (minimapFogsHolder == null)
            {
                GameObject obj = new GameObject("Minimap Fogs Holder");
                minimapFogsHolder = obj.transform;
                minimapFogsHolder.SetParent(minimapDataHolderObj.transform);
                minimapFogsHolder.localPosition = Vector3.zero;
                minimapFogsHolder.localEulerAngles = Vector3.zero;
            }
            if (minimapDataHolder.instancesOfMinimapFogsInThisScene.Contains(this) == false)
                minimapDataHolder.instancesOfMinimapFogsInThisScene.Add(this);

            //Create the minimap fog
            tempBaseGameObj = new GameObject("Minimap Fog (" + this.gameObject.transform.name + ")");
            tempBaseGameObj.transform.SetParent(minimapFogsHolder);
            tempBaseGameObj.transform.position = new Vector3(this.gameObject.transform.position.x, BASE_HEIGHT_IN_3D_WORLD, this.gameObject.transform.position.z);
            tempBaseGameObj.layer = LayerMask.NameToLayer("UI");

            //Add the activity monitor to the camera
            ActivityMonitor activeMonitor = tempBaseGameObj.AddComponent<ActivityMonitor>();
            activeMonitor.responsibleScriptComponentForThis = this;

            //------------ Start of FOG creation ------------ 

            //Reset all generated static fogs revealers in all chunks, to avoid crashs
            foreach (FogChunkItem fog in currentExistingFogChunks)
                fog.fogsOfThisChunk.Clear();

            //Create a sprite fog for each chunk
            foreach (FogChunkItem fog in currentExistingFogChunks)
            {
                //Create the GameObject of fog for this chunk
                GameObject fogObj = new GameObject("Chunk " + fog.thisFogChunkId);
                fogObj.transform.SetParent(tempBaseGameObj.transform);
                fogObj.transform.position = new Vector3(fog.globalPositionOfPivot.x, BASE_HEIGHT_IN_3D_WORLD, fog.globalPositionOfPivot.z);
                fogObj.layer = LayerMask.NameToLayer("UI");
                //Add the sprite renderer and set correct size for fog
                SpriteRenderer spriteRenderer = fogObj.AddComponent<SpriteRenderer>();
                spriteRenderer.sprite = fogsBaseSprite;
                spriteRenderer.sortingOrder = DEFAULT_ORDER_IN_LAYER_FOR_SPRITE;
                spriteRenderer.color = fogBaseColor;
                spriteRenderer.maskInteraction = SpriteMaskInteraction.VisibleOutsideMask;
                //Set the correct size for this sprite. Calculate the final size of the sprite in world, in meters, according the chunk size (add 4 units to chunk size to avoid incorrects borders)
                float areaResolutionAspectInSpriteRendererX = (float)(CHUNK_SIDES_SIZE_ON_UNITS + 4) / (float)fogsBaseSprite.texture.width;
                float areaResolutionAspectInSpriteRendererY = (float)(CHUNK_SIDES_SIZE_ON_UNITS + 4) / (float)fogsBaseSprite.texture.height;
                fogObj.transform.localScale = new Vector3(areaResolutionAspectInSpriteRendererX * spriteRenderer.sprite.pixelsPerUnit, areaResolutionAspectInSpriteRendererY * spriteRenderer.sprite.pixelsPerUnit, 1.0f);
                //Adjust the rotation
                fogObj.transform.localEulerAngles = new Vector3(90, 0, 0);
                //Add this new fog to database
                fog.fogChunkGameObject = fogObj;
            }
            //If FogsRemovalMethod is PathsTraveledByTargets create all Static Fog Removal to fill all chunks of fog
            if (fogRemovalMethod == FogRemovalMethod.PathsTraveledByTargets)
            {
                //Get the selected quantity of fogs per chunk and create parameteres
                FogsParametersForGeneration fogsParameters = GetFogsParametersForGenerationForFillingChunks();
                Vector3 startingPos = Vector3.zero;

                //Fill each chunk of fog, with Statics Fogs Removals disableds
                foreach (FogChunkItem fog in currentExistingFogChunks)
                {
                    //Get starting pos
                    startingPos = fog.globalPositionOfBottomLeftPoint;

                    //Fill with Statics Fogs Removals
                    for (int x = 0; x < fogsParameters.quantityInX; x++)
                        for (int y = 0; y < fogsParameters.quantityInY; y++)
                        {
                            //Calculate position of this fog
                            Vector3 thisFogPosition = new Vector3(startingPos.x + (fogsParameters.spaceBetweenEachFog * 0.5f) + (fogsParameters.spaceBetweenEachFog * x),
                                                                  BASE_HEIGHT_IN_3D_WORLD,
                                                                  startingPos.z + (fogsParameters.spaceBetweenEachFog * 0.5f) + (fogsParameters.spaceBetweenEachFog * y));
                            //Create the GameObject of fog
                            GameObject fogRemovalObj = new GameObject("Static Fog Removal");
                            fogRemovalObj.transform.position = thisFogPosition;
                            fogRemovalObj.layer = LayerMask.NameToLayer("UI");
                            //Add the sprite mask and set correct size for fog removal
                            SpriteMask spriteMask = fogRemovalObj.AddComponent<SpriteMask>();
                            spriteMask.sprite = revealerBaseSprite;
                            spriteMask.alphaCutoff = 0.0f;
                            //Set the correct size for this sprite. Calculate the final size of the sprite in world, in meters, according the chunk size (add 4 units to chunk size to avoid incorrects borders)
                            float areaResolutionAspectInSpriteRendererX = (float)(fogsParameters.sizeRadiusOfEachFog * 2.0f) / (float)fogsBaseSprite.texture.width;
                            float areaResolutionAspectInSpriteRendererY = (float)(fogsParameters.sizeRadiusOfEachFog * 2.0f) / (float)fogsBaseSprite.texture.height;
                            fogRemovalObj.transform.localScale = new Vector3(areaResolutionAspectInSpriteRendererX * spriteMask.sprite.pixelsPerUnit, areaResolutionAspectInSpriteRendererY * spriteMask.sprite.pixelsPerUnit, 1.0f);
                            fogRemovalObj.transform.SetParent(fog.fogChunkGameObject.transform);
                            fogRemovalObj.transform.localScale = new Vector3(fogRemovalObj.transform.localScale.x, fogRemovalObj.transform.localScale.x, 1.0f);
                            //Adjust the rotation
                            fogRemovalObj.transform.localEulerAngles = new Vector3(0, 0, 0);
                            //Disable this revealer
                            fogRemovalObj.SetActive(false);
                            //Add this new fog to database
                            fog.fogsOfThisChunk.Add(fogRemovalObj);
                        }
                }
            }
            //If FogsRemovalMethod is OnlyTargetsCurrentPosition create all Dynamic Fog Removal pool
            if (fogRemovalMethod == FogRemovalMethod.OnlyTargetsCurrentPosition)
            {
                //Create the pool holder
                GameObject poolObj = new GameObject("Pool");
                poolObj.transform.SetParent(tempBaseGameObj.transform);
                poolObj.transform.position = new Vector3(0, BASE_HEIGHT_IN_3D_WORLD, 0);
                poolObj.layer = LayerMask.NameToLayer("UI");
                //Create the pool of dynamic fog revealers
                for (int i = 0; i < MAX_QUANTITY_OF_DYNAMICS_FOG_REVEALERS_IN_POOL; i++)
                {
                    //Create the GameObject of fog
                    GameObject fogRemovalObj = new GameObject("Dynamic Fog Removal");
                    fogRemovalObj.transform.SetParent(poolObj.transform);
                    fogRemovalObj.transform.position = new Vector3(0, BASE_HEIGHT_IN_3D_WORLD, 0);
                    fogRemovalObj.layer = LayerMask.NameToLayer("UI");
                    //Add the sprite mask and set correct size for fog removal
                    SpriteMask spriteMask = fogRemovalObj.AddComponent<SpriteMask>();
                    spriteMask.sprite = revealerBaseSprite;
                    spriteMask.alphaCutoff = 0.0f;
                    //Adjust the rotation
                    fogRemovalObj.transform.localEulerAngles = new Vector3(90, 0, 0);
                    //Disable this revealer
                    fogRemovalObj.SetActive(false);
                    //Add this new fog to database
                    dynamicFogsRevealersPoolAndAssociatedTransforms.Add(spriteMask, null);
                }
            }

            //------------- End of FOG creation ------------- 
        }

        public void Update()
        {
            //If not have the starting config, and not have chunks, cancel
            if (currentExistingFogChunks.Count == 0)
                return;

            //If the Minimap Item created by this component is disabled, enable it
            if (tempBaseGameObj.activeSelf == false)
                tempBaseGameObj.SetActive(true);

            //Run update of each revealing method
            if (fogRemovalMethod == FogRemovalMethod.PathsTraveledByTargets)
                Update_PathsTraveledByTargets();
            if (fogRemovalMethod == FogRemovalMethod.OnlyTargetsCurrentPosition)
                Update_OnlyTargetsCurrentPosition();

            //Set height and rotation in zero
            this.transform.position = new Vector3(0, 0, 0);
            this.transform.rotation = Quaternion.Euler(0, 0, 0);
            this.transform.localScale = new Vector3(1, 1, 1);

            //Move the scan to follow this gameobject
            tempBaseGameObj.transform.position = new Vector3(0, BASE_HEIGHT_IN_3D_WORLD, 0);
            //Rotate the scan
            tempBaseGameObj.transform.rotation = Quaternion.Euler(0, 0, 0);
        }

        //Core methods for PathsTraveledByTargets

        public void OnDisable()
        {
            //Notify that the routine is not running
            isTargetsPathsRevealerRoutineRunning = false;
        }

        private void Update_PathsTraveledByTargets()
        {
            //If the routine of revealer is not running, start the routine
            if (isTargetsPathsRevealerRoutineRunning == false)
            {
                StartCoroutine(Update_TargetsPathsRevealerLooper());
                isTargetsPathsRevealerRoutineRunning = true;
            }
        }

        private float GetCorrespondingTimeBetweenEachUpdateAccordingToFogsUpdatesFrequency()
        {
            //Return the time interval that coroutines of fogs updates calculation will run updates
            switch (fogsUpdateFrequency)
            {
                case FogsUpdateFrequency.x1PerSecond:
                    return 1.0f;
                case FogsUpdateFrequency.x2PerSecond:
                    return 0.5f;
                case FogsUpdateFrequency.x3PerSecond:
                    return 0.33f;
                case FogsUpdateFrequency.x4PerSecond:
                    return 0.25f;
                case FogsUpdateFrequency.x5PerSecond:
                    return 0.2f;
                case FogsUpdateFrequency.x10PerSecond:
                    return 0.1f;
                case FogsUpdateFrequency.x15PerSecond:
                    return 0.066f;
                case FogsUpdateFrequency.x30PerSecond:
                    return 0.033f;
                case FogsUpdateFrequency.x45PerSecond:
                    return 0.022f;
                case FogsUpdateFrequency.x60PerSecond:
                    return 0.016f;
            }

            //If not found a valid value
            return 1.0f;
        }

        private void RevealAllFogsCloserAtThisGlobalPosition(Vector3 globalPosition)
        {
            //Run the loop to found all statics fogs revealers most closer to this global position
            foreach (FogChunkItem fog in currentExistingFogChunks)
            {
                //Calculate distance between this chunk and global position
                float chunkDistance = Vector3.Distance(new Vector3(globalPosition.x, 0, globalPosition.z), new Vector3(fog.globalPositionOfPivot.x, 0, fog.globalPositionOfPivot.z));

                //If this chunk is on field of view for chunks, check each static fog revealer of this chunk to enable most closests to global position
                if (chunkDistance <= GetSelectedViewDistanceForChunks())
                    foreach (GameObject staticFogRevealer in fog.fogsOfThisChunk)
                    {
                        //If this fog already is enabled, skip
                        if (staticFogRevealer.activeSelf == true)
                            continue;

                        //Calculate distance between this static fog revealer and global position
                        float fogDistance = Vector3.Distance(new Vector3(globalPosition.x, 0, globalPosition.z), new Vector3(staticFogRevealer.transform.position.x, 0, staticFogRevealer.transform.position.z));

                        //If this static fog revealer is closest to global position, mark this to be activated
                        if (fogDistance <= GetSelectedViewDistanceForFogs())
                            if (staticsFogsToBeEnabled_byRevealAllFogsCloserAtThisGlobalPosition.Contains(staticFogRevealer) == false)
                            {
                                staticsFogsToBeEnabled_byRevealAllFogsCloserAtThisGlobalPosition.Add(staticFogRevealer);

                                //Call the event to sign that this transform position is being revealed
                                if (onRevealFog != null)
                                    onRevealFog.Invoke(new Vector3(staticFogRevealer.transform.position.x, 0, staticFogRevealer.transform.position.z));
                            }
                    }
            }
        }

        private void UnRevealAllFogsCloserAtThisGlobalPosition(Vector3 globalPosition)
        {
            //Run the loop to found all statics fogs revealers most closer to this global position
            foreach (FogChunkItem fog in currentExistingFogChunks)
            {
                //Calculate distance between this chunk and global position
                float chunkDistance = Vector3.Distance(new Vector3(globalPosition.x, 0, globalPosition.z), new Vector3(fog.globalPositionOfPivot.x, 0, fog.globalPositionOfPivot.z));

                //If this chunk is on field of view for chunks, check each static fog revealer of this chunk to disable most closests to global position
                if (chunkDistance <= GetSelectedViewDistanceForChunks())
                    foreach (GameObject staticFogRevealer in fog.fogsOfThisChunk)
                    {
                        //If this fog already is disabled, skip
                        if (staticFogRevealer.activeSelf == false)
                            continue;

                        //Calculate distance between this static fog revealer and global position
                        float fogDistance = Vector3.Distance(new Vector3(globalPosition.x, 0, globalPosition.z), new Vector3(staticFogRevealer.transform.position.x, 0, staticFogRevealer.transform.position.z));

                        //If this static fog revealer is closest to global position, mark this to be deactivated
                        if (fogDistance <= GetSelectedViewDistanceForFogs())
                            if (staticsFogsToBeDisabled_byUnRevealAllFogsCloserAtThisGlobalPosition.Contains(staticFogRevealer) == false)
                            {
                                staticsFogsToBeDisabled_byUnRevealAllFogsCloserAtThisGlobalPosition.Add(staticFogRevealer);

                                //Call the event to sign that this transform position is being unrevealed
                                if (onUnrevealFog != null)
                                    onUnrevealFog.Invoke(new Vector3(staticFogRevealer.transform.position.x, 0, staticFogRevealer.transform.position.z));
                            }
                    }
            }
        }

        private bool isTheFogMostCloserAtThisGlobalPositionRevealed(Vector3 globalPosition)
        {
            //Prepare the response
            bool isRevealed = false;
            //Best static fog most closer to global position
            float distanceOfBestFog = float.MaxValue;
            GameObject bestFog = null;

            //Find the best fog
            foreach (FogChunkItem fog in currentExistingFogChunks)
            {
                //Calculate distance between this chunk and global position
                float chunkDistance = Vector3.Distance(new Vector3(globalPosition.x, 0, globalPosition.z), new Vector3(fog.globalPositionOfPivot.x, 0, fog.globalPositionOfPivot.z));

                //If this chunk is on field of view for chunks, check each static fog revealer of this chunk
                if (chunkDistance <= GetSelectedViewDistanceForChunks())
                    foreach (GameObject staticFogRevealer in fog.fogsOfThisChunk)
                    {
                        //Calculate distance between this static fog revealer and global position
                        float fogDistance = Vector3.Distance(new Vector3(globalPosition.x, 0, globalPosition.z), new Vector3(staticFogRevealer.transform.position.x, 0, staticFogRevealer.transform.position.z));

                        //If this static fog revealer is closest to global position, select this as best fog
                        if (fogDistance <= distanceOfBestFog)
                        {
                            bestFog = staticFogRevealer;
                            distanceOfBestFog = fogDistance;
                        }
                    }
            }

            //Process the response
            if (bestFog == null)
                isRevealed = false;
            if (bestFog != null && bestFog.activeSelf == true)
                isRevealed = true;
            //Return the response
            return isRevealed;
        }

        private IEnumerator Update_TargetsPathsRevealerLooper()
        {
            //Set the time between updates
            WaitForSecondsRealtime timeBetweenUpdates = new WaitForSecondsRealtime(GetCorrespondingTimeBetweenEachUpdateAccordingToFogsUpdatesFrequency());
            //Start the coroutine of the static fogs enabler and disabler looper
            StartCoroutine(Update_AutoEnablerAndDisablerOfStaticsFogsRevealersLooper());

            //Start the loop of revealing
            while (this.enabled == true)
            {
                //Reveal all fogs most closer to targets positions
                for (int i = 0; i < targetsThatCanRemoveFog.Count; i++)
                {
                    //Get the target
                    Transform target = targetsThatCanRemoveFog[i];

                    //Reveal the target
                    if (target != null)
                        RevealAllFogsCloserAtThisGlobalPosition(target.position);
                }

                //Wait the interval of loop
                yield return timeBetweenUpdates;
            }
        }

        private IEnumerator Update_AutoEnablerAndDisablerOfStaticsFogsRevealersLooper()
        {
            //Set the time between each activation
            WaitForSeconds timeBetweenUpdates = new WaitForSeconds(DELAY_BETWEEN_EACH_STATIC_FOG_ACTIVATION_OR_DEACTIVATION);
            //Prepare the cache of iterations count
            int iterationCount = 0;
            //Prepare the settings for activations or deactivations max iteration count for each method or tasks
            int maxIterationsFor_RevealAllFogsCloserAtThisGlobalPosition = 5;
            int maxIterationsFor_UnRevealAllFogsCloserAtThisGlobalPosition = 5;
            int maxIterationsFor_ResetAllEntireFogOfWar = 25;
            int maxIterationsFor_LoadStateOfFogOfWar = 25;

            //Start the loop of enable or disabling static fogs
            while (this.enabled == true)
            {
                //Wait delay for entry of this update
                yield return timeBetweenUpdates;

                //----------------------- LoadStateOfFogOfWar() -------------------------
                //Call the event to notify the start of fog load state, if have statics fogs to enable
                if (staticsFogsToBeEnabled_byLoadStateOfFogOfWar.Count > 0)
                    if (onStartLoadState != null)
                        onStartLoadState.Invoke();
                //If have statics fogs to enable, reset all statics fogs of war, disabling all
                if (staticsFogsToBeEnabled_byLoadStateOfFogOfWar.Count > 0)
                    for (int x = 0; x < currentExistingFogChunks.Count; x++) //<-- Run a loop that will mark all static fogs to be disabled
                        for (int y = 0; y < currentExistingFogChunks[x].fogsOfThisChunk.Count; y++) //<-- Check all static fogs of this chunk
                        {
                            //Enable or Disable this
                            if (currentExistingFogChunks[x].fogsOfThisChunk[y] != null && currentExistingFogChunks[x].fogsOfThisChunk[y].activeSelf == true)
                            {
                                currentExistingFogChunks[x].fogsOfThisChunk[y].SetActive(false);
                                //Increase the iteration counter
                                iterationCount += 1;
                            }
                            //If rechead the max of iterations in this update, wait a delay
                            if (iterationCount >= maxIterationsFor_ResetAllEntireFogOfWar) //<- X is the max of gameobjects activation or deactivation per each update
                            {
                                iterationCount = 0;
                                yield return timeBetweenUpdates;
                            }
                        }
                iterationCount = 0;
                //Enable all statics fogs of war that is in the list
                for (int i = 0; i < staticsFogsToBeEnabled_byLoadStateOfFogOfWar.Count; i++)
                {
                    //Enable or Disable this
                    if (staticsFogsToBeEnabled_byLoadStateOfFogOfWar[i] != null)
                        staticsFogsToBeEnabled_byLoadStateOfFogOfWar[i].SetActive(true);
                    //Increase the iteration counter
                    iterationCount += 1;
                    //If rechead the max of iterations in this update, wait a delay
                    if (iterationCount >= maxIterationsFor_LoadStateOfFogOfWar) //<- X is the max of gameobjects activation or deactivation per each update
                    {
                        iterationCount = 0;
                        yield return timeBetweenUpdates;
                    }
                }
                if (staticsFogsToBeEnabled_byLoadStateOfFogOfWar.Count > 0) //<-- Call the event to notify the end of fog load state, if have statics fogs to enable
                    if (onDoneLoadState != null)
                        onDoneLoadState.Invoke();
                staticsFogsToBeEnabled_byLoadStateOfFogOfWar.Clear();
                iterationCount = 0;

                //----------------------- RevealAllFogsCloserAtThisGlobalPosition() -------------------------
                for (int i = 0; i < staticsFogsToBeEnabled_byRevealAllFogsCloserAtThisGlobalPosition.Count; i++)
                {
                    //Enable or Disable this
                    if (staticsFogsToBeEnabled_byRevealAllFogsCloserAtThisGlobalPosition[i] != null)
                        staticsFogsToBeEnabled_byRevealAllFogsCloserAtThisGlobalPosition[i].SetActive(true);
                    //Increase the iteration counter
                    iterationCount += 1;
                    //If rechead the max of iterations in this update, wait a delay
                    if (iterationCount >= maxIterationsFor_RevealAllFogsCloserAtThisGlobalPosition) //<- X is the max of gameobjects activation or deactivation per each update
                    {
                        iterationCount = 0;
                        yield return timeBetweenUpdates;
                    }
                }
                staticsFogsToBeEnabled_byRevealAllFogsCloserAtThisGlobalPosition.Clear();
                iterationCount = 0;

                //----------------------- UnRevealAllFogsCloserAtThisGlobalPosition() -------------------------
                for (int i = 0; i < staticsFogsToBeDisabled_byUnRevealAllFogsCloserAtThisGlobalPosition.Count; i++)
                {
                    //Enable or Disable this
                    if (staticsFogsToBeDisabled_byUnRevealAllFogsCloserAtThisGlobalPosition[i] != null)
                        staticsFogsToBeDisabled_byUnRevealAllFogsCloserAtThisGlobalPosition[i].SetActive(false);
                    //Increase the iteration counter
                    iterationCount += 1;
                    //If rechead the max of iterations in this update, wait a delay
                    if (iterationCount >= maxIterationsFor_UnRevealAllFogsCloserAtThisGlobalPosition) //<- X is the max of gameobjects activation or deactivation per each update
                    {
                        iterationCount = 0;
                        yield return timeBetweenUpdates;
                    }
                }
                staticsFogsToBeDisabled_byUnRevealAllFogsCloserAtThisGlobalPosition.Clear();
                iterationCount = 0;

                //Wait delay for exit of this update
                yield return timeBetweenUpdates;
            }
        }

        //Core methods for OnlyTargetsCurrentPosition

        private void Update_OnlyTargetsCurrentPosition()
        {
            //Update the sizes of all Dynamics Revealers, if is changed the view distance or not updated yet
            if (viewDistanceForFogs != lastViewDistanceForFogs || isDynamicFogsRevealersUpdatedsInSize == false)
            {
                //Update size of all
                foreach (var item in dynamicFogsRevealersPoolAndAssociatedTransforms)
                {
                    float areaResolutionAspectInSpriteRendererX = (float)(GetSelectedViewDistanceForFogs() * 2.0f) / (float)fogsBaseSprite.texture.width;
                    float areaResolutionAspectInSpriteRendererY = (float)(GetSelectedViewDistanceForFogs() * 2.0f) / (float)fogsBaseSprite.texture.height;
                    item.Key.transform.localScale = new Vector3(areaResolutionAspectInSpriteRendererX * item.Key.sprite.pixelsPerUnit, areaResolutionAspectInSpriteRendererY * item.Key.sprite.pixelsPerUnit, 1.0f);
                }

                //Update the cache
                lastViewDistanceForFogs = viewDistanceForFogs;
                isDynamicFogsRevealersUpdatedsInSize = true;
            }

        RestartTheLoopOfDynamicsRevealersActivations:
            //Disable all dynamics revealers of pool that not contains more transforms binded
            foreach (var item in dynamicFogsRevealersPoolAndAssociatedTransforms)
            {
                //If the associated transform is not null
                if (item.Value != null)
                {
                    //Check if the associated transform exists
                    bool contains = targetsThatCanRemoveFog.Contains(item.Value);
                    //If the associated transform not exists in the list of targets more, disable
                    if (contains == false)
                    {
                        item.Key.gameObject.SetActive(false);
                        dynamicFogsRevealersPoolAndAssociatedTransforms[item.Key] = null;
                        goto RestartTheLoopOfDynamicsRevealersActivations;
                    }
                    //If the associated transform exists in the list of targets more, remove the association only to association be renewed below in script
                    if (contains == true)
                    {
                        dynamicFogsRevealersPoolAndAssociatedTransforms[item.Key] = null;
                        goto RestartTheLoopOfDynamicsRevealersActivations;
                    }
                }
                //If the associated transform is null, disable
                if (item.Value == null)
                    item.Key.gameObject.SetActive(false);
            }

            //Add all targets to be referenced on your own dynamic revealer
            for (int i = 0; i < targetsThatCanRemoveFog.Count; i++)
            {
                //If is null, skip
                if (targetsThatCanRemoveFog[i] == null)
                    continue;

                //Found a dynamic revealer to associate with this target
                foreach (var item in dynamicFogsRevealersPoolAndAssociatedTransforms)
                    if (item.Value == null)
                    {
                        dynamicFogsRevealersPoolAndAssociatedTransforms[item.Key] = targetsThatCanRemoveFog[i];
                        item.Key.gameObject.SetActive(true);
                        break;
                    }
            }

            //Update the position of dynamic revealer, for all revealers that is enabled
            foreach (var item in dynamicFogsRevealersPoolAndAssociatedTransforms)
                if (item.Key.gameObject.activeSelf == true)
                    item.Key.transform.position = new Vector3(item.Value.position.x, BASE_HEIGHT_IN_3D_WORLD, item.Value.position.z);
        }

        //Public methods (only available if PathsTraveledByTargets is Enabled)

        public void RevealThisWorldPosition(Vector3 globalPosition)
        {
            //If the fog removal method is not PathsTraveledByTargets
            if (fogRemovalMethod != FogRemovalMethod.PathsTraveledByTargets)
            {
                Debug.LogError("Could not execute Minimap Fog \"RevealThisWorldPosition()\" method on \"" + this.gameObject.name + "\". This method is only available if \"fogRemovalMethod\" is selected as PathsTraveledByTargets.");
                return;
            }

            //This method will reveal a world position
            RevealAllFogsCloserAtThisGlobalPosition(globalPosition);
        }

        public void UnRevealThisWorldPosition(Vector3 globalPosition)
        {
            //If the fog removal method is not PathsTraveledByTargets
            if (fogRemovalMethod != FogRemovalMethod.PathsTraveledByTargets)
            {
                Debug.LogError("Could not execute Minimap Fog \"UnRevealThisWorldPosition()\" method on \"" + this.gameObject.name + "\". This method is only available if \"fogRemovalMethod\" is selected as PathsTraveledByTargets.");
                return;
            }

            //This method will unreveal a world position
            UnRevealAllFogsCloserAtThisGlobalPosition(globalPosition);
        }

        public bool isThisWorldPositionRevealed(Vector3 globalPosition)
        {
            //If the fog removal method is not PathsTraveledByTargets
            if (fogRemovalMethod != FogRemovalMethod.PathsTraveledByTargets)
            {
                Debug.LogError("Could not execute Minimap Fog \"isThisWorldPositionRevealed()\" method on \"" + this.gameObject.name + "\". This method is only available if \"fogRemovalMethod\" is selected as PathsTraveledByTargets.");
                return false;
            }

            //Return true if the fog most closer of this global position, is already revealed
            return isTheFogMostCloserAtThisGlobalPositionRevealed(globalPosition);
        }

        public void SaveStateOfFogOfWar()
        {
            //If the fog removal method is not PathsTraveledByTargets
            if (fogRemovalMethod != FogRemovalMethod.PathsTraveledByTargets)
            {
                Debug.LogError("Could not execute Minimap Fog \"SaveStateOfFogOfWar()\" method on \"" + this.gameObject.name + "\". This method is only available if \"fogRemovalMethod\" is selected as PathsTraveledByTargets.");
                return;
            }

            //Create the SaveStateDataOfFogOfWar object
            SaveStateDataOfFogOfWar saveData = new SaveStateDataOfFogOfWar();

            //Prepare the list of all statics fogs revealeds
            List<Vector3> allStaticFogsRevealedPositions = new List<Vector3>();
            //Get positions of all statics fogs revealeds
            foreach (FogChunkItem fog in currentExistingFogChunks)
                foreach (GameObject staticFog in fog.fogsOfThisChunk)
                    if (staticFog != null && staticFog.activeSelf == true)
                        allStaticFogsRevealedPositions.Add(staticFog.transform.position);
            //Prepare the save data
            saveData.allStaticFogsRevealeds_xPos = new float[allStaticFogsRevealedPositions.Count];
            saveData.allStaticFogsRevealeds_yPos = new float[allStaticFogsRevealedPositions.Count];
            saveData.allStaticFogsRevealeds_zPos = new float[allStaticFogsRevealedPositions.Count];
            //Fill the save data
            for (int i = 0; i < allStaticFogsRevealedPositions.Count; i++)
            {
                saveData.allStaticFogsRevealeds_xPos[i] = allStaticFogsRevealedPositions[i].x;
                saveData.allStaticFogsRevealeds_yPos[i] = allStaticFogsRevealedPositions[i].y;
                saveData.allStaticFogsRevealeds_zPos[i] = allStaticFogsRevealedPositions[i].z;
            }

            //Save the data in file
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            FileStream file = File.Create(Application.persistentDataPath + "/" + fogsRevealedSaveId + ".bin");
            binaryFormatter.Serialize(file, saveData);
            file.Close();
        }

        public bool LoadStateOfFogOfWarAsync()
        {
            //If the fog removal method is not PathsTraveledByTargets
            if (fogRemovalMethod != FogRemovalMethod.PathsTraveledByTargets)
            {
                Debug.LogError("Could not execute Minimap Fog \"LoadStateOfFogOfWarAsync()\" method on \"" + this.gameObject.name + "\". This method is only available if \"fogRemovalMethod\" is selected as PathsTraveledByTargets.");
                //Return that is not possible to load save state
                return false;
            }

            //Create the SaveStateDataOfFogOfWar object
            SaveStateDataOfFogOfWar saveData = new SaveStateDataOfFogOfWar();
            //Check if file exists
            bool saveExists = File.Exists(Application.persistentDataPath + "/" + fogsRevealedSaveId + ".bin");

            //If the save file exists, load state of this
            if (saveExists == true)
            {
                //Load the file
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                FileStream file = File.Open(Application.persistentDataPath + "/" + fogsRevealedSaveId + ".bin", FileMode.Open);
                saveData = (SaveStateDataOfFogOfWar)binaryFormatter.Deserialize(file);
                file.Close();

                //Get all positions of revealing saved in file to a dictionary
                Dictionary<Vector3, int> allPositionsOfFile = new Dictionary<Vector3, int>();
                for (int i = 0; i < saveData.allStaticFogsRevealeds_xPos.Length; i++)
                {
                    //Fill the dictionary
                    Vector3 position = new Vector3(saveData.allStaticFogsRevealeds_xPos[i], saveData.allStaticFogsRevealeds_yPos[i], saveData.allStaticFogsRevealeds_zPos[i]);
                    //If not exists in dictionary, add this
                    if (allPositionsOfFile.ContainsKey(position) == false)
                        allPositionsOfFile.Add(position, 0);
                }

                //Mark to enable all static fog of war revealer that matches with positions saved
                foreach (FogChunkItem fog in currentExistingFogChunks)
                    foreach (GameObject staticFog in fog.fogsOfThisChunk)
                        if (staticFog != null && allPositionsOfFile.ContainsKey(staticFog.transform.position) == true)
                            if (staticsFogsToBeEnabled_byLoadStateOfFogOfWar.Contains(staticFog) == false)
                                staticsFogsToBeEnabled_byLoadStateOfFogOfWar.Add(staticFog);
            }
            //If the file not exists
            if (saveExists == false)
                Debug.LogError("Unable to Load a Fog Of War State in Minimap Fog \"" + this.gameObject.name + "\" as the Save ID \"" + fogsRevealedSaveId + "\" was never saved before, or the Save file was corrupted.");

            //Return if was possible to load the save file
            return saveExists;
        }

        public bool isFileOfSaveSateOfFogOfWarExistant()
        {
            //If the fog removal method is not PathsTraveledByTargets
            if (fogRemovalMethod != FogRemovalMethod.PathsTraveledByTargets)
            {
                Debug.LogError("Could not execute Minimap Fog \"isFileOfSaveSateOfFogOfWarExistant()\" method on \"" + this.gameObject.name + "\". This method is only available if \"fogRemovalMethod\" is selected as PathsTraveledByTargets.");
                return false;
            }

            //Return true if file of save state of fog of war, is existant
            return File.Exists(Application.persistentDataPath + "/" + fogsRevealedSaveId + ".bin");
        }

        public void DeleteFileOfSaveStateOfFogOfWar()
        {
            //If the fog removal method is not PathsTraveledByTargets
            if (fogRemovalMethod != FogRemovalMethod.PathsTraveledByTargets)
            {
                Debug.LogError("Could not execute Minimap Fog \"DeleteFileOfSaveStateOfFogOfWar()\" method on \"" + this.gameObject.name + "\". This method is only available if \"fogRemovalMethod\" is selected as PathsTraveledByTargets.");
                return;
            }

            //This method will delete the file of save state of fog of war
            File.Delete(Application.persistentDataPath + "/" + fogsRevealedSaveId + ".bin");
        }

        public bool isThisComponentLoadingSomeFogOfWarState()
        {
            //If the fog removal method is not PathsTraveledByTargets
            if (fogRemovalMethod != FogRemovalMethod.PathsTraveledByTargets)
            {
                Debug.LogError("Could not execute Minimap Fog \"isDoingSomeAsyncTaskNow()\" method on \"" + this.gameObject.name + "\". This method is only available if \"fogRemovalMethod\" is selected as PathsTraveledByTargets.");
                return false;
            }

            //Return true if this Minimap Fog is doing some async task for now
            if (staticsFogsToBeEnabled_byLoadStateOfFogOfWar.Count > 0)
                return true;
            return false;
        }
    }
}