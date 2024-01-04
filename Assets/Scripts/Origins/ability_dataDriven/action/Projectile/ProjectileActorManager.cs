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

            GameMsg.Instance.RegisterListener<int, string, Vector2, Vector2>(GameMsgDef.OnProjectileActorCreated, OnProjectileActorCreated);
            GameMsg.Instance.RegisterListener<int>(GameMsgDef.OnProjectileActorDestroy, OnProjectileActorDestroy);
            GameMsg.Instance.RegisterListener<int, Vector2, Vector2>(GameMsgDef.OnProjectileActorMoveTo, OnProjectileActorMoveTo);
        }

        public void OnUpdate() {
            foreach (var item in actorMap) {
                var actor = item.Value;
                actor.OnUpdate();
            }
        }

        public void OnClear() {
            GameMsg.Instance.UnRegisterListener(GameMsgDef.OnProjectileActorCreated, this);
            GameMsg.Instance.UnRegisterListener(GameMsgDef.OnProjectileActorDestroy, this);
            GameMsg.Instance.UnRegisterListener(GameMsgDef.OnProjectileActorMoveTo, this);

            foreach (var item in actorMap) {
                var actor = item.Value;
                actor.OnClear();
            }
        }

        private void OnProjectileActorCreated(int id, string effectName , Vector2 sourcePosition, Vector2 sourceForward) {
            if (string.IsNullOrEmpty(effectName)) {
                Debug.LogError("【OnProjectileActorCreated】子弹特效名为空！");
                return;
            }
            
            gameObjectPool.GetAsync(effectName, (gameObject) => {
                if (gameObject != null) {
                    gameObject.transform.SetParent(UIModule.Instance.GetParentTransform(ViewType.MAIN));
                    
                    var actor = projectileActorPool.Get();
                    actor.OnInit(id, gameObject, sourcePosition, sourceForward);
                    actorMap[id] = actor;
                } else {
                    Debug.LogError("[GetActorAsync] 回调返回的go为空！");
                }
            });
        }

        private void OnProjectileActorDestroy(int id) {
            if (actorMap.ContainsKey(id)) {
                gameObjectPool.Release(actorMap[id].GameObject);
            }
        }

        private void OnProjectileActorMoveTo(int id, Vector2 position, Vector2 localForward) {
            if (actorMap.ContainsKey(id)) {
                var actor = actorMap[id];
                actor.MoveTo(position, localForward);
            }
        }
    }
}