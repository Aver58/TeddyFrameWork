using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using UnityEngine.Assertions;

namespace HeurekaGames.AssetHunterPRO.BaseTreeviewImpl.AssetTreeView
{
    class AH_TreeViewWithTreeModel : TreeViewWithTreeModel<AH_TreeviewElement>
    {
        const float kRowHeights = 20f;
        const float kToggleWidth = 18f;

        AH_TreeViewSelectionInfo treeviewSelectionInfo = new AH_TreeViewSelectionInfo();

        GUIContent[] guiContents_toolbarShowSelection = new GUIContent[3]
{
                new GUIContent(AH_MultiColumnHeader.AssetShowMode.Unused.ToString(),"Show only assets that was NOT included in build"),
                new GUIContent(AH_MultiColumnHeader.AssetShowMode.Used.ToString(),"Show only assets that WAS included in build"),
                new GUIContent(AH_MultiColumnHeader.AssetShowMode.All.ToString(),"Show all assets in project")
};

        // All columns
        enum MyColumns
        {
            //Dummy,
            Icon,
            Name,
            AssetSize,
            FileSize,
            UsedInBuild,
            LevelUsage
        }

        public enum SortOption
        {
            AssetType,
            Name,
            AssetSize,
            FileSize,
            Used,
            LevelRefs,
        }

        // Sort options per column
        SortOption[] m_SortOptions =
        {
            //SortOption.Value1,
            SortOption.AssetType,
            SortOption.Name,
            SortOption.AssetSize,
            SortOption.FileSize,
            SortOption.Used,
            SortOption.LevelRefs
        };

        public static void TreeToList(TreeViewItem root, IList<TreeViewItem> result)
        {
            if (root == null)
                throw new NullReferenceException("root");
            if (result == null)
                throw new NullReferenceException("result");

            result.Clear();

            if (root.children == null)
                return;

            Stack<TreeViewItem> stack = new Stack<TreeViewItem>();
            for (int i = root.children.Count - 1; i >= 0; i--)
                stack.Push(root.children[i]);

            while (stack.Count > 0)
            {
                TreeViewItem current = stack.Pop();
                result.Add(current);

                if (current.hasChildren && current.children[0] != null)
                {
                    for (int i = current.children.Count - 1; i >= 0; i--)
                    {
                        stack.Push(current.children[i]);
                    }
                }
            }
        }

        public AH_TreeViewWithTreeModel(TreeViewState state, MultiColumnHeader multicolumnHeader, TreeModel<AH_TreeviewElement> model) : base(state, multicolumnHeader, model)
        {
            Assert.AreEqual(m_SortOptions.Length, Enum.GetValues(typeof(MyColumns)).Length, "Ensure number of sort options are in sync with number of MyColumns enum values");

            // Custom setup
            rowHeight = kRowHeights;
            columnIndexForTreeFoldouts = 1;
            showAlternatingRowBackgrounds = true;
            showBorder = true;
            customFoldoutYOffset = (kRowHeights - EditorGUIUtility.singleLineHeight) * 0.5f; // center foldout in the row since we also center content. See RowGUI
            extraSpaceBeforeIconAndLabel = kToggleWidth;
            multicolumnHeader.sortingChanged += OnSortingChanged;

            //IF we want to start expanded one level
            if (model.root.hasChildren)
                SetExpanded(model.root.children[0].id, true);

            Reload();
        }

        protected override bool RequiresSorting()
        {
            //Show as list if base requires sorting OR if we chose sortedList
            return base.RequiresSorting() || ((AH_MultiColumnHeader)multiColumnHeader).mode == AH_MultiColumnHeader.Mode.SortedList;
        }

