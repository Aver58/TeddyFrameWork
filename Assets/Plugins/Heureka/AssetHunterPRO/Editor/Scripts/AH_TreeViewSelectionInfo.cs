using System;
using System.Collections;
using System.Collections.Generic;
using HeurekaGames.AssetHunterPRO.BaseTreeviewImpl.AssetTreeView;
using UnityEngine;
using UnityEditor;
using System.Linq;
using HeurekaGames.AssetHunterPRO.BaseTreeviewImpl;

namespace HeurekaGames.AssetHunterPRO
{
    [System.Serializable]
    public class AH_TreeViewSelectionInfo
    {
        public delegate void AssetDeletedHandler();
        public static event AssetDeletedHandler OnAssetDeleted;

        private bool hasSelection;
        public bool HasSelection
        {
            get
            {
                return hasSelection;
            }
        }

        public const float Height = 64;

        private AH_MultiColumnHeader multiColumnHeader;
        private List<AH_TreeviewElement> selection;

        internal void Reset()
        {
            selection = null;
            hasSelection = false;
        }

        internal void SetSelection(AH_TreeViewWithTreeModel treeview, IList<int> selectedIds)
        {
            multiColumnHeader = (AH_MultiColumnHeader)(treeview.multiColumnHeader);
            selection = new List<AH_TreeviewElement>();

            foreach (var itemID in selectedIds)
            {
                selection.Add(treeview.treeModel.Find(itemID));
            }

            hasSelection = (selection.Count > 0);

            //If we have more, select the assets in project view
            if (hasSelection)
            {
                if (selection.Count > 1)
                {
                    UnityEngine.Object[] selectedObjects = new UnityEngine.Object[selection.Count];
                    for (int i = 0; i < selection.Count; i++)
                    {
                        selectedObjects[i] = AssetDatabase.LoadMainAssetAtPath(selection[i].RelativePath);
                    }
                    Selection.objects = selectedObjects;
                }
                else
                    Selection.activeObject = AssetDatabase.LoadMainAssetAtPath(selection[0].RelativePath);

                AH_Utils.PingObjectAtPath(selection[selection.Count - 1].RelativePath, false);
            }
        }

        internal void OnGUISelectionInfo(Rect selectionRect)
        {
            GUILayout.BeginArea(selectionRect);
            //TODO MAKE SURE WE DONT DO ALL OF THIS EACH FRAME, BUT CACHE THE SELECTION DATA

            using (new EditorGUILayout.HorizontalScope())
            {
                if (selection.Count == 1)
                {
                    drawSingle();
                }
                else
                {
                    drawMulti();
                }
            }
            GUILayout.EndArea();
        }

        private void drawSingle()
        {

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.BeginVertical();
            drawAssetPreview(true);
            EditorGUILayout.EndVertical();

            //Draw info from single asset
            EditorGUILayout.BeginVertical();

            GUILayout.Label(selection[0].RelativePath);
            if (!selection[0].IsFolder)
            {
                GUILayout.Label("(" + selection[0].AssetType + ")");
            }

            EditorGUILayout.EndVertical();
            GUILayout.FlexibleSpace();
            if (selection[0].IsFolder)
                DrawDeleteFolderButton(selection[0]);
            else
                drawDeleteAssetsButton();


            EditorGUILayout.EndHorizontal();
        }

        private void drawMulti()
        {
            //Make sure we have not selected folders
            bool allFiles = !selection.Any(val => val.IsFolder);
            var allSameType = selection.All(var => var.AssetType == selection[0].AssetType);
            drawAssetPreview(allSameType);

            EditorGUILayout.BeginHorizontal();
            //Draw info from multiple
            EditorGUILayout.BeginVertical();

            //Identical files
            if (allSameType && allFiles)
            {
                GUILayout.Label(selection[0].AssetType.ToString() + " (" + selection.Count() + ")");
            }
            //all folders
            else if (allSameType)
            {
                GUILayout.Label("Folders (" + selection.Count() + ")");
            }
            //Non identical selection
            else
            {
                GUILayout.Label("Items (" + selection.Count() + ")");
            }

            EditorGUILayout.EndVertical();

            if (allFiles)
                drawDeleteAssetsButton();
            EditorGUILayout.EndHorizontal();
        }

        private void drawDeleteAssetsButton()
        {
            if (multiColumnHeader.ShowMode != AH_MultiColumnHeader.AssetShowMode.Unused)
                return;

            long combinedSize = 0;
            foreach (var item in selection)
            {
                combinedSize += item.FileSize;
            }
            if (GUILayout.Button("Delete " + (AH_Utils.GetSizeAsString(combinedSize)), GUILayout.Width(160), GUILayout.Height(32)))
                deleteUnusedAssets();
        }

