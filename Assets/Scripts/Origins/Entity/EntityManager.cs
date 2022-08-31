using System.Collections.Generic;
using UnityEngine;

namespace Origins.Entity {
    public class EntityManager : Singleton<EntityManager> {
        public int AutoIndex = 0;
        private const int HERO_ID = 101;
        public HeroEntity HeroEntity;

        private List<int> entityIds;
        private List<Entity> entities;
        private List<EnemyEntity> enemyEntityPool;

        public EntityManager() {
            entityIds = new List<int>();
            entities = new List<Entity>();
            enemyEntityPool = new List<EnemyEntity>();
        }

        public void OnUpdate() {
            for (var i = 0; i < entities.Count; i++) {
                var entity = entities[i];
                entity.OnUpdate();
            }
        }

        #region Public

        public Entity AddEntity(Entity entity) {
            entityIds.Add(entity.Id);
            entities.Add(entity);
            return entity;
        }

        public void RemoveEntity(Entity entity) {
            for (int i = 0; i < entities.Count; i++) {
                if (entities[i] == entity) {
                    //todo remove swap back 移动到后面去删
                    entityIds.Remove(entity.Id);
                    entities.RemoveAt(i);
                    break;
                }
            }
        }

        public HeroEntity AddHeroEntity() {
            HeroEntity = new HeroEntity (HERO_ID){
                Position = Vector2.zero
            };

            AddEntity(HeroEntity);
            return HeroEntity;
        }

        public void AddEnemyEntity(int characterId) {
            var distance = Random.Range(10, 20);
            var randomPositionX = HeroEntity.Position.x + distance * Mathf.Cos(distance * Mathf.PI / 180);
            var randomPositionY = HeroEntity.Position.y + distance * Mathf.Sin(distance * Mathf.PI / 180);
            var entity = new EnemyEntity (characterId){
                Position = new Vector2(randomPositionX, randomPositionY)
            };
            Debug.Log($"[EntityManager] 生成敌人：characterId: {characterId} Position{entity.Position}");

            AddEntity(entity);
        }

        #endregion

        #region Private



        #endregion
    }
}