        protected override void AddChildrenRecursive(AH_TreeviewElement parent, int depth, IList<TreeViewItem> newRows)
        {
            AH_MultiColumnHeader.AssetShowMode showMode = ((AH_MultiColumnHeader)multiColumnHeader).ShowMode;

            foreach (AH_TreeviewElement child in parent.children)
            {
                bool isFolder = child.hasChildren;
                bool isFolderWithValidChildren = isFolder && (showMode == AH_MultiColumnHeader.AssetShowMode.All || child.HasChildrenThatMatchesState(showMode));

                //TODO EXTRACT THIS TO BE ABLE TO Ignore TYPES ETC OR MAYBE THAT SHOULD BE DONE WHEN POPULATING THE FIRST TIME...maybe better
                //If its a folder or an asset that matches showmode 
                if (isFolderWithValidChildren || child.AssetMatchesState(showMode))
                {
                    //Add new row
                    var item = new TreeViewItem<AH_TreeviewElement>(child.id, depth, child.Name, child);
                    newRows.Add(item);

                    if (isFolder)
                    {
                        if (IsExpanded(child.id))
                        {
                            AddChildrenRecursive(child, depth + 1, newRows);
                        }
                        else
                        {
                            item.children = CreateChildListForCollapsedParent();
                        }
                    }
                }
            }
        }

        public override void OnGUI(Rect rect)
        {
            Rect treeViewRect = rect;

            //If we have selection, we want to make room in the rect to show that
            if (treeviewSelectionInfo.HasSelection)
                treeViewRect.height -= AH_TreeViewSelectionInfo.Height;

            base.OnGUI(treeViewRect);

            Rect SelectionRect = new Rect(treeViewRect.x, treeViewRect.yMax, treeViewRect.width, AH_TreeViewSelectionInfo.Height);

            if (treeviewSelectionInfo.HasSelection/* && ((AH_MultiColumnHeader)multiColumnHeader).ShowMode == AH_MultiColumnHeader.AssetShowMode.Unused*/)
                treeviewSelectionInfo.OnGUISelectionInfo(SelectionRect);
        }
        // Note we We only build the visible rows, only the backend has the full tree information. 
        // The treeview only creates info for the row list.
        protected override IList<TreeViewItem> BuildRows(TreeViewItem root)
        {
            var rows = base.BuildRows(root);
            SortIfNeeded(root, rows);
            return rows;
        }

        void OnSortingChanged(MultiColumnHeader multiColumnHeader)
        {
            ModelChanged();
            SortIfNeeded(rootItem, GetRows());
        }

        void SortIfNeeded(TreeViewItem root, IList<TreeViewItem> rows)
        {
            if (rows.Count <= 1)
                return;

            if (multiColumnHeader.sortedColumnIndex == -1)
            {
                return; // No column to sort for (just use the order the data are in)
            }

            SortByMultipleColumns();
            TreeToList(root, rows);

            Repaint();
        }

        private void deselectItems()
        {
            treeviewSelectionInfo.Reset();
            IList<int> emptyList = new List<int>();
            this.SetSelection(emptyList);
        }

        void SortByMultipleColumns()
        {
            var sortedColumns = multiColumnHeader.state.sortedColumns;

            if (sortedColumns.Length == 0)
                return;

            var myTypes = rootItem.children.Cast<TreeViewItem<AH_TreeviewElement>>();
            var orderedQuery = InitialOrder(myTypes, sortedColumns);
            for (int i = 1; i < sortedColumns.Length; i++)
            {
                SortOption sortOption = m_SortOptions[sortedColumns[i]];
                bool ascending = multiColumnHeader.IsSortedAscending(sortedColumns[i]);

                switch (sortOption)
                {
                    case SortOption.Name:
                        orderedQuery = orderedQuery.ThenBy(l => l.data.Name, ascending);
                        break;
                    case SortOption.AssetSize:
                        orderedQuery = orderedQuery.ThenBy(l => l.data.AssetSize, ascending);
                        break;
                    case SortOption.FileSize:
                        orderedQuery = orderedQuery.ThenBy(l => l.data.FileSize, ascending);
                        break;
                    case SortOption.AssetType:
                        //Make sure that type is not null, if it is null then its a folder and we sort by path
                        orderedQuery = orderedQuery.ThenBy(l => ((l.data.AssetType != null) ? l.data.AssetType.ToString() : l.data.RelativePath), ascending);//.ThenBy(x => x.Pet != null ? x.Pet.Name : "");
                        break;
                    case SortOption.LevelRefs:
                        orderedQuery = orderedQuery.ThenBy(l => l.data.SceneRefCount, ascending);
                        break;
                    case SortOption.Used:
                        orderedQuery = orderedQuery.ThenBy(l => l.data.UsedInBuild, ascending);
                        break;
                    default:
                        Assert.IsTrue(false, "Unhandled enum");
                        break;
                }
            }
            rootItem.children = orderedQuery.Cast<TreeViewItem>().ToList();
        }