        private void DrawDeleteFolderButton(AH_TreeviewElement folder)
        {
            if (multiColumnHeader.ShowMode != AH_MultiColumnHeader.AssetShowMode.Unused)
                return;

            string description = "Delete unused assets from folder";
            GUIContent content = new GUIContent("Delete " + (AH_Utils.GetSizeAsString(folder.GetFileSizeRecursively(AH_MultiColumnHeader.AssetShowMode.Unused))), description);
            GUIStyle style = new GUIStyle(GUI.skin.button);
            DrawDeleteFolderButton(content, folder, style, description, "Do you want to delete all unused assets from:" + Environment.NewLine + folder.RelativePath, GUILayout.Width(160), GUILayout.Height(32));
        }

        public void DrawDeleteFolderButton(GUIContent content, AH_TreeviewElement folder, GUIStyle style, string dialogHeader, string dialogDescription, params GUILayoutOption[] layout)
        {
            if (GUILayout.Button(content, style, layout))
                deleteUnusedFromFolder(dialogHeader, dialogDescription, folder);
        }

        private void drawAssetPreview(bool bDraw)
        {
            GUIContent content = new GUIContent();

            //Draw asset preview
            if (bDraw && !selection[0].IsFolder)
            {
                var preview = AssetPreview.GetAssetPreview(AssetDatabase.LoadMainAssetAtPath(selection[0].RelativePath));
                content = new GUIContent(preview);
            }
            //Draw Folder icon
            else if (bDraw)
                content = EditorGUIUtility.IconContent("Folder Icon");

            GUILayout.Label(content,GUILayout.Width(Height), GUILayout.Height(Height));
        }

        private void deleteUnusedAssets()
        {
            int choice = EditorUtility.DisplayDialogComplex("Delete unused assets", "Do you want to delete the selected assets", "Yes", "Cancel", "Backup");
            List<string> affectedAssets = new List<string>();

            if (choice == 0)//Delete
            {
                foreach (var item in selection)
                {
                    affectedAssets.Add(item.RelativePath);
                }
                deleteMultipleAssets(affectedAssets);
            }
            else if (choice == 2)//Backup
            {
                foreach (var item in selection)
                {
                    affectedAssets.Add(item.RelativePath);
                    exportAssetsToPackage("Backup as unitypackage", affectedAssets);
                }
            }
        }


        private void deleteUnusedFromFolder(AH_TreeviewElement folder)
        {
            deleteUnusedFromFolder("Delete unused assets from folder", "Do you want to delete all unused assets from:" + Environment.NewLine + folder.RelativePath, folder);
        }

        private void deleteUnusedFromFolder(string header, string description, AH_TreeviewElement folder)
        {
            int choice = EditorUtility.DisplayDialogComplex(header, description, "Yes", "Cancel", "Backup (Slow)");

            List<string> affectedAssets = new List<string>();
            if (choice != 1)//Not Cancel
            {
                //Collect affected assets
                affectedAssets = folder.GetUnusedPathsRecursively();
            }
            if (choice == 0)//Delete
            {
                deleteMultipleAssets(affectedAssets);
            }
            else if (choice == 2)//Backup
            {
                exportAssetsToPackage("Backup as unitypackage", affectedAssets);
            }
        }

        private void exportAssetsToPackage(string header, List<string> affectedAssets)
        {
            string filename = Environment.UserName + "_Backup_" + "_" + AH_SerializationHelper.GetDateString();
            string savePath = EditorUtility.SaveFilePanel(
            header,
            AH_SerializationHelper.GetBackupFolder(),
            filename,
            "unitypackage");

            if (!string.IsNullOrEmpty(savePath))
            {
                EditorUtility.DisplayProgressBar("Backup", "Creating backup of " + affectedAssets.Count() + " assets", 0f);
                AssetDatabase.ExportPackage(affectedAssets.ToArray<string>(), savePath, ExportPackageOptions.Default);
                EditorUtility.ClearProgressBar();
                EditorUtility.RevealInFinder(savePath);

                deleteMultipleAssets(affectedAssets);
            }
        }

        private void deleteMultipleAssets(List<string> affectedAssets)
        {
            double startTime = EditorApplication.timeSinceStartup;

            for (int i = 0; i < affectedAssets.Count(); i++)
            {
                EditorUtility.DisplayProgressBar("Deleting unused assets", "Deleting " + i + "/" + affectedAssets.Count() + Environment.NewLine + affectedAssets[i], ((float)i) / ((float)affectedAssets.Count()));
                FileUtil.DeleteFileOrDirectory(affectedAssets[i]);
            }
            EditorUtility.ClearProgressBar();

            AssetDatabase.Refresh();

            if (OnAssetDeleted != null)
                OnAssetDeleted();
        }
    }
}