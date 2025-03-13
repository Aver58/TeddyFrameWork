#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Text;
using System.Linq;

namespace MTAssets.EasyMinimapSystem
{
    /*
     This class is responsible for the functioning of the "Minimap Routes" component, and all its functions.
    */
    /*
     * The Easy Minimap System was developed by Marcos Tomaz in 2019.
     * Need help? Contact me (mtassets@windsoft.xyz)
    */

    [AddComponentMenu("MT Assets/Easy Minimap System/Minimap Routes")] //Add this component in a category of addComponent menu
    public class MinimapRoutes : MonoBehaviour
    {
        //Private constants
        private const float BASE_HEIGHT_IN_3D_WORLD = 99002;
        private const string VERSION_OF_WDB_FILE_SUPPORTED = "1.0.0";

        //Private variables
        private GameObject minimapDataHolderObj;
        private MinimapDataHolder minimapDataHolder;
        private Transform minimapRoutesHolder;
        private GameObject tempLineObj;
        private LineRenderer tempLine;

        //Private cache of editor
        private bool showIdOfConnectedWaypointsOfCurrentSelectedWaypoint = true;
        private int currentSelectedWaypoint = 0;
        private bool showingEditionControls = true;
        private int currentToolEnabled = 0;
        private float timeOfCalculationOfLastRoute = 0.0f;

        //Private cache variables
        private bool alreadySettedDefaultMaterialOnLineRenderer = false;
        private CurrentDisplayingRoute currentRouteBeingDisplayedAndAllYourData = new CurrentDisplayingRoute();
        private bool isCurrentlyShowingRoute = false;
        private Coroutine currentlyRunningRoutineForCalcsAndBuildOfRoute = null;
        private RoutesUpdateFrequency lastRoutesUpdatesFrequency = RoutesUpdateFrequency.x1PerSecond;
        private Transform lastStartingPointDefined = null;
        private Transform lastDestinationPointDefined = null;
        private int lastQuantityOfWaypointsInCurrentExistingWaypointsItems = -1;

        //Classes of script
        public class TempAStarProcessmentData
        {
            //This class stores temporary data for processment of executions of AStar algorithm
            public List<WaypointItem> openWaypoints = new List<WaypointItem>();
            public List<WaypointItem> closedWaypoints = new List<WaypointItem>();
            public List<WaypointItem> waypointsOfRoute = new List<WaypointItem>();
            public long totalDistanceOfRoute = 0;
        }
        public class CurrentDisplayingRoute
        {
            //This class stores a data about current data to be retorned to user
            public bool isThisRouteHasSomeProblemToBeCalculated = false;
            public List<Vector3> positionsThatMakeUpThisRoute = new List<Vector3>();
            public List<int> waypointsIdsThatMakeUpThisRoute = new List<int>();
            public float totalDistanceOfRouteSinceStartPointToDestination = 0;
        }
        public class WaypointItemsForExportation
        {
            //This class is only a temp storage to store waypoint items to be exported or imported
            public string versionOfFile = "";
            public WaypointItem[] arrayOfWaypointItems = null;
        }
        [System.Serializable]
        public class WaypointItem
        {
            //This class stores data of a Waypoint that exists in the World and is saved in scene

            //---- Variables for persistent storage of this waypoint data in Editor/Runtime

            //Waypoing storage data (this values will be filled on editor when creating or editing waypoints)
            public int thisWaypointId = -1;
            public bool thisWaypointIsDeleted = false;                              //<-- If a Waypoint is DELETED, it is as if it never existed. It will not be considered in route calculations, not even displayed
            public bool thisWaypointIsEnabledAndUnobstructedForRoutesCalcs = true;  //<-- If a WAypoint is DISABLED OR OBSTRUCTED, it just won't be considered in route calculations.
            public Vector3 globalPosition = Vector3.zero;
            public List<int> connectedWaypointsIds = new List<int>();

            //---- Variables to be used in runtime for calcs and etc (will be filled in runtime)

            //Waypoint costs (using integer values *10 for more performance instead of float numbers)
            public int[] movementCostOfThisWaypointToEachConnectedWaypointOfThis = null;
            public int gCostOfOriginToThisWaypoint = 0;
            public int hCostOfThisWaypointToDestination = 0;
            public int fCostTotalOfThisWaypointGplusH = 0;
            public int idOfWaypointParentOfThisWaypoint = -1;

            //---- This Node management methods (to be easy manipulate each node of database of nodes)

            public int AutomaticallyAddThisWaypointInTheListOfWaypointsExistingAndGetIdOfThis(MinimapRoutes minimapRoutesInstance, Vector3 positionOfThisWaypoint)
            {
                //This method will fil this waypoint with basic data and get the future id of this new waypoint

                //Get the future ID of this waypoint
                int futureIdOfThisWaypoint = 0;
                if (minimapRoutesInstance.currentExistingWaypointsItems.Count > 0)
                    futureIdOfThisWaypoint = minimapRoutesInstance.currentExistingWaypointsItems.Count;

                //Fill this waypoint with basic data
                this.thisWaypointId = futureIdOfThisWaypoint;
                this.globalPosition = positionOfThisWaypoint;

                //Add this new waypoint in the list of existing waypoints
                minimapRoutesInstance.currentExistingWaypointsItems.Add(this);

                //Return the future id of this waypoint
                return futureIdOfThisWaypoint;
            }

            public void UpdateAllCostsOfThisWaypointToEachConnectedWaypointOfThis(MinimapRoutes minimapRoutesInstance)
            {
                //This method will calculate the distance (cost) to move of this waypoint to each connected waypoint of this and store the result of calc on cache
                //The result will be converted to int *10 to optimize and reduce the memory usage, because float precision may be slow
                this.movementCostOfThisWaypointToEachConnectedWaypointOfThis = new int[connectedWaypointsIds.Count];
                for (int i = 0; i < this.movementCostOfThisWaypointToEachConnectedWaypointOfThis.Length; i++)
                {
                    float distance = Vector3.Distance(this.globalPosition, minimapRoutesInstance.currentExistingWaypointsItems[this.connectedWaypointsIds[i]].globalPosition);
                    this.movementCostOfThisWaypointToEachConnectedWaypointOfThis[i] = (int)(distance * 10.0f);
                }
            }

            public void ConnectThisWaypointToOtherWaypoint(MinimapRoutes minimapRoutesInstance, int idOfWaypointToConnectThis)
            {
                //Validate the value
                if (idOfWaypointToConnectThis > (minimapRoutesInstance.currentExistingWaypointsItems.Count - 1))
                {
                    Debug.LogError("Please provide a valid ID so that it is possible to connect this Waypoint to another.");
                    return;
                }

                //This method connect this waypoint in a target waypoint, and automatically connect the target waypoint to this waypoint too
                if (this.connectedWaypointsIds.Contains(idOfWaypointToConnectThis) == false)
                    this.connectedWaypointsIds.Add(idOfWaypointToConnectThis);
                if (minimapRoutesInstance.currentExistingWaypointsItems[idOfWaypointToConnectThis].connectedWaypointsIds.Contains(this.thisWaypointId) == false)
                    minimapRoutesInstance.currentExistingWaypointsItems[idOfWaypointToConnectThis].connectedWaypointsIds.Add(this.thisWaypointId);
            }

            public void DisconnectThisWaypointFromOtherWaypoint(MinimapRoutes minimapRoutesInstance, int idOfWaypointToDisconnectFromThis)
            {
                //Validate the value
                if (idOfWaypointToDisconnectFromThis > (minimapRoutesInstance.currentExistingWaypointsItems.Count - 1))
                {
                    Debug.LogError("Please provide a valid ID so that it is possible to disconnect this Waypoint from another.");
                    return;
                }

                //This method disconnect this waypoint of a target waypoint, and automatically disconnect the target waypoint, from this too
                minimapRoutesInstance.currentExistingWaypointsItems[idOfWaypointToDisconnectFromThis].connectedWaypointsIds.RemoveAll(x => x == this.thisWaypointId);
                this.connectedWaypointsIds.RemoveAll(x => x == idOfWaypointToDisconnectFromThis);
            }

            public void DeleteAndDisableThisWaypoint(MinimapRoutes minimapRoutesInstance)
            {
                //Validate the value
                if (this.thisWaypointId == 0)
                {
                    Debug.LogWarning("It is not possible to delete the primary Waypoint!");
                    return;
                }

                //This method will disable this waypoint from the currentExistingWaypointsItems and disconnect all connections that this waypoint have
                //This method will mantain the data of this waypoint in currentExistingWaypointsItems to avoid problems of changing order/ids of other waypoints
                int[] currentConnectedWaypointsOnThis = this.connectedWaypointsIds.ToArray();
                foreach (int connectedWaypointId in currentConnectedWaypointsOnThis)
                    this.DisconnectThisWaypointFromOtherWaypoint(minimapRoutesInstance, connectedWaypointId);
                this.thisWaypointIsDeleted = true;
            }

            //---- Method to be used by algorithm A* (to be easy manipulate in algorithm)

            public void ResetAllRuntimeCacheVariablesOfThisWaypoint()
            {
                //This method resets all variables of cache used for algorithm A* to default values
                this.gCostOfOriginToThisWaypoint = 0;
                this.hCostOfThisWaypointToDestination = 0;
                this.fCostTotalOfThisWaypointGplusH = 0;
                this.idOfWaypointParentOfThisWaypoint = -1;
            }
        }

        //Current existing waypoints items (database)
        ///<summary>[WARNING] Do not change the value of this variable. This is a variable used for internal tool operations.</summary> 
        [HideInInspector]
        public List<WaypointItem> currentExistingWaypointsItems = new List<WaypointItem>();

        //Enums of script
        private enum ReturnBestWaypointFor
        {
            StartingPoint,
            DestinationPoint
        }
        private enum AStarProcessmentStatus
        {
            NotCompleted,
            FoundRoute,
            NotFoundRoute
        }
        public enum RoutesUpdateFrequency
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
        public enum StartingFindMethod
        {
            OptimizedMathCalcs,
            Vector3Distance,
            EstimatedMagnitude
        }

        //Public variables
        [HideInInspector]
        public Color lineRenderColor = Color.yellow;
        [HideInInspector]
        public float lineWidthSize = 1.0f;
        [HideInInspector]
        public Transform startingPoint;
        [HideInInspector]
        public Transform destinationPoint;
        [HideInInspector]
        public bool showRoutesOnStart = false;
        [HideInInspector]
        public RoutesUpdateFrequency routesUpdatesFrequency = RoutesUpdateFrequency.x3PerSecond;
        [HideInInspector]
        public float startingHeightTolerance = 3.5f;
        [HideInInspector]
        public StartingFindMethod startingPointFindMethod = StartingFindMethod.OptimizedMathCalcs;
        [HideInInspector]
        public float movementsSmoothing = 14;
        public UnityEvent onUpdateRoutesRenderization;

