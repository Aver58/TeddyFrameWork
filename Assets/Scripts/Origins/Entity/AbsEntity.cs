using UnityEngine;

namespace Origins {
    public abstract class AbsEntity {
        public int InstanceId;
        public int RoleId;

        private float hp;
        public float Hp {
            get => hp;
            set {
                hp = value;

                if (hp <= 0) {
                    Dead();
                }
            }
        }

        public float Mp;
        public int MaxHp;
        public int MaxMp;
        public int PhysicsAttack;
        public int Defense;
        public float MoveSpeed;
        public float AttackCooldown;

        public Vector2 Position { get; set; }
        public Vector2 Forward { get; set; }

        public abstract void OnUpdate();
        public abstract void OnInit();
        public abstract void OnClear();

        protected abstract void InitProperty(int roleId);

        public virtual void Move() { }
        public virtual void BeAttack(int value) { }
        protected virtual void Dead() { }
    }
}