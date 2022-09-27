using System;
using System.Collections.Generic;
using Battle.logic.ability_dataDriven;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Origins {
    public class EntityManager : Singleton<EntityManager> {
        public int AutoIndex = 0;
        private const int HERO_ID = 101;
        public HeroEntity HeroEntity;

        private List<AbsEntity> entities;
        private ObjectPool<EnemyEntity> enemyEntityPool;

        public EntityManager() {
            entities = new List<AbsEntity>();
            enemyEntityPool = new ObjectPool<EnemyEntity>();
        }

        public void OnUpdate() {
            for (var i = 0; i < entities.Count; i++) {
                var entity = entities[i];
                entity.OnUpdate();
            }
        }

        #region Public

        public void AddEntity(AbsEntity absEntity) {
            entities.Add(absEntity);
        }

        public void RemoveEntity(EnemyEntity entity) {
            for (int i = 0; i < entities.Count; i++) {
                if (entities[i] == entity) {
                    //todo remove swap back 移动到后面去删
                    entities.RemoveAt(i);
                    enemyEntityPool.Release(entity);
                    break;
                }
            }
        }

        public HeroEntity AddHeroEntity() {
            HeroEntity = new HeroEntity(HERO_ID);
            HeroEntity.OnInit();
            HeroEntity.SetPosition(Vector2.zero);

            AddEntity(HeroEntity);
            return HeroEntity;
        }

        private const int MIN_RANGE = 200;
        private const int MAX_RANGE = 1000;

        public void AddEnemyEntity(int roleId) {
            var distance = Random.Range(MIN_RANGE, MAX_RANGE);
            var randomPosX = HeroEntity.LocalPosition.x + distance * Mathf.Cos(distance * Mathf.PI / 180);
            var randomPosY = HeroEntity.LocalPosition.y + distance * Mathf.Sin(distance * Mathf.PI / 180);
            var entity = enemyEntityPool.Get();
            entity.RoleId = roleId;
            entity.OnInit();
            entity.SetPosition(new Vector2(randomPosX, randomPosY));
            Debug.Log($"[EntityManager] 生成敌人：{entity.InstanceId} roleId: {roleId} pos:{entity.LocalPosition}");

            AddEntity(entity);
        }

        public AbsEntity GetSingleTarget(AbsEntity caster, AbilityTarget abilityTarget) {
            if (abilityTarget.ActionSingleTarget == ActionSingleTarget.CASTER) {
                return caster;
            }

            if (abilityTarget.ActionSingleTarget == ActionSingleTarget.TARGET) {
                
            }

            BattleLog.LogError("【GetSingleTarget】没有找到目标：{0}", abilityTarget.ActionSingleTarget.ToString());
            return default;
        }

        #endregion

        #region Private



        #endregion
    }
}