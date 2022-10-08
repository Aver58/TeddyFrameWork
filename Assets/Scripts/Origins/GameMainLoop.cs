using System.Collections.Generic;
using UnityEngine;

namespace Origins {
    public class GameMainLoop : Singleton<GameMainLoop>, ILifeCycle {
        private float currentTime = 0;
        private const int MAX_LEVEL_NUM = 1000;
        private HeroEntity heroEntity;
        private List<LevelDetailItem> levels;

        #region LifeCycle

        // 虫王，吞噬天地，进化！！噬金虫王！
        public GameMainLoop() {
            TableConfig.Instance.LoadTableConfig();
            GenerateHero();
            LoadLevelConfig();
        }

        public void OnInit() {

        }

        public void OnUpdate() {
            currentTime += Time.fixedDeltaTime;
            if (levels == null || levels.Count <= 0) {
                return;
            }

            var config = levels[0];
            if (currentTime >= config.time) {
                GenerateEnemy(config);
                levels.RemoveAt(0);
            }
        }

        public void OnClear() {
        }

        #endregion

        #region Public

        public void GameOver() {
            Application.Quit();
        }

        #endregion

        #region Private

        private void GenerateHero() {
            heroEntity = EntityManager.instance.AddHeroEntity();
        }

        private void GenerateEnemy(LevelDetailItem levelDetailItem) {
            for (int i = 0; i < levelDetailItem.num; i++) {
                EntityManager.instance.AddEnemyEntity(levelDetailItem.characterId);
            }
        }

        private void LoadLevelConfig() {
            var levelId = 1;
            var config = LevelConfigTable.Instance.Get(levelId);
            levels = new List<LevelDetailItem>(config.totalLevel);
            for (var i = 0; i < config.totalLevel; i++) {
                var levelDetailId = levelId * MAX_LEVEL_NUM + i;
                var detailConfig = LevelDetailTable.Instance.Get(levelDetailId);
                if (detailConfig != null) {
                    levels.Add(detailConfig);
                }
            }

            Debug.Log($"[GameMainLoop] 加载地图配置成功，总条数：{levels.Count}");
        }

        #endregion
    }
}