        //The UI of this component
#if UNITY_EDITOR
        #region INTERFACE_CODE
        [UnityEditor.CustomEditor(typeof(MinimapRoutes))]
        public class CustomInspector : UnityEditor.Editor
        {
            public override void OnInspectorGUI()
            {
                //Start the undo event support, draw default inspector and monitor of changes
                MinimapRoutes script = (MinimapRoutes)target;
                EditorGUI.BeginChangeCheck();
                Undo.RecordObject(target, "Undo Event");

                //Support reminder
                GUILayout.Space(10);
                EditorGUILayout.HelpBox("Remember to read the Easy Minimap System documentation to understand how to use it.\nGet support at: mtassets@windsoft.xyz", MessageType.None);

                //Now that all components are validated, execution continues. ----------]
                GUILayout.Space(10);

                //Set height in 0
                script.gameObject.transform.position = new Vector3(0, 0, 0);
                script.gameObject.transform.rotation = Quaternion.Euler(0, 0, 0);
                script.gameObject.transform.localScale = new Vector3(1, 1, 1);

                //Start of settings
                EditionToolbar_ShowWorldWaypointEditingTool(script);

                GUILayout.Space(10);
                EditorGUILayout.LabelField("Settings For Routes Renderer", EditorStyles.boldLabel);
                GUILayout.Space(10);

                script.lineRenderColor = EditorGUILayout.ColorField(new GUIContent("Line Render Color",
                                        "The color in which the line showing the Routes will be rendered."),
                                        script.lineRenderColor);

                script.lineWidthSize = EditorGUILayout.Slider(new GUIContent("Line Width Size",
                                        "The width over which the line will be rendered on the map."),
                                        script.lineWidthSize, 0.5f, 100.0f);

                GUILayout.Space(10);
                EditorGUILayout.LabelField("Settings For Routes Calculation", EditorStyles.boldLabel);
                GUILayout.Space(10);

                script.startingPoint = (Transform)EditorGUILayout.ObjectField(new GUIContent("Starting Point",
                                                        "This is the Transform that represents the starting point of the route you want to display. This is usually the Player's transform."),
                                                        script.startingPoint, typeof(Transform), true, GUILayout.Height(16));

                script.destinationPoint = (Transform)EditorGUILayout.ObjectField(new GUIContent("Destination Point",
                                        "This is the Transform that represents the destination point of the route you want to display. Usually, this is the transformation of some location in your world."),
                                        script.destinationPoint, typeof(Transform), true, GUILayout.Height(16));

                script.showRoutesOnStart = EditorGUILayout.Toggle(new GUIContent("Show Routes On Start",
                                "If this option is checked, Minimap Routes will start displaying the route as soon as the game starts, if the variables \"startingPoint\" and \"destinationPoint\" are not null."),
                                script.showRoutesOnStart);

                script.routesUpdatesFrequency = (RoutesUpdateFrequency)EditorGUILayout.EnumPopup(new GUIContent("Routes Updates Frequency",
                                    "The number of times per second that the routes rendered by this component must be updated. Each time a route is rendered, the calculations for leaving the starting point and going to the destination point are also redone, and the rendering of the route is updated.\n\nA greater number of updates may bring greater smoothness in rendering and route updates, as well as greater fidelity, but it may consume more performance."),
                                    script.routesUpdatesFrequency);

                script.startingPointFindMethod = (StartingFindMethod)EditorGUILayout.EnumPopup(new GUIContent("Starting Point Find Method",
                                "Method that Minimap Routes will use to calculate the distances between all Waypoints in that world and the Starting or Destination points. Before Minimap Routes calculates the route, it will need to find the Waypoints that are closest to Starting and closest to Destination. With this option you can change the search method to try further optimization.\n\nOptimized Math Calcs - This method of calculating distance tries to be as optimized as possible using its own mathematical calculations to calculate distances, while trying to be as accurate as possible.\n\nVector3.Distance - This is the most accurate distance calculation method, however, it may require slightly higher performance cost in some cases.\n\nEstimated Magnitude - This is the calculation method that can be the most optimized of all, but its accuracy is the lowest of all. Using this method, you can sometimes notice that the route is departing from unexpected Waypoints."),
                                script.startingPointFindMethod);

                script.startingHeightTolerance = EditorGUILayout.FloatField(new GUIContent("Starting Height Tolerance",
                                "This value represents the maximum and minimum height (Units) allowed for the route to start close to the Start or Destination point.\n\nFor example, if a Waypoint is a difference greather than " + script.startingHeightTolerance + " in height, above or below the Start point, it will not be considered as eligible for a route to start from it.\n\nThis is very useful at times when the Start point is on a bridge for example. It is this value that will prevent the algorithm from considering Waypoints that are below the bridge, giving preference to Waypoints that are on the bridge too, along with the Start point.\n\nIn other words, the algorithm will always try to search as a Waypoint for the start of the route, a Waypoint that is at an approximate height of the Start or Destination point. And this value defines the limit height, both up and down."),
                                script.startingHeightTolerance);

                GUILayout.Space(10);
                EditorGUILayout.LabelField("Settings For Routes Movement", EditorStyles.boldLabel);
                GUILayout.Space(10);

                script.movementsSmoothing = EditorGUILayout.Slider(new GUIContent("Movement Smoothing",
                                "The speed at which this Minimap Item will follow GameObjects.\n\nThe higher the smoothing value, the faster this Item will rotate/move to the destination direction."), script.movementsSmoothing, 1f, 100f);

                GUILayout.Space(10);
                EditorGUILayout.LabelField("Minimap Routes Events", EditorStyles.boldLabel);
                GUILayout.Space(10);
                DrawDefaultInspector();

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

            private void EditionToolbar_ShowWorldWaypointEditingTool(MinimapRoutes script)
            {
                //Count quantity of waypoints existing in this world, ignoring deleted waypoints
                int waypointsExisting = 0;
                foreach (WaypointItem waypoint in script.currentExistingWaypointsItems)
                    if (waypoint.thisWaypointIsDeleted == false)
                        waypointsExisting += 1;

                //Show the quantity of existing waypoints in this world
                GUIStyle titulo = new GUIStyle();
                titulo.fontSize = 16;
                if (script.currentExistingWaypointsItems.Count == 0)
                    titulo.normal.textColor = Color.red;
                if (script.currentExistingWaypointsItems.Count > 0)
                    titulo.normal.textColor = new Color(0, 79.0f / 250.0f, 3.0f / 250.0f);
                titulo.alignment = TextAnchor.MiddleCenter;
                if (script.currentExistingWaypointsItems.Count == 0)
                    EditorGUILayout.LabelField("There are no Waypoints in this world yet. Create the first!", titulo);
                if (script.currentExistingWaypointsItems.Count > 0)
                    EditorGUILayout.LabelField("There are a total of " + waypointsExisting + " Waypoints in this World!", titulo);

                GUILayout.Space(10);

                EditorGUILayout.BeginVertical("box");

                //If the game is running
                if (Application.isPlaying == true)
                {
                    EditorGUILayout.HelpBox("Waypoint editing tools is not available while your game is running.", MessageType.Info);
                    if (script.isCurrentlyShowingRoute == true)
                    {
                        GUILayout.Space(10);
                        GUIStyle centeredStyle = GUI.skin.GetStyle("Label");
                        centeredStyle.alignment = TextAnchor.UpperCenter;
                        GUIStyle centeredStyleWithMinorFont = new GUIStyle();
                        centeredStyleWithMinorFont.alignment = TextAnchor.UpperCenter;
                        centeredStyleWithMinorFont.fontSize = 10;
                        centeredStyleWithMinorFont.normal.textColor = new Color(69.0f / 250.0f, 69.0f / 250.0f, 69.0f / 250.0f);
                        int waypointsUsedsInCurrentRoute = script.currentRouteBeingDisplayedAndAllYourData.positionsThatMakeUpThisRoute.Count;
                        waypointsUsedsInCurrentRoute -= 1;
                        if (script.currentRouteBeingDisplayedAndAllYourData.isThisRouteHasSomeProblemToBeCalculated == false)
                            waypointsUsedsInCurrentRoute -= 1;
                        EditorGUILayout.LabelField("Currently displaying a route that uses " + waypointsUsedsInCurrentRoute + " Waypoints. Total distance of " + script.currentRouteBeingDisplayedAndAllYourData.totalDistanceOfRouteSinceStartPointToDestination.ToString("F1") + " Units.", centeredStyle);
                        GUILayout.Space(-4);
                        EditorGUILayout.LabelField("The route was calculated in " + script.timeOfCalculationOfLastRoute + "ms", centeredStyleWithMinorFont);
                        if (script.currentRouteBeingDisplayedAndAllYourData.waypointsIdsThatMakeUpThisRoute.Count > 0)
                        {
                            StringBuilder waypointsOrder = new StringBuilder();
                            for (int i = 0; i < script.currentRouteBeingDisplayedAndAllYourData.waypointsIdsThatMakeUpThisRoute.Count; i++)
                            {
                                if (i == 0)
                                    waypointsOrder.Append("Starting > ");
                                waypointsOrder.Append(script.currentRouteBeingDisplayedAndAllYourData.waypointsIdsThatMakeUpThisRoute[i]);
                                if ((i + 1) != script.currentRouteBeingDisplayedAndAllYourData.waypointsIdsThatMakeUpThisRoute.Count)
                                    waypointsOrder.Append(" > ");
                                if ((i + 1) == script.currentRouteBeingDisplayedAndAllYourData.waypointsIdsThatMakeUpThisRoute.Count && script.currentRouteBeingDisplayedAndAllYourData.isThisRouteHasSomeProblemToBeCalculated == false)
                                    waypointsOrder.Append(" > Destination");
                                if ((i + 1) == script.currentRouteBeingDisplayedAndAllYourData.waypointsIdsThatMakeUpThisRoute.Count && script.currentRouteBeingDisplayedAndAllYourData.isThisRouteHasSomeProblemToBeCalculated == true)
                                    waypointsOrder.Append(" > [X]");
                            }
                            EditorGUILayout.HelpBox(waypointsOrder.ToString(), MessageType.None);
                        }
                        if (script.currentRouteBeingDisplayedAndAllYourData.positionsThatMakeUpThisRoute.Count == 0)
                            EditorGUILayout.HelpBox("Starting > Destination", MessageType.None);
                    }
                }

                //If have 0 waypoints, show button to create the first waypoint
                if (script.currentExistingWaypointsItems.Count == 0 && Application.isPlaying == false)
                    if (GUILayout.Button("Create First Waypoint", GUILayout.Height(30)))
                    {
                        //Get the current center position of camera scene view
                        Selection.activeObject = SceneView.currentDrawingSceneView;
                        SceneView lastSceneView = SceneView.lastActiveSceneView;
                        Camera sceneViewCamera = lastSceneView.camera;
                        Vector3 position = sceneViewCamera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 10f));
                        Selection.activeObject = script.gameObject;

                        WaypointItem newWaypoint = new WaypointItem();
                        newWaypoint.AutomaticallyAddThisWaypointInTheListOfWaypointsExistingAndGetIdOfThis(script, position);
                    }