        internal long GetCombinedUnusedSize()
        {
            return treeModel.root.GetFileSizeRecursively(AH_MultiColumnHeader.AssetShowMode.Unused);
        }

        internal void DrawDeleteAllButton(GUIContent content, GUIStyle style, params GUILayoutOption[] layout)
        {
            treeviewSelectionInfo.DrawDeleteFolderButton(content, treeModel.root, style, content.tooltip, "This will delete ALL assets not used in last build. Its recommended to review the unused asset list and to backup before proceding", layout);
        }

        internal void ShowTreeMode()
        {
            searchString = "";
            multiColumnHeader.sortedColumnIndex = -1;
            ((AH_MultiColumnHeader)multiColumnHeader).mode = AH_MultiColumnHeader.Mode.Treeview;

            //Make sure we dont cause an endless loop
            ModelChanged();
        }

        //Check if it matches state and searchstring
        protected override bool IsValidElement(TreeElement element, string searchString)
        {
            AH_TreeviewElement ahElement = (element as AH_TreeviewElement);
            AH_MultiColumnHeader.AssetShowMode hShowMode = ((AH_MultiColumnHeader)multiColumnHeader).ShowMode;

            bool validAsset = !ahElement.IsFolder && ahElement.AssetMatchesState(hShowMode);

            //Only show folders if we are in treeview mode and the folder has children that matches state
            bool validFolder = searchString == "" && (ahElement.IsFolder && (((AH_MultiColumnHeader)multiColumnHeader).mode == AH_MultiColumnHeader.Mode.Treeview) && ahElement.HasChildrenThatMatchesState(hShowMode));

            return (validAsset || validFolder) && base.IsValidElement(element, searchString);
        }

        internal void AssetSelectionToolBarGUI()
        {
            EditorGUI.BeginChangeCheck();
            ((AH_MultiColumnHeader)multiColumnHeader).ShowMode = (AH_MultiColumnHeader.AssetShowMode)GUILayout.Toolbar((int)((AH_MultiColumnHeader)multiColumnHeader).ShowMode, guiContents_toolbarShowSelection/*, GUILayout.MaxWidth(AH_SettingsManager.Instance.ShowMenuText ? 290 : 128)*/);
            if (EditorGUI.EndChangeCheck())
            {
                showModeChanged();
            }
        }

        private void showModeChanged()
        {
            deselectItems();
            ModelChanged();
        }

        IOrderedEnumerable<TreeViewItem<AH_TreeviewElement>> InitialOrder(IEnumerable<TreeViewItem<AH_TreeviewElement>> myTypes, int[] history)
        {
            SortOption sortOption = m_SortOptions[history[0]];
            bool ascending = multiColumnHeader.IsSortedAscending(history[0]);
            switch (sortOption)
            {
                case SortOption.Name:
                    return myTypes.Order(l => l.data.Name, ascending);
                case SortOption.AssetSize:
                    return myTypes.Order(l => l.data.AssetSize, ascending);
                case SortOption.FileSize:
                    return myTypes.Order(l => l.data.FileSize, ascending);
                case SortOption.AssetType:
                    return myTypes.Order(l => l.data.AssetTypeSerialized, ascending);
                case SortOption.LevelRefs:
                    return myTypes.Order(l => l.data.ScenesReferencingAsset.Count, ascending);
                case SortOption.Used:
                    return myTypes.Order(l => l.data.UsedInBuild, ascending);
                default:
                    Assert.IsTrue(false, "Unhandled enum");
                    break;
            }

            // default
            return myTypes.Order(l => l.data.Name, ascending);
        }

