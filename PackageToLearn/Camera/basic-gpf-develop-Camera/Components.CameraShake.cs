using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Framework.GPF {
    public struct ExplosionSourceDesc : IComponentData {
        public float radius;
        public float amplitude;
        public float frequency;
        public float duration;

        public float loopTime;
        public bool loop;
    }

    public struct ExplosionSourceState : IComponentData {
        public float elapsedTime;
    }

    public struct DestroyExplosionSource : IComponentData, IEnableableComponent { }

    public struct ExplosionShakeTask : IComponentData {
        public float amplitude;
        public float frequency;
        public float duration;
        public float elapsedTime;
        public float nextShakeTime;
        public float3 posOffset;
        public float3 rotOffset;
    }

    public struct StopExplosionShakeTask : IComponentData, IEnableableComponent { }

    public struct CameraShakeState : IComponentData {
        public float3 explosionRotation;
        public float3 explosionTranslation;

        public float3 rotation;
        public float3 translation;
    }

    public struct CameraShakeDesc : IComponentData {
        public float3 explosionViewRotateFraction;
        public float3 explosionViewTranslateFraction;
    }
}