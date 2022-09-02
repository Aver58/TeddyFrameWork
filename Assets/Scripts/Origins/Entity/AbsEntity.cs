using UnityEngine;

namespace Origins {
    public abstract class AbsEntity {
        public int InstanceId;
        public int RoleId;

        public int Hp;
        public int Mana;
        public int Attack;
        public int Defense;
        public float MoveSpeed;
        public float AttackSpeed;

        public Vector2 Position { get; set; }

        public abstract void OnUpdate();
        public abstract void OnInit();
        public abstract void OnClear();

        protected abstract void InitProperty(int roleId);

        public virtual void Move() { }
        public virtual void BeAttack(int value) { }
    }
}