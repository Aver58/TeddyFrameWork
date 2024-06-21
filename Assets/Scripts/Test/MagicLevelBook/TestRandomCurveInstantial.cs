using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TestRandomCurveInstantial : MonoBehaviour {
    public GameObject go;
    public Transform target;

    // Start is called before the first frame update
    void Start()
    {
        // for (int i = 0; i < 4; i++) {
        //     Instantiate();
        // }
        var go = GameObject.Find("UGCThumbnailCamera");
        Debug.LogError(go);
    }

    private void OnGUI() {
        if (GUILayout.Button("生成")) {
            Instantiate();
        }
    }

    private void Instantiate() {
        var ins = Instantiate(go);
        var MagicLevelBookEditor = ins.GetComponent<MagicLevelBookMono>();
        if (MagicLevelBookEditor != null) {
            var config = MagicLevelBookEditor.MagicLevelBookSO;

        }
        ins.transform.position = Vector3.zero;
    }
}