                //If have more than 0 waypoints, show the toolbar
                if (script.currentExistingWaypointsItems.Count > 0 && Application.isPlaying == false)
                {
                    //Load all resources
                    Texture2D showControlsIcon = null;
                    if (script.showingEditionControls == false)
                        showControlsIcon = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/MT Assets/Easy Minimap System/Editor/Images/EyeClose.png", typeof(Texture2D));
                    if (script.showingEditionControls == true)
                        showControlsIcon = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/MT Assets/Easy Minimap System/Editor/Images/EyeOpen.png", typeof(Texture2D));
                    Texture2D selectionIcon = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/MT Assets/Easy Minimap System/Editor/Images/Selection.png", typeof(Texture2D));
                    Texture2D moveIcon = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/MT Assets/Easy Minimap System/Editor/Images/Move.png", typeof(Texture2D));
                    Texture2D createIcon = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/MT Assets/Easy Minimap System/Editor/Images/Create.png", typeof(Texture2D));
                    Texture2D deleteIcon = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/MT Assets/Easy Minimap System/Editor/Images/Delete.png", typeof(Texture2D));
                    Texture2D connectIcon = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/MT Assets/Easy Minimap System/Editor/Images/Connect.png", typeof(Texture2D));
                    Texture2D disconnectIcon = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/MT Assets/Easy Minimap System/Editor/Images/Disconnect.png", typeof(Texture2D));
                    Texture2D cutterIcon = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/MT Assets/Easy Minimap System/Editor/Images/Cutter.png", typeof(Texture2D));
                    Texture2D aditionalButtonsBg = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/MT Assets/Easy Minimap System/Editor/Images/ButtonsBg.png", typeof(Texture2D));
                    Texture2D exportIcon = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/MT Assets/Easy Minimap System/Editor/Images/Export.png", typeof(Texture2D));
                    Texture2D importIcon = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/MT Assets/Easy Minimap System/Editor/Images/Import.png", typeof(Texture2D));

                    //Create style for toggleable button
                    GUIStyle toggleButtonStyle = new GUIStyle("Button");
                    GUIStyle leftAditionalButtonStyle = new GUIStyle("Button");
                    leftAditionalButtonStyle.margin = new RectOffset(-8, -8, 4, 2);
                    leftAditionalButtonStyle.padding = new RectOffset(6, 6, 2, 1);
                    leftAditionalButtonStyle.normal.background = aditionalButtonsBg;

                    EditorGUILayout.BeginHorizontal("box");

                    //Render control buttons
                    script.showingEditionControls = GUILayout.Toggle(script.showingEditionControls, new GUIContent(
                        showControlsIcon,
                        "Click here to show or hide all Waypoint edit controls that are rendered in your Editor."),
                        toggleButtonStyle, GUILayout.Width(32), GUILayout.Height(24));
                    if (GUILayout.Button(new GUIContent(
                        exportIcon,
                        "Export Waypoints Database\n\nClick here to export all Waypoints for this component and their data, to a WDB file to some location on your computer. They can be imported later if you want.\n\nThis component will generate WDB files on version \"" + VERSION_OF_WDB_FILE_SUPPORTED + "\"."),
                        leftAditionalButtonStyle, GUILayout.Width(40), GUILayout.Height(21)) == true)
                        ExportationImportation_ExportWaypointsDatabase(script);
                    if (GUILayout.Button(new GUIContent(
                        importIcon,
                        "Import Waypoints Database\n\nClick here to import all Waypoints and their saved data, from a WDB file. When performing this action, all Waypoints that currently exist in this component will be overwritten.\n\nThis component imports only WDB files that are in version \"" + VERSION_OF_WDB_FILE_SUPPORTED + "\"."),
                        leftAditionalButtonStyle, GUILayout.Width(40), GUILayout.Height(21)) == true)
                        ExportationImportation_ImportWaypointsDatabase(script);

                    GUILayout.Space(16);
                    GUILayout.FlexibleSpace();

                    script.currentToolEnabled = GUILayout.Toolbar(script.currentToolEnabled, new GUIContent[]{
                        new GUIContent(selectionIcon, "Select Waypoint (Tool)\n\nShortcut Key [A]"),
                        new GUIContent(moveIcon, "Move Waypoint (Tool)\n\nShortcut Key [V]"),
                        new GUIContent(createIcon, "Create New Waypoint (Tool)\n\nShortcut Key [N]"),
                        new GUIContent(deleteIcon, "Delete Waypoint (Tool)\n\nShortcut Key [D]"),
                        new GUIContent(connectIcon, "Connect Waypoints (Tool)\n\nShortcut Key [C]"),
                        new GUIContent(disconnectIcon, "Disconnect Waypoints (Tool)\n\nShortcut Key [X]"),
                        new GUIContent(cutterIcon, "Cutter Waypoints (Tool)\n\nShortcut Key [K]"),
                    }, GUILayout.Height(24), GUILayout.Width(Screen.width - 16 - 80 - 34 - 44));

                    EditorGUILayout.EndHorizontal();

                    //Show the description and aditional options of current selected tool
                    switch (script.currentToolEnabled)
                    {
                        case 0:
                            EditorGUILayout.HelpBox("The spheres represent the positions of the Waypoints. Click on any Waypoint to select that Waypoint.", MessageType.None);
                            GUILayout.Space(8);
                            EditorGUILayout.BeginHorizontal();
                            script.currentExistingWaypointsItems[script.currentSelectedWaypoint].thisWaypointIsEnabledAndUnobstructedForRoutesCalcs = GUILayout.Toggle(script.currentExistingWaypointsItems[script.currentSelectedWaypoint].thisWaypointIsEnabledAndUnobstructedForRoutesCalcs,
                                new GUIContent("", "If this box is checked, this Waypoint is ENABLED and ready to be used in route calculations.\n\nDeactivate this box to DISABLE this Waypoint. Disabled waypoints will never be considered in route calculations."), GUILayout.Width(16), GUILayout.Height(16));
                            EditorGUILayout.BeginVertical();
                            GUILayout.Space(0.5f);
                            EditorGUILayout.LabelField("Selected Waypoint Is " + script.currentExistingWaypointsItems[script.currentSelectedWaypoint].thisWaypointId + ((script.currentExistingWaypointsItems[script.currentSelectedWaypoint].thisWaypointIsEnabledAndUnobstructedForRoutesCalcs == false) ? " (DISABLED)" : ""), EditorStyles.boldLabel);
                            EditorGUILayout.EndVertical();
                            EditorGUILayout.EndHorizontal();
                            StringBuilder idsConnected = new StringBuilder();
                            for (int i = 0; i < script.currentExistingWaypointsItems[script.currentSelectedWaypoint].connectedWaypointsIds.Count; i++)
                            {
                                idsConnected.Append(script.currentExistingWaypointsItems[script.currentSelectedWaypoint].connectedWaypointsIds[i]);
                                if (i < script.currentExistingWaypointsItems[script.currentSelectedWaypoint].connectedWaypointsIds.Count - 1)
                                    idsConnected.Append(",");
                            }
                            string finalIdsConnected = idsConnected.ToString();
                            EditorGUILayout.BeginHorizontal();
                            script.showIdOfConnectedWaypointsOfCurrentSelectedWaypoint = GUILayout.Toggle(script.showIdOfConnectedWaypointsOfCurrentSelectedWaypoint,
                                new GUIContent("", "If this box is checked, the Waypoints editing interface on scene will show the ID of the Waypoints connected to the currently selected Waypoint. But only if the \"Selection\" tab is activated."), GUILayout.Width(16), GUILayout.Height(16));
                            EditorGUILayout.BeginVertical();
                            GUILayout.Space(0.7f);
                            if (string.IsNullOrEmpty(finalIdsConnected) == true)
                                EditorGUILayout.LabelField("Connected with zero other Waypoints");
                            if (string.IsNullOrEmpty(finalIdsConnected) == false)
                                EditorGUILayout.LabelField("Connected with " + script.currentExistingWaypointsItems[script.currentSelectedWaypoint].connectedWaypointsIds.Count + " other Waypoints (" + finalIdsConnected + ")");
                            EditorGUILayout.EndVertical();
                            EditorGUILayout.EndHorizontal();
                            break;
                        case 1:
                            EditorGUILayout.HelpBox("Use the arrows that appeared in the selected sphere, or Vector3 below to move it to the desired position.", MessageType.None);
                            GUILayout.Space(8);
                            script.currentExistingWaypointsItems[script.currentSelectedWaypoint].globalPosition = EditorGUILayout.Vector3Field(new GUIContent(
                                "Waypoint " + script.currentExistingWaypointsItems[script.currentSelectedWaypoint].thisWaypointId + " Position",
                                "Manually change the position of this Waypoint here."),
                                script.currentExistingWaypointsItems[script.currentSelectedWaypoint].globalPosition);
                            break;
                        case 2:
                            EditorGUILayout.HelpBox("Choose an option below to create a new Waypoint in this world. The new Waypoint will be created next to the currently selected Waypoint.", MessageType.None);
                            GUILayout.Space(8);
                            EditorGUILayout.BeginHorizontal();
                            if (GUILayout.Button(new GUIContent("Create Connected With This", "It will create a Waypoint that will be automatically connected to the Waypoint that is currently selected.\n\nShortcut Key [1]"), GUILayout.Height(24)))
                                EditionToolbar_CreateNewWaypointConnectedToCurrentSelectedWaypoint(script);
                            if (GUILayout.Button(new GUIContent("Create Disconnected", "It will create a Waypoint that will not be connected to any other Waypoint.\n\nShortcut Key [2]"), GUILayout.Height(24)))
                                EditionToolbar_CreateNewWaypointWithoutAnyConnectionToOther(script);
                            EditorGUILayout.EndHorizontal();
                            break;
                        case 3:
                            int deletedWaypoints = 0;
                            foreach (WaypointItem waypoint in script.currentExistingWaypointsItems)
                                if (waypoint.thisWaypointIsDeleted == true)
                                    deletedWaypoints += 1;
                            EditorGUILayout.HelpBox("Click on a desired Waypoint to delete it. Deleting a Waypoint will also delete the connections it has with other Waypoints. A total of " + deletedWaypoints + " Waypoints have already been deleted.\n\nYou can use the shortcut \"Ctrl + Z\" or \"Command + Z\" to undo the deletion of Waypoint, if you have not saved the scene, after deleting a Waypoint.", MessageType.None);
                            break;
                        case 4:
                            EditorGUILayout.HelpBox("First select the Waypoint A you prefer, using the \"Selection\" tool. With Waypoint A selected, select this tool and click on the desired Waypoint B and the two will be connected.\n\nAfter connecting two Waypoints, they will be treated as part of the same possible path and they will be able to communicate.", MessageType.None);
                            break;
                        case 5:
                            EditorGUILayout.HelpBox("Click in Yellow Cube between a Waypoint Connections to delete it. When deleting the connection between two Waypoints, they will no longer be connected and will not be treated as on the same path.\n\nYou can use the shortcut \"Ctrl + Z\" or \"Command + Z\" to undo the deletion of Waypoints Connection, if you have not saved the scene, after deleting a Waypoint Connections.", MessageType.None);
                            break;
                        case 6:
                            EditorGUILayout.HelpBox("Click in Green Cube between a Waypoint Connections to cut that connection and generate a new Waypoint in place of the Cube. You can use this tool to subdivide connections and generate more Waypoints to make routes smoother if you prefer.\n\nYou can use the shortcut \"Ctrl + Z\" or \"Command + Z\" to undo the cut of Waypoints Connections, if you have not saved the scene, after cutting a Waypoint Connections.", MessageType.None);
                            break;
                    }
                }

                EditorGUILayout.EndVertical();

