using System.Collections.Generic;
using UnityEngine;

namespace Origins {
    public class EntityManager : Singleton<EntityManager> {
        public int AutoIndex = 0;
        private const int HERO_ID = 101;
        public RoleEntity HeroEntity;

        private List<int> entityIds;
        private List<Entity> entities;
        private List<RoleEntity> enemyEntityPool;

        public EntityManager() {
            entityIds = new List<int>();
            entities = new List<Entity>();
            enemyEntityPool = new List<RoleEntity>();
        }

        public void OnUpdate() {
            for (var i = 0; i < entities.Count; i++) {
                var entity = entities[i];
                entity.OnUpdate();
            }
        }

        #region Public

        public Entity AddEntity(Entity entity) {
            entityIds.Add(entity.InstanceId);
            entities.Add(entity);
            return entity;
        }

        public void RemoveEntity(Entity entity) {
            for (int i = 0; i < entities.Count; i++) {
                if (entities[i] == entity) {
                    //todo remove swap back 移动到后面去删
                    entityIds.Remove(entity.InstanceId);
                    entities.RemoveAt(i);
                    break;
                }
            }
        }

        public RoleEntity AddHeroEntity() {
            HeroEntity = new RoleEntity(HERO_ID);
            HeroEntity.OnInit();
            HeroEntity.SetRoleType(GameDefine.RoleType.Hero);
            HeroEntity.SetPosition(Vector2.zero);

            AddEntity(HeroEntity);
            return HeroEntity;
        }

        public void AddEnemyEntity(int roleId) {
            var distance = Random.Range(1000, 2000);
            var randomPosX = HeroEntity.Position.x + distance * Mathf.Cos(distance * Mathf.PI / 180);
            var randomPosY = HeroEntity.Position.y + distance * Mathf.Sin(distance * Mathf.PI / 180);
            var entity = new RoleEntity(roleId);
            entity.OnInit();
            entity.SetRoleType(GameDefine.RoleType.Enemy);
            entity.SetPosition(new Vector2(randomPosX, randomPosY));
            Debug.Log($"[EntityManager] 生成敌人：{roleId} pos:{entity.Position}");

            AddEntity(entity);
        }

        #endregion

        #region Private



        #endregion
    }
}