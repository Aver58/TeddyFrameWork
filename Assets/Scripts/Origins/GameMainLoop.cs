using Origins.Entity;

namespace Origins {
    public class GameMainLoop : Singleton<GameMainLoop> {
        private HeroEntity heroEntity;



        #region LifeCycle

        public GameMainLoop() {
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

        #endregion
    }
}
