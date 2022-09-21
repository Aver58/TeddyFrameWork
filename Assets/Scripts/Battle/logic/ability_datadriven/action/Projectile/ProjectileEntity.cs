using Origins;
using UnityEngine;

namespace Battle.logic.ability_dataDriven {
    // 子弹数据层
    public class ProjectileEntity {
        public int Id;
        public Vector3 LocalPosition;
        
        private Quaternion rotation;
        private string effectName;
        private bool dodgeable;
        private float moveSpeed;
        private float durning = 3f;
        private float flyTime;
        private Vector3 flyToward;
        private AbsEntity casterEntity;
        private ProjectileActor actor;

        public void OnInit(AbsEntity casterEntity, Vector3 startPoint, Quaternion startRotation, AbilityTarget abilityTarget, string effectName, float moveSpeed, bool dodgeable = false) {
            this.effectName = effectName;
            this.moveSpeed = moveSpeed;
            this.dodgeable = dodgeable;
            this.casterEntity = casterEntity;
            LocalPosition = startPoint;
            rotation = startRotation;
            flyTime = 0f;
            flyToward = startRotation * Vector3.up;

            if (string.IsNullOrEmpty(effectName)) {
                Debug.LogError("[GetActorAsync]子弹特效名为空！");
                return;
            }
            
            ProjectileManager.instance.GetActorAsync(effectName, OnCreateProjectile);
        }

        public void OnUpdate() {
            if (moveSpeed > 0) {
                LocalPosition += flyToward * moveSpeed * Time.deltaTime;
            }

            durning += Time.deltaTime;
            if (flyTime >= durning) {
                FlyEnd();
            }
            
            if (actor != null) {
                actor.OnUpdate();
            }
        }

        public void OnClear() {
            actor = null;
            casterEntity = null;
        }

        private void OnCreateProjectile(GameObject instance) {
            if (instance != null) {
                instance.transform.SetParent(UIModule.Instance.GetParentTransform(ViewType.MAIN));
                actor = instance.GetComponent<ProjectileActor>();
                if (actor != null) {
                    var transform = actor.transform;
                    transform.localPosition = LocalPosition;
                    var angle = Vector3.SignedAngle(Vector3.zero, flyToward, Vector3.forward);
                    transform.localRotation = new Quaternion(0, 0, angle, 1);
                    actor.SetEntity(this);
                }
            } else {
                Debug.LogError("[GetActorAsync] 回调返回的go为空！");
            }
        }

        private void FlyEnd() {
            OnClear();
        }
    }
}