                EditionToolbar_ListenAndGetShortcutKeysIfPressed(script);
            }

            private void EditionToolbar_CreateNewWaypointConnectedToCurrentSelectedWaypoint(MinimapRoutes script)
            {
                //This method will create a new Waypoint connected to current selected waypoint. This method only will be called by the Button on tab "Create" or a Shortcut Key of tab "Create"
                WaypointItem newWaypoint = new WaypointItem();
                int id = newWaypoint.AutomaticallyAddThisWaypointInTheListOfWaypointsExistingAndGetIdOfThis(script,
                            script.currentExistingWaypointsItems[script.currentSelectedWaypoint].globalPosition + new Vector3(3, 0, 3));
                newWaypoint.ConnectThisWaypointToOtherWaypoint(script, script.currentSelectedWaypoint);
                script.currentSelectedWaypoint = id;
                script.currentToolEnabled = 1;
            }

            private void EditionToolbar_CreateNewWaypointWithoutAnyConnectionToOther(MinimapRoutes script)
            {
                //This method will create a new Waypoint without connection to any waypoint. This method only will be called by the Button on tab "Create" or a Shortcut Key of tab "Create"
                WaypointItem newWaypoint = new WaypointItem();
                int id = newWaypoint.AutomaticallyAddThisWaypointInTheListOfWaypointsExistingAndGetIdOfThis(script,
                            script.currentExistingWaypointsItems[script.currentSelectedWaypoint].globalPosition + new Vector3(3, 0, 3));
                script.currentSelectedWaypoint = id;
                script.currentToolEnabled = 1;
            }

            private void EditionToolbar_ListenAndGetShortcutKeysIfPressed(MinimapRoutes script)
            {
                //If not have Waypoints, don't listen to shortcuts
                if (script.currentExistingWaypointsItems.Count == 0 || Application.isPlaying == true)
                    return;

                //Listen to keys pressed and change the tool according to key pressed
                Event currentEvent = Event.current;
                if (currentEvent.type == EventType.KeyDown)
                    switch (currentEvent.keyCode)
                    {
                        case KeyCode.A:
                            script.currentToolEnabled = 0;
                            currentEvent.Use();
                            break;
                        case KeyCode.V:
                            script.currentToolEnabled = 1;
                            currentEvent.Use();
                            break;
                        case KeyCode.N:
                            script.currentToolEnabled = 2;
                            currentEvent.Use();
                            break;
                        case KeyCode.D:
                            script.currentToolEnabled = 3;
                            currentEvent.Use();
                            break;
                        case KeyCode.C:
                            script.currentToolEnabled = 4;
                            currentEvent.Use();
                            break;
                        case KeyCode.X:
                            script.currentToolEnabled = 5;
                            currentEvent.Use();
                            break;
                        case KeyCode.K:
                            script.currentToolEnabled = 6;
                            currentEvent.Use();
                            break;
                        case KeyCode.Alpha1:
                            if (script.currentToolEnabled == 2)
                            {
                                EditionToolbar_CreateNewWaypointConnectedToCurrentSelectedWaypoint(script);
                                currentEvent.Use();
                            }
                            break;
                        case KeyCode.Alpha2:
                            if (script.currentToolEnabled == 2)
                            {
                                EditionToolbar_CreateNewWaypointWithoutAnyConnectionToOther(script);
                                currentEvent.Use();
                            }
                            break;
                    }
            }

