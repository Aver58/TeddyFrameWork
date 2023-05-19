using System;
using UnityEngine;

public class ReplayTest : MonoBehaviour {
    void Start() {
        ReplayHelper.ReplayStreamer = new NetworkReplayStreamer();
    }

    private void Update() {
        ReplayHelper.OnUpdateRecording();
        ReplayHelper.UpdateReplay();
    }

    private void OnGUI() {
        if (GUILayout.Button("Start Recording")) {
            ReplayHelper.StartRecording("test");
        }

        if (GUILayout.Button("Stop Recording")) {
            ReplayHelper.StopRecording();
        }

        if (GUILayout.Button("Start Replay")) {
            ReplayHelper.StartReplay();
        }

        if (GUILayout.Button("Stop Replay")) {
            ReplayHelper.StopReplay();
        }
    }
}
