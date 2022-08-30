using System.Collections.Generic;
using UnityEngine;

namespace Origins.Entity {
    public class EntityManager : Singleton<EntityManager> {
        public int AutoIndex = 0;
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
                    entities.Remove(entity);
                    break;
                }
            }
        }

        public HeroEntity AddHeroEntity() {
            var entity = new HeroEntity {Position = Vector2.zero};

            AddEntity(entity);
            return entity;
        }

        #endregion

        #region Private

        private void AddEnemyEntity() {

        }

        #endregion
    }
}