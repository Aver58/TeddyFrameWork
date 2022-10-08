using System.Collections.Generic;
using Origins;
using UnityEngine;

namespace Battle.logic.ability_dataDriven {
    public class ProjectileActorManager : Singleton<ProjectileActorManager>, ILifeCycle {
        private GameObjectPool gameObjectPool;
        private ObjectPool<ProjectileActor> projectileActorPool;
        private Dictionary<int, ProjectileActor> actorMap;

        public void OnInit() {
            actorMap = new Dictionary<int, ProjectileActor>();
            projectileActorPool = new ObjectPool<ProjectileActor>();
            gameObjectPool = new GameObjectPool(LoadModule.BULLET_PATH_PREFIX);

            GameMsg.instance.RegisterListener<int, string, Vector3, Vector3>(GameMsgDef.OnProjectileActorCreated, OnProjectileActorCreated);
        }

        public void OnUpdate() {
            foreach (var item in actorMap) {
                var actor = item.Value;
                actor.OnUpdate();
            }
        }

        public void OnClear() {
            GameMsg.instance.UnRegisterListener(GameMsgDef.OnProjectileActorCreated, this);

            foreach (var item in actorMap) {
                var actor = item.Value;
                actor.OnClear();
            }
        }

        private void OnProjectileActorCreated(int id, string effectName , Vector3 sourcePosition, Vector3 sourceToward) {
            if (string.IsNullOrEmpty(effectName)) {
                Debug.LogError("【OnProjectileActorCreated】子弹特效名为空！");
                return;
            }
            
            gameObjectPool.GetAsync(effectName, (gameObject) => {
                if (gameObject != null) {
                    gameObject.transform.SetParent(UIModule.Instance.GetParentTransform(ViewType.MAIN));
                    var actor = projectileActorPool.Get();
                    actor.OnInit(id, gameObject, sourcePosition, sourceToward);
                    actorMap[id] = actor;
                } else {
                    Debug.LogError("[GetActorAsync] 回调返回的go为空！");
                }
            });
        }
    }
}