using Origins;
using UnityEngine;

namespace Battle.logic.ability_dataDriven {
    // 技能上下文
    // 技能运行时过程中会产生目标、选点等动态数据、经过服务器验证的动态数据，
    // 结算效果：modifier
    // 例如一个弹射技能，在多个目标间弹射时会发生多次目标变更，这就需要多次同步。
    // 同步方式是主控端发起并预表现，服务器校验并广播，主控端收到经校验的数据后会进行修正，模拟客户端收到广播包后进行后续表现。
    public struct AbilityRequestContext {
        public bool IsUnitRequest;
        public AbsEntity RequestUnit { get; set; } // 目标对象
        public Vector3 RequestPosition { get; set; } // 选点对象

        public void SetRequestUnit(AbsEntity value) {
            IsUnitRequest = true;
            RequestUnit = value;
        }
        
        public void SetRequestPosition(Vector3 value) {
            IsUnitRequest = false;
            RequestPosition = value;
        }
    }
}