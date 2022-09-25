using System.Collections.Generic;
using Battle.logic.ability_dataDriven;
using UnityEngine;

namespace Origins {
    public sealed class HeroEntity : AbsEntity {
        private int defaultSkillId;
        private List<Battle.logic.ability_dataDriven.Ability> abilities;

        public HeroEntity(int roleId) {
            RoleId = roleId;
            InstanceId = EntityManager.instance.AutoIndex++;
            abilities = new List<Battle.logic.ability_dataDriven.Ability>(1);
        }

        public override void OnUpdate() {
            if (abilities != null) {
                if (abilities.Count > 0) {
                    for (int i = 0; i < abilities.Count; i++) {
                        var ability = abilities[i];
                        ability.OnUpdate(Time.deltaTime);
                    }
                }
            }
        }

        public override void OnInit() {
            InitProperty(RoleId);
            InitDefaultSkill();

            ActorManager.instance.AddHeroActor(this);
        }

        public override void OnClear() {
            
        }

        #region Public

        public void SetPosition(Vector2 value) {
            LocalPosition = value;
            ActorManager.instance.SetHeroActorPosition(value);
        }

        #endregion

        #region Private

        protected override void InitProperty(int roleId) {
            var config = HeroConfigTable.Instance.Get(roleId);
            MaxHp = config.maxHp;
            MaxMp = config.maxMp;
            Hp = MaxHp;
            Mp = MaxMp;
            PhysicsAttack = config.physicAttack;
            MoveSpeed = config.moveSpeed;
            defaultSkillId = config.defaultSkillId;
        }

        protected override void Dead() {
            base.Dead();

            GameMainLoop.instance.GameOver();
        }

        #endregion

        #region SkillSystem

        private void InitDefaultSkill() {
            var ability = AbilityConfigParse.GetAbility(this, defaultSkillId);
            if (ability != null) {
                ability.OnInit();
                abilities.Add(ability);
            }
        }

        #endregion
    }
}