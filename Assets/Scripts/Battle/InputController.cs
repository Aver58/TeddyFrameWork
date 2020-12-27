using UnityEngine;
using UnityEngine.UI;

public class InputController : MonoBehaviour {
    public Animator anim;
    public float runSpeed = 2f;

    public Button btnAttack;
    public Button btnSkill1;
    public Button btnSkill2;
    public Button btnSkill3;

    public ETCJoystick joystick;
    public CharacterController characterController;

    private HeroActor m_PlayerActor;
    private BattleEntity m_PlayerEntity;
    private Vector3 moveDistance = Vector3.zero;
    // 脚本启用时触发 ，注册事件
    public void Init(HeroActor PlayerActor)
    {
        m_PlayerActor = PlayerActor;
        m_PlayerEntity = PlayerActor.battleEntity;

        btnAttack.onClick.AddListener(delegate { OnCastAbility(AbilityCastType.ATTACK); });
        btnSkill1.onClick.AddListener(delegate { OnCastAbility(AbilityCastType.SKILL1); });
        btnSkill2.onClick.AddListener(delegate { OnCastAbility(AbilityCastType.SKILL2); });
        btnSkill3.onClick.AddListener(delegate { OnCastAbility(AbilityCastType.SKILL3); });
        joystick.onMoveEnd.AddListener(() => OnMoveEnd());

        //方式一：按键方法注册
        joystick.OnPressLeft.AddListener(() => OnMoving());
        joystick.OnPressRight.AddListener(() => OnMoving());
        joystick.OnPressUp.AddListener(() => OnMoving());
        joystick.OnPressDown.AddListener(() => OnMoving());
    }

    void OnCastAbility(AbilityCastType castType)
    {
        Ability ability = m_PlayerEntity.GetAbility(castType);
        m_PlayerEntity.CastAbility(ability);
    }

    void OnMoving()
    {
        if(joystick.name != "Joystick")
            return;

        //获取虚拟摇杆偏移量  
        float h = joystick.axisX.axisValue;
        float v = joystick.axisY.axisValue;
        
        if(Mathf.Abs(h) > 0.05f || (Mathf.Abs(v) > 0.05f))
        {
            moveDistance.Set(h, 0, v);
            moveDistance *= runSpeed;
            characterController.SimpleMove(moveDistance);
            transform.LookAt(transform.position);
            m_PlayerActor.Set3DPosition(transform.position);

            m_PlayerActor.ChangeState(HeroState.MOVE);
        }
        else
        {
            m_PlayerActor.ChangeState(HeroState.IDLE);
        }
    }

    void OnMoveEnd()
    {
        m_PlayerActor.ChangeState(HeroState.IDLE);
    }
}
