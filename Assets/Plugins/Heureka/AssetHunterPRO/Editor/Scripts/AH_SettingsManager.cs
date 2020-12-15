using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace HeurekaGames.AssetHunterPRO
{
    public class AH_SettingsManager
    {
        private static readonly AH_SettingsManager instance = new AH_SettingsManager();

        #region singleton
        // Explicit static constructor to tell C# compiler
        // not to mark type as beforefieldinit
        static AH_SettingsManager()
        {
            instance.Init();
        }

        private AH_SettingsManager()
        {
        }

        public static AH_SettingsManager Instance
        {
            get
            {
                return instance;
            }
        }
        #endregion

        public delegate void IgnoreListUpdatedHandler();
        public event IgnoreListUpdatedHandler IgnoreListUpdatedEvent;

        #region Fields
        [SerializeField] private int ignoredListChosenIndex;

        private readonly static string ProjectPostFix = "." + Application.dataPath; // AssetDatabase.AssetPathToGUID(FileUtil.GetProjectRelativePath(Application.dataPath));

        private readonly static string PrefsAutoCreateLog = "AH.AutoCreateLog" + ProjectPostFix;
        private readonly static string PrefsAutoOpenLog = "AH.AutoOpenLog" + ProjectPostFix;
        private readonly static string PrefsHideButtonText = "AH.HideButtonText" + ProjectPostFix;
        private readonly static string PrefsIgnoreScriptFiles = "AH.IgnoreScriptfiles" + ProjectPostFix;
        private readonly static string PrefsIgnoredTypes = "AH.DefaultIgnoredTypes" + ProjectPostFix;
        private readonly static string PrefsIgnoredPathEndsWith = "AH.IgnoredPathEndsWith" + ProjectPostFix;
        private readonly static string PrefsIgnoredExtensions = "AH.IgnoredExtensions" + ProjectPostFix;
        private readonly static string PrefsIgnoredFiles = "AH.IgnoredFiles" + ProjectPostFix;
        private readonly static string PrefsIgnoredFolders = "AH.IgnoredFolders" + ProjectPostFix;
        private readonly static string PrefsUserPrefPath = "AH.UserPrefPath" + ProjectPostFix;
        private readonly static string PrefsBuildInfoPath = "AH.BuildInfoPath" + ProjectPostFix;

        internal readonly static bool InitialValueAutoCreateLog = true;
        internal readonly static bool InitialValueAutoOpenLog = false;
        internal readonly static bool InitialValueHideButtonText = false;
        internal readonly static bool InitialIgnoreScriptFiles = true;
        internal readonly static string InitialUserPrefPath = Application.dataPath + System.IO.Path.DirectorySeparatorChar + "AH_Prefs";
        internal readonly static string InitialBuildInfoPath = System.IO.Directory.GetParent(Application.dataPath).FullName + System.IO.Path.DirectorySeparatorChar + "SerializedBuildInfo";


        //Types to Ignore by default
#if UNITY_2017_3_OR_NEWER
        internal readonly static List<Type> InitialValueIgnoredTypes = new List<Type>() {
            typeof(UnityEditorInternal.AssemblyDefinitionAsset)
#if !AH_SCRIPT_ALLOW //DEFINED IN AH_PREPROCESSOR
            ,typeof(MonoScript)
#endif
        };
#else
        internal readonly static List<Type> InitialValueIgnoredTypes = new List<Type>() {
#if !AH_SCRIPT_ALLOW //DEFINED IN AH_PREPROCESSOR
            typeof(MonoScript)
#endif
        };
#endif

        //File extensions to Ignore by default
        internal readonly static List<string> InitialValueIgnoredExtensions = new List<string>() {
            ".dll",
            "."+AH_SerializationHelper.SettingsExtension,
            "."+AH_SerializationHelper.BuildInfoExtension
        };

        //List of strings which, if contained in asset path, is ignored (Editor, Resources, etc)
        internal readonly static List<string> InitialValueIgnoredPathEndsWith = new List<string>() {
            string.Format("{0}heureka", System.IO.Path.DirectorySeparatorChar),
            string.Format("{0}editor", System.IO.Path.DirectorySeparatorChar),
            string.Format("{0}plugins", System.IO.Path.DirectorySeparatorChar),
            string.Format("{0}gizmos", System.IO.Path.DirectorySeparatorChar),
            string.Format("{0}editor default resources", System.IO.Path.DirectorySeparatorChar)
        };

        internal readonly static List<string> InitialValueIgnoredFiles = new List<string>();
        internal readonly static List<string> InitialValueIgnoredFolders = new List<string>();

        [SerializeField] private AH_ExclusionTypeList ignoredListTypes;
        [SerializeField] private AH_IgnoreList ignoredListPathEndsWith;
        [SerializeField] private AH_IgnoreList ignoredListExtensions;
        [SerializeField] private AH_IgnoreList ignoredListFiles;
        [SerializeField] private AH_IgnoreList ignoredListFolders;
        #endregion

        #region Properties
        [SerializeField]
        public bool AutoCreateLog
        {
            get { return ((!EditorPrefs.HasKey(PrefsAutoCreateLog) && InitialValueAutoCreateLog) || AH_Utils.IntToBool(EditorPrefs.GetInt(PrefsAutoCreateLog))); }
            internal set { EditorPrefs.SetInt(PrefsAutoCreateLog, AH_Utils.BoolToInt(value)); }
        }

        [SerializeField]
        public bool AutoOpenLog
        {
            get { return ((!EditorPrefs.HasKey(PrefsAutoOpenLog) && InitialValueAutoOpenLog) || AH_Utils.IntToBool(EditorPrefs.GetInt(PrefsAutoOpenLog))); }
            internal set { EditorPrefs.SetInt(PrefsAutoOpenLog, AH_Utils.BoolToInt(value)); }
        }

        [SerializeField]
        public bool HideButtonText
        {
            get { return ((!EditorPrefs.HasKey(PrefsHideButtonText) && InitialValueHideButtonText) || AH_Utils.IntToBool(EditorPrefs.GetInt(PrefsHideButtonText))); }
            internal set { EditorPrefs.SetInt(PrefsHideButtonText, AH_Utils.BoolToInt(value)); }
        }

        [SerializeField]
        public bool IgnoreScriptFiles
        {
            get { return ((!EditorPrefs.HasKey(PrefsIgnoreScriptFiles) && InitialIgnoreScriptFiles) || AH_Utils.IntToBool(EditorPrefs.GetInt(PrefsIgnoreScriptFiles))); }
            internal set { EditorPrefs.SetInt(PrefsIgnoreScriptFiles, AH_Utils.BoolToInt(value)); }
        }

        [SerializeField]
        public string UserPreferencePath
        {
            get
            {
                if (EditorPrefs.HasKey(PrefsUserPrefPath))
                    return EditorPrefs.GetString(PrefsUserPrefPath);
                else
                    return InitialUserPrefPath;
            }
            internal set { EditorPrefs.SetString(PrefsUserPrefPath, value); }
        }

        [SerializeField]
        public string BuildInfoPath
        {
            get
            {
                if (EditorPrefs.HasKey(PrefsBuildInfoPath))
                    return EditorPrefs.GetString(PrefsBuildInfoPath);
                else
                    return InitialBuildInfoPath;
            }
            internal set { EditorPrefs.SetString(PrefsBuildInfoPath, value); }
        }

        public GUIContent[] GUIcontentignoredLists = new GUIContent[5]
     {
                new GUIContent("Endings"),
                new GUIContent("Types"),
                new GUIContent("Folders"),
                new GUIContent("Files"),
                new GUIContent("Extentions")
     };
        #endregion

        private void Init()
        {
            ignoredListPathEndsWith = new AH_IgnoreList(new IgnoredEventActionPathEndsWith(0, onIgnoreButtonDown), InitialValueIgnoredPathEndsWith, PrefsIgnoredPathEndsWith);
            ignoredListTypes = new AH_ExclusionTypeList(new IgnoredEventActionType(1, onIgnoreButtonDown), InitialValueIgnoredTypes, PrefsIgnoredTypes);
            ignoredListFolders = new AH_IgnoreList(new IgnoredEventActionFolder(2, onIgnoreButtonDown), InitialValueIgnoredFolders, PrefsIgnoredFolders);
            ignoredListFiles = new AH_IgnoreList(new IgnoredEventActionFile(3, onIgnoreButtonDown), InitialValueIgnoredFiles, PrefsIgnoredFiles);
            ignoredListExtensions = new AH_IgnoreList(new IgnoredEventActionExtension(4, onIgnoreButtonDown), InitialValueIgnoredExtensions, PrefsIgnoredExtensions);

            //Todo subscribing to these 5 times, means that we might refresh buildinfo 5 times when reseting...We might be able to batch that somehow
            ignoredListPathEndsWith.ListUpdatedEvent += OnListUpdatedEvent;
            ignoredListTypes.ListUpdatedEvent += OnListUpdatedEvent;
            ignoredListFolders.ListUpdatedEvent += OnListUpdatedEvent;
            ignoredListFiles.ListUpdatedEvent += OnListUpdatedEvent;
            ignoredListExtensions.ListUpdatedEvent += OnListUpdatedEvent;
        }

        private void OnListUpdatedEvent()
        {
            if (IgnoreListUpdatedEvent != null)
                IgnoreListUpdatedEvent();
        }

        internal void ResetAll()
        {
            ignoredListPathEndsWith.Reset();
            ignoredListTypes.Reset();
            ignoredListExtensions.Reset();
            ignoredListFiles.Reset();
            ignoredListFolders.Reset();

            AutoCreateLog = AH_SettingsManager.InitialValueAutoCreateLog;
            AutoOpenLog = AH_SettingsManager.InitialValueAutoOpenLog;
            HideButtonText = AH_SettingsManager.InitialValueHideButtonText;
            IgnoreScriptFiles = AH_SettingsManager.InitialIgnoreScriptFiles;
            UserPreferencePath = AH_SettingsManager.InitialUserPrefPath;
            BuildInfoPath = AH_SettingsManager.InitialBuildInfoPath;
        }

        internal void DrawIgnored()
        {
            EditorGUILayout.HelpBox("IGNORE ASSETS" + Environment.NewLine + "-Select asset in project view to ignore", MessageType.Info);

            EditorGUILayout.BeginHorizontal();
            ignoredListChosenIndex = GUILayout.Toolbar(ignoredListChosenIndex, GUIcontentignoredLists);
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            drawIgnoreButtons();

            switch (ignoredListChosenIndex)
            {
                case 0:
                    ignoredListPathEndsWith.OnGUI();
                    break;
                case 1:
                    ignoredListTypes.OnGUI();
                    break;
                case 2:
                    ignoredListFolders.OnGUI();
                    break;
                case 3:
                    ignoredListFiles.OnGUI();
                    break;
                case 4:
                    ignoredListExtensions.OnGUI();
                    break;
                default:
                    break;
            }
        }

        private void drawIgnoreButtons()
        {
            GUILayout.Space(12);
            ignoredListPathEndsWith.DrawIgnoreButton();
            ignoredListTypes.DrawIgnoreButton();
            ignoredListFolders.DrawIgnoreButton();
            ignoredListFiles.DrawIgnoreButton();
            ignoredListExtensions.DrawIgnoreButton();
            GUILayout.Space(4);
        }

        //Callback from Ignore button down
        void onIgnoreButtonDown(int exclusionIndex)
        {
            ignoredListChosenIndex = exclusionIndex;
        }

        //public List<Type> GetIgnoredTypes() { return ignoredListTypes.GetIgnored(); }
        public List<string> GetIgnoredPathEndsWith() { return ignoredListPathEndsWith.GetIgnored(); }
        public List<string> GetIgnoredFileExtentions() { return ignoredListExtensions.GetIgnored(); }
        public List<string> GetIgnoredFiles() { return ignoredListFiles.GetIgnored(); }
        public List<string> GetIgnoredFolders() { return ignoredListFolders.GetIgnored(); }

        private int drawSetting(string title, int value, int min, int max, string prefixAppend)
        {
            EditorGUILayout.PrefixLabel(title + prefixAppend);
            return EditorGUILayout.IntSlider(value, min, max);
        }

        internal void DrawSettings()
        {
            EditorGUILayout.HelpBox("File save locations", MessageType.None);

            UserPreferencePath = drawSettingsFolder("User prefs", UserPreferencePath, AH_SettingsManager.InitialUserPrefPath);
            BuildInfoPath = drawSettingsFolder("Build info", BuildInfoPath, AH_SettingsManager.InitialBuildInfoPath);
            EditorGUILayout.Space();

            EditorGUILayout.HelpBox("Settings", MessageType.None);
            AutoCreateLog = drawSetting("Auto create log when building", AutoCreateLog, AH_SettingsManager.InitialValueAutoCreateLog);
            AutoOpenLog = drawSetting("Auto open log location after building", AutoOpenLog, AH_SettingsManager.InitialValueAutoOpenLog);
            HideButtonText = drawSetting("Hide buttontexts", HideButtonText, AH_SettingsManager.InitialValueHideButtonText);

            EditorGUILayout.BeginHorizontal();
            EditorGUI.BeginChangeCheck();
            IgnoreScriptFiles = drawSetting("Ignore script files", IgnoreScriptFiles, AH_SettingsManager.InitialIgnoreScriptFiles);

            if (EditorGUI.EndChangeCheck())
            {
                //ADD OR REMOVE DEFINITION FOR PREPROCESSING
                AH_PreProcessor.AddDefineSymbols(AH_PreProcessor.DefineScriptAllow, !IgnoreScriptFiles);

                if (!IgnoreScriptFiles)
                    EditorUtility.DisplayDialog("Now detecting unused scripts", "This is an experimental feature, and it cannot promise with any certainty that script files marked as unused are indeed unused. Only works with scripts that are directly used in a scene - Use at your own risk", "Ok");
            }

            GUIContent content = new GUIContent("EXPERIMENTAL FEATURE!", EditorGUIUtility.IconContent("console.warnicon.sml").image, "Cant be 100% sure script files are usused, so you need to handle with care");
            //TODO PARTIAL CLASSES
            //INHERITANCE
            //AddComponent<Type>
            //Reflection
            //Interfaces

            EditorGUILayout.LabelField(content, EditorStyles.boldLabel);
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
        }

        private string drawSettingsFolder(string title, string path, string defaultVal)
        {
            string validPath = path;
            string newPath = "";

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Select", GUILayout.ExpandWidth(false)))
                newPath = EditorUtility.OpenFolderPanel("Select folder", path, "");

            if (newPath != "")
                validPath = newPath;

            GUIContent content = new GUIContent(title + ": " + AH_Utils.ShrinkPathMiddle(validPath, 44), title + " is saved at " + validPath);

            GUILayout.Label(content, (defaultVal != path) ? EditorStyles.boldLabel : EditorStyles.label);
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            return validPath;
        }

        private bool drawSetting(string title, bool value, bool defaultVal)
        {
            return EditorGUILayout.ToggleLeft(title, value, (defaultVal != value) ? EditorStyles.boldLabel : EditorStyles.label);
        }


        internal bool HasIgnoredFolder(string folderPath, string assetID)
        {
            bool IgnoredEnding = ignoredListPathEndsWith.ContainsElement(folderPath, assetID);
            bool folderIgnored = ignoredListFolders.ContainsElement(folderPath, assetID);

            return IgnoredEnding || folderIgnored;
        }

        internal void AddIgnoredFolder(string element)
        {
            ignoredListFolders.AddToignoredList(element);
        }
        internal void AddIgnoredAssetTypes(string element)
        {
            ignoredListTypes.AddToignoredList(element);
        }
        internal void AddIgnoredAssetGUIDs(string element)
        {
            ignoredListFiles.AddToignoredList(element);
        }

        internal bool HasIgnoredAsset(string relativePath, string assetID)
        {
            bool IgnoredType = ignoredListTypes.ContainsElement(relativePath, assetID);
            bool IgnoredFile = ignoredListFiles.ContainsElement(relativePath, assetID);
            bool IgnoredExtension = ignoredListExtensions.ContainsElement(relativePath, assetID);

            return IgnoredType || IgnoredFile || IgnoredExtension;
        }

        internal void SaveToFile()
        {
            var path = EditorUtility.SaveFilePanel(
               "Save current settings",
               AH_SerializationHelper.GetSettingFolder(),
               "AH_UserPrefs_" + Environment.UserName,
               AH_SerializationHelper.SettingsExtension);

            if (path.Length != 0)
                AH_SerializationHelper.SerializeAndSave(instance, path);

            AssetDatabase.Refresh();
        }

        internal void LoadFromFile()
        {
            var path = EditorUtility.OpenFilePanel(
                "settings",
                AH_SerializationHelper.GetSettingFolder(),
                AH_SerializationHelper.SettingsExtension
                );

            if (path.Length != 0)
            {
                AH_SerializationHelper.LoadSettings(instance, path);
                ignoredListTypes.Save();
                ignoredListPathEndsWith.Save();
                ignoredListTypes.Save();
                ignoredListExtensions.Save();
                ignoredListFolders.Save();
            }
        }
    }
}