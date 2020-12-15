using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;

namespace HeurekaGames.AssetHunterPRO.BaseTreeviewImpl.AssetTreeView
{
    [System.Serializable]
    public class AH_TreeviewElement : TreeElement, ISerializationCallbackReceiver
    {
        #region Fields
        [SerializeField]
        private string absPath;
        [SerializeField]
        private string relativePath;
        [SerializeField]
        private string guid;
        [SerializeField]
        private Type assetType;
        //[SerializeField]
        private string assetTypeSerialized;
        [SerializeField]
        private long assetSize;
        [SerializeField]
        private string assestSizeStringRepresentation;
        [SerializeField]
        private long fileSize;
        [SerializeField]
        private string fileSizeStringRepresentation;
        [SerializeField]
        private List<string> scenesReferencingAsset;
        [SerializeField]
        private bool usedInBuild;
        [SerializeField]
        private bool isFolder;
        [SerializeField]
        private Dictionary<AH_MultiColumnHeader.AssetShowMode, long> combinedAssetSizeInFolder = new Dictionary<AH_MultiColumnHeader.AssetShowMode, long>();

        //Dictionary of asset types and their icons (Cant be serialized)
        private static Dictionary<Type, Texture> iconDictionary = new Dictionary<Type, Texture>();

        #endregion

        #region Properties
        public string AbsPath
        {
            get
            {
                return absPath;
            }
        }

        public string RelativePath
        {
            get
            {
                return relativePath;
            }
        }

        public string GUID
        {
            get
            {
                return guid;
            }
        }

        public Type AssetType
        {
            get
            {
                return assetType;
            }
        }

        public string AssetTypeSerialized
        {
            get
            {
                return assetTypeSerialized;
            }
        }

        public long AssetSize
        {
            get
            {
                return assetSize;
            }

            /*set
            {
                assetSize = value;
            }*/
        }

        public string AssetSizeStringRepresentation
        {
            get
            {
                return assestSizeStringRepresentation;
            }

            /*set
            {
                assestSizeStringRepresentation = value;
            }*/
        }

        public long FileSize
        {
            get
            {
                return fileSize;
            }

            /* set
             {
                 fileSize = value;
             }*/
        }

        public string FileSizeStringRepresentation
        {
            get
            {
                return fileSizeStringRepresentation;
            }

            /*set
            {
                fileSizeStringRepresentation = value;
            }*/
        }

        public List<string> ScenesReferencingAsset
        {
            get { return scenesReferencingAsset; }
            //set { scenesReferencingAsset = value; }
        }

        public int SceneRefCount
        {
            get { return (scenesReferencingAsset != null) ? scenesReferencingAsset.Count : 0; }
            //set { scenesReferencingAsset = value; }
        }

        public bool UsedInBuild
        {
            get { return usedInBuild; }
            //set { usedInBuild = value; }
        }

        public bool IsFolder
        {
            get { return isFolder; }
            //set { isFolder = value; }
        }
        #endregion

        public AH_TreeviewElement(string absPath, int depth, int id, string relativepath, string assetID, List<string> scenesReferencing, bool isUsedInBuild) : base(System.IO.Path.GetFileName(absPath), depth, id)
        {
            this.absPath = absPath;
            this.relativePath = relativepath;
            this.guid = UnityEditor.AssetDatabase.AssetPathToGUID(relativepath);
            this.scenesReferencingAsset = scenesReferencing;
            this.usedInBuild = isUsedInBuild;

            //Return if its a folder
            if (isFolder = UnityEditor.AssetDatabase.IsValidFolder(relativepath))
                return;

            //Return if its not an asset
            if (!string.IsNullOrEmpty(this.guid))
            {
                this.assetType = UnityEditor.AssetDatabase.GetMainAssetTypeAtPath(relativepath);

                updateIconDictEntry();

                if (isUsedInBuild)
                {
                    UnityEngine.Object asset = UnityEditor.AssetDatabase.LoadMainAssetAtPath(relativepath);

                    //#if UNITY_2017_1_OR_NEWER
                    if (asset != null)
                        this.assetSize = Profiler.GetRuntimeMemorySizeLong(asset) / 2;
                }

                fileSize = new System.IO.FileInfo(absPath).Length;
                fileSizeStringRepresentation = AH_Utils.GetSizeAsString(fileSize);
                assestSizeStringRepresentation = AH_Utils.GetSizeAsString(assetSize);
            }
        }

