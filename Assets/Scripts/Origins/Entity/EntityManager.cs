using System.Collections.Generic;
using UnityEngine;

namespace Origins {
    public class EntityManager : Singleton<EntityManager> {
        public int AutoIndex = 0;
        private const int HERO_ID = 101;
        public HeroEntity HeroEntity;

        private List<AbsEntity> entities;
        private List<HeroEntity> enemyEntityPool;

        public EntityManager() {
            entities = new List<AbsEntity>();
            enemyEntityPool = new List<HeroEntity>();
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

        public void RemoveEntity(AbsEntity absEntity) {
            for (int i = 0; i < entities.Count; i++) {
                if (entities[i] == absEntity) {
                    //todo remove swap back 移动到后面去删
                    entities.RemoveAt(i);
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

        private const int MinRang = 200;
        private const int MaxRang = 1000;

        public void AddEnemyEntity(int roleId) {
            var distance = Random.Range(MinRang, MaxRang);
            var randomPosX = HeroEntity.Position.x + distance * Mathf.Cos(distance * Mathf.PI / 180);
            var randomPosY = HeroEntity.Position.y + distance * Mathf.Sin(distance * Mathf.PI / 180);
            var entity = new EnemyEntity(roleId);
            entity.OnInit();
            entity.SetPosition(new Vector2(randomPosX, randomPosY));
            Debug.Log($"[EntityManager] 生成敌人：{entity.InstanceId} roleId: {roleId} pos:{entity.Position}");

            AddEntity(entity);
        }

        #endregion

        #region Private



        #endregion
    }
}