        protected override void RowGUI(RowGUIArgs args)
        {
            var item = (TreeViewItem<AH_TreeviewElement>)args.item;

            for (int i = 0; i < args.GetNumVisibleColumns(); ++i)
            {
                CellGUI(args.GetCellRect(i), item, (MyColumns)args.GetColumn(i), ref args);
            }
        }

        void CellGUI(Rect cellRect, TreeViewItem<AH_TreeviewElement> item, MyColumns column, ref RowGUIArgs args)
        {
            // Center cell rect vertically (makes it easier to place controls, icons etc in the cells)
            CenterRectUsingSingleLineHeight(ref cellRect);
            AH_TreeviewElement element = (AH_TreeviewElement)item.data;

            switch (column)
            {

                case MyColumns.Icon:
                    {
                        if (item.data.AssetType != null)
                            GUI.DrawTexture(cellRect, AH_TreeviewElement.GetIcon(item.data.AssetType), ScaleMode.ScaleToFit);
                    }
                    break;

                case MyColumns.Name:
                    {
                        Rect nameRect = cellRect;
                        nameRect.x += GetContentIndent(item);
                        DefaultGUI.Label(nameRect, item.data.m_Name, args.selected, args.focused);
                    }
                    break;

                case MyColumns.AssetSize:
                case MyColumns.FileSize:
                    {
                        string value = "";
                        if (column == MyColumns.AssetSize && element.AssetSize > 0)
                            value = element.AssetSizeStringRepresentation;
                        if (column == MyColumns.FileSize && element.FileSize > 0)
                            value = element.FileSizeStringRepresentation;


                        if (element.IsFolder && column == MyColumns.FileSize/*&& !IsExpanded(element.id)*/)
                        {                           
                            value = "{"+AH_Utils.BytesToString(element.GetFileSizeRecursively(((AH_MultiColumnHeader)multiColumnHeader).ShowMode))+"}";
                            DefaultGUI.Label(cellRect, value, args.selected, args.focused);
                        }
                        else
                            DefaultGUI.LabelRightAligned(cellRect, value, args.selected, args.focused);
                    }
                    break;
                case MyColumns.UsedInBuild:
                    {
                        if (item.data.UsedInBuild)
                            DefaultGUI.LabelRightAligned(cellRect, "\u2713", args.selected, args.focused);
                    }
                    break;
                case MyColumns.LevelUsage:
                    {
                        if (item.data.UsedInBuild && item.data.ScenesReferencingAsset != null)
                        {
                            if (item.data.ScenesReferencingAsset.Count > 0)
                            {
                                string cellString = String.Format("Usage: {0}", item.data.ScenesReferencingAsset.Count.ToString());
                                if (args.selected && args.focused)
                                {
                                    if (GUI.Button(cellRect, cellString))
                                    {
                                        UnityEngine.Object[] sceneAssets = new UnityEngine.Object[item.data.ScenesReferencingAsset.Count];
                                        string message = "";

                                        for (int i = 0; i < item.data.ScenesReferencingAsset.Count; i++)
                                        {
                                            message += (item.data.ScenesReferencingAsset[i] + Environment.NewLine);
                                            sceneAssets[i] = AssetDatabase.LoadMainAssetAtPath(item.data.ScenesReferencingAsset[i]);
                                        }
                                        Selection.objects = sceneAssets;
                                        EditorUtility.DisplayDialog("Scenes referencing " + item.data.m_Name, message, "OK");
                                    }
                                }
                                else
                                    DefaultGUI.LabelRightAligned(cellRect, cellString, args.selected, args.focused);
                            }
                            /*else
                                DefaultGUI.LabelRightAligned(cellRect, "Global", args.selected, args.focused);*/
                        }
                    }
                    break;
            }
        }

