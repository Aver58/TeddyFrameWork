using System.Collections.Generic;

namespace HeurekaGames.AssetHunterPRO
{
    public interface AH_IIgnoreListActions
    {
        event System.EventHandler<IgnoreListEventArgs> IgnoredAddedEvent;
        string Header { get; }
        string FoldOutContent { get; }

        void DrawIgnored(AH_IgnoreList ignoredList);
        void IgnoreCallback(UnityEngine.Object obj, string identifier);
        string GetFormattedItem(string identifier);
        string GetFormattedItemShort(string identifier);
        string GetLabelFormattedItem(string item);
        bool ContainsElement(List<string> ignoredList, string element, string identifier = "");
    }
}