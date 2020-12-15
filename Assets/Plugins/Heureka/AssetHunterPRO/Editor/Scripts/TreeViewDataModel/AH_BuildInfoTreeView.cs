using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using System.IO;

namespace HeurekaGames.AssetHunterPRO.BaseTreeviewImpl.AssetTreeView
{
    [Serializable]
    public class AH_BuildInfoTreeView : ScriptableObject, ISerializationCallbackReceiver
    {
        //Serialization helper lists
        private List<string> serializationHelperListIconTypes;
        private List<Texture> serializationHelperListIconTextures;

        [SerializeField]
        List<AH_TreeviewElement> m_TreeElements;

        internal List<AH_TreeviewElement> treeElements
        {
            get { return m_TreeElements; }
            set { m_TreeElements = value; }
        }

        public void OnEnable()
        {
            hideFlags = HideFlags.HideAndDontSave;
        }

        public bool PopulateTreeView(AH_SerializedBuildInfo chosenBuildInfo)
        {
            //Todo, maybe not get ALL assets, but just the assets in the project folder (i.e. -meta etc)?
            treeElements = new List<AH_TreeviewElement>();

            int depth = -1;
            int id = 0;

            var root = new AH_TreeviewElement("Root", depth, id, "", "", new List<string>(), false);

            treeElements.Add(root);

            depth++;
            id++;

            //This is done because I cant find all assets in toplevel folders through the Unity API (Remove whem the API allows it)
            int folderCount = System.IO.Directory.GetDirectories(Application.dataPath).Count();
            int foldersProcessed = 0;

            bool populatedSuccesfully = AddFilesRecursively(Application.dataPath, chosenBuildInfo, depth, ref id, ref folderCount, ref foldersProcessed);

            //Cleanup garbage
            AssetDatabase.Refresh();
            GC.Collect();

            //Create tree
            if (populatedSuccesfully)
                TreeElementUtility.ListToTree(treeElements);

            EditorUtility.ClearProgressBar();

            return populatedSuccesfully;
        }

        private bool AddFilesRecursively(string absPath, AH_SerializedBuildInfo chosenBuildInfo, int treeViewDepth, ref int treeViewID, ref int folderCount, ref int foldersProcessed)
        {
            string relativePath;
            string folderID;
            AH_Utils.GetRelativePathAndAssetID(absPath, out relativePath, out folderID);

            //Check if this folder has been Ignored
            if (AH_SettingsManager.Instance.HasIgnoredFolder(relativePath, folderID))
                return false;

            //Add folder
            System.IO.DirectoryInfo dirInfo = new System.IO.DirectoryInfo(absPath);
            string dirInfoName = dirInfo.Name;

            //Increment folder process count
            foldersProcessed++;
            EditorUtility.DisplayProgressBar("Analyzing project", relativePath, ((float)foldersProcessed / (float)folderCount)); //Todo make cancellable

            //Increment ID
            treeViewID++;

            AH_TreeviewElement threeViewFolder = new AH_TreeviewElement(dirInfoName, treeViewDepth, treeViewID, ((treeViewDepth != -1) ? relativePath : ""), "", null, false);
            treeElements.Add(threeViewFolder);

            //Increment depth
            treeViewDepth++;

            //Track if this folder has valid children
            bool hasValidChildren = false;

            foreach (var assetPath in System.IO.Directory.GetFiles(absPath).Where(val => Path.GetExtension(val) != ".meta"))// !val.EndsWith(".meta")))
            {
                string relativepath;
                string assetID;
                AH_Utils.GetRelativePathAndAssetID(assetPath, out relativepath, out assetID);

                //If this is not an unity asset
                if (string.IsNullOrEmpty(assetID))
                    continue;

                //Has this file been Ignored?
                if (AH_SettingsManager.Instance.HasIgnoredAsset(relativepath, assetID))
                    continue;

                AH_SerializableAssetInfo usedAssetInfo = chosenBuildInfo.GetItemInfo(assetID);
                bool isAssetUsed = (usedAssetInfo != null);

                //TODO CONTINUE LOOP AND ADDING OF ASSETS
                treeViewID++;
                AH_TreeviewElement treeViewElement = new AH_TreeviewElement(assetPath, treeViewDepth, treeViewID, relativepath, assetID, ((isAssetUsed) ? usedAssetInfo.Refs : null), isAssetUsed);
                treeElements.Add(treeViewElement);

                hasValidChildren = true;
            }

            foreach (var dir in System.IO.Directory.GetDirectories(absPath))
            {
                if (AddFilesRecursively(dir, chosenBuildInfo, treeViewDepth, ref treeViewID, ref folderCount, ref foldersProcessed))
                    hasValidChildren = true;
            }

            if (!hasValidChildren && (treeViewDepth != -1))
            {
                treeElements.Remove(threeViewFolder);
                //Decrement ID
                treeViewID--;

                //Decrement depth
                treeViewDepth--;
            }

            //Return true if folder added succesfully
            return hasValidChildren;
        }

        internal bool HasUnused()
        {
            bool hasUnused = m_TreeElements.Any(val => !val.UsedInBuild && !val.IsFolder && val.depth != -1);
            return hasUnused;
        }

        private string[] getAssetsOfType(Type type)
        {
            return AssetDatabase.FindAssets("t:" + type.Name);
        }

        #region Serialization callbacks

        //Store serializable string so we can retrieve type after serialization
        public void OnBeforeSerialize()
        {
            serializationHelperListIconTypes = AH_TreeviewElement.GetStoredIconTypes();
            serializationHelperListIconTextures = AH_TreeviewElement.GetStoredIconTextures();
        }

        public void OnAfterDeserialize()
        {
            AH_TreeviewElement.UpdateIconDictAfterSerialization(serializationHelperListIconTypes, serializationHelperListIconTextures);
            serializationHelperListIconTypes = null;
            serializationHelperListIconTextures = null;
        }
        #endregion
    }
}
