using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MTAssets.EasyMinimapSystem
{
    public class FowSaveSate : MonoBehaviour
    {
        public MinimapFog fogOfWar;

        public void Start()
        {
            if (fogOfWar.isFileOfSaveSateOfFogOfWarExistant() == true)
                fogOfWar.LoadStateOfFogOfWarAsync();
        }

        public void OnApplicationQuit()
        {
            fogOfWar.SaveStateOfFogOfWar();
        }

        public void OnStartLoadNotifier()
        {
            Debug.Log("Minimap Fog has started loading a Fog State.");
        }

        public void OnDoneLoadNotifier()
        {
            Debug.Log("Minimap Fog has successfully finished loading a Fog State!");
        }
    }
}