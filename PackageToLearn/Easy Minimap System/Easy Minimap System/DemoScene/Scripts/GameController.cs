using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MTAssets.EasyMinimapSystem
{
    public class GameController : MonoBehaviour
    {
        //Private cache
        private float beforeFullscreenGlobalSizeMultiplier = -1;

        //Private cache to minimap renderer events
        private int clicksToCreateMarkInMinimap = 0;
        private Vector3 lastWorldPosClickInMinimap = Vector3.zero;
        private Dictionary<MinimapItem, Vector3> allMinimapItemsAndOriginalSizes = new Dictionary<MinimapItem, Vector3>();

        //Public variables
        public GameObject fullScreenMapObj;
        public PlayerScript player;
        public MinimapCamera playerMinimapCamera;
        public MinimapItem marker;
        public MinimapItem cursor;
        public MinimapItem playerFieldOfView;

        //On update

        void Update()
        {
            //On press M
            if (Input.GetKeyDown(KeyCode.M) == true && fullScreenMapObj.activeSelf == false)
                OpenFullscreenMap();
            if (Input.GetKeyDown(KeyCode.Escape) == true)
                if (fullScreenMapObj.activeSelf == true)
                    CloseFullscreenMap();
        }

        //Button methods

        public void OpenFullscreenMap()
        {
            fullScreenMapObj.SetActive(true);
            playerMinimapCamera.gameObject.SetActive(true);
            playerFieldOfView.enabled = false;
            Cursor.lockState = CursorLockMode.None;
            beforeFullscreenGlobalSizeMultiplier = MinimapDataGlobal.GetMinimapItemsSizeGlobalMultiplier();
            MinimapDataGlobal.SetMinimapItemsSizeGlobalMultiplier(1.5f);
            player.canHideCursor = false;
            if (cursor != null)
                cursor.gameObject.SetActive(true);
        }

        public void CloseFullscreenMap()
        {
            fullScreenMapObj.SetActive(false);
            playerMinimapCamera.gameObject.SetActive(false);
            playerFieldOfView.enabled = true;
            Cursor.lockState = CursorLockMode.Locked;
            MinimapDataGlobal.SetMinimapItemsSizeGlobalMultiplier(beforeFullscreenGlobalSizeMultiplier);
            player.canHideCursor = true;
            if (cursor != null)
                cursor.gameObject.SetActive(false);
        }

        public void OnClickInMinimapRendererArea(Vector3 clickWorldPos, MinimapItem clickedMinimapItem)
        {
            //If is the first click on map, start the routine of double click to mark
            if (clicksToCreateMarkInMinimap == 0)
                StartCoroutine(OnClickInMinimapRendererArea_DoubleClickRoutine());
            //Increase the counter of clicks
            clicksToCreateMarkInMinimap += 1;
            //Store the last click data
            lastWorldPosClickInMinimap = clickWorldPos;
            //Show the Minimap Item Clicked
            if (clickedMinimapItem != null)
                Debug.Log("You clicked on Minimap Item \"" + clickedMinimapItem.gameObject.name + "\".");
        }

        IEnumerator OnClickInMinimapRendererArea_DoubleClickRoutine()
        {
            int milisecondsPassed = 0;

            while (enabled)
            {
                if (milisecondsPassed >= 25) //<-- if is passed 25ms, reset the counter of clicks and break the loop
                {
                    clicksToCreateMarkInMinimap = 0;
                    break;
                }

                if (clicksToCreateMarkInMinimap >= 2)
                {
                    marker.gameObject.SetActive(true);
                    marker.transform.position = lastWorldPosClickInMinimap;
                    clicksToCreateMarkInMinimap = 0;
                    break;
                }

                yield return new WaitForSecondsRealtime(0.001f); //<-- 0.001 is 1ms
                milisecondsPassed += 1;
            }
        }

        public void OnDragInMinimapRendererArea(Vector3 onStartThisDragWorldPos, Vector3 onDraggingWorldPos)
        {
            //Use the position of drag start and current position of drag to move the Minimap Camera of fullscreen minimap
            Vector3 deltaPositionToMoveMap = (onDraggingWorldPos - onStartThisDragWorldPos) * -1.0f;
            playerMinimapCamera.transform.position += (deltaPositionToMoveMap * 10.0f * Time.deltaTime);
        }

        public void OnOverInMinimapRendererArea(bool isOverMinimapRendererArea, Vector3 mouseWorldPos, MinimapItem overMinimapItem)
        {
            //Hide the cursor
            if (isOverMinimapRendererArea == false)
                cursor.gameObject.SetActive(false);
            //Show the cursor and run logic of on mouse over
            if (isOverMinimapRendererArea == true)
            {
                //Show cursor
                cursor.gameObject.SetActive(true); //<- "Raycast Target" of this Minimap Item, is off

                //Move the cursor
                cursor.gameObject.transform.position = mouseWorldPos;

                //Reset all original sizes
                foreach (var key in allMinimapItemsAndOriginalSizes)
                {
                    if (overMinimapItem != null && key.Key == overMinimapItem)
                        continue;

                    key.Key.sizeOnMinimap = key.Value;
                }

                //Get all minimap items
                MinimapItem[] allMinimapItems = cursor.GetListOfAllMinimapItemsInThisScene();
                //Fill the dictionary of all minimap items
                for (int i = 0; i < allMinimapItems.Length; i++)
                {
                    //Get the minimap item
                    MinimapItem item = allMinimapItems[i];
                    //If is null, skip
                    if (item == null)
                        continue;
                    //Fill the dictionary
                    if (allMinimapItemsAndOriginalSizes.ContainsKey(item) == false)
                        allMinimapItemsAndOriginalSizes.Add(item, item.sizeOnMinimap);
                }

                //Increase size of the selected item (avoid increase size of same minimap item various times)
                if (overMinimapItem != null && overMinimapItem.sizeOnMinimap != (allMinimapItemsAndOriginalSizes[overMinimapItem] * 3.0f))
                    overMinimapItem.sizeOnMinimap = overMinimapItem.sizeOnMinimap * 3.0f;
            }
        }
    }
}