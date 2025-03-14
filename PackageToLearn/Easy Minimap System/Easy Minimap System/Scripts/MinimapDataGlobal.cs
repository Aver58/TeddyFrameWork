using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MTAssets.EasyMinimapSystem
{
    /*
     This class is responsible for global management of all Easy Minimap System components
    */
    /*
     * The Easy Minimap System was developed by Marcos Tomaz in 2019.
     * Need help? Contact me (mtassets@windsoft.xyz)
    */

    [AddComponentMenu("")] //Hide this script in component menu.
    public class MinimapDataGlobal : MonoBehaviour
    {
        //Private static variables
        private static float minimapItemsSizeMultiplier = 1.0f;

        //Public and static methods

        public static void SetMinimapItemsSizeGlobalMultiplier(float multiplier)
        {
            //Set a new value to minimapItemsSizeMultiplier
            minimapItemsSizeMultiplier = multiplier;
        }

        public static float GetMinimapItemsSizeGlobalMultiplier()
        {
            //Return the minimapItemsSizeMultiplier
            return minimapItemsSizeMultiplier;
        }
    }
}