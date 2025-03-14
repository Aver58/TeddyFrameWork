using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace MTAssets.EasyMinimapSystem
{
    /*
     This class is responsible for the functioning of the "Minimap Renderer Events" component, and all its functions.
    */
    /*
     * The Easy Minimap System was developed by Marcos Tomaz in 2019.
     * Need help? Contact me (mtassets@windsoft.xyz)
    */

    [AddComponentMenu("")] //Hide this script in component menu.
    public class MinimapRendererEvents : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerEnterHandler, IPointerExitHandler
    {
        //Private constants
        private const int LAYER_OF_MINIMAP_ITEMS_COLLIDERS = (1 << 5); //UI Layer

        //Private variables
        private Canvas thisParentCanvas;
        private RectTransform thisRectTransform;
        private RaycastHit temporaryRaycastHit;
        private bool isMouseOverTheMinimapRendererArea = false;
        private Vector3 startingWorldPositionOfOnPointerDownForCurrentOnDrag;

        //Public variables
        ///<summary>[WARNING] Do not change the value of this variable. This is a variable used for internal tool operations.</summary> 
        [HideInInspector]
        public MinimapRenderer minimapRenderer;

        // Default methods

        public void Start()
        {
            //Get the Canvas
            thisParentCanvas = this.gameObject.GetComponentInParent<Canvas>();
            //If not found canvas, send warning
            if (thisParentCanvas == null)
                Debug.LogError("The Canvas component could not be found. Please check if this Canvas has the Canvas component. Minimap Renderer Events will not work.");
            //If the canvas is not screen space overlay
            if (thisParentCanvas != null && thisParentCanvas.renderMode != RenderMode.ScreenSpaceOverlay)
                Debug.LogError("The Canvas component is not in the \"Screen Space - Overlay\" Render Mode. It will not be possible to run the Minimap Renderer Events.");

            //Fill the cache
            thisRectTransform = this.gameObject.GetComponent<RectTransform>();
        }

        //Core methods

        public virtual void OnPointerEnter(PointerEventData ped)
        {
            //If the canvas is null or not in ScreenSpace, cancel
            if (thisParentCanvas == null || thisParentCanvas.renderMode != RenderMode.ScreenSpaceOverlay)
                return;

            //Inform that pointer is in minimap renderer area
            isMouseOverTheMinimapRendererArea = true;
        }

        public virtual void OnPointerExit(PointerEventData ped)
        {
            //If the canvas is null or not in ScreenSpace, cancel
            if (thisParentCanvas == null || thisParentCanvas.renderMode != RenderMode.ScreenSpaceOverlay)
                return;

            //Inform that pointer is not in minimap renderer area
            isMouseOverTheMinimapRendererArea = false;
        }

        public void Update()
        {
            //If the canvas is null or not in ScreenSpace, cancel
            if (thisParentCanvas == null || thisParentCanvas.renderMode != RenderMode.ScreenSpaceOverlay)
                return;
            //If event is empty, return
            if (minimapRenderer.onInputOver == null)
                return;
            //If mouse is not over the minimap renderer area, report
            if (isMouseOverTheMinimapRendererArea == false)
            {
                minimapRenderer.onInputOver.Invoke(false, Vector3.zero, null);
                return;
            }

            //On Input Over
            Vector2 mouseCoordinatesInEventsArea = GetPositionOfMouseInEventsAreaAndConvertToCoordinatesOfEventsArea();
            Vector3 worldPositionOfMouse = TranslateCoordinatesOfEventsAreaToWorldPosition(mouseCoordinatesInEventsArea);
            MinimapItem minimapItemOfClick = TryToFindMinimapItemForInteractInThisWorldPosition(worldPositionOfMouse);

            //Call the event
            if (minimapRenderer.onInputOver != null)
                minimapRenderer.onInputOver.Invoke(true, worldPositionOfMouse, minimapItemOfClick);
        }

        public virtual void OnDrag(PointerEventData ped)
        {
            //If the canvas is null or not in ScreenSpace, cancel
            if (thisParentCanvas == null || thisParentCanvas.renderMode != RenderMode.ScreenSpaceOverlay)
                return;

            //On Drag
            Vector2 mouseCoordinatesInEventsArea = GetPositionOfMouseInEventsAreaAndConvertToCoordinatesOfEventsArea();
            Vector3 worldPositionOfMouse = TranslateCoordinatesOfEventsAreaToWorldPosition(mouseCoordinatesInEventsArea);

            //Call the event
            if (minimapRenderer.onInputDrag != null)
                minimapRenderer.onInputDrag.Invoke(startingWorldPositionOfOnPointerDownForCurrentOnDrag, worldPositionOfMouse);
        }

        public virtual void OnPointerDown(PointerEventData ped)
        {
            //If the canvas is null or not in ScreenSpace, cancel
            if (thisParentCanvas == null || thisParentCanvas.renderMode != RenderMode.ScreenSpaceOverlay)
                return;

            //On Pointer Down
            Vector2 mouseCoordinatesInEventsArea = GetPositionOfMouseInEventsAreaAndConvertToCoordinatesOfEventsArea();
            Vector3 worldPositionOfMouse = TranslateCoordinatesOfEventsAreaToWorldPosition(mouseCoordinatesInEventsArea);
            MinimapItem minimapItemOfClick = TryToFindMinimapItemForInteractInThisWorldPosition(worldPositionOfMouse);

            //Call the event
            if (minimapRenderer.onInputClick != null)
                minimapRenderer.onInputClick.Invoke(worldPositionOfMouse, minimapItemOfClick);

            //Save this starting position on cache, for future use of OnDrag
            startingWorldPositionOfOnPointerDownForCurrentOnDrag = worldPositionOfMouse;
        }

        //Tools methods

        private Vector2 GetPositionOfMouseInEventsAreaAndConvertToCoordinatesOfEventsArea()
        {
            //Convert mouse position in this Event Area to local position on rect transform of this minimap renderer
            Vector2 mousePosition = Vector2.zero;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(thisRectTransform, Input.mousePosition, null, out mousePosition);

            //Convert mousePositionOnLocalRectTransformPosition to coordinates of position in this Event Area
            Vector2 mouseCoordinates = Vector2.zero;
            //Prepare X coordinate
            if (mousePosition.x == 0.0f)
                mouseCoordinates.x = 0.5f;
            if (mousePosition.x < 0.0f)
            {
                float halfWidth = thisRectTransform.rect.width / 2.0f;
                float newX = mousePosition.x * -1.0f;
                mouseCoordinates.x = 0.5f - ((newX / halfWidth) * 0.5f);
            }
            if (mousePosition.x > 0.0f)
            {
                float halfWidth = thisRectTransform.rect.width / 2.0f;
                float newX = mousePosition.x + halfWidth;
                mouseCoordinates.x = (newX / (halfWidth * 2.0f));
            }
            //Prepare Y coordinate
            if (mousePosition.y == 0.0f)
                mouseCoordinates.y = 0.5f;
            if (mousePosition.y < 0.0f)
            {
                float halfHeight = thisRectTransform.rect.height / 2.0f;
                float newY = mousePosition.y * -1.0f;
                mouseCoordinates.y = 0.5f - ((newY / halfHeight) * 0.5f);
            }
            if (mousePosition.y > 0.0f)
            {
                float halfHeight = thisRectTransform.rect.height / 2.0f;
                float newY = mousePosition.y + halfHeight;
                mouseCoordinates.y = (newY / (halfHeight * 2.0f));
            }

            //Return the mouse coordinates
            return mouseCoordinates;
        }

        private Vector3 TranslateCoordinatesOfEventsAreaToWorldPosition(Vector2 coordinatesOfEventsArea)
        {
            //This method will translate coordinates of events area to the respective world position of Minimap
            Vector3 worldPosition = Vector3.zero;

            //Get the needed data for calcs
            Camera minimapCamera = minimapRenderer.minimapCameraToShow.GetGeneratedCameraAtRunTime();

            //Get world position
            worldPosition = minimapCamera.ViewportToWorldPoint(new Vector3(coordinatesOfEventsArea.x, coordinatesOfEventsArea.y, 0.0f), Camera.MonoOrStereoscopicEye.Mono);
            worldPosition.y = 0.0f;

            //Return the world position
            return worldPosition;
        }

        private MinimapItem TryToFindMinimapItemForInteractInThisWorldPosition(Vector3 worldPosition)
        {
            //This event will cast a raycast from world position to world, and try to find a Minimap Item of this interaction
            MinimapItem minimapItem = null;

            //Change origin of raycast to be above of the Minimap Camera, to hit in Minimap Items with big sizes of colliders
            Vector3 fixedWorldPosition = new Vector3(worldPosition.x, minimapRenderer.minimapCameraToShow.GetGeneratedCameraAtRunTime().transform.position.y + 900.0f, worldPosition.z);

            //Cast the ray
            if (Physics.Raycast(fixedWorldPosition, Vector3.down, out temporaryRaycastHit, 32.0f + 900.0f, LAYER_OF_MINIMAP_ITEMS_COLLIDERS) == true)
                if (temporaryRaycastHit.collider != null)
                    minimapItem = (MinimapItem)temporaryRaycastHit.collider.gameObject.GetComponent<ActivityMonitor>().responsibleScriptComponentForThis;

            //Return the response
            return minimapItem;
        }
    }
}