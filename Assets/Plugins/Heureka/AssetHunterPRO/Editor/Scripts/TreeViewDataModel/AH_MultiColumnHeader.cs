
using System;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace HeurekaGames.AssetHunterPRO.BaseTreeviewImpl.AssetTreeView
{
    internal class AH_MultiColumnHeader : MultiColumnHeader
    {
        AssetShowMode m_showMode;
        public enum AssetShowMode
        {
            Unused,
            Used,
            All
        }

        Mode m_Mode;
        public enum Mode
        {
            //LargeHeader,
            Treeview,
            SortedList
        }

        public AH_MultiColumnHeader(MultiColumnHeaderState state) : base(state)
        {
            mode = Mode.Treeview;
        }

        public Mode mode
        {
            get
            {
                return m_Mode;
            }
            set
            {
                m_Mode = value;
                switch (m_Mode)
                {
                    case Mode.Treeview:
                        canSort = true;
                        height = DefaultGUI.minimumHeight;
                        break;
                    case Mode.SortedList:
                        canSort = true;
                        height = DefaultGUI.defaultHeight;
                        break;
                }
            }
        }

        public AssetShowMode ShowMode
        {
            get
            {
                return m_showMode;
            }
            set
            {
                m_showMode = value;
            }
        }

        protected override void ColumnHeaderClicked(MultiColumnHeaderState.Column column, int columnIndex)
        {
            if (mode == Mode.Treeview)
            {
                mode = Mode.SortedList;
            }

            base.ColumnHeaderClicked(column, columnIndex);
        }
    }
}