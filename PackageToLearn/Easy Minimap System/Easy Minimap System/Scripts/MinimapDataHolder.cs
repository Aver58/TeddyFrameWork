#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MTAssets.EasyMinimapSystem
{
    /*
     This class is responsible for the functioning of the "Minimap Data Holder" component, and all its functions.
    */
    /*
     * The Easy Minimap System was developed by Marcos Tomaz in 2019.
     * Need help? Contact me (mtassets@windsoft.xyz)
    */

    [AddComponentMenu("")] //Hide this script in component menu.
    public class MinimapDataHolder : MonoBehaviour
    {
        //Public variables
        ///<summary>[WARNING] Do not change the value of this variable. This is a variable used for internal tool operations.</summary> 
        [HideInInspector]
        public Material defaultMaterialForMinimapItems;
        ///<summary>[WARNING] Do not change the value of this variable. This is a variable used for internal tool operations.</summary> 
        [HideInInspector]
        public List<MinimapRoutes> instancesOfMinimapRoutesInThisScene = new List<MinimapRoutes>();
        ///<summary>[WARNING] Do not change the value of this variable. This is a variable used for internal tool operations.</summary> 
        [HideInInspector]
        public List<MinimapScanner> instancesOfMinimapScannerInThisScene = new List<MinimapScanner>();
        ///<summary>[WARNING] Do not change the value of this variable. This is a variable used for internal tool operations.</summary> 
        [HideInInspector]
        public List<MinimapText> instancesOfMinimapTextInThisScene = new List<MinimapText>();
        ///<summary>[WARNING] Do not change the value of this variable. This is a variable used for internal tool operations.</summary> 
        [HideInInspector]
        public List<MinimapCamera> instancesOfMinimapCameraInThisScene = new List<MinimapCamera>();
        ///<summary>[WARNING] Do not change the value of this variable. This is a variable used for internal tool operations.</summary> 
        [HideInInspector]
        public List<MinimapItem> instancesOfMinimapItemInThisScene = new List<MinimapItem>();
        ///<summary>[WARNING] Do not change the value of this variable. This is a variable used for internal tool operations.</summary> 
        [HideInInspector]
        public List<MinimapLine> instancesOfMinimapLineInThisScene = new List<MinimapLine>();
        ///<summary>[WARNING] Do not change the value of this variable. This is a variable used for internal tool operations.</summary> 
        [HideInInspector]
        public List<MinimapRenderer> instancesOfMinimapRendererInThisScene = new List<MinimapRenderer>();
        ///<summary>[WARNING] Do not change the value of this variable. This is a variable used for internal tool operations.</summary> 
        [HideInInspector]
        public List<MinimapFog> instancesOfMinimapFogsInThisScene = new List<MinimapFog>();
        ///<summary>[WARNING] Do not change the value of this variable. This is a variable used for internal tool operations.</summary> 
        [HideInInspector]
        public List<MinimapCompass> instancesOfMinimapCompassInThisScene = new List<MinimapCompass>();

#if UNITY_EDITOR
        //Public variables of Interface
        private bool gizmosOfThisComponentIsDisabled = false;

        //The UI of this component
        #region INTERFACE_CODE
        [UnityEditor.CustomEditor(typeof(MinimapDataHolder))]
        public class CustomInspector : UnityEditor.Editor
        {
            //Private variables of Editor Only
            private Vector2 minimapRoutesScrollpos = Vector2.zero;
            private Vector2 minimapScannerScrollpos = Vector2.zero;
            private Vector2 minimapTextScrollpos = Vector2.zero;
            private Vector2 minimapCameraScrollpos = Vector2.zero;
            private Vector2 minimapItemScrollpos = Vector2.zero;
            private Vector2 minimapLineScrollpos = Vector2.zero;
            private Vector2 minimapRendererScrollpos = Vector2.zero;
            private Vector2 minimapFogsScrollpos = Vector2.zero;
            private Vector2 minimapCompassScrollpos = Vector2.zero;

            public override void OnInspectorGUI()
            {
                //Start the undo event support, draw default inspector and monitor of changes
                MinimapDataHolder script = (MinimapDataHolder)target;
                EditorGUI.BeginChangeCheck();
                Undo.RecordObject(target, "Undo Event");
                script.gizmosOfThisComponentIsDisabled = MTAssetsEditorUi.DisableGizmosInSceneView("MinimapDataHolder", script.gizmosOfThisComponentIsDisabled);

                //Support reminder
                GUILayout.Space(10);
                EditorGUILayout.HelpBox("Remember to read the Easy Minimap System documentation to understand how to use it.\nGet support at: mtassets@windsoft.xyz", MessageType.None);
                GUILayout.Space(10);

                //Minimap Routes
                minimapRoutesScrollpos = StartDrawListOfInstancesOfComponentHere(minimapRoutesScrollpos, "Minimap Routes", script.instancesOfMinimapRoutesInThisScene.Count);
                foreach (MinimapRoutes item in script.instancesOfMinimapRoutesInThisScene)
                    RenderDrawItemOfListOfComponentHere(item);
                FinalizeDrawListOfInstancesOfComponentHere();

                //Minimap Scanner
                minimapScannerScrollpos = StartDrawListOfInstancesOfComponentHere(minimapScannerScrollpos, "Minimap Scanners", script.instancesOfMinimapScannerInThisScene.Count);
                foreach (MinimapScanner item in script.instancesOfMinimapScannerInThisScene)
                    RenderDrawItemOfListOfComponentHere(item);
                FinalizeDrawListOfInstancesOfComponentHere();

                //Minimap Text
                minimapTextScrollpos = StartDrawListOfInstancesOfComponentHere(minimapTextScrollpos, "Minimap Texts", script.instancesOfMinimapTextInThisScene.Count);
                foreach (MinimapText item in script.instancesOfMinimapTextInThisScene)
                    RenderDrawItemOfListOfComponentHere(item);
                FinalizeDrawListOfInstancesOfComponentHere();

                //Minimap Camera
                minimapCameraScrollpos = StartDrawListOfInstancesOfComponentHere(minimapCameraScrollpos, "Minimap Cameras", script.instancesOfMinimapCameraInThisScene.Count);
                foreach (MinimapCamera item in script.instancesOfMinimapCameraInThisScene)
                    RenderDrawItemOfListOfComponentHere(item);
                FinalizeDrawListOfInstancesOfComponentHere();

                //Minimap Item
                minimapItemScrollpos = StartDrawListOfInstancesOfComponentHere(minimapItemScrollpos, "Minimap Items", script.instancesOfMinimapItemInThisScene.Count);
                foreach (MinimapItem item in script.instancesOfMinimapItemInThisScene)
                    RenderDrawItemOfListOfComponentHere(item);
                FinalizeDrawListOfInstancesOfComponentHere();

                //Minimap Item
                minimapLineScrollpos = StartDrawListOfInstancesOfComponentHere(minimapLineScrollpos, "Minimap Lines", script.instancesOfMinimapLineInThisScene.Count);
                foreach (MinimapLine item in script.instancesOfMinimapLineInThisScene)
                    RenderDrawItemOfListOfComponentHere(item);
                FinalizeDrawListOfInstancesOfComponentHere();

                //Minimap Renderer
                minimapRendererScrollpos = StartDrawListOfInstancesOfComponentHere(minimapRendererScrollpos, "Minimap Renderers", script.instancesOfMinimapRendererInThisScene.Count);
                foreach (MinimapRenderer item in script.instancesOfMinimapRendererInThisScene)
                    RenderDrawItemOfListOfComponentHere(item);
                FinalizeDrawListOfInstancesOfComponentHere();

                //Minimap Fogs
                minimapFogsScrollpos = StartDrawListOfInstancesOfComponentHere(minimapFogsScrollpos, "Minimap Fogs", script.instancesOfMinimapCompassInThisScene.Count);
                foreach (MinimapFog item in script.instancesOfMinimapFogsInThisScene)
                    RenderDrawItemOfListOfComponentHere(item);
                FinalizeDrawListOfInstancesOfComponentHere();

                //Minimap Compass
                minimapCompassScrollpos = StartDrawListOfInstancesOfComponentHere(minimapCompassScrollpos, "Minimap Compass", script.instancesOfMinimapCompassInThisScene.Count);
                foreach (MinimapCompass item in script.instancesOfMinimapCompassInThisScene)
                    RenderDrawItemOfListOfComponentHere(item);
                FinalizeDrawListOfInstancesOfComponentHere();

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

            private Vector2 StartDrawListOfInstancesOfComponentHere(Vector2 thisListPosition, string nameOfComponent, int quantityOfComponents)
            {
                //This method will renderize (the start) a list of components registered in some list
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(nameOfComponent + " On This Scene", GUILayout.Width(230));
                GUILayout.Space(MTAssetsEditorUi.GetInspectorWindowSize().x - 230);
                EditorGUILayout.LabelField("Size", GUILayout.Width(30));
                EditorGUILayout.IntField(quantityOfComponents, GUILayout.Width(50));
                EditorGUILayout.EndHorizontal();
                GUILayout.BeginVertical("box");
                return EditorGUILayout.BeginScrollView(thisListPosition, GUIStyle.none, GUI.skin.verticalScrollbar, GUILayout.Width(MTAssetsEditorUi.GetInspectorWindowSize().x), GUILayout.Height(100));
            }

            private void RenderDrawItemOfListOfComponentHere(MonoBehaviour item)
            {
                //This method will render a item in loop to render all items in a list
                GUILayout.Space(2);
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.BeginVertical();
                if (item != null)
                    EditorGUILayout.LabelField(item.gameObject.name, EditorStyles.boldLabel);
                if (item == null)
                    EditorGUILayout.LabelField("Component Not Found");
                GUILayout.Space(-3);
                if (item == null)
                    EditorGUILayout.LabelField("Component Not Found");
                if (item != null)
                {
                    if (item.enabled == false || item.gameObject.activeInHierarchy == false)
                        EditorGUILayout.LabelField("Disabled");
                    if (item.enabled == true && item.gameObject.activeInHierarchy == true)
                        EditorGUILayout.LabelField("Enabled");
                }
                EditorGUILayout.EndVertical();
                GUILayout.Space(20);
                EditorGUILayout.BeginVertical();
                GUILayout.Space(8);
                if (item != null)
                    if (GUILayout.Button("Game Object", GUILayout.Height(20)))
                        EditorGUIUtility.PingObject(item.gameObject);
                EditorGUILayout.EndVertical();
                EditorGUILayout.EndHorizontal();
                GUILayout.Space(2);
            }

            private void FinalizeDrawListOfInstancesOfComponentHere()
            {
                //This method will renderize (the end) a list of components registered in some list
                EditorGUILayout.EndScrollView();
                GUILayout.EndVertical();
            }
        }
        #endregion
#endif

        //Core methods

        private void Awake()
        {
            //Create a Sprite Renderer and Image to get your unity builtin default material/texture and store it
            SpriteRenderer spriteRenderer = this.gameObject.AddComponent<SpriteRenderer>();
            defaultMaterialForMinimapItems = new Material(spriteRenderer.materials[0].shader);
            defaultMaterialForMinimapItems.CopyPropertiesFromMaterial(spriteRenderer.materials[0]);
            defaultMaterialForMinimapItems.name = "Default Material For Minimap Items";
            GameObject imageHolder = new GameObject();
            Image image = imageHolder.AddComponent<Image>();
            defaultMaterialForMinimapItems.mainTexture = image.material.mainTexture;
            Destroy(spriteRenderer);
            Destroy(imageHolder);
        }
    }
}