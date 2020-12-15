using System;
using UnityEditor;
using UnityEngine;

namespace HeurekaGames.AssetHunterPRO
{
    [InitializeOnLoad]
    public class AH_VersionUpgrader
    {
#pragma warning disable 0414    // suppress value not used warning
        static readonly string prefKey = "AH_DONT_IMPORT_OLD_SETTINGS";
#pragma warning restore 0414    // restore value not used warning

        public static bool VersionUpgraderReady = false;

        static AH_VersionUpgrader()
        {
            runUpgradeTest();
        }

        private static void runUpgradeTest()
        {
            //Check if we have old asset hunter installed already
            VersionUpgraderReady = getOldSettings().Length > 0;
            AH_PreProcessor.AddDefineSymbols(AH_PreProcessor.DefineHasOldVersion, VersionUpgraderReady);

            //Make sure we haven't chosen NOT to import
            if (EditorPrefs.HasKey(prefKey))
            {
                if (AH_Utils.IntToBool(EditorPrefs.GetInt(prefKey)) == true)
                {
                    return;
                }
            }

            if(VersionUpgraderReady)
                EditorUtility.DisplayDialog("Old Asset Hunter settings found", "To transfer old settings open 'Window->Asset Hunter Pro->Transfer Settings'", "Ok");

            EditorPrefs.SetInt(prefKey, 1);
        }

        private static string[] getOldSettings()
        {
            return AssetDatabase.FindAssets("t: AssetHunterSettings");
        }

#if AH_HAS_OLD_INSTALLED
        [UnityEditor.MenuItem("Window/Heureka/Asset Hunter PRO/Transfer Old Settings", priority = AH_Window.WINDOWMENUITEMPRIO)]
        public static void OpenAssetHunter()
        {
            RunUpgrade();
        }

        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            if (deletedAssets != null && deletedAssets.Length > 0)
                runUpgradeTest();
        }

        public static void RunUpgrade()
        {
            if (EditorUtility.DisplayDialog("Import old Asset Hunter Settings", "Do you wish to import old Asset Hunter settings into PRO?", "Ok", "Cancel"))
            {
                //Check if we have old asset hunter installed with
                string[] oldSettingsGUIDS = getOldSettings();

                AssetHunter.AssetHunterSettings oldSettingData = AssetDatabase.LoadAssetAtPath<AssetHunter.AssetHunterSettings>(AssetDatabase.GUIDToAssetPath(oldSettingsGUIDS[0]));
                foreach (var item in (oldSettingData.m_AssetGUIDExcludes))
                {
                    if (item == null)
                        continue;

                    Debug.Log("AH: Exporting old asset excludes: " + item);
                    AH_SettingsManager.Instance.AddIgnoredAssetGUIDs(item);
                }
                foreach (var item in (oldSettingData.m_AssetTypeExcludes))
                {
                    if (item == null)
                        continue;

                    Debug.Log("AH: Exporting old type excludes: " + item.Name);
                    AH_SettingsManager.Instance.AddIgnoredAssetTypes(item.AssemblyQualifiedName);
                }
                foreach (var item in (oldSettingData.m_DirectoryExcludes))
                {
                    if (item == null)
                        continue;

                    string path = AssetDatabase.GetAssetPath(item);
                    Debug.Log("AH: Exporting old folder excludes: " + item.name + " - ID: " + AssetDatabase.AssetPathToGUID(path));

                    AH_SettingsManager.Instance.AddIgnoredFolder(AssetDatabase.AssetPathToGUID(path));
                }
                foreach (var item in (oldSettingData.m_AssetSubstringExcludes))
                {
                    if (item == null)
                        continue;

                    Debug.LogWarning("AH: Failed to export SUBSTRING exclude as the feature changed in Asset Hunter PRO: " + item);
                }

                //We set prefs so we dont import next time
                EditorPrefs.SetInt(prefKey, 1);

                //Remove define symbol
                onTransferComplete();

            }
            else //We choose NOT to import
            {
                EditorPrefs.SetInt(prefKey, 1);
            }
        }

        private static void onTransferComplete()
        {
            AH_PreProcessor.AddDefineSymbols(AH_PreProcessor.DefineHasOldVersion, false);

            string folderPath = AssetDatabase.GUIDToAssetPath("0e2e4a3c5c6237b448de7afa87377b2e"); //The original GUID of the old Asset Hunter folder
            UnityEngine.Object oldFolder = AssetDatabase.LoadMainAssetAtPath(folderPath);

            if (oldFolder == null)
            {
                EditorUtility.DisplayDialog("Delete old asset hunter", "Copy complete,feel free to delete old Asset Hunter folder", "Ok");
            }
            else
            {
                if (EditorUtility.DisplayDialog("Delete old asset hunter", "Copy complete, do you want to delete " + folderPath + " immediately?", "Ok", "Cancel"))
                {
                    FileUtil.DeleteFileOrDirectory(folderPath);
                    AssetDatabase.Refresh();
                    VersionUpgraderReady = false;
                }
            }
        }
#endif
    }
}