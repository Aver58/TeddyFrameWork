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
     This class is responsible for the functioning of the "Minimap Compass" component, and all its functions.
    */
    /*
     * The Easy Minimap System was developed by Marcos Tomaz in 2019.
     * Need help? Contact me (mtassets@windsoft.xyz)
    */

    [AddComponentMenu("MT Assets/Easy Minimap System/Minimap Compass")] //Add this component in a category of addComponent menu
    [RequireComponent(typeof(RectTransform))]
    public class MinimapCompass : MonoBehaviour
    {
        //Private constants
        private const float MIN_COUNT_OF_ITEMS_IN_POOL = 100;

        //Private variables
        private GameObject minimapDataHolderObj;
        private MinimapDataHolder minimapDataHolder;

        //Private variables to refer tho the gameobjects structure of minimap compass
        private RectTransform thisRectTransform;
        private GameObject maskImageGameobject;
        private Image maskImageComponent;
        private Mask maskComponent;
        private GameObject rendererRawImageGameobject;
        private RawImage rawImageComponent;
        private GameObject degreesTextGameobject;
        private Text degreesTextComponent;
        private GameObject poolRootForHighlightedItemsGameObject;

        //Private cache variables
        private NorthDirection lastNorthDirectionSelected = NorthDirection.AxisX;
        private Vector2 lastMinimapCompassSize = Vector2.zero;
        private float northDirectionOffset = 0;
        private int lastDegreesToNorth = -1;
        private bool alreadyCalculatedTheNorthDirectionOffset = false;
        private bool alreadyAdjustedSizeOfCompassOnChangeRawImage = false;
        private DegreesPosition lastDegreesPosition = DegreesPosition.Top;
        private bool alreadyAdjustedDegreesTextPosition = false;
        private List<MinimapItemOfPool> minimapItemsForHighlightPool = new List<MinimapItemOfPool>();

        //Enums of script
        public enum NorthDirection
        {
            AxisZ,
            AxisX,
            NegativeAxisZ,
            NegativeAxisX
        }
        public enum DegreesPosition
        {
            Top,
            Bottom
        }

        //Classes of script
        public class MinimapItemOfPool
        {
            //Data set of pools of minimap items
            public GameObject pivot;
            public Image icon;
            public MinimapItem minimapItemAssociated;

            public MinimapItemOfPool(GameObject pivot, Image icon)
            {
                this.pivot = pivot;
                this.icon = icon;
            }
        }

        //Public variables
        [HideInInspector]
        public Transform targetPlayerTransform;
        [HideInInspector]
        public NorthDirection northDirection = NorthDirection.AxisZ;
        [HideInInspector]
        public Texture compassTexture;
        [HideInInspector]
        public Color compassTextureColor = Color.white;
        [HideInInspector]
        public bool showDegreesToNorth = true;
        [HideInInspector]
        public DegreesPosition degreesPosition = DegreesPosition.Top;
        [HideInInspector]
        public Font degreesFont = null;
        [HideInInspector]
        public int degreesFontSize = 14;
        [HideInInspector]
        public Color degreesFontColor = Color.black;
        [HideInInspector]
        public FontStyle degreesFontStyle = FontStyle.Bold;
        [HideInInspector]
        public List<MinimapItem> minimapItemsToHightlight = new List<MinimapItem>();

#if UNITY_EDITOR
        //The UI of this component
        #region INTERFACE_CODE
        [UnityEditor.CustomEditor(typeof(MinimapCompass))]
        public class CustomInspector : UnityEditor.Editor
        {
            Vector2 minimapItemsToHighlight_ScrollPos;

            public override void OnInspectorGUI()
            {
                //Start the undo event support, draw default inspector and monitor of changes
                MinimapCompass script = (MinimapCompass)target;
                EditorGUI.BeginChangeCheck();
                Undo.RecordObject(target, "Undo Event");

                //Support reminder
                GUILayout.Space(10);
                EditorGUILayout.HelpBox("Remember to read the Easy Minimap System documentation to understand how to use it.\nGet support at: mtassets@windsoft.xyz", MessageType.None);

                //If not have the place holder, create them. If have place holder, and it's not updated, update them
                CreateOrUpdateThePlaceHoldersAppearence(script);
                //Show the warning about Render Mode of Canvas
                Canvas parentCanvas = script.gameObject.GetComponentInParent<Canvas>();
                if (parentCanvas != null && parentCanvas.renderMode != RenderMode.ScreenSpaceOverlay)
                    EditorGUILayout.HelpBox("It seems that this Canvas does not have \"Render Mode\" as \"Screen Space - Overlay\". This Minimap Compass may have its functionality compromised, things like Minimap Items in Highlight and etc, may not work as expected.", MessageType.Error);

                //Start of settings
                GUILayout.Space(10);
                EditorGUILayout.LabelField("Settings For Minimap Compass", EditorStyles.boldLabel);
                GUILayout.Space(10);

                script.targetPlayerTransform = (Transform)EditorGUILayout.ObjectField(new GUIContent("Target Player Transform",
                        "The minimap camera that will be displayed by this renderer."),
                        script.targetPlayerTransform, typeof(Transform), true, GUILayout.Height(16));
                if (script.targetPlayerTransform == null)
                    EditorGUILayout.HelpBox("Please assign a \"Transform\" component to this compass. The compass will rotate according to the rotation of this Transform, to display the direction corresponding to the North.", MessageType.Warning);

                script.northDirection = (NorthDirection)EditorGUILayout.EnumPopup(new GUIContent("North Direction In World",
                                                "The axis that will represent the north of your game in the world."),
                                                script.northDirection);

                script.compassTexture = (Texture)EditorGUILayout.ObjectField(new GUIContent("Compass Texture",
                                        "The texture that will be rendered as the compass background.\n\nIt is recommended that you use the \"BaseCompass\" sprite as a base if you want to create your own Compass texture. Also use a texture of the same resolution."),
                                        script.compassTexture, typeof(Texture), true, GUILayout.Height(16));

                script.compassTextureColor = EditorGUILayout.ColorField(new GUIContent("Compass Texture Color",
                                        "The color of the texture that will be rendered as the background of the compass."),
                                        script.compassTextureColor);

                GUILayout.Space(10);
                EditorGUILayout.LabelField("Settings For Degrees Display", EditorStyles.boldLabel);
                GUILayout.Space(10);

                script.showDegreesToNorth = EditorGUILayout.Toggle(new GUIContent("Show Degrees To North",
                            "If this option is enabled, Minimap Compass will display the current angle in relation to the north."),
                            script.showDegreesToNorth);
                if (script.showDegreesToNorth == true)
                {
                    EditorGUI.indentLevel += 1;
                    script.degreesPosition = (DegreesPosition)EditorGUILayout.EnumPopup(new GUIContent("Degrees Position",
                                                                    "The position where the degree text in relation to the north will be displayed."),
                                                                    script.degreesPosition);

                    script.degreesFont = (Font)EditorGUILayout.ObjectField(new GUIContent("Degrees Font",
                                        "The font that will render the degree display text relative to the north."),
                                        script.degreesFont, typeof(Font), true, GUILayout.Height(16));

                    script.degreesFontSize = EditorGUILayout.IntField(new GUIContent("Degrees Font Size",
                                                            "The font size that will render the degree display text relative to the north."),
                                                            script.degreesFontSize);

                    script.degreesFontColor = EditorGUILayout.ColorField(new GUIContent("Degrees Font Color",
                                        "The font color that will render the degree display text relative to the north."),
                                        script.degreesFontColor);

                    script.degreesFontStyle = (FontStyle)EditorGUILayout.EnumPopup(new GUIContent("Degrees Font Style",
                                                "The font style that will render the degree display text relative to the north."),
                                                script.degreesFontStyle);
                    EditorGUI.indentLevel -= 1;
                }

                GUILayout.Space(10);

                //Start of settings
                EditorGUILayout.LabelField("Minimap Items For Highlight", EditorStyles.boldLabel);
                GUILayout.Space(10);

                Texture2D removeItemIcon = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/MT Assets/Easy Minimap System/Editor/Images/Remove.png", typeof(Texture2D));
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Minimap Items For Highlight List", GUILayout.Width(180));
                GUILayout.Space(MTAssetsEditorUi.GetInspectorWindowSize().x - 145);
                EditorGUILayout.LabelField("Size", GUILayout.Width(30));
                EditorGUILayout.IntField(script.minimapItemsToHightlight.Count, GUILayout.Width(50));
                EditorGUILayout.EndHorizontal();
                GUILayout.BeginVertical("box");
                minimapItemsToHighlight_ScrollPos = EditorGUILayout.BeginScrollView(minimapItemsToHighlight_ScrollPos, GUIStyle.none, GUI.skin.verticalScrollbar, GUILayout.Width(MTAssetsEditorUi.GetInspectorWindowSize().x), GUILayout.Height(150));
                if (script.minimapItemsToHightlight.Count == 0)
                    EditorGUILayout.HelpBox("Oops! No Minimap Items was registered to be highlighted! If you want to subscribe any, click the button below!", MessageType.Info);
                if (script.minimapItemsToHightlight.Count > 0)
                    for (int i = 0; i < script.minimapItemsToHightlight.Count; i++)
                    {
                        GUILayout.BeginHorizontal();
                        if (GUILayout.Button(removeItemIcon, GUILayout.Width(25), GUILayout.Height(16)))
                            script.minimapItemsToHightlight.RemoveAt(i);
                        script.minimapItemsToHightlight[i] = (MinimapItem)EditorGUILayout.ObjectField(new GUIContent("Minimap Item " + i.ToString(), "This Minimap Item will be displayed on the edge of this Minimap Renderer while the Minimap Item is out of range of the Minimap Camera.\n\nClick the button to the left if you want to remove this Minimap Item from the list."), script.minimapItemsToHightlight[i], typeof(MinimapItem), true, GUILayout.Height(16));
                        GUILayout.EndHorizontal();
                    }
                EditorGUILayout.EndScrollView();
                GUILayout.EndVertical();
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("Add New Slot"))
                {
                    script.minimapItemsToHightlight.Add(null);
                    minimapItemsToHighlight_ScrollPos.y += 999999;
                }
                if (script.minimapItemsToHightlight.Count > 0)
                    if (GUILayout.Button("Remove Empty Slots", GUILayout.Width(Screen.width * 0.48f)))
                        for (int i = script.minimapItemsToHightlight.Count - 1; i >= 0; i--)
                            if (script.minimapItemsToHightlight[i] == null)
                                script.minimapItemsToHightlight.RemoveAt(i);
                GUILayout.EndHorizontal();

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

            public void CreateOrUpdateThePlaceHoldersAppearence(MinimapCompass script)
            {
                //If game is running cancel
                if (Application.isPlaying == true)
                    return;

                //If not have the place holder, create them. If have place holder, and it's not updated, update them
                bool hasChangedSomethingInPlaceHolders = false;
                GameObject placeHolderRenderObj = script.GetChildGameObjectByName(script.gameObject, "Place Holder Render");
                if (placeHolderRenderObj == null)
                {
                    placeHolderRenderObj = new GameObject("Place Holder Render");
                    placeHolderRenderObj.transform.SetParent(script.gameObject.transform);
                    placeHolderRenderObj.transform.SetSiblingIndex(0);
                    RectTransform rectTransform = placeHolderRenderObj.AddComponent<RectTransform>();
                    rectTransform.sizeDelta = new Vector2(script.gameObject.GetComponent<RectTransform>().sizeDelta.x, script.gameObject.GetComponent<RectTransform>().sizeDelta.y);
                    rectTransform.offsetMax = new Vector2(0, 0);
                    rectTransform.offsetMin = new Vector2(0, 0);
                    rectTransform.anchorMin = new Vector2(0, 0);
                    rectTransform.anchorMax = new Vector2(1, 1);
                    rectTransform.pivot = new Vector2(0.5f, 0.5f);
                    placeHolderRenderObj.AddComponent<Image>();
                    Mask mask = placeHolderRenderObj.AddComponent<Mask>();
                    mask.showMaskGraphic = false;
                    hasChangedSomethingInPlaceHolders = true;

                    //Load default assets, because is in editor
                    script.compassTexture = (Texture)AssetDatabase.LoadAssetAtPath("Assets/MT Assets/Easy Minimap System/Sprites/BaseCompass0.png", typeof(Texture));
                }
                GameObject placeHolderBgObj = script.GetChildGameObjectByName(placeHolderRenderObj, "Place Holder Bg");
                if (placeHolderBgObj == null)
                {
                    placeHolderBgObj = new GameObject("Place Holder Bg");
                    placeHolderBgObj.transform.SetParent(placeHolderRenderObj.gameObject.transform);
                    placeHolderBgObj.transform.SetSiblingIndex(0);
                    RectTransform rectTransform = placeHolderBgObj.AddComponent<RectTransform>();
                    rectTransform.offsetMax = new Vector2(0, 0);
                    rectTransform.offsetMin = new Vector2(0, 0);
                    rectTransform.anchorMin = new Vector2(0.5f, 0);
                    rectTransform.anchorMax = new Vector2(0.5f, 1);
                    rectTransform.pivot = new Vector2(0.5f, 0.5f);
                    rectTransform.sizeDelta = new Vector2(script.gameObject.GetComponent<RectTransform>().sizeDelta.x, 0);
                    placeHolderBgObj.AddComponent<RawImage>();
                    hasChangedSomethingInPlaceHolders = true;
                }
                if (placeHolderBgObj != null)
                {
                    RawImage placeHolderBg = placeHolderBgObj.GetComponent<RawImage>();
                    if (placeHolderBg.texture != script.compassTexture)
                    {
                        placeHolderBg.texture = script.compassTexture;
                        hasChangedSomethingInPlaceHolders = true;
                    }
                    placeHolderBg.SetNativeSize();
                    float nativeHeight = placeHolderBg.rectTransform.sizeDelta.y;
                    float nativeWidth = placeHolderBg.rectTransform.sizeDelta.x;
                    float componentHeight = script.gameObject.GetComponent<RectTransform>().rect.height;
                    float aspectRatio = componentHeight / nativeHeight;
                    placeHolderBg.rectTransform.offsetMax = new Vector2(0, 0);
                    placeHolderBg.rectTransform.offsetMin = new Vector2(0, 0);
                    placeHolderBg.rectTransform.anchorMin = new Vector2(0.5f, 0);
                    placeHolderBg.rectTransform.anchorMax = new Vector2(0.5f, 1);
                    placeHolderBg.rectTransform.pivot = new Vector2(0.5f, 0.5f);
                    placeHolderBg.rectTransform.sizeDelta = new Vector2(nativeWidth * aspectRatio, 0);
                    placeHolderBg.color = script.compassTextureColor;
                }
                //Auto update the canvas, if something was changed
                if (hasChangedSomethingInPlaceHolders == true)
                {
                    if (script.compassTexture != null)
                        AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(script.compassTexture));
                    AssetDatabase.Refresh();
                }
                //Force all maximized GameObjects childs of this, to be scale 1,1,1 does not matter the resolution
                placeHolderRenderObj.transform.localScale = new Vector3(1, 1, 1);
            }
        }
        #endregion
