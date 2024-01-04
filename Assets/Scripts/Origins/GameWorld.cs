using System.Collections.Generic;
using Battle.logic.ability_dataDriven;
using UnityEngine;
using Unity.Collections;

namespace Origins {
    public class GameWorld : MonoBehaviour {
        private List<ILifeCycle> managers;

        private void Awake() {
            managers = new List<ILifeCycle>();
            
            RegisterManager(GameMainLoop.Instance);
            RegisterManager(ProjectileManager.Instance);
            RegisterManager(ProjectileActorManager.Instance);
            RegisterManager(EntityManager.Instance);
            RegisterManager(ActorManager.Instance);
            
            for (int i = 0; i < managers.Count; i++) {
                var manager = managers[i];
                manager.OnInit();
            }
        }

        private void Update() {
            for (int i = 0; i < managers.Count; i++) {
                var manager = managers[i];
                manager.OnUpdate();
            }
        }

        private void OnDestroy() {
            for (int i = 0; i < managers.Count; i++) {
                var manager = managers[i];
                manager.OnClear();
                managers[i] = null;
            }
        }

        private void RegisterManager<T>(T manager)where T : ILifeCycle {
            managers.Add(manager);
        }
    }
}
