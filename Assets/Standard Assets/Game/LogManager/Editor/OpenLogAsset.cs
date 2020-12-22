using UnityEditor;
using UnityEngine;
using System.Reflection;
using System;
using System.Text.RegularExpressions;

public class OpenLogAsset
{
#if UNITY_EDITOR 

    [UnityEditor.Callbacks.OnOpenAssetAttribute(0)]
    static bool OnOpenAsset(int instanceID, int line)
    {
        string stackTrace = GetStackTrace();
        if (!string.IsNullOrEmpty(stackTrace) && (stackTrace.Contains("UnityEngine.Debug:Log") || stackTrace.Contains("UnityEngine.Debug:LogError")))
        {
            Match matches = Regex.Match(stackTrace, @"\(at (.+)\)", RegexOptions.IgnoreCase);
            string pathline = "";
            while(matches.Success)
            {
                pathline = matches.Groups[1].Value;
                if(!pathline.Contains("Debug.cs"))
                {
                    int splitIndex = pathline.LastIndexOf(":");
                    string path = pathline.Substring(0, splitIndex);
                    line = Convert.ToInt32(pathline.Substring(splitIndex + 1));
                    string fullPath = Application.dataPath.Substring(0, Application.dataPath.LastIndexOf("Assets"));
                    fullPath += path;
                    //Debug.LogError("fullPath = " + fullPath);
                    UnityEditorInternal.InternalEditorUtility.OpenFileAtLineExternal(fullPath.Replace('/','\\'),line);
                    break;
;                }
                matches = matches.NextMatch();
            }
            return true;
        }
        return false;
    }

    static string GetStackTrace()
    {
        Type consoleWindowType = typeof(EditorWindow).Assembly.GetType("UnityEditor.ConsoleWindow");
        FieldInfo fieldInfo = consoleWindowType.GetField("ms_ConsoleWindow", BindingFlags.Static | BindingFlags.NonPublic);
        object consoleWindowInstance = fieldInfo.GetValue(null);
        if(consoleWindowInstance != null)
        {
            if((object)EditorWindow.focusedWindow == consoleWindowInstance)
            {
                Type listViewStateType = typeof(EditorWindow).Assembly.GetType("UnityEditor.ListViewState");
                fieldInfo = consoleWindowType.GetField("m_ListView", BindingFlags.Instance | BindingFlags.NonPublic);
                object listView = fieldInfo.GetValue(consoleWindowInstance);

                fieldInfo = listViewStateType.GetField("row", BindingFlags.Instance | BindingFlags.Public);
                int row = (int)fieldInfo.GetValue(listView);

                fieldInfo = consoleWindowType.GetField("m_ActiveText", BindingFlags.Instance | BindingFlags.NonPublic);
                string activeText = fieldInfo.GetValue(consoleWindowInstance).ToString();
                return activeText;
            }
        }
        return null;
    }

#endif
}
