using System;
using UnityEditor;
using UnityEngine;

namespace HeurekaGames.AssetHunterPRO
{
    public class AH_EditorData : ScriptableObject
    {
        public delegate void EditorDataRefreshDelegate();
        public static event EditorDataRefreshDelegate OnEditorDataRefresh;

        private static AH_EditorData m_instance;
        public static AH_EditorData Instance
        {
            get
            {
                if (!m_instance)
                {
                    m_instance = loadData();
                }

                return m_instance;
            }
        }

        private static AH_EditorData loadData()
        {
            //LOGO ON WINDOW
            string[] configData = AssetDatabase.FindAssets("EditorData t:" + typeof(AH_EditorData).ToString(), null);
            if (configData.Length >= 1)
            {
                return AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(configData[0]), typeof(AH_EditorData)) as AH_EditorData;
            }

            Debug.LogError("Failed to find config data");
            return null;
        }

        internal void RefreshData()
        {
            if (OnEditorDataRefresh != null)
                OnEditorDataRefresh();
        }

        public UnityEditor.DefaultAsset Documentation;
        [SerializeField] public ConfigurableIcon WindowPaneIcon = new ConfigurableIcon();
        [SerializeField] public ConfigurableIcon WindowHeaderIcon = new ConfigurableIcon();
        [SerializeField] public ConfigurableIcon SceneIcon = new ConfigurableIcon();
        [SerializeField] public ConfigurableIcon Settings = new ConfigurableIcon();
        [SerializeField] public ConfigurableIcon LoadLogIcon = new ConfigurableIcon();
        [SerializeField] public ConfigurableIcon GenerateIcon = new ConfigurableIcon();
        [SerializeField] public ConfigurableIcon RefreshIcon = new ConfigurableIcon();
        [SerializeField] public ConfigurableIcon MergerIcon = new ConfigurableIcon();
        [SerializeField] public ConfigurableIcon HelpIcon = new ConfigurableIcon();
        [SerializeField] public ConfigurableIcon AchievementIcon = new ConfigurableIcon();
        [SerializeField] public ConfigurableIcon ReportIcon = new ConfigurableIcon();
        [SerializeField] public ConfigurableIcon DeleteIcon = new ConfigurableIcon();
    }
}