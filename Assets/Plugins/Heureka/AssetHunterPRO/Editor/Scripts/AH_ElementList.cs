using HeurekaGames.AssetHunterPRO.BaseTreeviewImpl.AssetTreeView;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace HeurekaGames.AssetHunterPRO
{
    [System.Serializable]
    public class AH_ElementList
    {
        public List<AssetDumpData> elements;

        public AH_ElementList(List<AssetDumpData> elements)
        {
            this.elements = elements;
        }

        internal static void DumpCurrentListToFile(AH_TreeViewWithTreeModel m_TreeView)
        {
            var path = EditorUtility.SaveFilePanel(
            "Dump current list to file",
            AH_SerializationHelper.GetBuildInfoFolder(),
            "AH_Listdump_" + System.Environment.UserName,
            AH_SerializationHelper.FileDumpExtension);

            if (path.Length != 0)
            {
                List<AssetDumpData> elements = new List<AssetDumpData>();

                foreach (var element in m_TreeView.GetRows())
                    populateDumpListRecursively(m_TreeView.treeModel.Find(element.id), ((AH_MultiColumnHeader)m_TreeView.multiColumnHeader).ShowMode, ref elements);

                AH_ElementList objectToSave = new AH_ElementList(elements);
                AH_SerializationHelper.SerializeAndSave(objectToSave, path);
            }
        }

        private static void populateDumpListRecursively(AH_TreeviewElement element, AH_MultiColumnHeader.AssetShowMode showmode, ref List<AssetDumpData> elements)
        {
            if (element.HasChildrenThatMatchesState(showmode))
            {
                foreach (var child in element.children)
                {
                    populateDumpListRecursively((AH_TreeviewElement)child, showmode, ref elements);
                }
            }
            else if(element.AssetMatchesState(showmode))
            {
               UnityEngine.Debug.Log("Adding " + element.Name);
                elements.Add(new AssetDumpData(element.GUID,element.AbsPath,element.RelativePath, element.AssetSize, element.FileSize, element.UsedInBuild,element.ScenesReferencingAsset));
            }
        }
        [System.Serializable]
        public struct AssetDumpData
        {
            #pragma warning disable
            [SerializeField] private string GUID;
            [SerializeField] private string absPath;
            [SerializeField] private string relativePath;
            [SerializeField] private long assetSize;
            [SerializeField] private long fileSize;
            [SerializeField] private bool usedInBuild;
            [SerializeField] private List<string> scenesReferencingAsset;
            #pragma warning restore

            public AssetDumpData(string guid, string absPath, string relativePath, long assetSize, long fileSize, bool usedInBuild, List<string> scenesReferencingAsset)
            {
                this.GUID = guid;
                this.absPath = absPath;
                this.relativePath = relativePath;
                this.assetSize = assetSize;
                this.fileSize = fileSize;
                this.usedInBuild = usedInBuild;
                this.scenesReferencingAsset = scenesReferencingAsset;
            }
        }
    }
    
}