using UnityEngine;

namespace Origins.Entity {
    public abstract class Entity {
        public int Id;
        public int CharacterId;

        public int Hp;
        public int Mana;
        public int Attack;
        public int Defense;
        public float MoveSpeed;
        public float AttackSpeed;
        public Vector2 Position;

        protected Entity(int characterId) {
            Id = EntityManager.instance.AutoIndex++;
            CharacterId = characterId;

            InitProperty(characterId);
        }

        private void InitProperty(int characterId) {
            var config = CharacterTable.Instance.Get(characterId);
            Hp = config.maxHp;
            Mana = config.magic;
            MoveSpeed = config.moveSpeed;
        }

        public virtual void OnUpdate() { }
    }
}