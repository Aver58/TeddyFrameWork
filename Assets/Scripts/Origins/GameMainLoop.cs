using Origins.Entity;

namespace Origins {
    public class GameMainLoop : Singleton<GameMainLoop> {
        private const int GAME_TOTAL_TIME = 300;
        private HeroEntity heroEntity;

        #region LifeCycle

        // 虫王，吞噬天地，进化！！噬金虫王！
        public GameMainLoop() {
            new TableConfig().LoadTableConfig();
            CreateHero();
        }

        public void OnUpdate() {

        }

        #endregion

        #region Public



        #endregion

        #region Private

        private void CreateHero() {
            heroEntity = EntityManager.instance.AddHeroEntity();
        }

        private void CreateEnemy() {

        }

        private void CreateFood() {

        }

        #endregion
    }
}
