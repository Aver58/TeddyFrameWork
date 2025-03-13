#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace MTAssets.EasyMinimapSystem
{
    /*
     This class is responsible for the functioning of the "Minimap Renderer" component, and all its functions.
    */
    /*
     * The Easy Minimap System was developed by Marcos Tomaz in 2019.
     * Need help? Contact me (mtassets@windsoft.xyz)
    */

    [AddComponentMenu("MT Assets/Easy Minimap System/Minimap Renderer")] //Add this component in a category of addComponent menu
    [RequireComponent(typeof(RectTransform))]
    public class MinimapRenderer : MonoBehaviour
    {
        //Private constants
        private const int LAYER_OF_BORDER_POLYGON_COLLIDER = (1 << 5); //UI Layer
        private const float MIN_COUNT_OF_ITEMS_IN_POOL = 100;

        //Private variables
        private GameObject minimapDataHolderObj;
        private MinimapDataHolder minimapDataHolder;
        private MinimapRendererEvents thisRendererEvents;

        //Private variables to refer tho the gameobjects structure of minimap renderer
        private RectTransform thisRectTransform;
        private GameObject baseShapeImageGameobject;
        private Image baseShapeImageComponent;
        private PolygonCollider2D baseShapePolygonCollider;
        private GameObject maskImageGameobject;
        private Image maskImageComponent;
        private Mask maskComponent;
        private RectTransform maskRectTransform;
        private GameObject backgroundImageGameobject;
        private Image backgroundImageComponent;
        private GameObject rendererRawImageGameobject;
        private RawImage rawImageComponent;
        private GameObject foregroundImageGameobject;
        private Image foregroundImageComponent;
        private GameObject holderForBorderItems;
        private GameObject northItemPivotGameObject;
        private Image northImageComponent;
        private GameObject poolRootForHighlightedItemsGameObject;
        private GameObject thisRendererEventsGameObject;
        private Image thisRendererEventsImageComponent;

        //Private cache variables
        private float lastContentTransparencyDefined = -1.0f;
        private NorthDirection lastNorthDirectionSelected = NorthDirection.AxisX;
        private Vector3 imaginaryNorthVector3InWorld = Vector3.zero;
        private IconsRotationMethod lastIconsRotationMethodDefined = IconsRotationMethod.CircleShape;
        private Vector2 lastMinimapRendererSize = Vector2.zero;
        private RaycastHit2D temporaryRaycastHit;
        private List<MinimapItemOfPool> minimapItemsForHighlightPool = new List<MinimapItemOfPool>();

        //Enums of script
        public enum IconsRotationMethod
        {
            CircleShape,
            SquareShape
        }
        public enum NorthDirection
        {
            AxisZ,
            AxisX,
            NegativeAxisZ,
            NegativeAxisX
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

        //Custom events of script
        [System.Serializable]
        public class OnInputClick : UnityEvent<Vector3, MinimapItem> { }
        [System.Serializable]
        public class OnInputDrag : UnityEvent<Vector3, Vector3> { }
        [System.Serializable]
        public class OnInputOver : UnityEvent<bool, Vector3, MinimapItem> { }

        //Public variables
        [HideInInspector]
        public MinimapCamera minimapCameraToShow;
        [HideInInspector]
        public Sprite baseShapeSprite = null;
        [HideInInspector]
        public Color borderColor = Color.black;
        [HideInInspector]
        public float borderWidthSize = 5f;
        [HideInInspector]
        public Sprite backgroundSprite = null;
        [HideInInspector]
        public Color backgroundColor = new Color(197.0f / 255.0f, 197 / 255.0f, 197 / 255.0f, 255.0f);
        [HideInInspector]
        public Sprite foregroundSprite = null;
        [HideInInspector]
        public Color foregroundColor = Color.white;
        [HideInInspector]
        public float contentTransparency = 100.0f;
        [HideInInspector]
        public bool renderContent = true;
        [HideInInspector]
        public IconsRotationMethod iconsRotationMethod = IconsRotationMethod.CircleShape;
        [HideInInspector]
        public bool showNorthIcon = false;
        [HideInInspector]
        public NorthDirection northDirection = NorthDirection.AxisZ;
        [HideInInspector]
        public bool autoRotateNorthIcon = true;
        [HideInInspector]
        public Sprite northIconSprite = null;
        [HideInInspector]
        public float northIconSize = 25;
        [HideInInspector]
        public List<MinimapItem> minimapItemsToHightlight = new List<MinimapItem>();

        //Input Events and Parameters
        [Space(10)]
        public OnInputClick onInputClick = new OnInputClick();
        public OnInputDrag onInputDrag = new OnInputDrag();
        public OnInputOver onInputOver = new OnInputOver();

#if UNITY_EDITOR
        //Public variables of interface
        [HideInInspector]
        [SerializeField]
        private bool showInputEventsOptions = false;

        //The UI of this component
        #region INTERFACE_CODE
        [UnityEditor.CustomEditor(typeof(MinimapRenderer))]
        public class CustomInspector : UnityEditor.Editor
        {
            Vector2 minimapItemsToHighlight_ScrollPos;

            public override void OnInspectorGUI()
            {
                //Start the undo event support, draw default inspector and monitor of changes
                MinimapRenderer script = (MinimapRenderer)target;
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
                    EditorGUILayout.HelpBox("It seems that this Canvas does not have \"Render Mode\" as \"Screen Space - Overlay\". This Minimap Renderer may have its functionality compromised, things like Input Events, Minimap Items in Highlight and etc, may not work as expected.", MessageType.Error);

                //Start of settings
                GUILayout.Space(10);
                EditorGUILayout.LabelField("Settings For Minimap Renderer", EditorStyles.boldLabel);
                GUILayout.Space(10);

                script.minimapCameraToShow = (MinimapCamera)EditorGUILayout.ObjectField(new GUIContent("Minimap Camera To Show",
                        "The minimap camera that will be displayed by this renderer."),
                        script.minimapCameraToShow, typeof(MinimapCamera), true, GUILayout.Height(16));
                if (script.minimapCameraToShow == null)
                    EditorGUILayout.HelpBox("Please assign a \"Minimap Camera\" component to this renderer. This renderer will display the image taken by the \"Minimap Camera\" component you assign.", MessageType.Warning);
                if (script.minimapCameraToShow != null)
                {
                    Vector2 rendererSize = script.gameObject.GetComponent<RectTransform>().rect.size;
                    if (rendererSize.x == rendererSize.y && script.minimapCameraToShow.captureShape != MinimapCamera.CaptureShape.Square)
                        EditorGUILayout.HelpBox("It looks like this Minimap Renderer is a perfect square, but your Minimap Camera is capturing things with a rectangular resolution. Consider using the \"Square\" option in \"captureShape\" to avoid distortions in the image. You can also make the height and width measurements the same in this Minimap Renderer.", MessageType.Warning);
                    if (rendererSize.x != rendererSize.y && script.minimapCameraToShow.captureShape == MinimapCamera.CaptureShape.Square)
                        EditorGUILayout.HelpBox("It looks like this Minimap Renderer is a rectangle, but your Minimap Camera is capturing things with a square resolution. Consider using the \"Rectangle\" option in \"captureShape\" to avoid distortions in the image. You can also adjust the height and width of this Minimap Renderer so that it becomes a rectangle.", MessageType.Warning);
                }

                EditorGUILayout.BeginHorizontal();
                script.baseShapeSprite = (Sprite)EditorGUILayout.ObjectField(new GUIContent("Base Shape Sprite",
                                                       "The sprite you insert here will be responsible for defining the shape of this Minimap. If the sprite is a Circle, for example, this Minimap will take the shape of a Circle, if the Sprite is a Square, this Minimap will take the shape of a Square.\n\n- It is highly recommended that the sprite provided is only White, Black and Gray scale."),
                                                       script.baseShapeSprite, typeof(Sprite), true, GUILayout.Height(16));
                Texture2D circle = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/MT Assets/Easy Minimap System/Editor/Images/Circle.png", typeof(Texture2D));
                Texture2D square = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/MT Assets/Easy Minimap System/Editor/Images/Square.png", typeof(Texture2D));
                if (GUILayout.Button(new GUIContent(circle, "Click here to load the basic Circle sprite and define the circular format for the Minimap."), GUILayout.Height(16), GUILayout.Width(32)))
                {
                    script.baseShapeSprite = (Sprite)AssetDatabase.LoadAssetAtPath("Assets/MT Assets/Easy Minimap System/Formats/Circle.png", typeof(Sprite));
                    script.iconsRotationMethod = IconsRotationMethod.CircleShape;
                }
                if (GUILayout.Button(new GUIContent(square, "Click here to load the basic Square sprite and define the squared format for the Minimap."), GUILayout.Height(16), GUILayout.Width(32)))
                {
                    script.baseShapeSprite = (Sprite)AssetDatabase.LoadAssetAtPath("Assets/MT Assets/Easy Minimap System/Formats/Square.png", typeof(Sprite));
                    script.iconsRotationMethod = IconsRotationMethod.SquareShape;
                }
                EditorGUILayout.EndHorizontal();

                script.borderColor = EditorGUILayout.ColorField(new GUIContent("Border Color",
                        "This will be the rendering color of the Minimap border."),
                        script.borderColor);

                script.borderWidthSize = EditorGUILayout.FloatField(new GUIContent("Border Width Size",
                        "This will be the width of the border of this Minimap."),
                        script.borderWidthSize);
                if (script.borderWidthSize < 0)
                    script.borderWidthSize = 0;

                script.backgroundSprite = (Sprite)EditorGUILayout.ObjectField(new GUIContent("Background Sprite",
                                       "This is the Sprite that will be rendered behind the content captured by the target Minimap Camera.\n\n- If you do not provide a Sprite for the background, the background will be disabled automatically."),
                                       script.backgroundSprite, typeof(Sprite), true, GUILayout.Height(16));

                script.backgroundColor = EditorGUILayout.ColorField(new GUIContent("Background Color",
                                        "The color of the minimap background image.\n\n- It will only work if you provide a Background sprite."),
                                        script.backgroundColor);

                script.foregroundSprite = (Sprite)EditorGUILayout.ObjectField(new GUIContent("Foreground Sprite",
                                        "This is the Sprite that will be rendered above the content captured by the target Minimap Camera.\n\n- If you do not provide a Sprite for the foreground, the foreground will be disabled automatically."),
                                        script.foregroundSprite, typeof(Sprite), true, GUILayout.Height(16));

                script.foregroundColor = EditorGUILayout.ColorField(new GUIContent("Foreground Color",
                                        "The color of the minimap foreground image.\n\n- It will only work if you provide a Foreground sprite."),
                                        script.foregroundColor);

                script.contentTransparency = EditorGUILayout.Slider(new GUIContent("Content Transparency",
                            "This value defines the transparency of all content captured by the target Minimap Camera and displayed on this Minimap."),
                            script.contentTransparency, 0f, 100.0f);

                script.renderContent = EditorGUILayout.Toggle(new GUIContent("Render Content",
                            "If you disable this option, all content captured by the target Minimap Camera will no longer be displayed within this Minimap."),
                            script.renderContent);

                GUILayout.Space(10);
                EditorGUILayout.LabelField("Settings For Border Icons", EditorStyles.boldLabel);
                GUILayout.Space(10);

                script.iconsRotationMethod = (IconsRotationMethod)EditorGUILayout.EnumPopup(new GUIContent("Icons Rotation Method",
                                                "Choose the way in which the highlighted items on the border will rotate according to the direction in which their respective Minimap Items are."),
                                                script.iconsRotationMethod);

                script.showNorthIcon = EditorGUILayout.Toggle(new GUIContent("Show North Icon",
                        "If you want to display the north icon on this Minimap, enable this option."),
                        script.showNorthIcon);
                if (script.showNorthIcon == true)
                {
                    EditorGUI.indentLevel += 1;

                    script.northDirection = (NorthDirection)EditorGUILayout.EnumPopup(new GUIContent("North Direction In World",
                                "The axis that will represent the north of your game in the world."),
                                script.northDirection);

                    script.autoRotateNorthIcon = EditorGUILayout.Toggle(new GUIContent("Auto Rotate",
                                "If this option is enabled, the north icon will always be rotated so that it is always rendered at a 0º angle, even if the minimap is rotated."),
                                script.autoRotateNorthIcon);

                    script.northIconSprite = (Sprite)EditorGUILayout.ObjectField(new GUIContent("North Icon Sprite",
                        "The sprite that will represent the north icon on this minimap."),
                        script.northIconSprite, typeof(Sprite), true, GUILayout.Height(16));

                    script.northIconSize = EditorGUILayout.FloatField(new GUIContent("North Icon Size",
                        "The size the north icon will render on the Minimap."),
                        script.northIconSize);

                    EditorGUI.indentLevel -= 1;
                }

                GUILayout.Space(10);
                EditorGUILayout.LabelField("Settings For Input Events", EditorStyles.boldLabel);
                GUILayout.Space(10);
                script.showInputEventsOptions = EditorGUILayout.Foldout(script.showInputEventsOptions, (script.showInputEventsOptions == true ? "Hide Input Events Parameters" : "Show Input Events Parameters"));
                if (script.showInputEventsOptions == true)
                {
                    DrawDefaultInspector();
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

            public void CreateOrUpdateThePlaceHoldersAppearence(MinimapRenderer script)
            {
                //If game is running cancel
                if (Application.isPlaying == true)
                    return;

                //If not have the place holder, create them. If have place holder, and it's not updated, update them
                bool hasChangedSomethingInPlaceHolders = false;
                GameObject placeHolderBaseObj = script.GetChildGameObjectByName(script.gameObject, "Place Holder Base");
                if (placeHolderBaseObj == null)
                {
                    placeHolderBaseObj = new GameObject("Place Holder Base");
                    placeHolderBaseObj.transform.SetParent(script.gameObject.transform);
                    placeHolderBaseObj.transform.SetSiblingIndex(0);
                    RectTransform rectTransform = placeHolderBaseObj.AddComponent<RectTransform>();
                    rectTransform.sizeDelta = new Vector2(script.gameObject.GetComponent<RectTransform>().sizeDelta.x, script.gameObject.GetComponent<RectTransform>().sizeDelta.y);
                    rectTransform.offsetMax = new Vector2(0, 0);
                    rectTransform.offsetMin = new Vector2(0, 0);
                    rectTransform.anchorMin = new Vector2(0, 0);
                    rectTransform.anchorMax = new Vector2(1, 1);
                    rectTransform.pivot = new Vector2(0.5f, 0.5f);
                    placeHolderBaseObj.AddComponent<Image>();
                    hasChangedSomethingInPlaceHolders = true;

                    //Load default assets, because is in editor
                    script.baseShapeSprite = (Sprite)AssetDatabase.LoadAssetAtPath("Assets/MT Assets/Easy Minimap System/Formats/Circle.png", typeof(Sprite));
                    script.northIconSprite = (Sprite)AssetDatabase.LoadAssetAtPath("Assets/MT Assets/Easy Minimap System/Sprites/North0.png", typeof(Sprite));
                }
                GameObject placeHolderRenderObj = script.GetChildGameObjectByName(placeHolderBaseObj, "Place Holder Render");
                if (placeHolderRenderObj == null)
                {
                    placeHolderRenderObj = new GameObject("Place Holder Render");
                    placeHolderRenderObj.transform.SetParent(placeHolderBaseObj.gameObject.transform);
                    placeHolderRenderObj.transform.SetSiblingIndex(0);
                    RectTransform rectTransform = placeHolderRenderObj.AddComponent<RectTransform>();
                    rectTransform.sizeDelta = new Vector2(script.gameObject.GetComponent<RectTransform>().sizeDelta.x, script.gameObject.GetComponent<RectTransform>().sizeDelta.y);
                    rectTransform.offsetMax = new Vector2(0, 0);
                    rectTransform.offsetMin = new Vector2(0, 0);
                    rectTransform.anchorMin = new Vector2(0, 0);
                    rectTransform.anchorMax = new Vector2(1, 1);
                    rectTransform.pivot = new Vector2(0.5f, 0.5f);
                    placeHolderRenderObj.AddComponent<Image>();
                    placeHolderRenderObj.AddComponent<Mask>();
                    hasChangedSomethingInPlaceHolders = true;
                }
                GameObject placeHolderBgObj = script.GetChildGameObjectByName(placeHolderRenderObj, "Place Holder Bg");
                if (placeHolderBgObj == null)
                {
                    placeHolderBgObj = new GameObject("Place Holder Bg");
                    placeHolderBgObj.transform.SetParent(placeHolderRenderObj.gameObject.transform);
                    placeHolderBgObj.transform.SetSiblingIndex(0);
                    RectTransform rectTransform = placeHolderBgObj.AddComponent<RectTransform>();
                    rectTransform.sizeDelta = new Vector2(script.gameObject.GetComponent<RectTransform>().sizeDelta.x, script.gameObject.GetComponent<RectTransform>().sizeDelta.y);
                    rectTransform.offsetMax = new Vector2(0, 0);
                    rectTransform.offsetMin = new Vector2(0, 0);
                    rectTransform.anchorMin = new Vector2(0, 0);
                    rectTransform.anchorMax = new Vector2(1, 1);
                    rectTransform.pivot = new Vector2(0.5f, 0.5f);
                    placeHolderBgObj.AddComponent<Image>();
                    hasChangedSomethingInPlaceHolders = true;
                }
                GameObject placeHolderFgObj = script.GetChildGameObjectByName(placeHolderRenderObj, "Place Holder Fg");
                if (placeHolderFgObj == null)
                {
                    placeHolderFgObj = new GameObject("Place Holder Fg");
                    placeHolderFgObj.transform.SetParent(placeHolderRenderObj.gameObject.transform);
                    placeHolderFgObj.transform.SetSiblingIndex(1);
                    RectTransform rectTransform = placeHolderFgObj.AddComponent<RectTransform>();
                    rectTransform.sizeDelta = new Vector2(script.gameObject.GetComponent<RectTransform>().sizeDelta.x, script.gameObject.GetComponent<RectTransform>().sizeDelta.y);
                    rectTransform.offsetMax = new Vector2(0, 0);
                    rectTransform.offsetMin = new Vector2(0, 0);
                    rectTransform.anchorMin = new Vector2(0, 0);
                    rectTransform.anchorMax = new Vector2(1, 1);
                    rectTransform.pivot = new Vector2(0.5f, 0.5f);
                    placeHolderFgObj.AddComponent<Image>();
                    hasChangedSomethingInPlaceHolders = true;
                }
                GameObject placeHolderNorthObj = script.GetChildGameObjectByName(placeHolderBaseObj, "Place Holder North");
                if (placeHolderNorthObj == null)
                {
                    placeHolderNorthObj = new GameObject("Place Holder North");
                    placeHolderNorthObj.transform.SetParent(placeHolderBaseObj.gameObject.transform);
                    placeHolderNorthObj.transform.SetSiblingIndex(1);
                    RectTransform rectTransform = placeHolderNorthObj.AddComponent<RectTransform>();
                    rectTransform.sizeDelta = new Vector2(script.gameObject.GetComponent<RectTransform>().sizeDelta.x, script.gameObject.GetComponent<RectTransform>().sizeDelta.y);
                    rectTransform.offsetMax = new Vector2(0, 0);
                    rectTransform.offsetMin = new Vector2(0, 0);
                    rectTransform.anchorMin = new Vector2(0.5f, 1);
                    rectTransform.anchorMax = new Vector2(0.5f, 1);
                    rectTransform.pivot = new Vector2(0.5f, 1);
                    placeHolderNorthObj.AddComponent<Image>();
                    hasChangedSomethingInPlaceHolders = true;
                }
                if (placeHolderBaseObj != null)
                {
                    Image placeHolderBase = placeHolderBaseObj.GetComponent<Image>();
                    if (placeHolderBase.sprite != script.baseShapeSprite)
                    {
                        placeHolderBase.sprite = script.baseShapeSprite;
                        hasChangedSomethingInPlaceHolders = true;
                    }
                    placeHolderBase.color = script.borderColor;
                }
                if (placeHolderRenderObj != null)
                {
                    Image placeHolderRender = placeHolderRenderObj.GetComponent<Image>();
                    Mask placeHolderMask = placeHolderRenderObj.GetComponent<Mask>();
                    if (placeHolderRender.sprite != script.baseShapeSprite)
                    {
                        placeHolderRender.sprite = script.baseShapeSprite;
                        hasChangedSomethingInPlaceHolders = true;
                    }
                    placeHolderRender.color = new Color(197.0f / 255.0f, 197.0f / 255.0f, 197.0f / 255.0f, ((script.contentTransparency / 100.0f) * 255.0f) / 255.0f);
                    if (placeHolderMask.showMaskGraphic != script.renderContent)
                    {
                        placeHolderMask.showMaskGraphic = script.renderContent;
                        hasChangedSomethingInPlaceHolders = true;
                    }
                }
                if (placeHolderBgObj != null)
                {
                    Image placeHolderBg = placeHolderBgObj.GetComponent<Image>();
                    if (placeHolderBg.sprite != script.backgroundSprite)
                    {
                        placeHolderBg.sprite = script.backgroundSprite;
                        hasChangedSomethingInPlaceHolders = true;
                    }
                    placeHolderBg.color = script.backgroundColor;
                    if (placeHolderBg.enabled != (script.backgroundSprite == null) ? false : true)
                    {
                        placeHolderBg.enabled = (script.backgroundSprite == null) ? false : true;
                        hasChangedSomethingInPlaceHolders = true;
                    }
                }
                if (placeHolderFgObj != null)
                {
                    Image placeHolderFg = placeHolderFgObj.GetComponent<Image>();
                    if (placeHolderFg.sprite != script.foregroundSprite)
                    {
                        placeHolderFg.sprite = script.foregroundSprite;
                        hasChangedSomethingInPlaceHolders = true;
                    }
                    placeHolderFg.color = script.foregroundColor;
                    if (placeHolderFg.enabled != (script.foregroundSprite == null) ? false : true)
                    {
                        placeHolderFg.enabled = (script.foregroundSprite == null) ? false : true;
                        hasChangedSomethingInPlaceHolders = true;
                    }
                }
                RectTransform rectTransformRender = placeHolderRenderObj.GetComponent<RectTransform>();
                rectTransformRender.offsetMax = new Vector2(script.borderWidthSize * -1, script.borderWidthSize * -1);
                rectTransformRender.offsetMin = new Vector2(script.borderWidthSize, script.borderWidthSize);
                if (placeHolderNorthObj != null)
                {
                    Image placeHolderNorth = placeHolderNorthObj.GetComponent<Image>();
                    if (placeHolderNorth.sprite != script.northIconSprite)
                    {
                        placeHolderNorth.sprite = script.northIconSprite;
                        hasChangedSomethingInPlaceHolders = true;
                    }
                    if (placeHolderNorthObj.activeSelf != script.showNorthIcon)
                    {
                        placeHolderNorthObj.SetActive(script.showNorthIcon);
                        hasChangedSomethingInPlaceHolders = true;
                    }
                }
                RectTransform rectTransformNorth = placeHolderNorthObj.GetComponent<RectTransform>();
                rectTransformNorth.anchoredPosition = new Vector2(0, (script.northIconSize / 2f) - (script.borderWidthSize / 2f));
                rectTransformNorth.sizeDelta = new Vector2(script.northIconSize, script.northIconSize);
                //Auto update the canvas, if something was changed
                if (hasChangedSomethingInPlaceHolders == true)
                {
                    if (script.baseShapeSprite != null)
                        AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(script.baseShapeSprite));
                    if (script.northIconSprite != null)
                        AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(script.northIconSprite));
                    AssetDatabase.Refresh();
                }
                //Force all maximized GameObjects childs of this, to be scale 1,1,1 does not matter the resolution
                placeHolderBaseObj.transform.localScale = new Vector3(1, 1, 1);
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
            if (GetChildGameObjectByName(this.gameObject, "Base Shape") == null)
            {
                //Create the minimap base shape
                baseShapeImageGameobject = new GameObject("Base Shape");
                baseShapeImageGameobject.transform.SetParent(this.gameObject.transform);
                RectTransform rectTransformBg = baseShapeImageGameobject.AddComponent<RectTransform>();
                rectTransformBg.sizeDelta = new Vector2(this.gameObject.GetComponent<RectTransform>().sizeDelta.x, this.gameObject.GetComponent<RectTransform>().sizeDelta.y);
                rectTransformBg.offsetMax = new Vector2(0, 0);
                rectTransformBg.offsetMin = new Vector2(0, 0);
                rectTransformBg.anchorMin = new Vector2(0, 0);
                rectTransformBg.anchorMax = new Vector2(1, 1);
                rectTransformBg.pivot = new Vector2(0.5f, 0.5f);
                baseShapeImageComponent = baseShapeImageGameobject.AddComponent<Image>();
                //Create the GameObject for the collider 2d
                GameObject colliderObj = new GameObject("Shape Bound");
                colliderObj.transform.SetParent(baseShapeImageGameobject.transform);
                RectTransform rectTransform = colliderObj.AddComponent<RectTransform>();
                rectTransform.sizeDelta = new Vector2(1, 1);
                rectTransform.offsetMax = new Vector2(0, 0);
                rectTransform.offsetMin = new Vector2(0, 0);
                rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
                rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
                rectTransform.pivot = new Vector2(0.5f, 0.5f);
                colliderObj.layer = LayerMask.NameToLayer("UI");
                baseShapePolygonCollider = colliderObj.AddComponent<PolygonCollider2D>();
                baseShapePolygonCollider.isTrigger = true;
            }
            if (GetChildGameObjectByName(this.gameObject, "Mask") == null)
            {
                //Create the minimap mask
                maskImageGameobject = new GameObject("Mask");
                maskImageGameobject.transform.SetParent(this.gameObject.transform);
                maskRectTransform = maskImageGameobject.AddComponent<RectTransform>();
                maskRectTransform.sizeDelta = new Vector2(this.gameObject.GetComponent<RectTransform>().sizeDelta.x, this.gameObject.GetComponent<RectTransform>().sizeDelta.y);
                maskRectTransform.offsetMax = new Vector2(-5, -5);
                maskRectTransform.offsetMin = new Vector2(5, 5);
                maskRectTransform.anchorMin = new Vector2(0, 0);
                maskRectTransform.anchorMax = new Vector2(1, 1);
                maskRectTransform.pivot = new Vector2(0.5f, 0.5f);
                maskImageComponent = maskImageGameobject.AddComponent<Image>();
                maskComponent = maskImageGameobject.AddComponent<Mask>();
                maskComponent.showMaskGraphic = false;
            }
            if (GetChildGameObjectByName(maskImageGameobject, "Background") == null)
            {
                //Create the foreground gameobject
                backgroundImageGameobject = new GameObject("Background");
                backgroundImageGameobject.transform.SetParent(maskImageGameobject.transform);
                RectTransform rectTransformBg = backgroundImageGameobject.AddComponent<RectTransform>();
                rectTransformBg.sizeDelta = new Vector2(maskImageGameobject.GetComponent<RectTransform>().sizeDelta.x, maskImageGameobject.GetComponent<RectTransform>().sizeDelta.y);
                rectTransformBg.offsetMax = new Vector2(0, 0);
                rectTransformBg.offsetMin = new Vector2(0, 0);
                rectTransformBg.anchorMin = new Vector2(0, 0);
                rectTransformBg.anchorMax = new Vector2(1, 1);
                rectTransformBg.pivot = new Vector2(0.5f, 0.5f);
                backgroundImageComponent = backgroundImageGameobject.AddComponent<Image>();
            }
            if (GetChildGameObjectByName(maskImageGameobject, "Renderer") == null)
            {
                //Create the renderer gameobject
                rendererRawImageGameobject = new GameObject("Renderer");
                rendererRawImageGameobject.transform.SetParent(maskImageGameobject.transform);
                RectTransform rectTransformBg = rendererRawImageGameobject.AddComponent<RectTransform>();
                rectTransformBg.sizeDelta = new Vector2(maskImageGameobject.GetComponent<RectTransform>().sizeDelta.x, maskImageGameobject.GetComponent<RectTransform>().sizeDelta.y);
                rectTransformBg.offsetMax = new Vector2(0, 0);
                rectTransformBg.offsetMin = new Vector2(0, 0);
                rectTransformBg.anchorMin = new Vector2(0, 0);
                rectTransformBg.anchorMax = new Vector2(1, 1);
                rectTransformBg.pivot = new Vector2(0.5f, 0.5f);
                rawImageComponent = rendererRawImageGameobject.AddComponent<RawImage>();
            }
            if (GetChildGameObjectByName(maskImageGameobject, "Foreground") == null)
            {
                //Create the foreground gameobject
                foregroundImageGameobject = new GameObject("Foreground");
                foregroundImageGameobject.transform.SetParent(maskImageGameobject.transform);
                RectTransform rectTransformBg = foregroundImageGameobject.AddComponent<RectTransform>();
                rectTransformBg.sizeDelta = new Vector2(maskImageGameobject.GetComponent<RectTransform>().sizeDelta.x, maskImageGameobject.GetComponent<RectTransform>().sizeDelta.y);
                rectTransformBg.offsetMax = new Vector2(0, 0);
                rectTransformBg.offsetMin = new Vector2(0, 0);
                rectTransformBg.anchorMin = new Vector2(0, 0);
                rectTransformBg.anchorMax = new Vector2(1, 1);
                rectTransformBg.pivot = new Vector2(0.5f, 0.5f);
                foregroundImageComponent = foregroundImageGameobject.AddComponent<Image>();
            }
            if (GetChildGameObjectByName(this.gameObject, "Items") == null)
            {
                //Create the holder for items
                holderForBorderItems = new GameObject("Items");
                holderForBorderItems.transform.SetParent(this.gameObject.transform);
                RectTransform rectTransformBg = holderForBorderItems.AddComponent<RectTransform>();
                rectTransformBg.offsetMax = new Vector2(0, 0);
                rectTransformBg.offsetMin = new Vector2(0, 0);
                rectTransformBg.anchorMin = new Vector2(0, 0);
                rectTransformBg.anchorMax = new Vector2(1, 1);
                rectTransformBg.pivot = new Vector2(0.5f, 0.5f);
            }
            if (GetChildGameObjectByName(holderForBorderItems, "North") == null)
            {
                //Create the pivot for North Icon
                northItemPivotGameObject = new GameObject("North");
                northItemPivotGameObject.transform.SetParent(holderForBorderItems.transform);
                RectTransform rectTransformBg = northItemPivotGameObject.AddComponent<RectTransform>();
                rectTransformBg.offsetMax = new Vector2(0, 0);
                rectTransformBg.offsetMin = new Vector2(0, 0);
                rectTransformBg.anchorMin = new Vector2(0, 0);
                rectTransformBg.anchorMax = new Vector2(1, 1);
                rectTransformBg.pivot = new Vector2(0.5f, 0.5f);
            }
            if (GetChildGameObjectByName(northItemPivotGameObject, "Icon") == null)
            {
                //Create the pivot for North Icon
                GameObject northIconObj = new GameObject("Icon");
                northIconObj.transform.SetParent(northItemPivotGameObject.transform);
                RectTransform rectTransformBg = northIconObj.AddComponent<RectTransform>();
                rectTransformBg.anchorMin = new Vector2(0.5f, 1);
                rectTransformBg.anchorMax = new Vector2(0.5f, 1);
                rectTransformBg.pivot = new Vector2(0.5f, 0.5f);
                rectTransformBg.offsetMax = new Vector2(0, 0);
                rectTransformBg.offsetMin = new Vector2(0, 0);
                rectTransformBg.sizeDelta = new Vector2(25, 25);
                rectTransformBg.anchoredPosition = new Vector2(0, borderWidthSize / 2 * -1);
                northImageComponent = northIconObj.AddComponent<Image>();
            }
            if (GetChildGameObjectByName(holderForBorderItems, "Pool") == null)
            {
                //Clear the list
                minimapItemsForHighlightPool.Clear();

                //Create the pool of Minimap Items
                poolRootForHighlightedItemsGameObject = new GameObject("Pool");
                poolRootForHighlightedItemsGameObject.transform.SetParent(holderForBorderItems.transform);
                RectTransform rectTransformBg = poolRootForHighlightedItemsGameObject.AddComponent<RectTransform>();
                rectTransformBg.offsetMax = new Vector2(0, 0);
                rectTransformBg.offsetMin = new Vector2(0, 0);
                rectTransformBg.anchorMin = new Vector2(0, 0);
                rectTransformBg.anchorMax = new Vector2(1, 1);
                rectTransformBg.pivot = new Vector2(0.5f, 0.5f);

                for (int i = 0; i < MIN_COUNT_OF_ITEMS_IN_POOL; i++)
                {
                    //Create the pivot for this pool item
                    GameObject pivot = new GameObject("Item Pivot");
                    pivot.transform.SetParent(poolRootForHighlightedItemsGameObject.transform);
                    RectTransform rectTransform = pivot.AddComponent<RectTransform>();
                    rectTransform.offsetMax = new Vector2(0, 0);
                    rectTransform.offsetMin = new Vector2(0, 0);
                    rectTransform.anchorMin = new Vector2(0, 0);
                    rectTransform.anchorMax = new Vector2(1, 1);
                    rectTransform.pivot = new Vector2(0.5f, 0.5f);

                    //Create the icon for put in the pivot
                    GameObject icon = new GameObject("Icon");
                    icon.transform.SetParent(pivot.transform);
                    RectTransform rectTransform2 = icon.AddComponent<RectTransform>();
                    rectTransform2.anchorMin = new Vector2(0.5f, 1);
                    rectTransform2.anchorMax = new Vector2(0.5f, 1);
                    rectTransform2.pivot = new Vector2(0.5f, 0.5f);
                    rectTransform2.offsetMax = new Vector2(0, 0);
                    rectTransform2.offsetMin = new Vector2(0, 0);
                    rectTransform2.sizeDelta = new Vector2(25, 25);
                    rectTransform2.anchoredPosition = new Vector2(0, 12.5f - (borderWidthSize / 2));
                    Image image = icon.AddComponent<Image>();
                    minimapItemsForHighlightPool.Add(new MinimapItemOfPool(pivot, image));

                    //Disable the pivot
                    pivot.SetActive(false);
                }
            }
            if (GetChildGameObjectByName(this.gameObject, "Events") == null)
            {
                //Create the holder for items
                thisRendererEventsGameObject = new GameObject("Events");
                thisRendererEventsGameObject.transform.SetParent(this.gameObject.transform);
                RectTransform rectTransformBg = thisRendererEventsGameObject.AddComponent<RectTransform>();
                rectTransformBg.offsetMax = new Vector2(0, 0);
                rectTransformBg.offsetMin = new Vector2(0, 0);
                rectTransformBg.anchorMin = new Vector2(0, 0);
                rectTransformBg.anchorMax = new Vector2(1, 1);
                rectTransformBg.pivot = new Vector2(0.5f, 0.5f);
                thisRendererEventsImageComponent = thisRendererEventsGameObject.AddComponent<Image>();
                thisRendererEventsImageComponent.color = new Color(0, 0, 0, 0);
                thisRendererEvents = thisRendererEventsGameObject.AddComponent<MinimapRendererEvents>();
                thisRendererEvents.minimapRenderer = this;
            }

            //Delete the place holder image in this GameObject
            GameObject placeHolderObj = GetChildGameObjectByName(this.gameObject, "Place Holder Base");
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
            if (minimapDataHolder.instancesOfMinimapRendererInThisScene.Contains(this) == false)
                minimapDataHolder.instancesOfMinimapRendererInThisScene.Add(this);
        }

        public void Update()
        {
            //Cancel update of this component, if not have a mini map camera linked
            if (minimapCameraToShow == null || minimapCameraToShow.renderTexture == null || minimapCameraToShow.GetGeneratedCameraAtRunTime() == null)
            {
                Debug.LogError("Please provide data for variable \"minimapCameraToShow\" to make it possible for Minimap Renderer to work.");
                return;
            }

            //Change the render texture of this minimap if changed in minimap camera
            if (minimapCameraToShow.renderTexture != rawImageComponent.texture)
                rawImageComponent.texture = minimapCameraToShow.renderTexture;

            //Change the sprite of base shape, if changed
            if (baseShapeImageComponent.sprite != baseShapeSprite)
                baseShapeImageComponent.sprite = baseShapeSprite;

            //Change the base shape color, if border color was changed
            if (baseShapeImageComponent.color != borderColor)
                baseShapeImageComponent.color = borderColor;

            //Change the background sprite if changed
            if (backgroundImageComponent.sprite != backgroundSprite)
                backgroundImageComponent.sprite = backgroundSprite;
            if (backgroundImageComponent.enabled != (backgroundSprite == null) ? false : true)
                backgroundImageComponent.enabled = (backgroundSprite == null) ? false : true;

            //Change the background color if changed
            if (backgroundImageComponent.color != backgroundColor)
                backgroundImageComponent.color = backgroundColor;

            //Change the foreground sprite if changed (if the foreground sprite is null, disable the foreground image)
            if (foregroundImageComponent.sprite != foregroundSprite)
                foregroundImageComponent.sprite = foregroundSprite;
            if (foregroundImageComponent.enabled != (foregroundSprite == null) ? false : true)
                foregroundImageComponent.enabled = (foregroundSprite == null) ? false : true;

            //Change thr foreground color if changed
            if (foregroundImageComponent.color != foregroundColor)
                foregroundImageComponent.color = foregroundColor;

            //Change the render texture transparency if changed
            if (contentTransparency != lastContentTransparencyDefined)
            {
                rawImageComponent.color = new Color(255.0f, 255.0f, 255.0f, ((contentTransparency / 100.0f) * 255.0f) / 255.0f);
                lastContentTransparencyDefined = contentTransparency;
            }

            //Change the border width (between background image and raw image of render texture) if changed
            if (maskRectTransform.offsetMax.x != (borderWidthSize * -1) || maskRectTransform.offsetMax.y != (borderWidthSize * -1))
                maskRectTransform.offsetMax = new Vector2(borderWidthSize * -1, borderWidthSize * -1);
            if (maskRectTransform.offsetMin.x != borderWidthSize || maskRectTransform.offsetMin.y != borderWidthSize)
                maskRectTransform.offsetMin = new Vector2(borderWidthSize, borderWidthSize);

            //Change the content sprite of mask if changed
            if (maskImageComponent.sprite != baseShapeSprite)
                maskImageComponent.sprite = baseShapeSprite;

            //Disable or enable raw image if renderContent option is disabled or enabled
            if (rawImageComponent.enabled != renderContent)
                rawImageComponent.enabled = renderContent;

            //Update the form of polygon collider according to current rotation method and set the size of polygon collider according to the total size of minimap renderer
            Rect rectOfThisRenderer = thisRectTransform.rect;
            Vector2 totalSizeOfThisRenderer = new Vector2(rectOfThisRenderer.width / 2.0f, rectOfThisRenderer.height / 2.0f);
            if (iconsRotationMethod != lastIconsRotationMethodDefined || totalSizeOfThisRenderer != lastMinimapRendererSize)
            {
                if (iconsRotationMethod == IconsRotationMethod.CircleShape)
                    baseShapePolygonCollider.CreatePrimitive(32, new Vector2(1, 1), new Vector2(0, 0));
                if (iconsRotationMethod == IconsRotationMethod.SquareShape)
                {
                    baseShapePolygonCollider.CreatePrimitive(4, new Vector2(1, 1), new Vector2(0, 0));
                    Vector2[] squarePoints = new Vector2[] { new Vector2(1, 1), new Vector2(-1, 1), new Vector2(-1, -1), new Vector2(1, -1) };
                    baseShapePolygonCollider.points = squarePoints;
                    baseShapePolygonCollider.SetPath(0, squarePoints);
                }
                baseShapePolygonCollider.transform.localScale = new Vector3(totalSizeOfThisRenderer.x, totalSizeOfThisRenderer.y, 1);
                //Set on cache
                lastMinimapRendererSize = totalSizeOfThisRenderer;
                lastIconsRotationMethodDefined = iconsRotationMethod;
            }
            //Disable or enable the north icon if option showNorthIcon is disabled or enabled
            if (northItemPivotGameObject.activeSelf != showNorthIcon)
                northItemPivotGameObject.SetActive(showNorthIcon);

            //------------------------------------- START UPDATING NORTH ICON AND ALL YOUR PARAMETERS ----------------------------------

            //If showNorthIcon is enable, update all options and the icon
            if (showNorthIcon == true)
            {
                //If the north sprite was changed, update it
                if (northImageComponent.sprite != northIconSprite)
                    northImageComponent.sprite = northIconSprite;

                //If the size of icon is changed, update
                if (northImageComponent.rectTransform.sizeDelta.x != northIconSize)
                    northImageComponent.rectTransform.sizeDelta = new Vector2(northIconSize, northIconSize);

                //Mantain the north rotation in Z as zero
                if (autoRotateNorthIcon == true)
                    northImageComponent.transform.eulerAngles = new Vector3(0, 0, 0);
                if (autoRotateNorthIcon == false)
                    northImageComponent.transform.localEulerAngles = new Vector3(0, 0, 0);

                //Push the north icon to far away (half of minimap rect size multiplied) in forward and grab it to the border of polygon collider of minimap renderer (put the icon in center of border of minimap renderer too)
                northImageComponent.rectTransform.anchoredPosition = new Vector2(0, (((totalSizeOfThisRenderer.x > totalSizeOfThisRenderer.y) ? totalSizeOfThisRenderer.x : totalSizeOfThisRenderer.y) * 1.5f)); //Older distance is 4096 as static
                temporaryRaycastHit = Physics2D.Linecast(northImageComponent.transform.position, northItemPivotGameObject.transform.position, LAYER_OF_BORDER_POLYGON_COLLIDER);
                if (temporaryRaycastHit == true && temporaryRaycastHit.collider != null && temporaryRaycastHit.collider == baseShapePolygonCollider)
                {
                    northImageComponent.rectTransform.position = temporaryRaycastHit.point;
                    northImageComponent.rectTransform.anchoredPosition = new Vector2(0, northImageComponent.rectTransform.anchoredPosition.y - (borderWidthSize / 2.0f));
                }

                //Update the "virtual" position of imaginary north in the world when the option northDirection was changed
                if (minimapCameraToShow.GetGeneratedCameraAtRunTime() != null)
                    if (northDirection != lastNorthDirectionSelected || imaginaryNorthVector3InWorld.y == 0)
                    {
                        if (northDirection == NorthDirection.AxisX)
                            imaginaryNorthVector3InWorld = new Vector3(9999999999999999999, minimapCameraToShow.GetGeneratedCameraAtRunTime().transform.position.y, 0);
                        if (northDirection == NorthDirection.AxisZ)
                            imaginaryNorthVector3InWorld = new Vector3(0, minimapCameraToShow.GetGeneratedCameraAtRunTime().transform.position.y, 9999999999999999999);
                        if (northDirection == NorthDirection.NegativeAxisX)
                            imaginaryNorthVector3InWorld = new Vector3(-999999999999999999, minimapCameraToShow.GetGeneratedCameraAtRunTime().transform.position.y, 0);
                        if (northDirection == NorthDirection.NegativeAxisZ)
                            imaginaryNorthVector3InWorld = new Vector3(0, minimapCameraToShow.GetGeneratedCameraAtRunTime().transform.position.y, -999999999999999999);
                        lastNorthDirectionSelected = northDirection;
                    }

                //Update the rotation of north icon
                Vector3 targetDir = imaginaryNorthVector3InWorld - minimapCameraToShow.GetGeneratedCameraAtRunTime().transform.position;
                Vector3 forward = minimapCameraToShow.GetGeneratedCameraAtRunTime().transform.up;
                float angle = Vector3.SignedAngle(targetDir, forward, Vector3.up);
                northItemPivotGameObject.transform.localRotation = Quaternion.Euler(0, 0, angle);
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

            //Update all data (like, sprite, color, rotation, activity etc) for each minimap item to highlight in border of this minimap renderer
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

                //Get the this associated Minimap Item position in 3d World and target Minimap Camera position in 3d World (converted at same height)
                Vector3 itemPosition = item.minimapItemAssociated.GetGeneratedSpriteAtRunTime().transform.position;
                itemPosition.y = minimapCameraToShow.GetGeneratedCameraAtRunTime().transform.position.y;
                Vector3 minimapCameraPosition = minimapCameraToShow.GetGeneratedCameraAtRunTime().transform.position;
                Quaternion minimapCameraRotation = Quaternion.Euler(0, minimapCameraToShow.GetGeneratedCameraAtRunTime().transform.root.eulerAngles.y, 0);
                Vector3 minimapCameraForward = minimapCameraToShow.GetGeneratedCameraAtRunTime().transform.up;

                //Disable this highlighted item, if this associated Minimap Item is in field of view of target Minimap Camera
                if (iconsRotationMethod == IconsRotationMethod.CircleShape) //<- If is circle shape
                {
                    float distanceBetweenItemPositionAndMinimapCameraPosition = Vector3.Distance(itemPosition, minimapCameraPosition);
                    if (distanceBetweenItemPositionAndMinimapCameraPosition < minimapCameraToShow.fieldOfView)
                        item.icon.gameObject.SetActive(false);
                    if (distanceBetweenItemPositionAndMinimapCameraPosition >= minimapCameraToShow.fieldOfView)
                        item.icon.gameObject.SetActive(true);
                }
                if (iconsRotationMethod == IconsRotationMethod.SquareShape) //<- If is square shape
                {
                    Vector3 minimapItemWorldToViewportPointOfMinimapCamera = minimapCameraToShow.GetGeneratedCameraAtRunTime().WorldToViewportPoint(item.minimapItemAssociated.GetGeneratedSpriteAtRunTime().transform.position);
                    if (minimapItemWorldToViewportPointOfMinimapCamera.x < 0 || minimapItemWorldToViewportPointOfMinimapCamera.x > 1 || minimapItemWorldToViewportPointOfMinimapCamera.y < 0 || minimapItemWorldToViewportPointOfMinimapCamera.y > 1)
                        item.icon.gameObject.SetActive(true);
                    else
                        item.icon.gameObject.SetActive(false);
                }

                //Update the rotation of this minimap highlighted item, corresponding to the direction of associated Minimap Item
                Vector3 targetDir = itemPosition - minimapCameraPosition;
                float angle = Vector3.SignedAngle(targetDir, minimapCameraForward, Vector3.up);
                item.pivot.transform.localRotation = Quaternion.Euler(0, 0, angle);

                //Push the highlighted icon to far away (half of minimap rect size multiplied) in forward and grab it to the border of polygon collider of minimap renderer (put the icon in center of border of minimap renderer too)
                item.icon.rectTransform.anchoredPosition = new Vector2(0, (((totalSizeOfThisRenderer.x > totalSizeOfThisRenderer.y) ? totalSizeOfThisRenderer.x : totalSizeOfThisRenderer.y) * 1.5f)); //Older distance is 4096 as static
                temporaryRaycastHit = Physics2D.Linecast(item.icon.transform.position, item.pivot.transform.position, LAYER_OF_BORDER_POLYGON_COLLIDER);
                if (temporaryRaycastHit == true && temporaryRaycastHit.collider != null && temporaryRaycastHit.collider == baseShapePolygonCollider)
                {
                    item.icon.rectTransform.position = temporaryRaycastHit.point;
                    item.icon.rectTransform.anchoredPosition = new Vector2(0, item.icon.rectTransform.anchoredPosition.y - (borderWidthSize / 2.0f));
                }
            }

            //Force all maximized GameObjects childs of this, to be scale 1,1,1 does not matter the resolution
            baseShapeImageComponent.transform.localScale = new Vector3(1, 1, 1);
            maskImageComponent.transform.localScale = new Vector3(1, 1, 1);
            holderForBorderItems.transform.localScale = new Vector3(1, 1, 1);
            thisRendererEventsGameObject.transform.localScale = new Vector3(1, 1, 1);
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

        public MinimapRenderer[] GetListOfAllMinimapRenderersInThisScene()
        {
            //If is not playing, cancel
            if (Application.isPlaying == false)
            {
                Debug.LogError("It is only possible to obtain the list of Minimap Renderers in this scene, if the application is being executed.");
                return null;
            }

            //Return a list that contains reference to all of this component in this scene
            return minimapDataHolder.instancesOfMinimapRendererInThisScene.ToArray();
        }
    }
}