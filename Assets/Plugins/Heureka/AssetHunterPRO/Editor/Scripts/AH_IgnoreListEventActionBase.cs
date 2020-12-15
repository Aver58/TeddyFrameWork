using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace HeurekaGames.AssetHunterPRO
{
    public class AH_ignoredListEventActionBase
    {
        public event System.EventHandler<IgnoreListEventArgs> IgnoredAddedEvent;

        public virtual string Header { get; protected set; }
        public virtual string FoldOutContent { get; protected set; }

        [SerializeField] private int ignoredListIndex;
        [SerializeField] private Action<int> buttonDownCallBack;

        public AH_ignoredListEventActionBase(int ignoredListIndex, Action<int> buttonDownCallBack)
        {
            this.ignoredListIndex = ignoredListIndex;
            this.buttonDownCallBack = buttonDownCallBack;
        }

        protected void getObjectInfo(out string path, out bool isMain, out bool isFolder)
        {
            path = AssetDatabase.GetAssetPath(Selection.activeObject);
            isMain = AssetDatabase.IsMainAsset(Selection.activeObject);
            isFolder = AssetDatabase.IsValidFolder(path);
        }

        public void IgnoreCallback(UnityEngine.Object obj, string identifier)
        {
            IgnoredAddedEvent(obj, new IgnoreListEventArgs(identifier));

            //Notify the list was changed
            buttonDownCallBack(ignoredListIndex);
        }

        /// <summary>
        /// draw the Ignore button
        /// </summary>
        /// <param name="buttonText">What should the button read</param>
        /// <param name="ignoredList">The current list of Ignores this is supposed to be appended to</param>
        /// <param name="identifier">The unique identifier of the Ignore</param>
        /// <param name="optionalLegibleIdentifier">Humanly legible identifier</param>
        protected void drawButton(bool validSelected, string buttonText, AH_IgnoreList ignoredList, string identifier, string optionalLegibleIdentifier = "")
        {
            bool btnUsable = validSelected && !ignoredList.ExistsInList(identifier);

            EditorGUILayout.BeginHorizontal();

            EditorGUI.BeginDisabledGroup(!btnUsable);
            if (GUILayout.Button(buttonText, GUILayout.Width(150)) && btnUsable)
            {
                IgnoreCallback(Selection.activeObject, identifier);
            }
            EditorGUI.EndDisabledGroup();

            //Select the proper string to write on label
            string label = (!btnUsable) ?
                ((validSelected) ?
                    "Already Ignored" : "Invalid selection")
                : (string.IsNullOrEmpty(optionalLegibleIdentifier) ?
                    identifier : optionalLegibleIdentifier);

            GUIContent content = new GUIContent(label, EditorGUIUtility.IconContent((!btnUsable) ? ((validSelected) ? "d_lightOff" : "d_orangeLight") : "d_greenLight").image);

            GUILayout.Label(content, GUILayout.MaxHeight(16));
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
        }

        protected bool hasValidSelectionObject()
        {
            return (Selection.activeObject != null && Selection.objects.Length == 1);
        }

        public virtual string GetFormattedItem(string identifier)
        {
            return identifier;
        }

        public virtual string GetFormattedItemShort(string identifier)
        {
            return GetFormattedItem(identifier);
        }

        public virtual string GetLabelFormattedItem(string identifier)
        {
            return GetFormattedItem(identifier);
        }
    }

    public class IgnoredEventActionExtension : AH_ignoredListEventActionBase, AH_IIgnoreListActions
    {
        public IgnoredEventActionExtension(int ignoredListIndex, Action<int> buttonDownCallBack) : base(ignoredListIndex, buttonDownCallBack)
        {
            Header = "File extensions ignored from search";
            FoldOutContent = "See ignored file extensions";
        }

        public bool ContainsElement(List<string> ignoredList, string path, string assetId)
        {
            string element;
            pathContainsValidElement(path, out element);

            return ignoredList.Contains(element.ToLower());
        }

        private bool pathContainsValidElement(string path, out string extension)
        {
            extension = "";
            bool hasExtension = System.IO.Path.HasExtension(path);
            if (hasExtension)
                extension = System.IO.Path.GetExtension(path).ToLower();

            return hasExtension;
        }

        //Check if the currectly selected asset if excludable as file extension
        public void DrawIgnored(AH_IgnoreList ignoredList)
        {
            if (!hasValidSelectionObject())
                return;

            string path;
            bool isMain;
            bool isFolder;
            getObjectInfo(out path, out isMain, out isFolder);

            //if (isMain)
            {
                string extension;
                bool validElement = (pathContainsValidElement(path, out extension));

                drawButton(isMain && validElement, "Ignore file extension", ignoredList, extension);
            }
        }
    }

    public class IgnoredEventActionPathEndsWith : AH_ignoredListEventActionBase, AH_IIgnoreListActions
    {
        public IgnoredEventActionPathEndsWith(int ignoredListIndex, Action<int> buttonDownCallBack) : base(ignoredListIndex, buttonDownCallBack)
        {
            Header = "Folder paths with the following ending are ignored";
            FoldOutContent = "See ignored folder endings";
        }

        public bool ContainsElement(List<string> ignoredList, string path, string assetId)
        {
            return ignoredList.Contains(getIdentifier(path));
        }

        private string getIdentifier(string path)
        {
            if (string.IsNullOrEmpty(path))
                return "";

            string fullPath = System.IO.Path.GetFullPath(path).TrimEnd(System.IO.Path.DirectorySeparatorChar);
            return System.IO.Path.DirectorySeparatorChar + System.IO.Path.GetFileName(fullPath).ToLower();
        }

        //Check if the currectly selected asset if excludable as path ending
        public void DrawIgnored(AH_IgnoreList ignoredList)
        {
            if (!hasValidSelectionObject())
                return;

            string path;
            bool isMain;
            bool isFolder;
            getObjectInfo(out path, out isMain, out isFolder);

            drawButton((isMain && isFolder), "Ignore folder ending", ignoredList, getIdentifier(path));
        }
    }

    public class IgnoredEventActionType : AH_ignoredListEventActionBase, AH_IIgnoreListActions
    {
        public IgnoredEventActionType(int ignoredListIndex, Action<int> buttonDownCallBack) : base(ignoredListIndex, buttonDownCallBack)
        {
            Header = "Asset types ignored";
            FoldOutContent = "See ignored types";
        }

        public bool ContainsElement(List<string> ignoredList, string path, string assetId)
        {
            Type assetType;
            return ignoredList.Contains(getIdentifier(path, out assetType));
        }

        private string getIdentifier(string path, out Type assetType)
        {
            assetType = AssetDatabase.GetMainAssetTypeAtPath(path);
            if (assetType != null)
                return Heureka_Serializer.SerializeType(assetType);
            else
                return "";
        }

        //Check if the currectly selected asset if excludable as type
        public void DrawIgnored(AH_IgnoreList ignoredList)
        {
            if (!hasValidSelectionObject())
                return;

            string path;
            bool isMain;
            bool isFolder;
            getObjectInfo(out path, out isMain, out isFolder);

            Type assetType;
            drawButton((isMain && !isFolder), "Ignore Type", ignoredList, getIdentifier(path, out assetType), assetType != null ? assetType.ToString() : "");
        }
    }

    public class IgnoredEventActionFile : AH_ignoredListEventActionBase, AH_IIgnoreListActions
    {
        public IgnoredEventActionFile(int ignoredListIndex, Action<int> buttonDownCallBack) : base(ignoredListIndex, buttonDownCallBack)
        {
            Header = "Specific files ignored";
            FoldOutContent = "See ignored files";
        }

        public bool ContainsElement(List<string> ignoredList, string path, string assetId)
        {
            if (!string.IsNullOrEmpty(assetId))
                return ignoredList.Contains(assetId);
            else
                return ignoredList.Contains(getIdentifier(path));
        }

        private string getIdentifier(string path)
        {
            return AssetDatabase.AssetPathToGUID(path);
        }

        //Check if the currectly selected asset if excludable as file
        public void DrawIgnored(AH_IgnoreList ignoredList)
        {
            if (!hasValidSelectionObject())
                return;

            string path;
            bool isMain;
            bool isFolder;
            getObjectInfo(out path, out isMain, out isFolder);

            string assetGUID = getIdentifier(path);
            drawButton((isMain && !isFolder), "Ignore file", ignoredList, assetGUID, GetFormattedItemShort(assetGUID, 45));
        }

        public override string GetFormattedItem(string identifier)
        {
            return AssetDatabase.GUIDToAssetPath(identifier);
        }

        public string GetFormattedItemShort(string identifier, int charCount)
        {
            return AH_Utils.ShrinkPathEnd(GetFormattedItem(identifier), charCount);
        }

        public override string GetFormattedItemShort(string identifier)
        {
            return GetFormattedItemShort(identifier, 50);
        }

        public override string GetLabelFormattedItem(string identifier)
        {
            return GetFormattedItemShort(identifier, 60);
        }
    }

    public class IgnoredEventActionFolder : AH_ignoredListEventActionBase, AH_IIgnoreListActions
    {
        public IgnoredEventActionFolder(int ignoredListIndex, Action<int> buttonDownCallBack) : base(ignoredListIndex, buttonDownCallBack)
        {
            Header = "Specific folders ignored";
            FoldOutContent = "See ignored folders";
        }

        public bool ContainsElement(List<string> ignoredList, string path, string assetId)
        {
            if (!string.IsNullOrEmpty(assetId))
                return ignoredList.Contains(assetId);
            else
                return ignoredList.Contains(getIdentifier(path));
        }

        private string getIdentifier(string path)
        {
            return AssetDatabase.AssetPathToGUID(path);
        }

        //Check if the currectly selected asset if excludable as folder
        public void DrawIgnored(AH_IgnoreList ignoredList)
        {
            if (!hasValidSelectionObject())
                return;

            string path;
            bool isMain;
            bool isFolder;
            getObjectInfo(out path, out isMain, out isFolder);

            string assetGUID = getIdentifier(path);
            drawButton((isMain && isFolder), "Ignore folder", ignoredList, assetGUID, GetFormattedItemShort(assetGUID, 40));
        }

        public override string GetFormattedItemShort(string identifier)
        {
            return GetFormattedItemShort(identifier, 50);
        }

        private string GetFormattedItemShort(string identifier, int charCount)
        {
            return AH_Utils.ShrinkPathEnd(GetFormattedItem(identifier), charCount);
        }

        public override string GetFormattedItem(string identifier)
        {
            return AssetDatabase.GUIDToAssetPath(identifier);
        }

        public override string GetLabelFormattedItem(string identifier)
        {
            return GetFormattedItemShort(identifier, 60);
        }
    }
}
