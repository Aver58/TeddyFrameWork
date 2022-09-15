using Origins;
using UnityEngine;

namespace Battle.logic.ability_dataDriven {
    // 子弹数据层
    public class ProjectileEntity {
        public int Id;
        private Vector2 position;

        private string effectName;
        private bool dodgeable;
        private float moveSpeed;
        private AbsEntity casterEntity;
        private ProjectileActor actor;

        public void OnInit(AbsEntity casterEntity, AbilityTarget abilityTarget, string effectName, float moveSpeed, bool dodgeable = false) {
            this.effectName = effectName;
            this.moveSpeed = moveSpeed;
            this.dodgeable = dodgeable;
            this.casterEntity = casterEntity;
            if (casterEntity != null) {
                position = casterEntity.Position;
            }

            if (!string.IsNullOrEmpty(effectName)) {
                Debug.LogError("[GetActorAsync]子弹特效名为空！");
                return;
            }
            ProjectileManager.instance.GetActorAsync(effectName, OnCreateProjectile);
        }

        public void OnUpdate() {
            if (moveSpeed  > 0 && casterEntity != null) {

            }
        }

        public void OnClear() {
            
        }

        private void OnCreateProjectile(GameObject instance) {
            if (instance != null) {
                actor = instance.GetComponent<ProjectileActor>();
                if (actor != null) {
                    Transform transform = actor.transform;
                    transform.SetParent(UIModule.Instance.GetParentTransform(ViewType.MAIN));
                    transform.localPosition = position;
                }
            } else {
                Debug.LogError("[GetActorAsync] 回调返回的go为空！");
            }
        }
    }
}