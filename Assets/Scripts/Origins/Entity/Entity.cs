using UnityEngine;

namespace Origins.Entity {
    public abstract class Entity {
        public int Id;
        public int Hp;
        public int Mana;
        public int Attack;
        public int Defense;
        public float MoveSpeed;
        public float AttackSpeed;
        public Vector2 Position;

        protected Entity() {
            Id = EntityManager.instance.AutoIndex++;
        }

        public virtual void OnUpdate() { }

        public void SetPosition(Vector2 value) {
            Position = value;
        }
    }
}