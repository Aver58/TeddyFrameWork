using UnityEngine;

namespace Origins {
    public abstract class AbsEntity : ILifeCycle {
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
        private ILifeCycle lifeCycleImplementation;

        // 因为做的是2d游戏，所以是 LocalPosition
        public Vector3 LocalPosition { get; set; }
        public Quaternion LocalRotation { get; set; } = Quaternion.identity;

        protected virtual void InitProperty(int roleId){ }

        public virtual void Move() { }
        public virtual void BeAttack(int value) { }
        protected virtual void Dead() { }
        public abstract void OnInit();
        public abstract void OnUpdate();
        public abstract void OnClear();
    }
}