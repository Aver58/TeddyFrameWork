#region Copyright © 2020 Aver. All rights reserved.
/*
=====================================================
 AverFrameWork v1.0
 Filename:    CheatButtonItem.cs
 Author:      Zeng Zhiwei
 Time:        2021/2/7 16:40:07
=====================================================
*/
#endregion

using UnityEngine;
using UnityEngine.UI;

public partial class CheatButtonItem : ViewBase
{
    private Text text;
    private System.Action callBack;
    public CheatButtonItem(GameObject go, Transform parent) : base(go, parent)
    {
        text = this.Text;
        this.Button.onClick.AddListener(OnBtnClick);
    }

    public void Init(string btnText, System.Action action)
    {
        text.text = btnText;
        callBack = action;
    }

    private void OnBtnClick()
    {
        if(callBack != null)
            callBack.Invoke();
    }
}