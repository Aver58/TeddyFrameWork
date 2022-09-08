using System.Collections.Generic;
using UnityEditor;
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
            if (HeroActor) {
                HeroActor.OnUpdate();
            }

            for (var i = 0; i < entities.Count; i++) {
                var entity = entities[i];
                entity.OnUpdate();
            }
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

            var path = "Assets/Data/character/HeroActor101.prefab";
            var go1 = AssetDatabase.LoadAssetAtPath(path, typeof(GameObject));
            Debug.LogError(go1);
            var asset = LoadModule.Instance.LoadPrefab(characterItem.modelPath);
            if (asset != null) {
                var go = Object.Instantiate(asset, UIModule.Instance.GetParentTransform(ViewType.MAIN));
                HeroActor = go.GetComponent<HeroActor>();
                if (HeroActor != null) {
                    HeroActor.OnInit();
                    HeroActor.SetEntity(heroEntity);
                }
            } else {
                Debug.LogError("[AddHeroActor]没有找到指定模型：" + characterItem.modelPath);
            }
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

        public EnemyActor GetActor(int instanceId) {
            return enemyActorMap.ContainsKey(instanceId) ? enemyActorMap[instanceId] : null;
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

        private EnemyActor AddEnemyActor(EnemyEntity enemyEntity) {
            if (enemyEntity == null) {
                Debug.LogError("[AddEnemyActor]没有传入敌人对象！");
                return null;
            }
            
            var characterItem = EnemyConfigTable.Instance.Get(enemyEntity.RoleId);
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
            } else {
                Debug.LogError("[AddEnemyActor]没有找到指定模型：" + characterItem.modelPath);
            }
       
            return null;
        }
        
        #endregion
    }
}