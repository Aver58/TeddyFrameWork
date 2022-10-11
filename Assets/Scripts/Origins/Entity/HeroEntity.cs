using System.Collections.Generic;
using Battle.logic.ability_dataDriven;
using UnityEngine;

namespace Origins {
    public sealed class HeroEntity : RoleEntity {
        private int defaultSkillId;
        private List<Battle.logic.ability_dataDriven.Ability> abilities;

        public HeroEntity(int roleId) {
            RoleId = roleId;
            abilities = new List<Battle.logic.ability_dataDriven.Ability>(1);
            LocalForward = Vector2.up;
        }

        public override void OnUpdate() {
            if (abilities != null && abilities.Count > 0) {
                for (int i = 0; i < abilities.Count; i++) {
                    var ability = abilities[i];
                    ability.OnUpdate(Time.deltaTime);
                }
            }

            AutoCastAbility();
        }

        public override void OnInit() {
            BattleCamp = BattleCamp.FRIENDLY;
            InitProperty(RoleId);
            InitDefaultSkill();

            ActorManager.instance.AddHeroActor(this);
        }

        public override void OnClear() {
            
        }

        #region Public

        public void SetPosition(Vector2 value) {
            Position = value;
            ActorManager.instance.SetHeroActorPosition(value);
        }

        #endregion

        #region Private

        private void InitProperty(int roleId) {
            var config = HeroConfigTable.Instance.Get(roleId);
            MaxHp = config.maxHp;
            MaxMp = config.maxMp;
            Hp = MaxHp;
            Mp = MaxMp;
            PhysicsAttack = config.physicAttack;
            MoveSpeed = config.moveSpeed;
            defaultSkillId = config.defaultSkillId;
        }

        private void Dead() {
            GameMainLoop.instance.GameOver();
        }

        #endregion

        #region SkillSystem

        private void InitDefaultSkill() {
            var ability = AbilityConfigParse.GetAbility(this, defaultSkillId);
            if (ability != null) {
                ability.OnInit();
                var targetEntity = EntityManager.instance.GetAbilityRequestTarget(this, ability);
                ability.AbilityRequestContext.SetRequestUnit(targetEntity);
                abilities.Add(ability);
            }
        }

        private void AutoCastAbility() {
            if (abilities != null && abilities.Count > 0) {
                for (int i = 0; i < abilities.Count; i++) {
                    var ability = abilities[i];
                    if (ability.IsCastable()) {
                        CastAbility(ability);
                    }
                }
            }
        }

        private void CastAbility(Battle.logic.ability_dataDriven.Ability ability) {
            ability.CastAbility();
        }
        
        #endregion
    }
}