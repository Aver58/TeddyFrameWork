using System.Collections.Generic;
using libx;
using UnityEngine;

namespace Origins {
    public class ActorManager : Singleton<ActorManager>, ILifeCycle {
        public HeroActor HeroActor;
        private readonly List<EnemyActor> entities;
        private Dictionary<int, EnemyActor> enemyActorMap;
        private GameObjectPool enemyActorPool;
        public ActorManager() {
            entities = new List<EnemyActor>();
            enemyActorPool = new GameObjectPool(LoadModule.MODEL_PATH_PREFIX);
            enemyActorMap = new Dictionary<int, EnemyActor>();
        }

        public void OnInit() {

        }

        public void OnUpdate() {
            if (HeroActor) {
                HeroActor.OnUpdate();
            }

            for (var i = 0; i < entities.Count; i++) {
                var entity = entities[i];
                entity.OnUpdate();
            }
        }

        public void OnClear() {

        }

        public void AddHeroActor(HeroEntity heroEntity) {
            if (heroEntity == null) {
                Debug.LogError("[AddHeroActor]没有传入敌人对象！");
                return;
            }
            
            var characterItem = HeroConfigTable.Instance.Get(heroEntity.RoleId);
            if (characterItem == null) {
                Debug.LogError("[AddHeroActor]没有找到指定角色配置：" + heroEntity.RoleId);
                return;
            }

            var asset = LoadModule.LoadModel(characterItem.modelPath, delegate(AssetRequest request) {
                if (request.asset != null) {
                    var go = Object.Instantiate(request.asset as GameObject, UIModule.Instance.GetParentTransform(ViewType.MAIN));
                    HeroActor = go.GetComponent<HeroActor>();
                    if (HeroActor != null) {
                        HeroActor.OnInit();
                        HeroActor.SetEntity(heroEntity);
                    }
                } else {
                    Debug.LogError("[AddHeroActor]没有找到指定模型：" + characterItem.modelPath);
                }
            });
        }
        
        public void RemoveEnemyActor(EnemyActor enemyActor) {
            if (enemyActor == null) {
                Debug.LogError("[RemoveEnemyActor]没有传入敌人对象！");
                return;
            }
            for (int i = 0; i < entities.Count; i++) {
                if (entities[i] == enemyActor) {
                    //todo remove swap back 移动到后面去删
                    entities.RemoveAt(i);
                    enemyActorMap[enemyActor.InstanceId] = null;
                    enemyActorPool.Release(enemyActor.gameObject);
                    enemyActor.gameObject.SetActive(false);
                    break;
                }
            }
        }

        public void GetActorAsync(EnemyEntity enemyEntity) {
            if (enemyEntity == null) {
                Debug.LogError("[GetActorAsync]没有传入敌人对象！");
                return;
            }
            
            var characterItem = EnemyConfigTable.Instance.Get(enemyEntity.RoleId);
            if (characterItem == null) {
                Debug.LogError("[GetActorAsync]没有找到指定角色配置：" + enemyEntity.RoleId);
                return;
            }
            
            enemyActorPool.GetAsync(characterItem.modelPath, delegate(GameObject instance) {
                if (instance != null) {
                    instance.transform.SetParent(UIModule.Instance.GetParentTransform(ViewType.MAIN));
                    instance.transform.localPosition = enemyEntity.Position;

                    var enemyActor = instance.GetComponent<EnemyActor>();
                    if (enemyActor != null) {
                        enemyActor.OnInit();
                        enemyActor.SetEntity(enemyEntity);
                    }

                    entities.Add(enemyActor);
                    enemyActorMap[enemyActor.InstanceId] = enemyActor;
                } else {
                    Debug.LogError("[GetActorAsync]没有找到指定模型：" + characterItem.modelPath);
                }
            });
        }

        public void SetHeroActorPosition(Vector2 value) {
            if (HeroActor) {
                HeroActor.SetPosition(value);
            }
        }

        public void SetActorPosition(int instanceId, Vector2 value) {
            if (enemyActorMap.ContainsKey(instanceId)) {
                enemyActorMap[instanceId].SetPosition(value);
            }
        }
        
        #region Private
        
        #endregion
    }
}