        protected override void SelectionChanged(IList<int> selectedIds)
        {
            base.SelectionChanged(selectedIds);

            //Only set selection if we are in "Unused Mode"
            //if (((AH_MultiColumnHeader)multiColumnHeader).ShowMode == AH_MultiColumnHeader.AssetShowMode.Unused)
            treeviewSelectionInfo.SetSelection(this, selectedIds);
        }

        protected override bool CanMultiSelect(TreeViewItem item)
        {
            return true;
        }

        public static MultiColumnHeaderState CreateDefaultMultiColumnHeaderState(float treeViewWidth)
        {
            var columns = new[]
            {
                new MultiColumnHeaderState.Column
                {
                    headerContent = new GUIContent(EditorGUIUtility.FindTexture("FilterByType"), "Type of asset"),
                    contextMenuText = "Type",
                    headerTextAlignment = TextAlignment.Center,
                    sortedAscending = true,
                    sortingArrowAlignment = TextAlignment.Right,
                    width = 30,
                    minWidth = 30,
                    maxWidth = 30,
                    autoResize = false,
                    allowToggleVisibility = true
                },
                new MultiColumnHeaderState.Column
                {
                    headerContent = new GUIContent("Name"),
                    headerTextAlignment = TextAlignment.Left,
                    sortedAscending = true,
                    sortingArrowAlignment = TextAlignment.Center,
                    width = 320,
                    minWidth = 175,
                    autoResize = true,
                    allowToggleVisibility = false
                },
                new MultiColumnHeaderState.Column
                {
                    headerContent = new GUIContent("Asset size", "Size of the asset in project"),
                    headerTextAlignment = TextAlignment.Left,
                    sortedAscending = true,
                    sortingArrowAlignment = TextAlignment.Center,
                    width = 80,
                    minWidth = 30,
                    maxWidth = 130,
                    autoResize = true
                },
                new MultiColumnHeaderState.Column
                {
                    headerContent = new GUIContent("Disc size", "Size of the file on disc"),
                    headerTextAlignment = TextAlignment.Left,
                    sortedAscending = true,
                    sortingArrowAlignment = TextAlignment.Center,
                    width = 80,
                    minWidth = 30,
                    maxWidth = 130,
                    autoResize = true,
                    allowToggleVisibility = true
                },
                new MultiColumnHeaderState.Column
                {
                    headerContent = new GUIContent("Used", "If this asset is used in build"),
                    headerTextAlignment = TextAlignment.Left,
                    sortedAscending = true,
                    sortingArrowAlignment = TextAlignment.Center,
                    width = 45,
                    minWidth = 30,
                    maxWidth = 45,
                    autoResize = true
                },
                new MultiColumnHeaderState.Column
                {
                    headerContent = new GUIContent("Level refs", "How many scenes are using this asset"),
                    headerTextAlignment = TextAlignment.Left,
                    sortedAscending = true,
                    sortingArrowAlignment = TextAlignment.Center,
                    width = 70,
                    minWidth = 30,
                    maxWidth = 100,
                    autoResize = true
                }
            };

            Assert.AreEqual(columns.Length, Enum.GetValues(typeof(MyColumns)).Length, "Number of columns should match number of enum values: You probably forgot to update one of them.");

            var state = new MultiColumnHeaderState(columns);
            return state;
        }
    }

    static class MyExtensionMethods
    {
        public static IOrderedEnumerable<T> Order<T, TKey>(this IEnumerable<T> source, Func<T, TKey> selector, bool ascending)
        {
            if (ascending)
            {
                return source.OrderBy(selector);
            }
            else
            {
                return source.OrderByDescending(selector);
            }
        }

        public static IOrderedEnumerable<T> ThenBy<T, TKey>(this IOrderedEnumerable<T> source, Func<T, TKey> selector, bool ascending)
        {
            if (ascending)
            {
                return source.ThenBy(selector);
            }
            else
            {
                return source.ThenByDescending(selector);
            }
        }
    }
}