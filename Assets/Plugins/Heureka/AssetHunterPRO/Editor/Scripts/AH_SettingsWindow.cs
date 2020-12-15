using UnityEditor;
using UnityEngine;

namespace HeurekaGames.AssetHunterPRO
{
    public class AH_SettingsWindow : EditorWindow
    {
        private const string WINDOWNAME = "AH Settings";
        private Vector2 scrollPos;
        private static AH_SettingsWindow m_window;

        [UnityEditor.MenuItem("Window/Heureka/Asset Hunter PRO/Open settings")]
        public static void OpenAssetHunter()
        {
            Init(false);
        }

        public static void Init(bool attemptDock)
        {
            bool firstInit = (m_window == null);

            m_window = AH_SettingsWindow.GetWindow<AH_SettingsWindow>(WINDOWNAME, true/*, typeof(AH_Window)*/);
            m_window.titleContent.image = AH_EditorData.Instance.Settings.Icon;

            AH_Window[] mainWindows = Resources.FindObjectsOfTypeAll<AH_Window>();
            if (attemptDock && mainWindows.Length != 0 && firstInit)
            {
                HeurekaGames.Docker.Dock(mainWindows[0], m_window, Docker.DockPosition.Right);
            }
        }

        void OnInspectorUpdate()
        {
            Repaint();
        }

        void OnGUI()
        {
            if (!m_window)
                Init(true);

            Heureka_WindowStyler.DrawGlobalHeader(m_window, AH_EditorData.Instance.WindowHeaderIcon.Icon, Heureka_WindowStyler.clr_dBlue, "SETTINGS");

            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("Reset Settings"))
            {
                if (EditorUtility.DisplayDialog("Reset Settings", "Are you sure you want to reset Settings completely", "OK", "CANCEL"))
                {
                    AH_SettingsManager.Instance.ResetAll();
                }
            }
            if (GUILayout.Button("Save prefs to file"))
                AH_SettingsManager.Instance.SaveToFile();
            if (GUILayout.Button("Load prefs from file"))
                AH_SettingsManager.Instance.LoadFromFile();

            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();

            AH_SettingsManager.Instance.DrawSettings();

            EditorGUILayout.Space();

            AH_SettingsManager.Instance.DrawIgnored();

            EditorGUILayout.EndScrollView();
        }
    }
}