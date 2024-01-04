using System;
using UnityEngine;

public class TestReflection : MonoBehaviour {
    private BreakRoleState breakRoleState;

    void Start() {
        breakRoleState = new BreakRoleState();
        breakRoleState.Init();

        var breakRoleStateType = breakRoleState.GetType();
        var method = breakRoleStateType.GetMethod("TestReflection");
        method.Invoke(breakRoleState, Array.Empty<object>());
    }
}
