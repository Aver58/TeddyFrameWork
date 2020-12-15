using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

namespace HeurekaGames.AssetHunterPRO
{
    public class AH_BuildInfoMergerWindow : EditorWindow
    {
        private static AH_BuildInfoMergerWindow m_window;
        private string buildInfoFolder;

        private Vector2 scrollPos;
        private List<BuildInfoSelection> buildInfoFiles;

        [MenuItem("Window/Heureka/Asset Hunter PRO/Open merge tool")]
        public static void Init()
        {
            m_window = GetWindow<AH_BuildInfoMergerWindow>("AH Merger", true, typeof(AH_Window));
            m_window.titleContent.image = AH_EditorData.Instance.MergerIcon.Icon;

            m_window.buildInfoFolder = AH_SerializationHelper.GetBuildInfoFolder();
            m_window.updateBuildInfoFiles();
        }

        private void updateBuildInfoFiles()
        {
            buildInfoFiles = new List<BuildInfoSelection>();

            System.IO.DirectoryInfo directoryInfo = new System.IO.DirectoryInfo(buildInfoFolder);
            foreach (var item in directoryInfo.GetFiles("*." + AH_SerializationHelper.BuildInfoExtension).OrderByDescending(val=>val.LastWriteTime))
            {
                buildInfoFiles.Add(new BuildInfoSelection(item));
            } 
        }

        private void OnGUI()
        {
            if (!m_window)
                Init();

            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
            Heureka_WindowStyler.DrawGlobalHeader(m_window, AH_EditorData.Instance.WindowHeaderIcon.Icon, Heureka_WindowStyler.clr_Dark, "BUILD INFO MERGER");
            EditorGUILayout.Space();

            EditorGUILayout.HelpBox("Select a folder that contains buildinfo files", MessageType.Info);

            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("Change", GUILayout.ExpandWidth(false)))
            {
                buildInfoFolder = EditorUtility.OpenFolderPanel("Buildinfo folder", buildInfoFolder, "");
                updateBuildInfoFiles();
            }
            EditorGUILayout.LabelField("Current folder: " + buildInfoFolder);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();

            //Show all used types
            EditorGUILayout.BeginVertical();

            foreach (var item in buildInfoFiles)
            {
                item.Selected = EditorGUILayout.ToggleLeft(item.BuildInfoFile.Name, item.Selected);
            }

            EditorGUILayout.Space();
            EditorGUI.BeginDisabledGroup(buildInfoFiles.Count(val=>val.Selected==true) < 2);
            if (GUILayout.Button("Merge Selected", GUILayout.ExpandWidth(false)))
            {
                AH_SerializedBuildInfo merged = new AH_SerializedBuildInfo();
                foreach (var item in buildInfoFiles.FindAll(val=>val.Selected))
                {
                    merged.MergeWith(item.BuildInfoFile.FullName);
                }
                merged.SaveAfterMerge();

                EditorUtility.DisplayDialog("Merge completed", "A new buildinfo was created by combined existing buildinfos", "Ok");
                //Reset
                buildInfoFiles.ForEach(val => val.Selected = false);
                updateBuildInfoFiles();
            }
            EditorGUI.EndDisabledGroup();
            //Make sure this window has focus to update contents
            Repaint();

            EditorGUILayout.EndVertical();
            EditorGUILayout.EndScrollView();
        }

        [System.Serializable]
        private class BuildInfoSelection
        {
            public System.IO.FileInfo BuildInfoFile;
            public bool Selected = false;

            public BuildInfoSelection(System.IO.FileInfo buildInfoFile)
            {
                this.BuildInfoFile = buildInfoFile;
            }
        }
    }
}