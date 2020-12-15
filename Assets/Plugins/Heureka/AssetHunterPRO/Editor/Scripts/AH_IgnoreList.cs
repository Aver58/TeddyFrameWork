using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using HeurekaGames;

namespace HeurekaGames.AssetHunterPRO
{
    [System.Serializable]
    public class AH_IgnoreList
    {
        public delegate void ListUpdatedHandler();
        public event ListUpdatedHandler ListUpdatedEvent;

        private List<string> DefaultIgnored;

        /// <summary>
        /// List of the Ignored items
        /// </summary>
        /*[SerializeField]
        private List<string> CombinedIgnored = new List<string>();*/
        [SerializeField]
        private SerializedIgnoreArray CombinedIgnored = new SerializedIgnoreArray();
        /// <summary>
        /// Id of the playerpref location
        /// </summary>
        private string playerPrefKey;

        /// <summary>
        /// Interface that deals with drawing and excluding in a way that is specific to the type of exclusion we are doing
        /// </summary>
        private AH_IIgnoreListActions exclusionActions;

        //Size of buttons
        public const float ButtonSpacer = 70;
        private string toBeDeleted;
        private bool isDirty;

        public AH_IgnoreList(AH_IIgnoreListActions exclusionActions, List<string> Ignored, string playerPrefKey)
        {
            this.exclusionActions = exclusionActions;
            this.exclusionActions.IgnoredAddedEvent += onAddedToignoredList;
            this.playerPrefKey = playerPrefKey;
            this.DefaultIgnored = new List<string>(Ignored);
        }

        internal List<string> GetIgnored()
        {
            //We already have a list of Ignores
            if (CombinedIgnored.Ignored.Count >= 1)
                return CombinedIgnored.Ignored;
            //We have no list, so read the defaults
            else if (EditorPrefs.HasKey(playerPrefKey))
            {
                //Populates this class from prefs
                CombinedIgnored = LoadFromPrefs();
            }
            else
            {
                //Save the default values into prefs
                SaveDefault();
                //Try to get values again after having set default to prefs
                return GetIgnored();
            }

            //Make sure default and combined are synced
            //If combined has element that doesn't exist in combined, add it!
            if (DefaultIgnored.Exists(val => !CombinedIgnored.Ignored.Contains(val)))
                CombinedIgnored.Ignored.AddRange(DefaultIgnored.FindAll(val => !CombinedIgnored.Ignored.Contains(val)));

            //Returns the values that have been read from prefs
            return CombinedIgnored.Ignored;
        }

        public SerializedIgnoreArray LoadFromPrefs()
        {
            return JsonUtility.FromJson<SerializedIgnoreArray>(EditorPrefs.GetString(playerPrefKey));
        }

        internal bool IsDirty()
        {
            return isDirty;
        }

        public void Save()
        {
            

            string newJsonString = JsonUtility.ToJson(CombinedIgnored);
            string oldJsonString = EditorPrefs.GetString(playerPrefKey);

            //If we haven't changed anything, dont save
            if (newJsonString.Equals(oldJsonString))
                return;

            SetDirty(true);

            //Copy the default values into the other list
            EditorPrefs.SetString(playerPrefKey, newJsonString);

            //Send event that list was update
            if (ListUpdatedEvent != null)
                ListUpdatedEvent();
        }

        public void SaveDefault()
        {
            //Copy the default values into the other list
            CombinedIgnored = new SerializedIgnoreArray(DefaultIgnored);
            Save();
        }

        internal void Reset()
        {
            SaveDefault();
        }

        internal void DrawIgnoreButton()
        {
            /*if (isDirty)
                return;*/

            exclusionActions.DrawIgnored(this);
        }

