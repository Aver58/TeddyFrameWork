using UnityEngine;
using System.Collections.Generic;

public delegate void UpdateDelegate(float delta);

public class UpdateRegister : MonoSingleton<UpdateRegister> {
    private static UpdateRegister instance;
    private List<UpdateDelegate> updateables = new List<UpdateDelegate>();
    private List<UpdateDelegate> fixedUpdateables = new List<UpdateDelegate>();
    private List<UpdateDelegate> lateUpdateables = new List<UpdateDelegate>();

    // 注册需要Update的对象
    public void RegisterUpdate(UpdateDelegate updateable) {
        if (!updateables.Contains(updateable)) {
            updateables.Add(updateable);
        }
    }

    // 注册需要FixedUpdate的对象
    public void RegisterFixedUpdate(UpdateDelegate updateable) {
        if (!fixedUpdateables.Contains(updateable)) {
            fixedUpdateables.Add(updateable);
        }
    }

    // 注册需要LateUpdate的对象
    public void RegisterLateUpdate(UpdateDelegate updateable) {
        if (!lateUpdateables.Contains(updateable)) {
            lateUpdateables.Add(updateable);
        }
    }

    // 取消注册Update
    public void UnregisterUpdate(UpdateDelegate updateable) {
        updateables.Remove(updateable);
    }

    // 取消注册FixedUpdate
    public void UnregisterFixedUpdate(UpdateDelegate updateable) {
        fixedUpdateables.Remove(updateable);
    }

    // 取消注册LateUpdate
    public void UnregisterLateUpdate(UpdateDelegate updateable) {
        lateUpdateables.Remove(updateable);
    }

    private void Update() {
        for (int i = updateables.Count - 1; i >= 0; i--) {
            if (updateables[i] != null) {
                updateables[i].Invoke(Time.deltaTime);
            } else {
                updateables.RemoveAt(i);
            }
        }
    }

    private void FixedUpdate() {
        for (int i = fixedUpdateables.Count - 1; i >= 0; i--) {
            if (fixedUpdateables[i] != null) {
                fixedUpdateables[i].Invoke(Time.deltaTime);
            } else {
                fixedUpdateables.RemoveAt(i);
            }
        }
    }

    private void LateUpdate() {
        for (int i = lateUpdateables.Count - 1; i >= 0; i--) {
            if (lateUpdateables[i] != null) {
                lateUpdateables[i].Invoke(Time.deltaTime);
            } else {
                lateUpdateables.RemoveAt(i);
            }
        }
    }
}