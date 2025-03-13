#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;
#endif
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace MTAssets.EasyMinimapSystem
{
    /*
     This class is responsible for the functioning of the "Minimap Item" component, and all its functions.
    */
    /*
     * The Easy Minimap System was developed by Marcos Tomaz in 2019.
     * Need help? Contact me (mtassets@windsoft.xyz)
    */

    [AddComponentMenu("MT Assets/Easy Minimap System/Minimap Item")] //Add this component in a category of addComponent menu
    public class MinimapItem : MonoBehaviour
    {
        //Private constants
        private const float BASE_HEIGHT_IN_3D_WORLD = 99004;

        //Private variables
        private GameObject minimapDataHolderObj;
        private MinimapDataHolder minimapDataHolder;
        private Transform minimapItemsHolder;
        private GameObject tempSpriteObj;
        private SpriteRenderer tempSprite;
        private ParticleSystem tempParticles;
        private ParticleSystemRenderer tempParticlesRenderer;
        private SphereCollider tempSphereColliderForInputEvents;

        //Cache variables
        private Vector3 lastRenderItemSize = new Vector3(-1, -1, -1);
        private ParticlesHighlightMode lastParticlesHighlightMode = ParticlesHighlightMode.Disabled;
        private Sprite lastSpriteDefinedInParticles = null;
        private Color lastColorDefinedInParticles = Color.clear;
        private float lastParticlesSizeMultiplierDefined = -1.0f;
        private int lastOrderInLayerDefinedInParticles = -1;
        private bool alreadyUpdatedSettingsOfParticles = false;

        //Enums of script
        public enum ParticlesHighlightMode
        {
            Disabled,
            WavesIncrease,
            WavesDecrease
        }
        public enum FollowRotationOf
        {
            ThisGameObject,
            CustomGameObject
        }

        //Public variables
        [HideInInspector]
        public Sprite itemSprite;
        [HideInInspector]
        public Color spriteColor = new Color(1, 1, 1, 1);
        [HideInInspector]
        public bool flipInX = false;
        [HideInInspector]
        public bool flipInY = false;
        [HideInInspector]
        public bool raycastTarget = true;
        [HideInInspector]
        public Vector3 sizeOnMinimap = new Vector3(2, 0, 2);
        [HideInInspector]
        public float sizeOnHighlight = 20.0f;
        [HideInInspector]
        public int orderInLayer = 0;
        [HideInInspector]
        public ParticlesHighlightMode particlesHighlightMode = ParticlesHighlightMode.Disabled;
        [HideInInspector]
        public Sprite particlesSprite = null;
        [HideInInspector]
        public Color particlesColor = Color.gray;
        [HideInInspector]
        public float particlesSizeMultiplier = 1.0f;
        [HideInInspector]
        public FollowRotationOf followRotationOf = FollowRotationOf.ThisGameObject;
        [HideInInspector]
        public Transform customGameObjectToFollowRotation = null;
        [HideInInspector]
        public float movementsSmoothing = 14;

#if UNITY_EDITOR
        //The UI of this component
        #region INTERFACE_CODE
        [UnityEditor.CustomEditor(typeof(MinimapItem))]
        public class CustomInspector : UnityEditor.Editor
        {
            public override void OnInspectorGUI()
            {
                //Start the undo event support, draw default inspector and monitor of changes
                MinimapItem script = (MinimapItem)target;
                EditorGUI.BeginChangeCheck();
                Undo.RecordObject(target, "Undo Event");

                //Support reminder
                GUILayout.Space(10);
                EditorGUILayout.HelpBox("Remember to read the Easy Minimap System documentation to understand how to use it.\nGet support at: mtassets@windsoft.xyz", MessageType.None);
                GUILayout.Space(10);

                //Validate sizeOnMinimap variable
                if (script.sizeOnMinimap.y != 0)
                    script.sizeOnMinimap.y = 0;

                //Start of settings
                EditorGUILayout.LabelField("Settings For Minimap Item", EditorStyles.boldLabel);
                GUILayout.Space(10);

                script.itemSprite = (Sprite)EditorGUILayout.ObjectField(new GUIContent("Sprite of Item",
                        "The sprite that represents the Minimap Item."),
                        script.itemSprite, typeof(Sprite), true, GUILayout.Height(16));
                if (script.itemSprite == null)
                    EditorGUILayout.HelpBox("Please associate a sprite with this Minimap Item. If the sprite is empty, this item will not be represented on the Minimap Renderer.", MessageType.Warning);

                script.spriteColor = EditorGUILayout.ColorField(new GUIContent("Sprite Color",
                        "The sprite color of the Minimap Item."),
                        script.spriteColor);

                script.flipInX = EditorGUILayout.Toggle(new GUIContent("Flip In X Axis",
                "Rotate 180 degrees on x axis?"),
                script.flipInX);

                script.flipInY = EditorGUILayout.Toggle(new GUIContent("Flip In Y Axis",
                        "Rotate 180 degrees on y axis?"),
                        script.flipInY);

                script.raycastTarget = EditorGUILayout.Toggle(new GUIContent("Raycast Target",
                        "If this variable is activated, this Minimap Item can receive interactions (Inputs) from the player. In other words, it can be selected, or clicked (in some Minimap Renderer)."),
                        script.raycastTarget);

                script.sizeOnMinimap = EditorGUILayout.Vector3Field(new GUIContent("Size On Minimap",
                                        "This is the scale of the item while it is being rendered inside the Minimap. If the item seems too small on Minimap, increase this scale.\n\nSet this scale, taking into account the Field of View of the Minimap Camera.\n\nThe size that the sprite will be rendered on the Minimap is given in Units (1 Unit = 1 Meter)."),
                                        script.sizeOnMinimap);

                script.sizeOnHighlight = EditorGUILayout.FloatField(new GUIContent("Size On Highlight",
                        "This is the scale of the item while it is highlighted in a Minimap Renderer (on the edge of the Minimap when it is far away) or in a Minimap Compass."),
                        script.sizeOnHighlight);

                script.orderInLayer = EditorGUILayout.IntSlider(new GUIContent("Order In Layer",
                        "The order in which this sprite will be rendered. The bigger it is, the higher it is over other sprites."),
                        script.orderInLayer, 0, 5);

                GUILayout.Space(10);
                EditorGUILayout.LabelField("Settings For Particles", EditorStyles.boldLabel);
                GUILayout.Space(10);

                script.particlesHighlightMode = (ParticlesHighlightMode)EditorGUILayout.EnumPopup(new GUIContent("Particles Highlight Mode",
                                                    "Activate this option if you want to highlight this Minimap Item from others, using a particle mini-system with pre-defined effects."),
                                                    script.particlesHighlightMode);
                if (script.particlesHighlightMode != ParticlesHighlightMode.Disabled)
                {
                    EditorGUI.indentLevel += 1;
                    script.particlesSprite = (Sprite)EditorGUILayout.ObjectField(new GUIContent("Particles Sprite",
                                            "The sprite that will be used on the particles.\n\nIf you do not provide a sprite, a standard square sprite is displayed."),
                                            script.particlesSprite, typeof(Sprite), true, GUILayout.Height(16));

                    script.particlesColor = EditorGUILayout.ColorField(new GUIContent("Particles Color",
                                            "The color in which the highlighted particles will be rendered."),
                                            script.particlesColor);

                    script.particlesSizeMultiplier = EditorGUILayout.Slider(new GUIContent("Particles Size Multiplier",
                                            "Increase or decrease this value to increase or decrease the rendering scale of the highlighted particles of this Minimap Item.\n\nNote that the default multiplier value is 1.0. But the highlighted particles, by default, scale with the width and height of this Minimap Item, but if you need a larger scale, you can control that scale through this variable."),
                                            script.particlesSizeMultiplier, 0.001f, 10.0f);
                    EditorGUI.indentLevel -= 1;
                }

                GUILayout.Space(10);
                EditorGUILayout.LabelField("Settings For Movement", EditorStyles.boldLabel);
                GUILayout.Space(10);

                script.followRotationOf = (FollowRotationOf)EditorGUILayout.EnumPopup(new GUIContent("Follow Rotation Of",
                                    "Choose a GameObject for this Minimap Item to follow its rotation.\n\nThis GameObject - This minimap item will follow the rotation of this GameObject.\n\nCustom GameObject - This minimap item will follow the rotation of another GameObject of your choice."),
                                    script.followRotationOf);
                if (script.followRotationOf == FollowRotationOf.CustomGameObject)
                {
                    EditorGUI.indentLevel += 1;
                    script.customGameObjectToFollowRotation = (Transform)EditorGUILayout.ObjectField(new GUIContent("GameObject To Follow",
                        "This minimap item will follow the rotation of this GameObject."),
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
                //Draw the controls of this Scan
                MinimapItem script = (MinimapItem)target;

                //Start the handle ui to draw the texture in screen
                Handles.BeginGUI();
                GUIStyle style = new GUIStyle();
                style.fontStyle = FontStyle.Bold;
                style.alignment = TextAnchor.MiddleCenter;

                if (script.itemSprite != null)
                {
                    GUI.Box(new Rect(Screen.width - 133, Screen.height - 193, 126, 146), "");
                    GUI.Label(new Rect(Screen.width - 120, Screen.height - 195, 100, 20), "Minimap Item Of", style);
                    GUI.Label(new Rect(Screen.width - 120, Screen.height - 183, 100, 20), "This GameObject", style);
                    GUI.DrawTexture(new Rect(Screen.width - 110, Screen.height - 155, 80, 80), script.itemSprite.texture, ScaleMode.ScaleToFit, true, 1.0f);
                    if (GUI.Button(new Rect(Screen.width - 130, Screen.height - 65, 120, 16), "Select Asset"))
                        EditorGUIUtility.PingObject(script.itemSprite);
                }
                Handles.EndGUI();
            }
        }

        public void OnDrawGizmosSelected()
        {
            //If not have a item sprite, cancel render of gizmos
            if (itemSprite == null)
                return;

            //Get Y angle correspondent of option "Follow Rotation Of"
            float yAnle = this.gameObject.transform.eulerAngles.y;
            if (followRotationOf == FollowRotationOf.CustomGameObject && customGameObjectToFollowRotation != null)
                yAnle = customGameObjectToFollowRotation.eulerAngles.y;

            //Set color of gizmos
            Gizmos.color = Color.white;
            Handles.color = Color.white;
            Matrix4x4 rotationMatrix = Matrix4x4.TRS(this.gameObject.transform.position,
                                                     Quaternion.Euler(0 + ((flipInY == true) ? 180 : 0), yAnle, 0 + ((flipInX == true) ? 180 : 0)),
                                                     new Vector3(1, 1, 1));
            Handles.matrix = rotationMatrix;

            //Build the position and size of rectangle in screen, on position of this GameObject
            Vector3 positionAdjust = Vector3.zero;
            Vector2 pivotPosition = new Vector2(0.5f, 0.5f);
            pivotPosition = new Vector2(itemSprite.pivot.x / itemSprite.texture.width, itemSprite.pivot.y / itemSprite.texture.height);
            positionAdjust.x -= sizeOnMinimap.x * (pivotPosition.x);
            positionAdjust.z -= sizeOnMinimap.z * (pivotPosition.y);

            //Show the lines
            Handles.DrawSolidRectangleWithOutline(new Vector3[] { new Vector3(0 + positionAdjust.x, 0, 0 + positionAdjust.z),                               //Bottom left
                                                                  new Vector3(0 + positionAdjust.x, 0, sizeOnMinimap.z + positionAdjust.z),                 //Top left
                                                                  new Vector3(sizeOnMinimap.x + positionAdjust.x, 0, sizeOnMinimap.z + positionAdjust.z),   //Top right
                                                                  new Vector3(sizeOnMinimap.x + positionAdjust.x, 0, 0 + positionAdjust.z)},                //Bottom right
                                                                  new Color(1, 1, 1, 0.03f), Color.white);
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
            minimapItemsHolder = minimapDataHolderObj.transform.Find("Minimap Items Holder");
            if (minimapItemsHolder == null)
            {
                GameObject obj = new GameObject("Minimap Items Holder");
                minimapItemsHolder = obj.transform;
                minimapItemsHolder.SetParent(minimapDataHolderObj.transform);
                minimapItemsHolder.localPosition = Vector3.zero;
                minimapItemsHolder.localEulerAngles = Vector3.zero;
            }
            if (minimapDataHolder.instancesOfMinimapItemInThisScene.Contains(this) == false)
                minimapDataHolder.instancesOfMinimapItemInThisScene.Add(this);

            //Create the minimap item
            tempSpriteObj = new GameObject("Minimap Item (" + this.gameObject.transform.name + ")");
            tempSpriteObj.transform.SetParent(minimapItemsHolder);
            tempSpriteObj.transform.position = new Vector3(this.gameObject.transform.position.x, BASE_HEIGHT_IN_3D_WORLD, this.gameObject.transform.position.z);
            tempSprite = tempSpriteObj.AddComponent<SpriteRenderer>();
            tempParticles = tempSpriteObj.AddComponent<ParticleSystem>();
            tempParticles.Stop();
            tempParticlesRenderer = tempParticles.GetComponent<ParticleSystemRenderer>();
            tempSphereColliderForInputEvents = tempSpriteObj.AddComponent<SphereCollider>();
            tempSphereColliderForInputEvents.isTrigger = true;
            tempSpriteObj.layer = LayerMask.NameToLayer("UI");

            //Add the activity monitor to the camera
            ActivityMonitor activeMonitor = tempSpriteObj.AddComponent<ActivityMonitor>();
            activeMonitor.responsibleScriptComponentForThis = this;
        }

        void ApplyNewParticlesHighlightMode()
        {
            //This method will apply the new highlight mode on temporary particle system

            //If particle highlight is disabled, and return
            if (particlesHighlightMode == ParticlesHighlightMode.Disabled)
            {
                tempParticles.Stop();
                return;
            }

            //----- Waves Increase
            if (particlesHighlightMode == ParticlesHighlightMode.WavesIncrease)
            {
                var main = tempParticles.main;
                main.maxParticles = 5;
                main.startLifetime = 1.0f;
                main.startSpeed = 0.0f;
                main.scalingMode = ParticleSystemScalingMode.Shape;
                var emission = tempParticles.emission;
                emission.enabled = true;
                emission.rateOverTime = 0;
                emission.burstCount = 1;
                ParticleSystem.Burst burst = new ParticleSystem.Burst();
                burst.time = 0.0f;
                burst.count = 1.0f;
                burst.cycleCount = 0;
                burst.repeatInterval = 0.5f;
                burst.probability = 1.0f;
                emission.SetBurst(0, burst);
                var shape = tempParticles.shape;
                shape.enabled = true;
                shape.shapeType = ParticleSystemShapeType.Circle;
                shape.radius = 0.0001f;
                shape.position = new Vector3(0, 0, 0.1f);
                var sizeOverLifetime = tempParticles.sizeOverLifetime;
                sizeOverLifetime.enabled = true;
                AnimationCurve sizeCurve = new AnimationCurve();
                sizeCurve.AddKey(0.0f, 0.2f);
                sizeCurve.AddKey(1.0f, 1.0f);
                sizeOverLifetime.size = new ParticleSystem.MinMaxCurve(1.0f, sizeCurve);
                var colorOverLifetime = tempParticles.colorOverLifetime;
                colorOverLifetime.enabled = true;
                Gradient colorCurve = new Gradient();
                colorCurve.SetKeys(
                    new GradientColorKey[]{
                        new GradientColorKey(Color.white, 0.0f),
                        new GradientColorKey(Color.white, 1.0f),
                    },
                    new GradientAlphaKey[]{
                        new GradientAlphaKey(0.0f, 0.0f),
                        new GradientAlphaKey(1.0f, 0.36f),
                        new GradientAlphaKey(1.0f, 0.73f),
                        new GradientAlphaKey(0.0f, 1.0f)
                    }
                );
                colorOverLifetime.color = new ParticleSystem.MinMaxGradient(colorCurve);
                var renderer = tempParticlesRenderer;
                renderer.enabled = true;
                renderer.renderMode = ParticleSystemRenderMode.Billboard;
                renderer.material = minimapDataHolder.defaultMaterialForMinimapItems;
                renderer.maxParticleSize = 100.0f;
            }

            //----- Waves Decrease
            if (particlesHighlightMode == ParticlesHighlightMode.WavesDecrease)
            {
                var main = tempParticles.main;
                main.maxParticles = 5;
                main.startLifetime = 1.0f;
                main.startSpeed = 0.0f;
                main.scalingMode = ParticleSystemScalingMode.Shape;
                var emission = tempParticles.emission;
                emission.enabled = true;
                emission.rateOverTime = 0;
                emission.burstCount = 1;
                ParticleSystem.Burst burst = new ParticleSystem.Burst();
                burst.time = 0.0f;
                burst.count = 1.0f;
                burst.cycleCount = 0;
                burst.repeatInterval = 0.5f;
                burst.probability = 1.0f;
                emission.SetBurst(0, burst);
                var shape = tempParticles.shape;
                shape.enabled = true;
                shape.shapeType = ParticleSystemShapeType.Circle;
                shape.radius = 0.0001f;
                shape.position = new Vector3(0, 0, 0.1f);
                var sizeOverLifetime = tempParticles.sizeOverLifetime;
                sizeOverLifetime.enabled = true;
                AnimationCurve sizeCurve = new AnimationCurve();
                sizeCurve.AddKey(0.0f, 1.0f);
                sizeCurve.AddKey(1.0f, 0.2f);
                sizeOverLifetime.size = new ParticleSystem.MinMaxCurve(1.0f, sizeCurve);
                var colorOverLifetime = tempParticles.colorOverLifetime;
                colorOverLifetime.enabled = true;
                Gradient colorCurve = new Gradient();
                colorCurve.SetKeys(
                    new GradientColorKey[]{
                        new GradientColorKey(Color.white, 0.0f),
                        new GradientColorKey(Color.white, 1.0f),
                    },
                    new GradientAlphaKey[]{
                        new GradientAlphaKey(0.0f, 0.0f),
                        new GradientAlphaKey(1.0f, 0.36f),
                        new GradientAlphaKey(1.0f, 0.73f),
                        new GradientAlphaKey(0.0f, 1.0f)
                    }
                );
                colorOverLifetime.color = new ParticleSystem.MinMaxGradient(colorCurve);
                var renderer = tempParticlesRenderer;
                renderer.enabled = true;
                renderer.renderMode = ParticleSystemRenderMode.Billboard;
                renderer.material = minimapDataHolder.defaultMaterialForMinimapItems;
                renderer.maxParticleSize = 100.0f;
            }

            //If particle highlight is not disabled, play it
            if (tempParticles.isPlaying == false)
            {
                tempParticles.Clear();
                tempParticles.Play();
            }
        }

        void ApplyNewCustomizationParametersInParticles()
        {
            //This method will apply the new customization parameters in temp particles

            var cMain = tempParticles.main;
            cMain.startSize = ((sizeOnMinimap.x + (sizeOnMinimap.x * 2.5f)) * particlesSizeMultiplier) * MinimapDataGlobal.GetMinimapItemsSizeGlobalMultiplier();
            cMain.startColor = particlesColor;

            var cTextureSheetAnimation = tempParticles.textureSheetAnimation;
            cTextureSheetAnimation.enabled = true;
            cTextureSheetAnimation.mode = ParticleSystemAnimationMode.Sprites;
            if (particlesSprite == null)
                if (cTextureSheetAnimation.spriteCount > 0)
                    cTextureSheetAnimation.RemoveSprite(0);
            if (particlesSprite != null)
            {
                if (cTextureSheetAnimation.spriteCount > 0)
                    cTextureSheetAnimation.RemoveSprite(0);
                cTextureSheetAnimation.AddSprite(particlesSprite);
            }

            var cRenderer = tempParticlesRenderer;
            cRenderer.flip = new Vector3((flipInX == true) ? 1 : 0, (flipInY == true) ? 1 : 0, cRenderer.flip.z);
            if (orderInLayer > 0)
                cRenderer.sortingOrder = orderInLayer - 1;
            if (orderInLayer == 0)
                cRenderer.sortingOrder = 0;
        }

        void Update()
        {
            //If the Minimap Item created by this component is disabled, enable it
            if (tempSpriteObj.activeSelf == false)
                tempSpriteObj.SetActive(true);

            //If not have a sprite yet, cancel
            if (itemSprite == null)
                return;

            //Update the sprite if changed
            if (itemSprite != tempSprite.sprite)
            {
                tempSprite.sprite = itemSprite;
                lastRenderItemSize = new Vector3(-1, -1, -1);
            }
            //Updates the size of sprite if changed (converts the size of the sprite, so that every 1 in sizeOnMinimap equals 1 meter in the world.)
            if ((sizeOnMinimap.x * MinimapDataGlobal.GetMinimapItemsSizeGlobalMultiplier()) != lastRenderItemSize.x || (sizeOnMinimap.z * MinimapDataGlobal.GetMinimapItemsSizeGlobalMultiplier()) != lastRenderItemSize.z)
            {
                //Calculate the final size of the sprite in world, in meters, according the item size in vector 2
                float areaResolutionAspectInSpriteRendererX = (float)(sizeOnMinimap.x * MinimapDataGlobal.GetMinimapItemsSizeGlobalMultiplier()) / (float)itemSprite.texture.width;
                float areaResolutionAspectInSpriteRendererY = (float)(sizeOnMinimap.z * MinimapDataGlobal.GetMinimapItemsSizeGlobalMultiplier()) / (float)itemSprite.texture.height;
                tempSpriteObj.transform.localScale = new Vector3(areaResolutionAspectInSpriteRendererX * tempSprite.sprite.pixelsPerUnit, areaResolutionAspectInSpriteRendererY * tempSprite.sprite.pixelsPerUnit, 1.0f);
                //Update the radius of sphere collider of this item according of this size, for minimap renderer input events
                float bestSizeToUseAsRadius = (((sizeOnMinimap.x > sizeOnMinimap.z) ? sizeOnMinimap.x : sizeOnMinimap.z) / 2.0f) + 2.0f; //<-- Add 2.0f to increase a little, the hitbox
                float bestScaleOfSpriteRender = (tempSpriteObj.transform.localScale.x > tempSpriteObj.transform.localScale.y) ? tempSpriteObj.transform.localScale.x : tempSpriteObj.transform.localScale.y;
                tempSphereColliderForInputEvents.center = tempSpriteObj.transform.InverseTransformPoint(tempSprite.bounds.center);
                tempSphereColliderForInputEvents.radius = bestSizeToUseAsRadius * (bestSizeToUseAsRadius / (bestSizeToUseAsRadius * bestScaleOfSpriteRender));

                //Save the new last render item size defined
                lastRenderItemSize.x = sizeOnMinimap.x * MinimapDataGlobal.GetMinimapItemsSizeGlobalMultiplier();
                lastRenderItemSize.z = sizeOnMinimap.z * MinimapDataGlobal.GetMinimapItemsSizeGlobalMultiplier();
                ApplyNewCustomizationParametersInParticles();
            }

            //Update the color of sprite, if changed
            if (spriteColor != tempSprite.color)
                tempSprite.color = spriteColor;

            //Update the flip of sprite, if changed
            if (flipInX != tempSprite.flipX)
            {
                tempSprite.flipX = flipInX;
                ApplyNewCustomizationParametersInParticles();
            }
            if (flipInY != tempSprite.flipY)
            {
                tempSprite.flipY = flipInY;
                ApplyNewCustomizationParametersInParticles();
            }

            //Update the raycast target
            if (tempSphereColliderForInputEvents.enabled != raycastTarget)
                tempSphereColliderForInputEvents.enabled = raycastTarget;

            //Update the order in layer, if changed
            tempSprite.sortingOrder = orderInLayer;
            if (orderInLayer != lastOrderInLayerDefinedInParticles)
            {
                ApplyNewCustomizationParametersInParticles();
                lastOrderInLayerDefinedInParticles = orderInLayer;
            }

            //Reset the layer if is out of bound
            if (orderInLayer < 0)
                orderInLayer = 0;
            if (orderInLayer > 5)
                orderInLayer = 5;

            //Particles settings application
            if (alreadyUpdatedSettingsOfParticles == false || lastParticlesHighlightMode != particlesHighlightMode)
            {
                //Call the method to apply new particles highlight mode
                ApplyNewParticlesHighlightMode();
                ApplyNewCustomizationParametersInParticles();

                //Save the updates on cache
                lastParticlesHighlightMode = particlesHighlightMode;
                alreadyUpdatedSettingsOfParticles = true;
            }
            if (particlesColor != lastColorDefinedInParticles)
            {
                ApplyNewCustomizationParametersInParticles();
                lastColorDefinedInParticles = particlesColor;
            }
            if (particlesSprite != lastSpriteDefinedInParticles)
            {
                ApplyNewCustomizationParametersInParticles();
                lastSpriteDefinedInParticles = particlesSprite;
            }
            if (particlesSizeMultiplier != lastParticlesSizeMultiplierDefined)
            {
                ApplyNewCustomizationParametersInParticles();
                lastParticlesSizeMultiplierDefined = particlesSizeMultiplier;
            }
            //Reset the particles size multiplier if is out of bound
            if (particlesSizeMultiplier < 0.001f)
                particlesSizeMultiplier = 0.001f;
            if (particlesSizeMultiplier > 10.0f)
                particlesSizeMultiplier = 10.0f;
            //Update the execution of particles (without this, the particle system will play desordened, if this minimap item script is enabled or disabled)
            if (particlesHighlightMode == ParticlesHighlightMode.Disabled && tempParticles.isPlaying == true)
                tempParticles.Stop();
            if (particlesHighlightMode != ParticlesHighlightMode.Disabled && tempParticles.isPlaying == false)
                tempParticles.Play();

            //Validate sizeOnMinimap variable
            if (sizeOnMinimap.y != 0)
                sizeOnMinimap.y = 0;

            //Move the item to follow this gameobject
            tempSpriteObj.transform.position = Vector3.Lerp(tempSpriteObj.transform.position, new Vector3(this.gameObject.transform.position.x, BASE_HEIGHT_IN_3D_WORLD, this.gameObject.transform.position.z), movementsSmoothing * Time.deltaTime);
            //Rotate the item
            if (followRotationOf == FollowRotationOf.ThisGameObject)
                tempSpriteObj.transform.rotation = Quaternion.Lerp(tempSpriteObj.transform.rotation, Quaternion.Euler(90, this.gameObject.transform.rotation.eulerAngles.y, 0), movementsSmoothing * Time.deltaTime);
            if (followRotationOf == FollowRotationOf.CustomGameObject && customGameObjectToFollowRotation != null)
                tempSpriteObj.transform.rotation = Quaternion.Lerp(tempSpriteObj.transform.rotation, Quaternion.Euler(90, customGameObjectToFollowRotation.rotation.eulerAngles.y, 0), movementsSmoothing * Time.deltaTime);
        }

        //Public methods

        public bool isThisMinimapItemBeingVisibleByAnyMinimapCamera(bool takeIntoAccountTheParticlesHighlightToo)
        {
            //This method will return true if this Minimap Item sprite is being visualized by any minimap camera
            bool isVisible = false;
            if (tempSprite.isVisible == true)
                isVisible = true;
            if (takeIntoAccountTheParticlesHighlightToo == true && tempParticlesRenderer.isVisible == true)
                isVisible = true;
            return isVisible;
        }

        public SpriteRenderer GetGeneratedSpriteAtRunTime()
        {
            //Return the sprite generated at runtime by this component
            return tempSprite;
        }

        public ParticleSystem GetGeneratedParticleAtRunTime()
        {
            //Return the particle generated at runtime by this component
            return tempParticles;
        }

        public MinimapItem[] GetListOfAllMinimapItemsInThisScene()
        {
            //If is not playing, cancel
            if (Application.isPlaying == false)
            {
                Debug.LogError("It is only possible to obtain the list of Minimap Items in this scene, if the application is being executed.");
                return null;
            }

            //Return a list that contains reference to all of this component in this scene
            return minimapDataHolder.instancesOfMinimapItemInThisScene.ToArray();
        }
    }
}