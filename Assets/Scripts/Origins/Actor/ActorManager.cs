using System.Collections.Generic;
using UnityEngine;

namespace Origins {
    public class ActorManager : Singleton<ActorManager> {
        public HeroActor HeroActor;
        public List<EnemyActor> entities;
        private Dictionary<int, EnemyActor> enemyActorMap;
        private List<EnemyActor> enemyActorPool;
        public ActorManager() {
            entities = new List<EnemyActor>();
            enemyActorPool = new List<EnemyActor>();
            enemyActorMap = new Dictionary<int, EnemyActor>();
        }

        public void OnUpdate() {
            HeroActor.OnUpdate();
            
            for (var i = 0; i < entities.Count; i++) {
                var entity = entities[i];
                entity.OnUpdate();
            }
        }
        
        public HeroActor AddHeroActor(HeroEntity heroEntity) {
            if (heroEntity == null) {
                Debug.LogError("[AddEnemyActor]没有传入敌人对象！");
                return null;
            }
            
            var characterItem = CharacterTable.Instance.Get(heroEntity.RoleId);
            if (characterItem == null) {
                Debug.LogError("[AddEnemyActor]没有找到指定角色配置：" + heroEntity.RoleId);
                return null;
            }

            var asset = LoadModule.Instance.LoadPrefab(characterItem.modelPath);
            if (asset != null) {
                var go = Object.Instantiate(asset, UIModule.Instance.GetParentTransform(ViewType.MAIN));
                go.transform.localPosition = heroEntity.Position;
                
                var actor = go.GetComponent<HeroActor>();
                if (actor != null) {
                    actor.OnInit();
                    actor.SetEntity(heroEntity);
                }
                
                return actor;
            } else {
                Debug.LogError("[AddEnemyActor]没有找到指定模型：" + characterItem.modelPath);
            }

            return null;
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
                    enemyActorPool.Add(enemyActor);
                    enemyActor.gameObject.SetActive(false);
                    break;
                }
            }
        }

        public EnemyActor GetActorFromPool(EnemyEntity enemyEntity) {
            EnemyActor enemyActor = null;
            var length = enemyActorPool.Count;
            if (length == 0) {
                enemyActor = AddEnemyActor(enemyEntity);
            } else {
                enemyActor = enemyActorPool[length - 1];
                enemyActorPool.RemoveAt(length - 1);
                enemyActor.gameObject.SetActive(true);
            }

            return enemyActor;
        }

        public void SetActorPosition(int instanceId, Vector2 value) {
            if (enemyActorMap.ContainsKey(instanceId)) {
                enemyActorMap[instanceId].SetPosition(value);
            }
        }
        
        #region Private

        private EnemyActor AddEnemyActor(EnemyEntity enemyEntity) {
            if (enemyEntity == null) {
                Debug.LogError("[AddEnemyActor]没有传入敌人对象！");
                return null;
            }
            
            var characterItem = CharacterTable.Instance.Get(enemyEntity.RoleId);
            if (characterItem == null) {
                Debug.LogError("[AddEnemyActor]没有找到指定角色配置：" + enemyEntity.RoleId);
                return null;
            }

            var asset = LoadModule.Instance.LoadPrefab(characterItem.modelPath);
            if (asset != null) {
                var go = Object.Instantiate(asset, UIModule.Instance.GetParentTransform(ViewType.MAIN));
                go.transform.localPosition = enemyEntity.Position;
                
                var enemyActor = go.GetComponent<EnemyActor>();
                if (enemyActor != null) {
                    enemyActor.OnInit();
                    enemyActor.SetEntity(enemyEntity);
                }
                
                entities.Add(enemyActor);
                enemyActorMap[enemyActor.InstanceId] = enemyActor;
                return enemyActor;
            } else {
                Debug.LogError("[AddEnemyActor]没有找到指定模型：" + characterItem.modelPath);
            }

            return null;
        }
        
        #endregion
    }
}