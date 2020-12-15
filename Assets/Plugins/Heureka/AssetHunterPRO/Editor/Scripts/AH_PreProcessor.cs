using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class AH_PreProcessor : MonoBehaviour
{
    public const string DefineScriptAllow = "AH_SCRIPT_ALLOW";
    public const string DefineHasOldVersion = "AH_HAS_OLD_INSTALLED";

    /// <summary>
    /// Add define symbols as soon as Unity gets done compiling.
    /// </summary>
    public static void AddDefineSymbols(string symbol, bool addDefine)
    {

        string definesString = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);

        List<string> allDefines = definesString.Split(';').ToList();

        bool updateDefines = false;
        if (addDefine && !allDefines.Contains(symbol))
        {
            allDefines.Add(symbol);
            updateDefines = true;
        }
        else if (!addDefine && allDefines.Contains(symbol))
        {
            allDefines.Remove(symbol);
            updateDefines = true;
        }

        if (updateDefines)
            PlayerSettings.SetScriptingDefineSymbolsForGroup(
                EditorUserBuildSettings.selectedBuildTargetGroup,
                string.Join(";", allDefines.ToArray()));
    }
}
