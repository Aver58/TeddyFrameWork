using GameFramework.DataTable;
using StarForce;
using UnityGameFramework.Runtime;

public static class UIExtension {
    public static int? OpenUIForm(this UIComponent uiComponent, UIFormId uiFormId, object userData = null)
    {
        return uiComponent.OpenUIForm((int)uiFormId, userData);
    }

    public static int? OpenUIForm(this UIComponent uiComponent, int uiFormId, object userData = null)
    {
        IDataTable<DRUIForm> dtUIForm = GameEntry.DataTable.GetDataTable<DRUIForm>();
        DRUIForm drUIForm = dtUIForm.GetDataRow(uiFormId);
        if (drUIForm == null)
        {
            Log.Warning("Can not load UI form '{0}' from data table.", uiFormId.ToString());
            return null;
        }

        string assetName = AssetUtility.GetUIFormAsset(drUIForm.AssetName);
        if (!drUIForm.AllowMultiInstance)
        {
            if (uiComponent.IsLoadingUIForm(assetName))
            {
                return null;
            }

            if (uiComponent.HasUIForm(assetName))
            {
                return null;
            }
        }

        return uiComponent.OpenUIForm(assetName, drUIForm.UIGroupName, Constant.AssetPriority.UIFormAsset, drUIForm.PauseCoveredUIForm, userData);
    }
}