using UnityEngine;
using UnityGameFramework.Runtime;

public class PuzzleForgeForm : FullScreenForm {
    protected override void OnInit(object userData) {
        base.OnInit(userData);

        Debug.LogError("OnInit");
    }
}
