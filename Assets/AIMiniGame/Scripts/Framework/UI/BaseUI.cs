using System;
using UnityEngine;

namespace AIMiniGame.Scripts.Framework.UI {
    public abstract class BaseUI : MonoBehaviour {
        public UILayer Layer;// 所属层级
        public virtual void Init(UILayer layer) {}
        public virtual void OnShow() {}     // 界面显示回调
        public virtual void OnHide() {}     // 界面隐藏回调

        // 数据绑定快捷方法
        protected void Bind<T>(ref T component, string path) where T : Component {
            component = transform.Find(path).GetComponent<T>();
        }
    }
}