        internal void OnGUI()
        {
            if (Event.current.type != EventType.Layout && isDirty)
            {
                SetDirty(false);
                return;
            }

            //See if we are currently deleting an Ignore
            checkToDelete();

            EditorGUILayout.BeginVertical();
            EditorGUILayout.Space();

            EditorGUILayout.HelpBox(exclusionActions.Header + " (" + GetIgnored().Count + ")", MessageType.None);

            if (GetIgnored().Count > 0)
            {

                EditorGUI.indentLevel++;
                foreach (var item in GetIgnored())
                {
                    drawIgnored(item, exclusionActions.GetLabelFormattedItem(item));
                }
                EditorGUI.indentLevel--;

            }
            EditorGUILayout.EndVertical();
        }

        private void checkToDelete()
        {
            if (string.IsNullOrEmpty(toBeDeleted))
                return;

            removeFromignoredList(toBeDeleted);
        }

        private void drawIgnored(string identifier, string legible)
        {
            if (string.IsNullOrEmpty(identifier))
                return;

            EditorGUILayout.BeginHorizontal();
            if (!DefaultIgnored.Contains(identifier))
            {
                if (GUILayout.Button("Un-Ignore", EditorStyles.miniButton, GUILayout.Width(ButtonSpacer)))
                {
                    markForDeletion(identifier);
                }
            }
            //Hidden button to help align, probably not the most elegant solution, but Ill fix later
            else if (DefaultIgnored.Count != GetIgnored().Count)
                GUILayout.Button("", GUIStyle.none, GUILayout.Width(ButtonSpacer + 4));

            float curWidth = EditorGUIUtility.labelWidth;
            EditorGUILayout.LabelField(getLabelContent(legible), GUILayout.MaxWidth(1024));
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
        }

        private void markForDeletion(string item)
        {
            toBeDeleted = item;
        }

        protected virtual string getLabelContent(string item)
        {
            return item;
        }

        public bool ExistsInList(string element)
        {
            return (GetIgnored().Contains(element));
        }

        private void onAddedToignoredList(object sender, IgnoreListEventArgs e)
        {
            AddToignoredList(e.Item);
        }

        public void AddToignoredList(string element)
        {
            if (GetIgnored().Contains(element))
            {
                Debug.LogWarning("AH: Element already ignored: " + element);
                return;
            }

            GetIgnored().Add(element);
            //Save to prefs
            Save();
        }

        protected void removeFromignoredList(string element)
        {
            toBeDeleted = "";
            GetIgnored().Remove(element);
            //Save to prefs
            Save();
        }

        //Sets dirty so we know we need to manage the IMGUI (Mismatched LayoutGroup.Repaint)
        private void SetDirty(bool bDirty)
        {
            isDirty = bDirty;
        }

        internal bool ContainsElement(string localpath, string identifier = "")
        {
            return exclusionActions.ContainsElement(GetIgnored(), localpath, identifier);
        }

        [Serializable]
        public class SerializedIgnoreArray
        {
            /// <summary>
            /// List of the Ignored items
            /// </summary>
            [SerializeField]
            public List<string> Ignored = new List<string>();

            public SerializedIgnoreArray() { }

            public SerializedIgnoreArray(List<string> defaultIgnored)
            {
                this.Ignored = new List<string>(defaultIgnored);
            }
        }
    }

    public class AH_ExclusionTypeList : AH_IgnoreList
    {
        //Call base constructor but convert the types into serializable values
        public AH_ExclusionTypeList(AH_IIgnoreListActions exclusionAction, List<Type> Ignored, string playerPrefsKey) : base(exclusionAction, Ignored.ConvertAll<string>(val => Heureka_Serializer.SerializeType(val)), playerPrefsKey)
        {
        }

        //Return the type tostring instead of the fully qualified type identifier
        protected override string getLabelContent(string item)
        {
            Type deserializedType = Heureka_Serializer.DeSerializeType(base.getLabelContent(item));

            if (deserializedType != null)
                return deserializedType.ToString();
            //The Ignored type does no longer exist in project
            else
                return "Unrecognized type : " + item;
        }
    }

    public class IgnoreListEventArgs : EventArgs
    {
        public string Item;

        public IgnoreListEventArgs(Type item)
        {
            this.Item = Heureka_Serializer.SerializeType(item);
        }

        public IgnoreListEventArgs(string item)
        {
            this.Item = item;
        }
    }
}