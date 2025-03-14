using System.Collections.Generic;
using UnityEngine;

namespace TestCamera {
    public enum CameraType {
        CodeDriven,
        Designer,
        SpecialCodeDriven
    }

    // 用于实际 Blend 的相机堆栈
    public struct CameraStack {
        public Camera Camera;
        public float fadeRatio; // 当前相机的权重
        public float fadeDuration;  // ratio 从 0 到 1 的时间
    }

    // Code-Driven Camera
    // 如 Follow Camera、Zoom Camera、Dead Camera
    // Code-Driven Camera 基于本帧角色每帧发起入栈请求。
    public struct CodeDrivenCameraRequest {
        public Camera Camera;
        public int Priority;
        public float Duration;
    }

    // Designer Camera
    // 如 Fixed Camera、Animated Camera
    // Designer Camera 在特殊情况下进行 Enable 与 Disable。
    // Designer Camera 激活时会被添加到 Persistent 列表中，优先级最高的会发出入栈请求。
    public struct DesignerCameraRequest {
        public Camera Camera;
        public int Priority;
        public float Duration;
        public bool Active;
    }

    public class CameraManager : MonoSingleton<CameraManager> {
        private List<CodeDrivenCameraRequest> codeDrivenCameraRequests = new List<CodeDrivenCameraRequest>(4);
        private List<DesignerCameraRequest> designerCameraRequests = new List<DesignerCameraRequest>(2);
        private List<CameraStack> cameraStacks = new List<CameraStack>(4);


        public void RequestCodeDrivenCamera(Camera camera, int priority, float duration) {
            var request = new CodeDrivenCameraRequest {
                Camera = camera,
                Priority = priority,
                Duration = duration
            };

            codeDrivenCameraRequests.Add(request);
        }

        public void RequestDesignerCamera(Camera camera, int priority, float duration, bool active) {
            var request = new DesignerCameraRequest {
                Camera = camera,
                Priority = priority,
                Duration = duration,
                Active = active
            };

            designerCameraRequests.Add(request);
        }

        public void OnUpdate() {
            var highestPriorityCodeDrivenCameraRequest = GetHighestPriorityCodeDrivenCameraRequest();
            var highestPriorityDesignerCameraRequest = GetHighestPriorityDesignerCameraRequest();
            UpdateCameraStack(highestPriorityCodeDrivenCameraRequest, highestPriorityDesignerCameraRequest);
            Blend();
        }

        // 比较 Code-Driven 与 Designer 的请求，取优先级最高的入栈，如果该相机已经在 CameraStack 的栈顶，则忽视该请求。否则将该相机压入到 CameraStack 中。
        private void UpdateCameraStack(CodeDrivenCameraRequest codeDrivenCameraRequest, DesignerCameraRequest designerCameraRequest) {
            var targetCameraRequest = designerCameraRequest.Priority > codeDrivenCameraRequest.Priority
                ? designerCameraRequest.Camera : codeDrivenCameraRequest.Camera;
            var currentCameraRequest = cameraStacks.Count > 0 ? cameraStacks[0].Camera : default;
            // todo .Camera不大合理，应该是一个CameraRequest
            if (currentCameraRequest != null && targetCameraRequest != currentCameraRequest) {
                float fadeDuration = designerCameraRequest.Priority > codeDrivenCameraRequest.Priority
                    ? codeDrivenCameraRequest.Duration : designerCameraRequest.Duration;
                var cameraStack = new CameraStack {
                    Camera = targetCameraRequest,
                    fadeDuration = fadeDuration,
                    fadeRatio = cameraStacks.Count == 0 ? 1 : 0
                };
                cameraStacks.Add(cameraStack);
            }
        }

        private void Blend() {
            if (cameraStacks.Count == 0) {
                return;
            }

            // 更新 cameraStacks fadeRatio
            var dt = Time.deltaTime;
            for (var i = 0; i < cameraStacks.Count; i++) {
                var cameraStack = cameraStacks[i];
                cameraStack.fadeRatio += dt / cameraStack.fadeDuration;
                cameraStack.fadeRatio = Mathf.Min(cameraStack.fadeRatio, 1);
                cameraStacks[i] = cameraStack;
                // 当某一层 Camera Ratio 为 1，移除底层所有 Camera
                // if (cameraStackInfo.fadeRatio == 1 && i > 0) {
                //     cameraStack.RemoveRange(0, i);
                //     break;
                // }
            }

            // 计算所有混合最终值
            var currentCameraRequest = cameraStacks[0];
            var cameraTransform = currentCameraRequest.Camera.transform;
            var currentTransition = cameraTransform.position;
            var currentRotation = cameraTransform.rotation;
            var currentFov = currentCameraRequest.Camera.fieldOfView;
            for (int i = 0; i < cameraStacks.Count; i++) {
                var cameraStack = cameraStacks[i];
                var targetCameraTransform = cameraStack.Camera.transform;
                var targetTranslation = targetCameraTransform.position;
                var targetRotation = targetCameraTransform.rotation;
                var targetFov = cameraStack.Camera.fieldOfView;

                currentTransition = Vector3.Lerp(currentTransition, targetTranslation, cameraStack.fadeRatio);
                currentRotation = Quaternion.Lerp(currentRotation, targetRotation, cameraStack.fadeRatio);
                currentFov = Mathf.Lerp(currentFov, targetFov, cameraStack.fadeRatio);
            }

            // 最后把混合出来的值赋给相机
            cameraTransform.position = currentTransition;
            cameraTransform.rotation = currentRotation;
            currentCameraRequest.Camera.fieldOfView = currentFov;
        }

        //获取最高优先级的相机请求
        private CodeDrivenCameraRequest GetHighestPriorityCodeDrivenCameraRequest() {
            if (codeDrivenCameraRequests.Count <= 0) {
                return default;
            }

            CodeDrivenCameraRequest highestPriorityCodeDrivenCameraRequest = default;
            for (var i = 0; i < codeDrivenCameraRequests.Count; i++) {
                if (highestPriorityCodeDrivenCameraRequest.Camera == null) {
                    highestPriorityCodeDrivenCameraRequest = codeDrivenCameraRequests[i];
                } else {
                    if (highestPriorityCodeDrivenCameraRequest.Priority < codeDrivenCameraRequests[i].Priority) {
                        highestPriorityCodeDrivenCameraRequest = codeDrivenCameraRequests[i];
                    }
                }
            }

            return highestPriorityCodeDrivenCameraRequest;
        }

        private DesignerCameraRequest GetHighestPriorityDesignerCameraRequest() {
            if (designerCameraRequests.Count <= 0) {
                return default;
            }

            // todo Active 没有用上
            DesignerCameraRequest highestPriorityDesignerCameraRequest = default;
            for (var i = 0; i < designerCameraRequests.Count; i++) {
                if (highestPriorityDesignerCameraRequest.Camera == null) {
                    highestPriorityDesignerCameraRequest = designerCameraRequests[i];
                } else {
                    if (highestPriorityDesignerCameraRequest.Camera.depth < designerCameraRequests[i].Camera.depth) {
                        highestPriorityDesignerCameraRequest = designerCameraRequests[i];
                    }
                }
            }

            return highestPriorityDesignerCameraRequest;
        }
    }
}