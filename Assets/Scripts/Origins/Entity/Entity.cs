using UnityEngine;

namespace Origins {
    public abstract class Entity {
        public int InstanceId;
        public int RoleId;

        public int Hp;
        public int Mana;
        public int Attack;
        public int Defense;
        public float MoveSpeed;
        public float AttackSpeed;

        private Vector2 position;
        public Vector2 Position {
            get => position;
            set {
                position = value;
            }
        }

        public abstract void OnUpdate();
        public abstract void OnInit();
        public abstract void OnClear();

        protected abstract void InitProperty(int roleId);
        protected abstract void InitActor();

        public virtual void Move() { }
        public virtual void BeAttack(int value) { }
    }
}