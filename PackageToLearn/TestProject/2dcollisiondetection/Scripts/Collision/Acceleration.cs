using Unity.Mathematics;
using UnityEngine;

namespace CustomPhysics.Collision {
    public enum AccelerationType {
        ForAWhile,
        ReachTargetVelocity,
    }

    public abstract class Acceleration {
        public AccelerationType type;
        public float3 acceleration;
        public float3 curVelocity;
        public bool isEnded = false;

        public Acceleration(AccelerationType type, float3 acceleration,
            float3 initVelocity = new float3()) {
            this.type = type;
            this.acceleration = acceleration;
            this.curVelocity = initVelocity;
        }

        public abstract void Tick(float timeSpan);
    }

    public class AccelerationForAWhile : Acceleration {
        public float remainingTime = 0;

        public AccelerationForAWhile(float duration, float3 acceleration, float initVelocity) :
            base(AccelerationType.ForAWhile, acceleration) {
            this.remainingTime = duration;
            this.curVelocity = initVelocity * math.normalizesafe(acceleration);
        }

        public override void Tick(float timeSpan) {
            if (timeSpan <= 0) {
                isEnded = true;
                return;
            }

            curVelocity += acceleration * timeSpan;
            remainingTime -= timeSpan;
        }
    }

    public class AccelerationForTargetVelocity : Acceleration {
        public float3 targetVelocity;
        public float3 lastVelocity;

        public AccelerationForTargetVelocity(float3 acceleration,
            float targetVelocityValue, float initVelocityValue) :
            base(AccelerationType.ReachTargetVelocity, acceleration) {
            this.targetVelocity = targetVelocityValue * math.normalizesafe(acceleration);
            this.curVelocity = initVelocityValue * math.normalizesafe(acceleration);
            this.lastVelocity = curVelocity;
        }

        public override void Tick(float timeSpan) {
            float targetX = targetVelocity.x;
            float targetZ = targetVelocity.z;
            if (lastVelocity.x > targetX != curVelocity.x > targetX ||
                lastVelocity.z > targetZ != curVelocity.z > targetZ) {
                isEnded = true;
                return;
            }

            lastVelocity = curVelocity;
            curVelocity += acceleration * timeSpan;
        }
    }
}