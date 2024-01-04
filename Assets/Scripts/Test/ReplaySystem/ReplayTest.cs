using System;
using UnityEngine;

namespace Test.ReplaySystem {
    public class ReplayTest : MonoBehaviour {
        private void Awake() {
            ReplaySystem.Instance.Init();
        }

        private void OnDestroy() {
            ReplaySystem.Instance.UnInit();
        }

        void Start() {
            ReplayHelper.ReplayStreamer = new ReplayStreamer();
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

            if (GUILayout.Button("Load Cube Actor")) {
                ReplayHelper.LoadCubeActor("Cube", ReplaySystem.ActorInstanceId++);
            }
        }
    }
}
