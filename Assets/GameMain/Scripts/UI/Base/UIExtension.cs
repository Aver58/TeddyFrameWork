using System;
using GameFramework.DataTable;
using StarForce;
using UnityEngine;
using UnityGameFramework.Runtime;
using UnityEngine.UI;

public static class UIExtension {
    public static int? OpenUIForm(this UIComponent uiComponent, UIFormId uiFormId, object userData = null)
    {
        return uiComponent.OpenUIForm((int)uiFormId, userData);
    }

    public static int? OpenUIForm(this UIComponent uiComponent, int uiFormId, object userData = null)
    {
        UIFormConfig config = UIFormConfig.Get(uiFormId);
        if (config == null) {
            Log.Warning("Can not load UI form '{0}' from data table.", uiFormId.ToString());
            return null;
        }

        string assetName = AssetUtility.GetUIFormAsset(config.AssetName);
        if (!config.AllowMultiInstance)
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

        return uiComponent.OpenUIForm(assetName, config.UIGroupName, Constant.AssetPriority.UIFormAsset, config.PauseCoveredUIForm, userData);
    }
}