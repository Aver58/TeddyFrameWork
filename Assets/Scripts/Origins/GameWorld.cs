using System.Collections.Generic;
using Battle.logic.ability_dataDriven;
using UnityEngine;

namespace Origins {
    public class GameWorld : MonoBehaviour {
        private GameMainLoop gameMainLoop;
        private EntityManager entityManager;
        private ActorManager actorManager;
        private ProjectileManager projectileManager;
        private List<ILifeCycle> managers;

        private void Awake() {
            managers = new List<ILifeCycle>();
            RegisterManager(GameMainLoop.instance);
            RegisterManager(EntityManager.instance);
            RegisterManager(ActorManager.instance);
            RegisterManager(ProjectileManager.instance);
            RegisterManager(ProjectileActorManager.instance);
        }

        private void Update() {
            gameMainLoop.OnUpdate();
            entityManager.OnUpdate();
            actorManager.OnUpdate();
            projectileManager.OnUpdate();

            for (int i = 0; i < managers.Count; i++) {
                var manager = managers[i];
                manager.OnUpdate();
            }
        }

        private void FixedUpdate() {
            gameMainLoop.OnFixedUpdate();
        }

        private void RegisterManager<T>(T manager)where T : ILifeCycle {
            managers.Add(manager);
        }
    }
}