            protected virtual void OnSceneGUI()
            {
                //Draw the controls of this Scan
                MinimapRoutes script = (MinimapRoutes)target;

                EditionToolbar_ListenAndGetShortcutKeysIfPressed(script);

                //If have 0 waypoints, return
                if (script.currentExistingWaypointsItems.Count == 0)
                    return;

                EditorGUI.BeginChangeCheck();
                Undo.RecordObject(target, "Undo Event");

                //Only render the controls, if desired
                if (script.showingEditionControls == true && Application.isPlaying == false)
                {
                    //Set the color of controls
                    Handles.color = Color.blue;

                //Render all waypoints
                StartTheLoopAndRenderAllWaypointsAndConnections:
                    foreach (WaypointItem waypoint in script.currentExistingWaypointsItems)
                    {
                        //If waypoint is deleted, skip
                        if (waypoint.thisWaypointIsDeleted == true)
                            continue;

                        //If this waypoint is not selected, render with color of blue
                        if (script.currentSelectedWaypoint != waypoint.thisWaypointId)
                            Handles.color = Color.blue;
                        //If this waypoint is not selected and is disabled for route calcs, render with color of black
                        if (script.currentSelectedWaypoint != waypoint.thisWaypointId && waypoint.thisWaypointIsEnabledAndUnobstructedForRoutesCalcs == false)
                            Handles.color = Color.black;
                        //If this waypoint is selected, render with color of red
                        if (script.currentSelectedWaypoint == waypoint.thisWaypointId)
                            Handles.color = Color.red;

                        //Render this waypoint sphere (if selection tool)
                        if (script.currentToolEnabled == 0)
                        {
                            //Render the waypoint
                            if (Handles.Button(waypoint.globalPosition, Quaternion.identity, 2, 2, Handles.SphereHandleCap) == true)
                                script.currentSelectedWaypoint = waypoint.thisWaypointId;
                        }
                        //Render this waypoint sphere (if move tool)
                        if (script.currentToolEnabled == 1)
                        {
                            //If this not is the waypoint selected
                            if (script.currentSelectedWaypoint != waypoint.thisWaypointId)
                                if (Handles.Button(waypoint.globalPosition, Quaternion.identity, 2, 2, Handles.SphereHandleCap) == true)
                                    script.currentSelectedWaypoint = waypoint.thisWaypointId;
                            //If this is the waypoint selected
                            if (script.currentSelectedWaypoint == waypoint.thisWaypointId)
                                Handles.SphereHandleCap(waypoint.thisWaypointId, waypoint.globalPosition, Quaternion.identity, 2, EventType.Repaint);
                        }
                        //Render this waypoint sphere (if is create tool)
                        if (script.currentToolEnabled == 2)
                            Handles.SphereHandleCap(waypoint.thisWaypointId, waypoint.globalPosition, Quaternion.identity, 2, EventType.Repaint);
                        //Render this waypoint sphere (if delete tool)
                        if (script.currentToolEnabled == 3)
                            //If this not is the waypoint selected
                            if (Handles.Button(waypoint.globalPosition, Quaternion.identity, 2, 2, Handles.SphereHandleCap) == true)
                            {
                                waypoint.DeleteAndDisableThisWaypoint(script);
                                continue;
                            }
                        //Render this waypoint sphere (if is connect tool)
                        if (script.currentToolEnabled == 4)
                        {
                            //If this not is the waypoint selected
                            if (Handles.Button(waypoint.globalPosition, Quaternion.identity, 2, 2, Handles.SphereHandleCap) == true)
                                if (waypoint.thisWaypointId != script.currentSelectedWaypoint)
                                {
                                    waypoint.ConnectThisWaypointToOtherWaypoint(script, script.currentSelectedWaypoint);
                                    script.currentSelectedWaypoint = waypoint.thisWaypointId;
                                    script.currentToolEnabled = 1;
                                }
                                else
                                    Debug.LogWarning("You must click on a Waypoint other than the one selected to connect them!");
                        }
                        //Render this waypoint sphere (if is disconnect tool)
                        if (script.currentToolEnabled == 5)
                            Handles.SphereHandleCap(waypoint.thisWaypointId, waypoint.globalPosition, Quaternion.identity, 2, EventType.Repaint);
                        //Render this waypoint sphere (if is cutter tool)
                        if (script.currentToolEnabled == 6)
                            Handles.SphereHandleCap(waypoint.thisWaypointId, waypoint.globalPosition, Quaternion.identity, 2, EventType.Repaint);

                        //Render the line showing connection between waypoints
                        Handles.color = Color.blue;
                        if (waypoint.connectedWaypointsIds.Count > 0)
                            foreach (int connection in waypoint.connectedWaypointsIds)
                            {
                                //If this waypoint and this connected child waypoint is enabled, change to render a blue line
                                if (waypoint.thisWaypointIsEnabledAndUnobstructedForRoutesCalcs == true && script.currentExistingWaypointsItems[connection].thisWaypointIsEnabledAndUnobstructedForRoutesCalcs == true)
                                    Handles.color = Color.blue;
                                //If this waypoint or this connected child waypoint is disabled, change to render a black line
                                if (waypoint.thisWaypointIsEnabledAndUnobstructedForRoutesCalcs == false || script.currentExistingWaypointsItems[connection].thisWaypointIsEnabledAndUnobstructedForRoutesCalcs == false)
                                    Handles.color = Color.black;
                                //Draw half line starting of this waypoint position to current child connection id
                                Handles.DrawAAPolyLine(8f, waypoint.globalPosition, MTAssetsMathematics.GetHalfPositionBetweenTwoPoints(waypoint.globalPosition, script.currentExistingWaypointsItems[connection].globalPosition));
                                //If is the tool to delete connections
                                if (script.currentToolEnabled == 5)
                                {
                                    Handles.color = Color.yellow;
                                    if (Handles.Button(MTAssetsMathematics.GetHalfPositionBetweenTwoPoints(waypoint.globalPosition, script.currentExistingWaypointsItems[connection].globalPosition), Quaternion.identity, 1, 1, Handles.CubeHandleCap) == true)
                                    {
                                        waypoint.DisconnectThisWaypointFromOtherWaypoint(script, connection);
                                        break;
                                    }
                                    Handles.color = Color.blue;
                                }
                                //If is the tool cutter connections
                                if (script.currentToolEnabled == 6)
                                {
                                    Handles.color = Color.green;
                                    Vector3 halfPositionBetweenThisWaypointAndConnected = MTAssetsMathematics.GetHalfPositionBetweenTwoPoints(waypoint.globalPosition, script.currentExistingWaypointsItems[connection].globalPosition);
                                    if (Handles.Button(halfPositionBetweenThisWaypointAndConnected, Quaternion.identity, 1, 1, Handles.CubeHandleCap) == true)
                                    {
                                        WaypointItem newWaypoint = new WaypointItem();
                                        newWaypoint.AutomaticallyAddThisWaypointInTheListOfWaypointsExistingAndGetIdOfThis(script, halfPositionBetweenThisWaypointAndConnected);
                                        newWaypoint.ConnectThisWaypointToOtherWaypoint(script, waypoint.thisWaypointId);
                                        newWaypoint.ConnectThisWaypointToOtherWaypoint(script, connection);
                                        waypoint.DisconnectThisWaypointFromOtherWaypoint(script, connection);
                                        //Restart the loop to render all waypoints and connections again, to avoid log of errors by changing the foreach list size
                                        goto StartTheLoopAndRenderAllWaypointsAndConnections;
                                    }
                                    Handles.color = Color.blue;
                                }
                            }
                    }

                    //Set the color of controls
                    Handles.color = Color.blue;

                    //Validate the current waypoint selected
                    bool isValidSelection = (script.currentSelectedWaypoint >= script.currentExistingWaypointsItems.Count) ? false : true;

                    //If is not a valid selection, return to seletion tool and select the waypoint 0
                    if (isValidSelection == false || script.currentExistingWaypointsItems[script.currentSelectedWaypoint].thisWaypointIsDeleted == true || script.currentSelectedWaypoint == -1)
                    {
                        Debug.LogWarning("Please select a valid Waypoint before going to this tool.");
                        script.currentSelectedWaypoint = 0;
                        script.currentToolEnabled = 0;
                    }

                    //Show desired controls on selected waypoint according to the enabled tool
                    if (isValidSelection == true && script.currentSelectedWaypoint > -1 && script.currentExistingWaypointsItems[script.currentSelectedWaypoint].thisWaypointIsDeleted == false)
                    {
                        //Selection
                        if (script.currentToolEnabled == 0)
                        {
                            //Show the name of waypoint
                            GUIStyle styleForSelected = new GUIStyle();
                            styleForSelected.normal.textColor = Color.white;
                            styleForSelected.alignment = TextAnchor.MiddleCenter;
                            styleForSelected.fontStyle = FontStyle.Bold;
                            styleForSelected.contentOffset = new Vector2(-14, 30);
                            Handles.Label(script.currentExistingWaypointsItems[script.currentSelectedWaypoint].globalPosition, "Waypoint " + script.currentSelectedWaypoint, styleForSelected);
                            //If desired, show the ID of connected waypoints of current selected waypoint
                            if (script.showIdOfConnectedWaypointsOfCurrentSelectedWaypoint == true)
                            {
                                GUIStyle styleForConnecteds = new GUIStyle();
                                styleForConnecteds.normal.textColor = Color.white;
                                styleForConnecteds.alignment = TextAnchor.MiddleCenter;
                                styleForConnecteds.fontStyle = FontStyle.Bold;
                                styleForConnecteds.contentOffset = new Vector2(0, 20);
                                foreach (int connected in script.currentExistingWaypointsItems[script.currentSelectedWaypoint].connectedWaypointsIds)
                                    Handles.Label(script.currentExistingWaypointsItems[connected].globalPosition, connected.ToString(), styleForConnecteds);
                            }
                        }
                        //Move
                        if (script.currentToolEnabled == 1)
                        {
                            //Get position of selected waypoint
                            Vector3 position = script.currentExistingWaypointsItems[script.currentSelectedWaypoint].globalPosition;
                            //Show the axis for move waypoint and get new position
                            script.currentExistingWaypointsItems[script.currentSelectedWaypoint].globalPosition = Handles.DoPositionHandle(position, Quaternion.identity);
                        }
                    }

                    //Set the color of controls
                    Handles.color = Color.blue;
                }

                //If application is playing and showing routes, render the current list of positions
                if (Application.isPlaying == true)
                {
                    Handles.color = Color.blue;
                    GUIStyle style = new GUIStyle();
                    style.normal.textColor = Color.white;
                    style.alignment = TextAnchor.MiddleCenter;
                    style.fontStyle = FontStyle.Bold;
                    style.contentOffset = new Vector2(0, 0);
                    foreach (WaypointItem waypoint in script.currentExistingWaypointsItems)
                        if (waypoint.thisWaypointIsDeleted == false)
                        {
                            //Set blue color if this Waypoint is enabled
                            if (waypoint.thisWaypointIsEnabledAndUnobstructedForRoutesCalcs == true)
                                Handles.color = Color.blue;
                            //Set black color if this Waypoint is disabled
                            if (waypoint.thisWaypointIsEnabledAndUnobstructedForRoutesCalcs == false)
                                Handles.color = Color.black;
                            //Render the waypoint and waypoint name
                            Handles.SphereHandleCap(waypoint.thisWaypointId, waypoint.globalPosition, Quaternion.identity, 2, EventType.Repaint);
                            Handles.Label(waypoint.globalPosition, waypoint.thisWaypointId.ToString(), style);
                        }

                    //Draw the line if Minimap Routes is showing a route currently
                    Handles.color = script.lineRenderColor;
                    if (script.isCurrentlyShowingRoute)
                        Handles.DrawAAPolyLine(8f, script.currentRouteBeingDisplayedAndAllYourData.positionsThatMakeUpThisRoute.ToArray());
                }

                //Render a representation of startingHeightTolerance on start, if not null
                if (script.startingPoint != null)
                {
                    Handles.color = Color.green;
                    Vector3 startingPointPositiveHeight = new Vector3(script.startingPoint.position.x, script.startingPoint.position.y + script.startingHeightTolerance, script.startingPoint.position.z);
                    Vector3 startingPointNegativeHeight = new Vector3(script.startingPoint.position.x, script.startingPoint.position.y - script.startingHeightTolerance, script.startingPoint.position.z);
                    Handles.DrawAAPolyLine(8f, new Vector3[] { script.startingPoint.position, startingPointPositiveHeight });
                    Handles.DrawAAPolyLine(8f, new Vector3[] { script.startingPoint.position, startingPointNegativeHeight });
                    Handles.DrawSolidDisc(startingPointPositiveHeight, new Vector3(0, 1, 0), script.startingHeightTolerance * 0.5f);
                    Handles.DrawSolidDisc(startingPointNegativeHeight, new Vector3(0, 1, 0), script.startingHeightTolerance * 0.5f);
                    Handles.color = Color.blue;
                }
                //Render a representation of startingHeightTolerance on destination, if not null
                if (script.destinationPoint != null)
                {
                    Handles.color = script.lineRenderColor;
                    Vector3 destinationPointPositiveHeight = new Vector3(script.destinationPoint.position.x, script.destinationPoint.position.y + script.startingHeightTolerance, script.destinationPoint.position.z);
                    Vector3 destinationPointNegativeHeight = new Vector3(script.destinationPoint.position.x, script.destinationPoint.position.y - script.startingHeightTolerance, script.destinationPoint.position.z);
                    Handles.DrawAAPolyLine(8f, new Vector3[] { script.destinationPoint.position, destinationPointPositiveHeight });
                    Handles.DrawAAPolyLine(8f, new Vector3[] { script.destinationPoint.position, destinationPointNegativeHeight });
                    Handles.DrawSolidDisc(destinationPointPositiveHeight, new Vector3(0, 1, 0), script.startingHeightTolerance * 0.5f);
                    Handles.DrawSolidDisc(destinationPointNegativeHeight, new Vector3(0, 1, 0), script.startingHeightTolerance * 0.5f);
                    Handles.color = Color.blue;
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

            private void ExportationImportation_ExportWaypointsDatabase(MinimapRoutes script)
            {
                //This method will store all current waypoints in a class and convert this class to JSON and save into a file on desired path

                //Open the export window
                string folder = EditorUtility.OpenFolderPanel("Select Folder To Export", "", "");
                if (System.String.IsNullOrEmpty(folder) == true)
                    return;

                //If the count of waypoints is zero, return
                if (script.currentExistingWaypointsItems.Count == 0)
                {
                    Debug.LogError("It was not possible to export the Waypoints Database for this component. It is necessary that there is at least one Waypoint here, in order to be able to export the Database of this component.");
                    return;
                }

                //Create the class
                WaypointItemsForExportation exportation = new WaypointItemsForExportation();
                exportation.versionOfFile = VERSION_OF_WDB_FILE_SUPPORTED;
                exportation.arrayOfWaypointItems = script.currentExistingWaypointsItems.ToArray();
                string json = JsonUtility.ToJson(exportation);

                //Show progress bar
                EditorUtility.DisplayProgressBar("A moment", "Exporting Waypoint Database as JSON", 1.0f);

                //Save the json in the desired path
                System.IO.File.WriteAllText(folder + "/" + script.gameObject.name + " (Waypoints Database).wdb", json);

                //Clear progress bar
                EditorUtility.ClearProgressBar();

                //Show warning
                Debug.Log("The Waypoints Database was successfully exported to the directory \"" + folder + "\".");
            }

            private void ExportationImportation_ImportWaypointsDatabase(MinimapRoutes script)
            {
                //This method will read a WDB file and replace all waypoints inside this currentExistingWaypointsItems with the waypoints of the WDB file

                //Show the warning
                if (EditorUtility.DisplayDialog("Warning", "Importing a new Waypoints Database will overwrite all Waypoints that this \"Minimap Routes\" component has already saved. Are you sure you want to proceed with an import from another Waypoints Database?", "Yes", "No") == false)
                    return;

                //Open the import window
                string file = EditorUtility.OpenFilePanel("Select WDB File To Import", "", "wdb");
                if (System.String.IsNullOrEmpty(file) == true)
                    return;

                //Read the file
                string json = System.IO.File.ReadAllText(file);

                //Create the class
                WaypointItemsForExportation importation = JsonUtility.FromJson<WaypointItemsForExportation>(json);

                //Check if version of file is same of this, if is not equal, stop the importation
                if (importation.versionOfFile != VERSION_OF_WDB_FILE_SUPPORTED)
                {
                    Debug.LogError("It was not possible to import the Waypoint Database from file \"" + file + "\". The database in the file is in version \"" + importation.versionOfFile + "\" and the version required by this component is \"" + VERSION_OF_WDB_FILE_SUPPORTED + "\".");
                    return;
                }

                //Reset all waypoints stored in this component
                script.currentExistingWaypointsItems.Clear();

                //Import all waypoints from file to list of currentExistingWaypointsItems in this component
                foreach (WaypointItem fileWaypoint in importation.arrayOfWaypointItems)
                    script.currentExistingWaypointsItems.Add(fileWaypoint);
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
            minimapRoutesHolder = minimapDataHolderObj.transform.Find("Minimap Routes Holder");
            if (minimapRoutesHolder == null)
            {
                GameObject obj = new GameObject("Minimap Routes Holder");
                minimapRoutesHolder = obj.transform;
                minimapRoutesHolder.SetParent(minimapDataHolderObj.transform);
                minimapRoutesHolder.localPosition = Vector3.zero;
                minimapRoutesHolder.localEulerAngles = Vector3.zero;
            }
            if (minimapDataHolder.instancesOfMinimapRoutesInThisScene.Contains(this) == false)
                minimapDataHolder.instancesOfMinimapRoutesInThisScene.Add(this);

            //Create the minimap text
            tempLineObj = new GameObject("Minimap Route (" + this.gameObject.transform.name + ")");
            tempLineObj.transform.SetParent(minimapRoutesHolder);
            tempLineObj.transform.position = new Vector3(this.gameObject.transform.position.x, BASE_HEIGHT_IN_3D_WORLD, this.gameObject.transform.position.z);
            tempLine = tempLineObj.AddComponent<LineRenderer>();
            tempLineObj.layer = LayerMask.NameToLayer("UI");
            tempLine.SetPositions(new Vector3[] { new Vector3(0, 0, 0), new Vector3(0, 0, 0) });
            tempLine.alignment = LineAlignment.TransformZ;
            tempLine.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            tempLine.receiveShadows = false;
            tempLine.lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.Off;
            tempLine.reflectionProbeUsage = UnityEngine.Rendering.ReflectionProbeUsage.Off;
            tempLine.numCapVertices = 90;
            tempLine.numCornerVertices = 90;

            //Add the activity monitor to the camera
            ActivityMonitor activeMonitor = tempLineObj.AddComponent<ActivityMonitor>();
            activeMonitor.responsibleScriptComponentForThis = this;
        }

        void Start()
        {
            //Start the game doing the cache calcs needed
            DoOrUpdateAllCacheCalcsOfAllWaypointsInCurrentExistingWaypointsItems();

            //Start to show routes, if desired
            if (showRoutesOnStart == true)
                StartCalculatingAndShowRotesToDestination();
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
            tempLine.widthMultiplier = (lineWidthSize * MinimapDataGlobal.GetMinimapItemsSizeGlobalMultiplier());

            //If is not currently showing routes, disable the line renderer, if is showing routes, enable the line renderer
            tempLine.enabled = isCurrentlyShowingRoute;

            //If line size is minor or major than normal, reset
            if (lineWidthSize < 0.5f)
                lineWidthSize = 0.5f;
            if (lineWidthSize > 100.0f)
                lineWidthSize = 100.0f;

            //If is currently showing routes, and the destination or starting changed, restart calcs
            if (isCurrentlyShowingRoute == true)
                if (startingPoint != lastStartingPointDefined || destinationPoint != lastDestinationPointDefined)
                {
                    StopCalculatingAndHideRotesToDestination();
                    StartCalculatingAndShowRotesToDestination();
                    Debug.Log("The destination or start point of Minimap Routes has been changed. The algorithm is restarting the calculations to display the new route.");
                }
            //If the quantity of Waypoints in currentExistingWaypointsItems has changed, was created a new waypoint, refresh the cache calcs
            if (lastQuantityOfWaypointsInCurrentExistingWaypointsItems != currentExistingWaypointsItems.Count)
            {
                DoOrUpdateAllCacheCalcsOfAllWaypointsInCurrentExistingWaypointsItems();
                lastQuantityOfWaypointsInCurrentExistingWaypointsItems = currentExistingWaypointsItems.Count;
            }

            //Set height and rotation in zero
            this.transform.position = new Vector3(0, 0, 0);
            this.transform.rotation = Quaternion.Euler(0, 0, 0);
            this.transform.localScale = new Vector3(1, 1, 1);

            //Move the camera to follow this gameobject
            tempLineObj.transform.position = Vector3.Lerp(tempLineObj.transform.position, new Vector3(this.gameObject.transform.position.x, BASE_HEIGHT_IN_3D_WORLD, this.gameObject.transform.position.z), movementsSmoothing * Time.deltaTime);
            //Rotate the camera
            tempLineObj.transform.rotation = Quaternion.Lerp(tempLineObj.transform.rotation, Quaternion.Euler(90, 0, 0), movementsSmoothing * Time.deltaTime);
        }

        private void DoOrUpdateAllCacheCalcsOfAllWaypointsInCurrentExistingWaypointsItems()
        {
            //Calculate the distance between all Waypoints and yours neighboars and store in cache, and save the last quantity of Waypoints in currentExistingWaypointsItems
            foreach (WaypointItem waypoint in currentExistingWaypointsItems)
                waypoint.UpdateAllCostsOfThisWaypointToEachConnectedWaypointOfThis(this);

            //Run garbage colect after do this cache calcs
            System.GC.Collect();
            Resources.UnloadUnusedAssets();

            //Notify if this not is the first time that this calcs update is do
            if (lastQuantityOfWaypointsInCurrentExistingWaypointsItems != -1)
                Debug.Log("Apparently an existing Waypoint in that scene was added or removed. Because of this, the entire Waypoints network in this Minimap Routes component has been updated, including its cached calculations. Note that this update can consume part of your game's performance, until completed.");
            lastQuantityOfWaypointsInCurrentExistingWaypointsItems = currentExistingWaypointsItems.Count;
        }

        private float GetCorrespondingTimeBetweenEachRouteUpdateAccordingToRoutesUpdatesFrequency()
        {
            //Return the time interval that coroutines of routes calculation will run updates
            switch (routesUpdatesFrequency)
            {
                case RoutesUpdateFrequency.x1PerSecond:
                    return 1.0f;
                case RoutesUpdateFrequency.x2PerSecond:
                    return 0.5f;
                case RoutesUpdateFrequency.x3PerSecond:
                    return 0.33f;
                case RoutesUpdateFrequency.x4PerSecond:
                    return 0.25f;
                case RoutesUpdateFrequency.x5PerSecond:
                    return 0.2f;
                case RoutesUpdateFrequency.x10PerSecond:
                    return 0.1f;
                case RoutesUpdateFrequency.x15PerSecond:
                    return 0.066f;
                case RoutesUpdateFrequency.x30PerSecond:
                    return 0.033f;
                case RoutesUpdateFrequency.x45PerSecond:
                    return 0.022f;
                case RoutesUpdateFrequency.x60PerSecond:
                    return 0.016f;
            }

            //If not found a valid value
            return 1.0f;
        }

        private void CalculateDistanceBetweenTwoPositionsAccordingToStartingFindMethod(Vector3 point1, Vector3 point2, ref Vector3 heading, ref float distance, ref float distanceSquared)
        {
            //This method will calculate distance between two points using the method of calc defined in startingFindMethod
            if (startingPointFindMethod == StartingFindMethod.OptimizedMathCalcs) //<- Optimized math calcs
            {
                heading.x = point1.x - point2.x;
                heading.y = point1.y - point2.y;
                heading.z = point1.z - point2.z;
                distanceSquared = heading.x * heading.x + heading.y * heading.y + heading.z * heading.z;
                distance = (int)(Mathf.Sqrt(distanceSquared) * 10.0f);
            }
            if (startingPointFindMethod == StartingFindMethod.Vector3Distance) //<- Vector3.Distance
                distance = (int)(Vector3.Distance(point1, point2) * 10.0f);
            if (startingPointFindMethod == StartingFindMethod.EstimatedMagnitude) //<- Estimated distance
            {
                heading = point1 - point2;
                distance = heading.sqrMagnitude;
            }
        }

        private WaypointItem GetBestWaypointMostCloserToStartingOrDestinationPoint(ReturnBestWaypointFor returnBestWaypointFor)
        {
            //This method will return the waypoint most closer to destination or starting point

            //Get the position of selected desired point
            Vector3 desiredPointPosition = Vector3.zero;
            if (returnBestWaypointFor == ReturnBestWaypointFor.StartingPoint)
                desiredPointPosition = startingPoint.position;
            if (returnBestWaypointFor == ReturnBestWaypointFor.DestinationPoint)
                desiredPointPosition = destinationPoint.position;

            //Temp variables for optimized calcs of distance
            Vector3 heading = Vector3.zero;
            float distance = 0.0f;
            float distanceSquared = 0.0f;

            //ID and distance of best last waypoint found, most closer to desired position
            int idOfBestLastWaypointVerified = -1;
            float distanceOfBestLastWaypointVerified = float.MaxValue;

            //Start the loop in waypoints database to find the waypoint most closer to desired position (respecting the startingHeightTolerance value)
            foreach (WaypointItem currentWaypoint in currentExistingWaypointsItems)
            {
                //Calculate the distance between this waypoint and desired point using a optimized function (using find method desired)
                CalculateDistanceBetweenTwoPositionsAccordingToStartingFindMethod(currentWaypoint.globalPosition, desiredPointPosition, ref heading, ref distance, ref distanceSquared);

                //------- Waypoint validation, ignore invalids waypoints

                //If this waypoint is deleted, ignore
                if (currentWaypoint.thisWaypointIsDeleted == true)
                    continue;
                //If this waypoint is disabled, ignore
                if (currentWaypoint.thisWaypointIsEnabledAndUnobstructedForRoutesCalcs == false)
                    continue;

                //------- Best Waypoint pick conditions and rules

                //If this Waypoint has a height difference greater than startingHeightTolerance (for above or below) the starting/destination point, it is not eligible
                if (Vector3.Distance(new Vector3(0, currentWaypoint.globalPosition.y, 0), new Vector3(0, desiredPointPosition.y, 0)) > startingHeightTolerance)
                    continue;

                //If this waypoint have a distance to desired position, smaller than the last smaller distance found, take this waypoint as best waypoint most closer to desired position and save informations about this waypoint
                if (distance <= distanceOfBestLastWaypointVerified)
                {
                    idOfBestLastWaypointVerified = currentWaypoint.thisWaypointId;
                    distanceOfBestLastWaypointVerified = distance;
                }
            }
            //If not found a best waypoint respecting the startingHeightTolerance value, try to find a best waypoint most closer to desired position, without respect startingHeightTolerance
            if (idOfBestLastWaypointVerified == -1)
                foreach (WaypointItem currentWaypoint in currentExistingWaypointsItems)
                {
                    //Calculate the distance between this waypoint and desired point using a optimized function (using find method desired)
                    CalculateDistanceBetweenTwoPositionsAccordingToStartingFindMethod(currentWaypoint.globalPosition, desiredPointPosition, ref heading, ref distance, ref distanceSquared);

                    //------- Waypoint validation, ignore invalids waypoints

                    //If this waypoint is deleted, ignore
                    if (currentWaypoint.thisWaypointIsDeleted == true)
                        continue;
                    //If this waypoint is disabled, ignore
                    if (currentWaypoint.thisWaypointIsEnabledAndUnobstructedForRoutesCalcs == false)
                        continue;

                    //------- Best Waypoint pick conditions and rules

                    //If this waypoint have a distance to desired position, smaller than the last smaller distance found, take this waypoint as best waypoint most closer to desired position and save informations about this waypoint
                    if (distance <= distanceOfBestLastWaypointVerified)
                    {
                        idOfBestLastWaypointVerified = currentWaypoint.thisWaypointId;
                        distanceOfBestLastWaypointVerified = distance;
                    }
                }

            //If found a waypoint most closer to desired position
            if (idOfBestLastWaypointVerified != -1)
            {
                //If the distance of start and destination point already much closer than the minor distance possible of waypoints, cancel the pick of a first/last waypoint
                if ((int)(Vector3.Distance(startingPoint.position, destinationPoint.position) * 10.0f) <= (int)(Vector3.Distance(desiredPointPosition, currentExistingWaypointsItems[idOfBestLastWaypointVerified].globalPosition) * 10.0f))
                    return null;
                //If the best waypoint checked is most closer of desired point than the distance between starting and destination point, return the best waypoint found
                return currentExistingWaypointsItems[idOfBestLastWaypointVerified];
            }

            //Return null if not found a waypoint most closer to desired position
            return null;
        }

        private AStarProcessmentStatus GetSmallerPathOfFirstWaypointToLastWaypointUsingAStarAlgorithm(WaypointItem bestFirstWaypoint, WaypointItem bestLastWaypointFound, ref TempAStarProcessmentData storageForTemporaryDataOfThisProcessment)
        {
            //Uses the A* algorithm to find best waypoints route between best first waypoint and best last waypoint
            AStarProcessmentStatus aStarProcessmentStatus = AStarProcessmentStatus.NotCompleted;

            //Reset the lists and all waypoints
            storageForTemporaryDataOfThisProcessment.openWaypoints.Clear();
            storageForTemporaryDataOfThisProcessment.closedWaypoints.Clear();
            storageForTemporaryDataOfThisProcessment.waypointsOfRoute.Clear();
            storageForTemporaryDataOfThisProcessment.totalDistanceOfRoute = 0;
            foreach (WaypointItem waypoint in currentExistingWaypointsItems)
                waypoint.ResetAllRuntimeCacheVariablesOfThisWaypoint();

            //Initialize the waypoint of start (origin)
            bestFirstWaypoint.gCostOfOriginToThisWaypoint = 0;
            bestFirstWaypoint.hCostOfThisWaypointToDestination = (int)(Vector3.Distance(bestFirstWaypoint.globalPosition, bestLastWaypointFound.globalPosition) * 10.0f);
            bestFirstWaypoint.fCostTotalOfThisWaypointGplusH = bestFirstWaypoint.gCostOfOriginToThisWaypoint + bestFirstWaypoint.hCostOfThisWaypointToDestination;
            bestFirstWaypoint.idOfWaypointParentOfThisWaypoint = -1; //<- Set this ID because this is the waypoint of origin
            storageForTemporaryDataOfThisProcessment.openWaypoints.Add(bestFirstWaypoint);

            //Start the loop to calculate full route between start and destination point
            while (aStarProcessmentStatus == AStarProcessmentStatus.NotCompleted)
            {
                //If the list of open Waypoints is empty, a route could not be found
                if (storageForTemporaryDataOfThisProcessment.openWaypoints.Count == 0)
                {
                    aStarProcessmentStatus = AStarProcessmentStatus.NotFoundRoute;
                    continue;
                }

                //Order the open list by the smaller F cost foreach waypoint
                storageForTemporaryDataOfThisProcessment.openWaypoints = storageForTemporaryDataOfThisProcessment.openWaypoints.OrderBy(waypoint => waypoint.fCostTotalOfThisWaypointGplusH).ToList();
                //Get the current waypoint of smaller F cost
                WaypointItem waypointSmallerFCost = storageForTemporaryDataOfThisProcessment.openWaypoints[0];
                //Remove this waypoint from openlist
                storageForTemporaryDataOfThisProcessment.openWaypoints.RemoveAt(0);
                //Add this waypoint to closedlist
                storageForTemporaryDataOfThisProcessment.closedWaypoints.Add(waypointSmallerFCost);

                //Check if this waypoint is the desired final waypoint (if is, stop the algorithm and make a reverse way to go from this to the origin waypoint and track the route)
                if (waypointSmallerFCost.thisWaypointId == bestLastWaypointFound.thisWaypointId)
                {
                    aStarProcessmentStatus = AStarProcessmentStatus.FoundRoute;
                    WaypointItem currentWaypointBeingAnalyzed = waypointSmallerFCost;
                    while (currentWaypointBeingAnalyzed.thisWaypointId != -1)
                    {
                        //If this already is origin waypoint, break the loop and stop the algorithm
                        if (currentWaypointBeingAnalyzed.idOfWaypointParentOfThisWaypoint == -1)
                            break;
                        //Add this waypoint in the list of waypoints that make this route
                        storageForTemporaryDataOfThisProcessment.waypointsOfRoute.Add(currentWaypointBeingAnalyzed);
                        //Sum the distance between this waypoint, and parent waypoint of this to make total distance of this route (ignore the best first waypoint in total distance of route, because not will included in the line route renderization)
                        if (currentWaypointBeingAnalyzed.idOfWaypointParentOfThisWaypoint != bestFirstWaypoint.thisWaypointId)
                            for (int i = 0; i < currentWaypointBeingAnalyzed.connectedWaypointsIds.Count; i++)
                                if (currentWaypointBeingAnalyzed.connectedWaypointsIds[i] == currentWaypointBeingAnalyzed.idOfWaypointParentOfThisWaypoint)
                                    storageForTemporaryDataOfThisProcessment.totalDistanceOfRoute += currentWaypointBeingAnalyzed.movementCostOfThisWaypointToEachConnectedWaypointOfThis[i];
                        //Set the waypoint parent of this, to be the next to be analyzed
                        currentWaypointBeingAnalyzed = currentExistingWaypointsItems[currentWaypointBeingAnalyzed.idOfWaypointParentOfThisWaypoint];
                    }
                    continue;
                }

                //Process all connected waypoints
                for (int i = 0; i < waypointSmallerFCost.connectedWaypointsIds.Count; i++)
                {
                    //Get current node
                    WaypointItem currentNode = currentExistingWaypointsItems[waypointSmallerFCost.connectedWaypointsIds[i]];
                    //If this waypoint is deleted, skip this
                    if (currentNode.thisWaypointIsDeleted == true)
                        continue;
                    //If this waypoint is disabled, skip this
                    if (currentNode.thisWaypointIsEnabledAndUnobstructedForRoutesCalcs == false)
                        continue;
                    //If this waypoint is in list of closedwaypoints, skip this
                    if (storageForTemporaryDataOfThisProcessment.closedWaypoints.Contains(currentNode) == true)
                        continue;
                    //If this connected waypoint is already on openlist, try to find a new best route to he, through this current waypoint
                    if (storageForTemporaryDataOfThisProcessment.openWaypoints.Contains(currentNode) == true)
                    {
                        int betterGCost = waypointSmallerFCost.gCostOfOriginToThisWaypoint + waypointSmallerFCost.movementCostOfThisWaypointToEachConnectedWaypointOfThis[i];
                        if (betterGCost < currentNode.gCostOfOriginToThisWaypoint)
                        {
                            currentNode.idOfWaypointParentOfThisWaypoint = waypointSmallerFCost.thisWaypointId;
                            currentNode.gCostOfOriginToThisWaypoint = betterGCost;
                            currentNode.fCostTotalOfThisWaypointGplusH = currentNode.gCostOfOriginToThisWaypoint + currentNode.hCostOfThisWaypointToDestination;
                        }
                    }
                    else
                    {
                        currentNode.idOfWaypointParentOfThisWaypoint = waypointSmallerFCost.thisWaypointId;
                        currentNode.gCostOfOriginToThisWaypoint = waypointSmallerFCost.gCostOfOriginToThisWaypoint + waypointSmallerFCost.movementCostOfThisWaypointToEachConnectedWaypointOfThis[i];
                        currentNode.hCostOfThisWaypointToDestination = (int)(Vector3.Distance(currentNode.globalPosition, bestLastWaypointFound.globalPosition) * 10.0f);
                        currentNode.fCostTotalOfThisWaypointGplusH = currentNode.gCostOfOriginToThisWaypoint + currentNode.hCostOfThisWaypointToDestination;
                        storageForTemporaryDataOfThisProcessment.openWaypoints.Add(currentNode);
                    }
                }
            }

            //Return the response if found a route
            return aStarProcessmentStatus;
        }

        private IEnumerator RotesCalculationAndRenderizationRoutine()
        {
            //Set the time between updates
            WaitForSecondsRealtime timeBetweenUpdates = new WaitForSecondsRealtime(GetCorrespondingTimeBetweenEachRouteUpdateAccordingToRoutesUpdatesFrequency());
            lastRoutesUpdatesFrequency = routesUpdatesFrequency;

            //Create a new class to store all temporary data of AStart algorithm processment data, of this loop
            TempAStarProcessmentData temporaryAStarAlgorithmExecutionsOfThisLoop = new TempAStarProcessmentData();

            //Start the loop of calculation of route
            while (true)
            {
#if UNITY_EDITOR
                //Start the stopwatch to analyze the routes calculation time between each update
                var watch = System.Diagnostics.Stopwatch.StartNew();
#endif

                //Start the calculation of routes logic and renderization
                if (currentExistingWaypointsItems.Count > 0)
                {
                    //First, reset or clear all data of currentRouteBeingDisplayedAndAllYourData, to store all data of route that will be processed by this interation of loop
                    currentRouteBeingDisplayedAndAllYourData.positionsThatMakeUpThisRoute.Clear();
                    currentRouteBeingDisplayedAndAllYourData.positionsThatMakeUpThisRoute.Add(startingPoint.position);
                    currentRouteBeingDisplayedAndAllYourData.waypointsIdsThatMakeUpThisRoute.Clear();
                    currentRouteBeingDisplayedAndAllYourData.totalDistanceOfRouteSinceStartPointToDestination = 0.0f;
                    currentRouteBeingDisplayedAndAllYourData.isThisRouteHasSomeProblemToBeCalculated = false;

                    //Now pick the most closest waypoint of starting point to be the first waypoint of route
                    WaypointItem bestFirstWaypointClosestToStartingPoint = GetBestWaypointMostCloserToStartingOrDestinationPoint(ReturnBestWaypointFor.StartingPoint);
                    if (bestFirstWaypointClosestToStartingPoint != null)
                    {
                        currentRouteBeingDisplayedAndAllYourData.positionsThatMakeUpThisRoute.Add(bestFirstWaypointClosestToStartingPoint.globalPosition);
                        currentRouteBeingDisplayedAndAllYourData.waypointsIdsThatMakeUpThisRoute.Add(bestFirstWaypointClosestToStartingPoint.thisWaypointId);
                    }

                    //Now pick the most closest waypoint of destination point to be the last waypoint of route
                    WaypointItem bestLastWaypointClosestToDestinationPoint = GetBestWaypointMostCloserToStartingOrDestinationPoint(ReturnBestWaypointFor.DestinationPoint);

                    //If have differents first waypoint and last waypoints found, calculate the full smallest path between these two waypoints using AStar algorithm
                    if (bestFirstWaypointClosestToStartingPoint != null && bestLastWaypointClosestToDestinationPoint != null)
                    {
                        if (bestFirstWaypointClosestToStartingPoint.thisWaypointId != bestLastWaypointClosestToDestinationPoint.thisWaypointId)
                        {
                            //Pass the bestFirstWaypointClosestToStartingPoint and bestLastWaypointClosestToDestinationPoint to AStar algorithm to check if is possible to find a best smaller route between first and last waypoints
                            AStarProcessmentStatus response = GetSmallerPathOfFirstWaypointToLastWaypointUsingAStarAlgorithm(bestFirstWaypointClosestToStartingPoint, bestLastWaypointClosestToDestinationPoint, ref temporaryAStarAlgorithmExecutionsOfThisLoop);
                            //If the AStar algorithm found a route, does the reverse way, starting from the final waypoint, to the origin waypoint calculated by the AStar algorithm 
                            if (response == AStarProcessmentStatus.FoundRoute)
                            {
                                for (int i = (temporaryAStarAlgorithmExecutionsOfThisLoop.waypointsOfRoute.Count - 1); i > 0; i--)
                                {
                                    WaypointItem currentWaypoint = temporaryAStarAlgorithmExecutionsOfThisLoop.waypointsOfRoute[i];
                                    currentRouteBeingDisplayedAndAllYourData.positionsThatMakeUpThisRoute.Add(currentWaypoint.globalPosition);
                                    currentRouteBeingDisplayedAndAllYourData.waypointsIdsThatMakeUpThisRoute.Add(currentWaypoint.thisWaypointId);
                                }
                                currentRouteBeingDisplayedAndAllYourData.totalDistanceOfRouteSinceStartPointToDestination += (temporaryAStarAlgorithmExecutionsOfThisLoop.totalDistanceOfRoute) / 10.0f;

                                //Calculate distances between best last waypoint and destination.
                                //Remove the best first waypoint from the list of positions that make the route, to avoid problems where the line of route stay on back of starting point (player for example) on rendering
                                currentRouteBeingDisplayedAndAllYourData.positionsThatMakeUpThisRoute.RemoveAt(1);
                                currentRouteBeingDisplayedAndAllYourData.waypointsIdsThatMakeUpThisRoute.RemoveAt(0);
                                if (currentRouteBeingDisplayedAndAllYourData.positionsThatMakeUpThisRoute.Count() == 1)
                                    currentRouteBeingDisplayedAndAllYourData.totalDistanceOfRouteSinceStartPointToDestination += Vector3.Distance(startingPoint.position, bestLastWaypointClosestToDestinationPoint.globalPosition);
                                if (currentRouteBeingDisplayedAndAllYourData.positionsThatMakeUpThisRoute.Count() > 1)
                                    currentRouteBeingDisplayedAndAllYourData.totalDistanceOfRouteSinceStartPointToDestination += Vector3.Distance(startingPoint.position, currentRouteBeingDisplayedAndAllYourData.positionsThatMakeUpThisRoute[1]);
                                currentRouteBeingDisplayedAndAllYourData.totalDistanceOfRouteSinceStartPointToDestination += Vector3.Distance(destinationPoint.position, bestLastWaypointClosestToDestinationPoint.globalPosition);
                            }
                            //If the AStar algorithm not found a route
                            if (response == AStarProcessmentStatus.NotFoundRoute)
                                currentRouteBeingDisplayedAndAllYourData.isThisRouteHasSomeProblemToBeCalculated = true;
                        }
                        //If the best first and best last waypoints is same, calculate distance
                        if (bestFirstWaypointClosestToStartingPoint.thisWaypointId == bestLastWaypointClosestToDestinationPoint.thisWaypointId)
                        {
                            currentRouteBeingDisplayedAndAllYourData.positionsThatMakeUpThisRoute.RemoveAt(1);
                            currentRouteBeingDisplayedAndAllYourData.waypointsIdsThatMakeUpThisRoute.RemoveAt(0);
                            currentRouteBeingDisplayedAndAllYourData.totalDistanceOfRouteSinceStartPointToDestination += Vector3.Distance(startingPoint.position, destinationPoint.position);
                        }
                    }
                    //If the best first waypoint or last is null, calculate the distance between starting point to destination point
                    if (bestFirstWaypointClosestToStartingPoint == null || bestLastWaypointClosestToDestinationPoint == null)
                        currentRouteBeingDisplayedAndAllYourData.totalDistanceOfRouteSinceStartPointToDestination += Vector3.Distance(startingPoint.position, destinationPoint.position);

                    //Last add the position of last best waypoint (if have) after it, destination (and if not ocurred problems during the build of route between best first and best last waypoins)
                    if (currentRouteBeingDisplayedAndAllYourData.isThisRouteHasSomeProblemToBeCalculated == false)
                    {
                        if (bestFirstWaypointClosestToStartingPoint != null && bestLastWaypointClosestToDestinationPoint != null)
                            if (bestFirstWaypointClosestToStartingPoint.idOfWaypointParentOfThisWaypoint != bestLastWaypointClosestToDestinationPoint.idOfWaypointParentOfThisWaypoint)
                            {
                                currentRouteBeingDisplayedAndAllYourData.positionsThatMakeUpThisRoute.Add(bestLastWaypointClosestToDestinationPoint.globalPosition);
                                currentRouteBeingDisplayedAndAllYourData.waypointsIdsThatMakeUpThisRoute.Add(bestLastWaypointClosestToDestinationPoint.thisWaypointId);
                            }
                        currentRouteBeingDisplayedAndAllYourData.positionsThatMakeUpThisRoute.Add(destinationPoint.position);
                    }
                    if (currentRouteBeingDisplayedAndAllYourData.isThisRouteHasSomeProblemToBeCalculated == true)
                        Debug.LogWarning("There were problems building the route between the start point and the destination point. Check that all Waypoints in your scene have at least 1 connection each and if the current path has Waypoints disabled at the entrance and exit. Also check if the Waypoints in your scene, obey the good practices of creating Waypoints. Please read the documentation to find help, but if you still can't solve the problem, please contact MT Assets support email.");

                    //Show the positions in line renderer of this component
                    tempLine.positionCount = currentRouteBeingDisplayedAndAllYourData.positionsThatMakeUpThisRoute.Count;
                    for (int i = 0; i < currentRouteBeingDisplayedAndAllYourData.positionsThatMakeUpThisRoute.Count; i++)
                        tempLine.SetPosition(i, new Vector3(currentRouteBeingDisplayedAndAllYourData.positionsThatMakeUpThisRoute[i].x, BASE_HEIGHT_IN_3D_WORLD, currentRouteBeingDisplayedAndAllYourData.positionsThatMakeUpThisRoute[i].z));
                }

                //Run events registered if have
                if (onUpdateRoutesRenderization != null)
                    onUpdateRoutesRenderization.Invoke();

                //Verify if time between updates has changed
                if (routesUpdatesFrequency != lastRoutesUpdatesFrequency)
                {
                    timeBetweenUpdates = new WaitForSecondsRealtime(GetCorrespondingTimeBetweenEachRouteUpdateAccordingToRoutesUpdatesFrequency());
                    lastRoutesUpdatesFrequency = routesUpdatesFrequency;
                }

#if UNITY_EDITOR
                //End the stopwatch to analyze the routes calculation time between each update
                watch.Stop();
                timeOfCalculationOfLastRoute = (float)System.Math.Round((float)watch.ElapsedTicks / (float)System.TimeSpan.TicksPerMillisecond, 1);
#endif

                //Wait the time for update route
                yield return timeBetweenUpdates;
            }
        }

        //Public methods

        public bool isCurrentlyShowingRoutes()
        {
            //Return true if this component already displaying routes
            return isCurrentlyShowingRoute;
        }

        public void StartCalculatingAndShowRotesToDestination()
        {
            //If is currently already showing a route
            if (isCurrentlyShowingRoute == true)
            {
                Debug.Log("The Minimap Routes component is already displaying a route, so it cannot be displayed again.");
                return;
            }
            //If the start or destination point is null, cancel
            if (startingPoint == null || destinationPoint == null)
            {
                Debug.LogError("It was not possible to start the calculations and display routes because the start point or the destination point were not informed!");
                return;
            }

            //Start the routine for calculation and render of routes
            if (currentlyRunningRoutineForCalcsAndBuildOfRoute == null)
                currentlyRunningRoutineForCalcsAndBuildOfRoute = StartCoroutine(RotesCalculationAndRenderizationRoutine());

            //Update the last destination and starting points defined
            lastDestinationPointDefined = destinationPoint;
            lastStartingPointDefined = startingPoint;

            //Inform that is showing routes
            isCurrentlyShowingRoute = true;
        }

        public void StopCalculatingAndHideRotesToDestination()
        {
            //If is currently not showing a route
            if (isCurrentlyShowingRoute == false)
            {
                Debug.Log("The Minimap Routes component is not displaying a route, so it is not possible to stop the display.");
                return;
            }

            //If this component is showing a route
            if (currentlyRunningRoutineForCalcsAndBuildOfRoute != null)
                StopCoroutine(currentlyRunningRoutineForCalcsAndBuildOfRoute);
            currentlyRunningRoutineForCalcsAndBuildOfRoute = null;

            //Clear the list of points that make the current route, and reset the all data about current being displayed route
            currentRouteBeingDisplayedAndAllYourData.isThisRouteHasSomeProblemToBeCalculated = false;
            currentRouteBeingDisplayedAndAllYourData.positionsThatMakeUpThisRoute.Clear();
            currentRouteBeingDisplayedAndAllYourData.waypointsIdsThatMakeUpThisRoute.Clear();
            currentRouteBeingDisplayedAndAllYourData.totalDistanceOfRouteSinceStartPointToDestination = 0.0f;

            //Inform that is not showing routes
            isCurrentlyShowingRoute = false;
        }

        public CurrentDisplayingRoute GetCurrentGeneratedRouteIfIsCalculingAndShowingRoutes()
        {
            //If is not showing routes, cancel
            if (isCurrentlyShowingRoute == false)
            {
                Debug.LogError("It was not possible to get the current Route from the start point to the destination point as Minimap Routes is not currently displaying routes.");
                return null;
            }

            //Return a CurrentDisplayingRoute object with all data about current route being displayed be this Minimap Routes
            return currentRouteBeingDisplayedAndAllYourData;
        }

        public void SetWaypointEnabled(int waypointId, bool enable)
        {
            //This method can disable or enable a especific waypoint
            currentExistingWaypointsItems[waypointId].thisWaypointIsEnabledAndUnobstructedForRoutesCalcs = enable;
        }

        public bool isWaypointEnabled(int waypointId)
        {
            //This method will return true, if a waypoint is enabled
            return currentExistingWaypointsItems[waypointId].thisWaypointIsEnabledAndUnobstructedForRoutesCalcs;
        }

        public void SetAllWaypointsEnabled(bool enable)
        {
            //This method will enable or disable all waypoints
            for (int i = 0; i < currentExistingWaypointsItems.Count; i++)
                if (currentExistingWaypointsItems[i].thisWaypointIsDeleted == false)
                    currentExistingWaypointsItems[i].thisWaypointIsEnabledAndUnobstructedForRoutesCalcs = enable;
        }

        public Vector3 GetWaypointPosition(int waypointId)
        {
            //This method return the world position of a waypoint
            return currentExistingWaypointsItems[waypointId].globalPosition;
        }

        public int[] GetIdOfAllExistantsWaypoints()
        {
            //This method will retun a list that contains all ids of all existants waypoints
            List<int> allWaypointsIds = new List<int>();

            //Fill the list
            for (int i = 0; i < currentExistingWaypointsItems.Count; i++)
                if (currentExistingWaypointsItems[i].thisWaypointIsDeleted == false)
                    allWaypointsIds.Add(currentExistingWaypointsItems[i].thisWaypointId);

            //Return the list
            return allWaypointsIds.ToArray();
        }

        public int[] GetIdsOfWaypointsConnectedsToThis(int waypointId)
        {
            //This method will retun a list of ids of all waypoints connected to a especific waypoint
            List<int> allWaypointsIds = new List<int>();

            //Fill the list
            for (int i = 0; i < currentExistingWaypointsItems[waypointId].connectedWaypointsIds.Count; i++)
                allWaypointsIds.Add(currentExistingWaypointsItems[waypointId].connectedWaypointsIds[i]);

            //Return the list
            return allWaypointsIds.ToArray();
        }

        public void ForceMinimapRoutesToCalculateOrUpdateAllCachesAgain()
        {
            //This method force this component to re-do all calcs and caches processed, to update the data etc
            DoOrUpdateAllCacheCalcsOfAllWaypointsInCurrentExistingWaypointsItems();
        }

        public bool isThisMinimapLineRouteBeingVisibleByAnyMinimapCamera()
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

        public MinimapRoutes[] GetListOfAllMinimapRoutesInThisScene()
        {
            //If is not playing, cancel
            if (Application.isPlaying == false)
            {
                Debug.LogError("It is only possible to obtain the list of Minimap Routes in this scene, if the application is being executed.");
                return null;
            }

            //Return a list that contains reference to all of this component in this scene
            return minimapDataHolder.instancesOfMinimapRoutesInThisScene.ToArray();
        }
    }
}