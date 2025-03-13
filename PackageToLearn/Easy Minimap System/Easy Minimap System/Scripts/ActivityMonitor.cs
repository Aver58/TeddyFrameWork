using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MTAssets.EasyMinimapSystem
{
    /*
     This class is responsible for the functioning of the "Activity Monitor" component, and all its functions.
    */
    /*
     * The Easy Minimap System was developed by Marcos Tomaz in 2019.
     * Need help? Contact me (mtassets@windsoft.xyz)
    */

    [AddComponentMenu("")] //Hide this script in component menu.
    public class ActivityMonitor : MonoBehaviour
    {
        //Script responsible for disabling Minimap Items if parent GameObject is disabled.

        //Public variables
        ///<summary>[WARNING] Do not change the value of this variable. This is a variable used for internal tool operations.</summary> 
        [HideInInspector]
        public MonoBehaviour responsibleScriptComponentForThis;

        //Core methods

        public void LateUpdate()
        {
            //If the script (component) responsible for this not exists
            if (responsibleScriptComponentForThis == null)
            {
                this.gameObject.SetActive(false);
                return;
            }

            //If the script responsible for this is deactived, disable this gameobject too
            if (responsibleScriptComponentForThis.enabled == false || responsibleScriptComponentForThis.gameObject.activeInHierarchy == false)
                this.gameObject.SetActive(false);
        }
    }
}