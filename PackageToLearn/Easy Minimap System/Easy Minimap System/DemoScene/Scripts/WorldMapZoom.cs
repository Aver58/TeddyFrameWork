using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MTAssets.EasyMinimapSystem
{
    public class WorldMapZoom : MonoBehaviour
    {
        private float defaultZoomMultiplier = 0.0f;
        private float defaultFieldOfView = 0.0f;

        public MinimapCamera minimapCamera;
        public Slider zoomSlider;
        public float maxZoomPossible = 75.0f;
        public float minItensMultiplierPossible = 0.25f;

        void Start()
        {
            //Get default param
            defaultZoomMultiplier = MinimapDataGlobal.GetMinimapItemsSizeGlobalMultiplier();
            defaultFieldOfView = minimapCamera.fieldOfView;
        }

        void Update()
        {
            //Calculate zoom and apply
            MinimapDataGlobal.SetMinimapItemsSizeGlobalMultiplier((defaultZoomMultiplier - (minItensMultiplierPossible * zoomSlider.value)));
            minimapCamera.fieldOfView = defaultFieldOfView - (maxZoomPossible * zoomSlider.value);
        }
    }
}