#endif

        //Core methods

        private GameObject GetChildGameObjectByName(GameObject obj, string name)
        {
            Transform trans = obj.transform;
            Transform childTrans = trans.Find(name);
            if (childTrans != null)
                return childTrans.gameObject;
            else
                return null;
        }

        public void Awake()
        {
            //Get this rect transform
            thisRectTransform = this.gameObject.GetComponent<RectTransform>();
            //Verify if exists the gameobjects, if not exists, create them
            if (GetChildGameObjectByName(this.gameObject, "Mask") == null)
            {
                maskImageGameobject = new GameObject("Mask");
                maskImageGameobject.transform.SetParent(this.gameObject.transform);
                maskImageGameobject.transform.SetSiblingIndex(0);
                RectTransform rectTransform = maskImageGameobject.AddComponent<RectTransform>();
                rectTransform.sizeDelta = new Vector2(this.gameObject.GetComponent<RectTransform>().sizeDelta.x, this.gameObject.GetComponent<RectTransform>().sizeDelta.y);
                rectTransform.offsetMax = new Vector2(0, 0);
                rectTransform.offsetMin = new Vector2(0, 0);
                rectTransform.anchorMin = new Vector2(0, 0);
                rectTransform.anchorMax = new Vector2(1, 1);
                rectTransform.pivot = new Vector2(0.5f, 0.5f);
                maskImageComponent = maskImageGameobject.AddComponent<Image>();
                maskComponent = maskImageGameobject.AddComponent<Mask>();
                maskComponent.showMaskGraphic = false;
            }
            if (GetChildGameObjectByName(maskImageGameobject, "Background") == null)
            {
                rendererRawImageGameobject = new GameObject("Background");
                rendererRawImageGameobject.transform.SetParent(maskImageGameobject.gameObject.transform);
                rendererRawImageGameobject.transform.SetSiblingIndex(0);
                RectTransform rectTransform = rendererRawImageGameobject.AddComponent<RectTransform>();
                rectTransform.offsetMax = new Vector2(0, 0);
                rectTransform.offsetMin = new Vector2(0, 0);
                rectTransform.anchorMin = new Vector2(0.5f, 0);
                rectTransform.anchorMax = new Vector2(0.5f, 1);
                rectTransform.pivot = new Vector2(0.5f, 0.5f);
                rectTransform.sizeDelta = new Vector2(this.gameObject.GetComponent<RectTransform>().sizeDelta.x, 0);
                rawImageComponent = rendererRawImageGameobject.AddComponent<RawImage>();
            }
            if (GetChildGameObjectByName(rendererRawImageGameobject, "Items Pool") == null)
            {
                poolRootForHighlightedItemsGameObject = new GameObject("Items Pool");
                poolRootForHighlightedItemsGameObject.transform.SetParent(rendererRawImageGameobject.gameObject.transform);
                poolRootForHighlightedItemsGameObject.transform.SetSiblingIndex(0);
                RectTransform rectTransform = poolRootForHighlightedItemsGameObject.AddComponent<RectTransform>();
                rectTransform.offsetMax = new Vector2(0, 0);
                rectTransform.offsetMin = new Vector2(0, 0);
                rectTransform.anchorMin = new Vector2(0, 0);
                rectTransform.anchorMax = new Vector2(1, 1);
                rectTransform.pivot = new Vector2(0.5f, 0.5f);
                rectTransform.sizeDelta = new Vector2(0, 0);

                for (int i = 0; i < MIN_COUNT_OF_ITEMS_IN_POOL; i++)
                {
                    //Create the icon for highlighted item
                    GameObject item = new GameObject("Item");
                    item.transform.SetParent(poolRootForHighlightedItemsGameObject.transform);
                    RectTransform itemRectTransform = item.AddComponent<RectTransform>();
                    itemRectTransform.offsetMax = new Vector2(0, 0);
                    itemRectTransform.offsetMin = new Vector2(0, 0);
                    itemRectTransform.anchorMin = new Vector2(0, 0.5f);
                    itemRectTransform.anchorMax = new Vector2(0, 0.5f);
                    itemRectTransform.pivot = new Vector2(0.5f, 0.5f);
                    itemRectTransform.sizeDelta = new Vector2(20, 20);
                    Image image = item.AddComponent<Image>();
                    minimapItemsForHighlightPool.Add(new MinimapItemOfPool(item, image));

                    //Disable the pivot
                    item.SetActive(false);
                }
            }
            if (GetChildGameObjectByName(this.gameObject, "Degrees") == null)
            {
                degreesTextGameobject = new GameObject("Degrees");
                degreesTextGameobject.transform.SetParent(this.gameObject.transform);
                degreesTextGameobject.transform.SetSiblingIndex(1);
                RectTransform rectTransform = degreesTextGameobject.AddComponent<RectTransform>();
                rectTransform.offsetMax = new Vector2(0, 0);
                rectTransform.offsetMin = new Vector2(0, 0);
                rectTransform.sizeDelta = new Vector2(40, 40);
                degreesTextComponent = degreesTextGameobject.AddComponent<Text>();
            }

            //Delete the place holder image in this GameObject
            GameObject placeHolderObj = GetChildGameObjectByName(this.gameObject, "Place Holder Render");
            if (placeHolderObj != null)
                Destroy(placeHolderObj);

            //Create the holder, if not exists
            minimapDataHolderObj = GameObject.Find("Minimap Data Holder");
            if (minimapDataHolderObj == null)
            {
                minimapDataHolderObj = new GameObject("Minimap Data Holder");
                minimapDataHolder = minimapDataHolderObj.AddComponent<MinimapDataHolder>();
            }
            if (minimapDataHolderObj != null)
                minimapDataHolder = minimapDataHolderObj.GetComponent<MinimapDataHolder>();
            if (minimapDataHolder.instancesOfMinimapCompassInThisScene.Contains(this) == false)
                minimapDataHolder.instancesOfMinimapCompassInThisScene.Add(this);
        }

        public void Update()
        {
            //Cancel update of this component, if not have a target player transform, or compass texture linked
            if (targetPlayerTransform == null || compassTexture == null)
            {
                Debug.LogError("Please provide data for variables \"targetPlayerTransform\" and \"compassTexture\" to make it possible for Minimap Compass to work.");
                return;
            }

            //Update the values for minimap compass, if changed
            if (rawImageComponent.texture != compassTexture)
            {
                rawImageComponent.texture = compassTexture;
                alreadyAdjustedSizeOfCompassOnChangeRawImage = false;
            }

            //Update the color of raw image of compass
            if (rawImageComponent.color != compassTextureColor)
                rawImageComponent.color = compassTextureColor;

            //Update the size of raw image of compass if not yet adjusted (the size will be automatically adjusted to the best responsive size, according to the original size in pixels of the provided texture)
            if (thisRectTransform.rect.width != lastMinimapCompassSize.x || thisRectTransform.rect.height != lastMinimapCompassSize.y || alreadyAdjustedSizeOfCompassOnChangeRawImage == false)
            {
                rawImageComponent.SetNativeSize();
                float nativeHeight = rawImageComponent.rectTransform.sizeDelta.y;
                float nativeWidth = rawImageComponent.rectTransform.sizeDelta.x;
                float componentHeight = thisRectTransform.rect.height;
                float aspectRatio = componentHeight / nativeHeight;
                rawImageComponent.rectTransform.offsetMax = new Vector2(0, 0);
                rawImageComponent.rectTransform.offsetMin = new Vector2(0, 0);
                rawImageComponent.rectTransform.anchorMin = new Vector2(0.5f, 0);
                rawImageComponent.rectTransform.anchorMax = new Vector2(0.5f, 1);
                rawImageComponent.rectTransform.pivot = new Vector2(0.5f, 0.5f);
                rawImageComponent.rectTransform.sizeDelta = new Vector2(nativeWidth * aspectRatio, 0);
                lastMinimapCompassSize = new Vector2(thisRectTransform.rect.width, thisRectTransform.rect.height);
                alreadyAdjustedSizeOfCompassOnChangeRawImage = true;
            }
            //If the option of show degrees to north text, is enabled, update preferences for the text
            if (showDegreesToNorth == true)
            {
                //Enable the text of degrees
                degreesTextGameobject.SetActive(true);

                //Update the position of text of degrees
                if (degreesPosition != lastDegreesPosition || alreadyAdjustedDegreesTextPosition == false)
                {
                    if (degreesPosition == DegreesPosition.Top)
                    {
                        degreesTextComponent.rectTransform.offsetMax = new Vector2(0, 0);
                        degreesTextComponent.rectTransform.offsetMin = new Vector2(0, 0);
                        degreesTextComponent.rectTransform.anchorMin = new Vector2(0.5f, 1);
                        degreesTextComponent.rectTransform.anchorMax = new Vector2(0.5f, 1);
                        degreesTextComponent.rectTransform.pivot = new Vector2(0.5f, 1);
                        degreesTextComponent.rectTransform.sizeDelta = new Vector2(80, 40);
                        degreesTextComponent.rectTransform.anchoredPosition = new Vector2(0, 42);
                        degreesTextComponent.alignment = TextAnchor.LowerCenter;
                    }
                    if (degreesPosition == DegreesPosition.Bottom)
                    {
                        degreesTextComponent.rectTransform.offsetMax = new Vector2(0, 0);
                        degreesTextComponent.rectTransform.offsetMin = new Vector2(0, 0);
                        degreesTextComponent.rectTransform.anchorMin = new Vector2(0.5f, 0);
                        degreesTextComponent.rectTransform.anchorMax = new Vector2(0.5f, 0);
                        degreesTextComponent.rectTransform.pivot = new Vector2(0.5f, 0);
                        degreesTextComponent.rectTransform.sizeDelta = new Vector2(80, 40);
                        degreesTextComponent.rectTransform.anchoredPosition = new Vector2(0, -42);
                        degreesTextComponent.alignment = TextAnchor.UpperCenter;
                    }
                    lastDegreesPosition = degreesPosition;
                    alreadyAdjustedDegreesTextPosition = true;
                }
                //Update the degrees text font
                if (degreesTextComponent.font != degreesFont)
                    degreesTextComponent.font = degreesFont;

                //Update the degrees text font size
                if (degreesTextComponent.fontSize != degreesFontSize)
                    degreesTextComponent.fontSize = degreesFontSize;

                //Update the degrees text font color
                if (degreesTextComponent.color != degreesFontColor)
                    degreesTextComponent.color = degreesFontColor;

                //Update the degrees text font style
                if (degreesTextComponent.fontStyle != degreesFontStyle)
                    degreesTextComponent.fontStyle = degreesFontStyle;
            }
            //If the option of show degrees to north text, is disabled, disable the text
            if (showDegreesToNorth == false)
                degreesTextGameobject.SetActive(false);

            //Calculate the north direction offset for the raw image of compass
            if (northDirection != lastNorthDirectionSelected || alreadyCalculatedTheNorthDirectionOffset == false)
            {
                if (northDirection == NorthDirection.AxisX)
                    northDirectionOffset = -0.25f;
                if (northDirection == NorthDirection.AxisZ)
                    northDirectionOffset = 0f;
                if (northDirection == NorthDirection.NegativeAxisX)
                    northDirectionOffset = 0.25f;
                if (northDirection == NorthDirection.NegativeAxisZ)
                    northDirectionOffset = 0.5f;
                lastNorthDirectionSelected = northDirection;
                alreadyCalculatedTheNorthDirectionOffset = true;
            }

            //Update the rotation of compass and current degrees to north according to the rotation of target player transform
            rawImageComponent.uvRect = new Rect((targetPlayerTransform.eulerAngles.y / 360f) + northDirectionOffset, 0, 1, 1);
            int currentDegreesToNorth = (int)(360.0f * (targetPlayerTransform.eulerAngles.y / 360f));
            if (currentDegreesToNorth >= 360)
                currentDegreesToNorth = 0;
            if (showDegreesToNorth == true && currentDegreesToNorth != lastDegreesToNorth)
            {
                degreesTextComponent.text = currentDegreesToNorth.ToString();
                lastDegreesToNorth = currentDegreesToNorth;
            }

            //------------------------------------- START UPDATING EACH ICON DEFINED TO BE HIGHLIGHTED ----------------------------------

            //Associate a Minimap Item to a Minimap Item To Highlight (GameObject of Pool), if not is associated
            foreach (MinimapItem item in minimapItemsToHightlight)
            {
                //If item to highlight is null, continue
                if (item == null)
                    continue;

                //Associate this minimap item to a item of pool, if is already associated, cancel
                bool alreadyAssociated = false;
                foreach (MinimapItemOfPool itemOfPool in minimapItemsForHighlightPool)
                    if (itemOfPool.minimapItemAssociated == item)
                        alreadyAssociated = true;

                //If is not associated, associate this item to first minimap items for highlight in the GameObject pool
                if (alreadyAssociated == false)
                    foreach (MinimapItemOfPool itemOfPool in minimapItemsForHighlightPool)
                        if (itemOfPool.minimapItemAssociated == null)
                        {
                            itemOfPool.minimapItemAssociated = item;
                            itemOfPool.pivot.SetActive(true);
                            break;
                        }
            }

            //Update all data (like, sprite, color, rotation, activity etc) for each minimap item to highlight in this minimap compass
            foreach (MinimapItemOfPool item in minimapItemsForHighlightPool)
            {
                //If this minimap item to highlight in pool, not have a associated original Minimap Item, disable the pivot of GameObject in pool and continue
                if (item.minimapItemAssociated == null)
                {
                    item.pivot.SetActive(false);
                    continue;
                }
                //Skip this item if is missing some importante component
                if (item.minimapItemAssociated.GetGeneratedSpriteAtRunTime() == null)
                {
                    item.pivot.SetActive(false);
                    continue;
                }

                //If the list of minimapItemsToHightlight not have this associated Minimap Item no more, disable the pivot of GameObject in pool and continue
                bool foundThisAssociatedMinimapItemInMinimapItemsToHightlight = false;
                foreach (MinimapItem itemInList in minimapItemsToHightlight)
                    if (itemInList == item.minimapItemAssociated)
                        foundThisAssociatedMinimapItemInMinimapItemsToHightlight = true;
                if (foundThisAssociatedMinimapItemInMinimapItemsToHightlight == false)
                {
                    item.minimapItemAssociated = null;
                    item.pivot.SetActive(false);
                    continue;
                }

                //Update the activity of this highlighted item, on original Minimap Item is enabled or disabled
                item.pivot.SetActive(item.minimapItemAssociated.GetGeneratedSpriteAtRunTime().gameObject.activeSelf);

                //If sprite, flip or color changed in Minimap Item, update here too (and mantain the rotation in Z as zero)
                if (item.icon.sprite != item.minimapItemAssociated.itemSprite)
                    item.icon.sprite = item.minimapItemAssociated.itemSprite;
                if (item.icon.color != item.minimapItemAssociated.spriteColor)
                    item.icon.color = item.minimapItemAssociated.spriteColor;
                item.icon.transform.eulerAngles = new Vector3((item.minimapItemAssociated.flipInY == true) ? 180 : 0, (item.minimapItemAssociated.flipInX == true) ? 180 : 0, 0);

                //Update the size of this item, if is different from highlighted items size
                if (item.icon.rectTransform.sizeDelta.x != item.minimapItemAssociated.sizeOnHighlight)
                    item.icon.rectTransform.sizeDelta = new Vector2(item.minimapItemAssociated.sizeOnHighlight, item.minimapItemAssociated.sizeOnHighlight);

                //Calculate angle between target player position and this minimap item associated, and position the highlighted icon accordding to angle in compass (using positions at same height)
                Vector3 playerPosition = new Vector3(targetPlayerTransform.position.x, 0, targetPlayerTransform.position.z);
                Vector3 thisItemPosition = new Vector3(item.minimapItemAssociated.transform.position.x, 0, item.minimapItemAssociated.transform.position.z);
                Vector3 targetDir = thisItemPosition - playerPosition;
                float angleToItem = Vector3.SignedAngle(targetDir, targetPlayerTransform.forward, Vector3.up) / 360f * -1;
                item.pivot.transform.localPosition = new Vector3(rawImageComponent.rectTransform.sizeDelta.x * angleToItem, 0, 0);
            }

            //Force all maximized GameObjects childs of this, to be scale 1,1,1 does not matter the resolution
            maskImageComponent.transform.localScale = new Vector3(1, 1, 1);
            degreesTextComponent.transform.localScale = new Vector3(1, 1, 1);
        }

        //Public methods

        public void AddMinimapItemToBeHighlighted(MinimapItem minimapItem)
        {
            //If this minimap item already added, cancel
            if (isMinimapItemAddedToHighlight(minimapItem) == true)
                return;

            //Add a minimap item on list to be highlighted
            minimapItemsToHightlight.Add(minimapItem);
        }

        public void RemoveMinimapItemOfHighlight(MinimapItem minimapItem)
        {
            //Read all list of minimap items to be hightlited and remove the expected
            for (int i = 0; i < minimapItemsToHightlight.Count; i++)
                if (minimapItemsToHightlight[i] == minimapItem)
                    minimapItemsToHightlight[i] = null;
        }

        public bool isMinimapItemAddedToHighlight(MinimapItem minimapItem)
        {
            //Read all list of minimap items to be hightlited and check if this minimap item already exists
            for (int i = 0; i < minimapItemsToHightlight.Count; i++)
                if (minimapItemsToHightlight[i] == minimapItem)
                    return true;

            return false;
        }

        public void RemoveAllMinimapItemsFromHighlight()
        {
            //Clear the list of minimap items to be highlighted
            minimapItemsToHightlight.Clear();
        }

        public int CountQuantityOfMinimapItemsInHighlight()
        {
            //Return the quantity of minimap items, currently in highlights
            int itemsOnHighlight = 0;
            for (int i = 0; i < minimapItemsToHightlight.Count; i++)
                if (minimapItemsToHightlight[i] != null)
                    itemsOnHighlight += 1;
            return itemsOnHighlight;
        }

        public int GetCurrentDegreesToNorth()
        {
            //Return current degrees to north
            return lastDegreesToNorth;
        }

        public MinimapCompass[] GetListOfAllMinimapCompassInThisScene()
        {
            //If is not playing, cancel
            if (Application.isPlaying == false)
            {
                Debug.LogError("It is only possible to obtain the list of Minimap Compass in this scene, if the application is being executed.");
                return null;
            }

            //Return a list that contains reference to all of this component in this scene
            return minimapDataHolder.instancesOfMinimapCompassInThisScene.ToArray();
        }
    }
}