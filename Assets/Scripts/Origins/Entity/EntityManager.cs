using System;
using System.Collections.Generic;
using Battle.logic.ability_dataDriven;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Origins {
    public class EntityManager : Singleton<EntityManager>, ILifeCycle {
        public int AutoIndex = 0;
        private const int HERO_ID = 101;
        public HeroEntity HeroEntity;

        private List<AbsEntity> entities;
        private ObjectPool<EnemyEntity> enemyEntityPool;

        public EntityManager() {
            entities = new List<AbsEntity>();
            enemyEntityPool = new ObjectPool<EnemyEntity>();
        }

        public void OnInit() {

        }

        public void OnUpdate() {
            for (var i = 0; i < entities.Count; i++) {
                var entity = entities[i];
                entity.OnUpdate();
            }
        }

        public void OnClear() {

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
            var randomPosX = HeroEntity.Position.x + distance * Mathf.Cos(distance * Mathf.PI / 180);
            var randomPosY = HeroEntity.Position.y + distance * Mathf.Sin(distance * Mathf.PI / 180);
            var entity = enemyEntityPool.Get();
            entity.RoleId = roleId;
            entity.OnInit();
            entity.SetPosition(new Vector2(randomPosX, randomPosY));
            Debug.Log($"[EntityManager] 生成敌人：{entity.InstanceId} roleId: {roleId} pos:{entity.Position}");

            AddEntity(entity);
        }

        public AbsEntity GetSingleTarget(AbsEntity caster, AbilityTarget abilityTarget, AbilityRequestContext abilityRequestContext) {
            if (abilityTarget.ActionSingleTarget == ActionSingleTarget.CASTER) {
                return caster;
            }

            if (abilityTarget.ActionSingleTarget == ActionSingleTarget.TARGET) {
                if (!abilityRequestContext.IsUnitRequest) {
                    BattleLog.LogError("【单体技能】目标配置为 TARGET，REQUEST TARGET却是 POINT 类型");
                    return caster;
                }

                return abilityRequestContext.RequestUnit;
            }

            if (abilityTarget.ActionSingleTarget == ActionSingleTarget.POINT) {
                if (abilityRequestContext.IsUnitRequest) {
                    BattleLog.LogError("【单体技能】目标配置为 POINT，REQUEST TARGET却是 Unit 类型");
                    return caster;
                }

                return caster;
            }
            
            BattleLog.LogError("【GetSingleTarget】没有找到目标：{0}", abilityTarget.ActionSingleTarget.ToString());
            return caster;
        }

        #endregion

        #region Private



        #endregion
    }
}