        internal long GetFileSizeRecursively(AH_MultiColumnHeader.AssetShowMode showMode)
        {
            if (combinedAssetSizeInFolder == null)
                combinedAssetSizeInFolder = new Dictionary<AH_MultiColumnHeader.AssetShowMode, long>();

            if (combinedAssetSizeInFolder.ContainsKey(showMode))
                return combinedAssetSizeInFolder[showMode];

            //TODO store these values instead of calculating each and every time?
            long combinedChildrenSize = 0;
            //Combine the size of all the children
            if (hasChildren)
                foreach (AH_TreeviewElement item in children)
                {
                    bool validAsset = (showMode == AH_MultiColumnHeader.AssetShowMode.All) ||
                        ((showMode == AH_MultiColumnHeader.AssetShowMode.Unused && !item.usedInBuild) ||
                        (showMode == AH_MultiColumnHeader.AssetShowMode.Used && item.usedInBuild));

                    //Loop thropugh folders and assets thats used not in build
                    if (validAsset || item.isFolder)
                        combinedChildrenSize += item.GetFileSizeRecursively(showMode);
                }

            combinedChildrenSize += this.fileSize;

            //Cache the value
            combinedAssetSizeInFolder.Add(showMode, combinedChildrenSize);

            return combinedChildrenSize;
        }

        #region Serialization callbacks
        //TODO Maybe we can store type infos in BuildInfoTreeView instead of on each individual element, might be performance heavy

        //Store serializable string so we can retrieve type after serialization
        public void OnBeforeSerialize()
        {
            if (assetType != null)
                assetTypeSerialized = Heureka_Serializer.SerializeType(assetType);
        }

        //Set type from serialized property
        public void OnAfterDeserialize()
        {
            if (!string.IsNullOrEmpty(AssetTypeSerialized))
            {
                this.assetType = Heureka_Serializer.DeSerializeType(AssetTypeSerialized);
                //assetTypeSerialized = "";
            }
        }
        #endregion

        internal bool AssetMatchesState(AH_MultiColumnHeader.AssetShowMode showMode)
        {
            //Test if we want to add this element (We dont want to show "used" when window searches for "unused"
            return (AssetType != null && ((showMode == AH_MultiColumnHeader.AssetShowMode.All) || ((showMode == AH_MultiColumnHeader.AssetShowMode.Used && usedInBuild) || (showMode == AH_MultiColumnHeader.AssetShowMode.Unused && !usedInBuild))));
        }

        internal bool HasChildrenThatMatchesState(AH_MultiColumnHeader.AssetShowMode showMode)
        {
            if (!hasChildren)
                return false;

            //Check if a valid child exit somewhere in this branch
            foreach (AH_TreeviewElement child in children)
            {
                if (child.AssetMatchesState(showMode))
                    return true;
                else if (child.HasChildrenThatMatchesState(showMode))
                    return true;
                else
                    continue;
            }
            return false;
        }

        internal List<string> GetUnusedPathsRecursively()
        {
            List<string> unusedAssetsInFolder = new List<string>();

            //Combine the size of all the children
            if (hasChildren)
                foreach (AH_TreeviewElement item in children)
                {
                    if (item.isFolder)
                        unusedAssetsInFolder.AddRange(item.GetUnusedPathsRecursively());
                    //Loop thropugh folders and assets thats used not in build
                    else if (!item.usedInBuild)
                        unusedAssetsInFolder.Add(item.relativePath);
                }
            return unusedAssetsInFolder;
        }

        internal string GetInfoString()
        {
            return String.Format("{0} : {1}", relativePath, fileSizeStringRepresentation);
        }

        internal static List<string> GetStoredIconTypes()
        {
            List<string> iconTypesSerialized = new List<string>();
            foreach (var item in iconDictionary)
            {
                iconTypesSerialized.Add(Heureka_Serializer.SerializeType(item.Key));
            }
            return iconTypesSerialized;
        }

        internal static List<Texture> GetStoredIconTextures()
        {
            List<Texture> iconTexturesSerialized = new List<Texture>();
            foreach (var item in iconDictionary)
            {
                iconTexturesSerialized.Add(item.Value);
            }
            return iconTexturesSerialized;
        }

        private void updateIconDictEntry()
        {
            if (assetType != null && !iconDictionary.ContainsKey(assetType))
                iconDictionary.Add(assetType, UnityEditor.EditorGUIUtility.ObjectContent(null, assetType).image); //UnityEditor.AssetDatabase.GetCachedIcon(relativePath));
        }

        internal static void UpdateIconDictAfterSerialization(List<string> serializationHelperListIconTypes, List<Texture> serializationHelperListIconTextures)
        {
            iconDictionary = new Dictionary<Type, Texture>();
            for (int i = 0; i < serializationHelperListIconTypes.Count; i++)
            {
                Type deserializedType = Heureka_Serializer.DeSerializeType(serializationHelperListIconTypes[i]);
                if (deserializedType != null)
                    iconDictionary.Add(Heureka_Serializer.DeSerializeType(serializationHelperListIconTypes[i]), serializationHelperListIconTextures[i]);
            }
        }

        internal static Texture GetIcon(Type assetType)
        {
            return iconDictionary[assetType];
        }
    }
}