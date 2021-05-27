///[[Notice:This file is auto generate by ExportPanelHierarchy，don't modify it manually! it will be call OnLoaded--]]

public partial class MobaMainView
{
	//widgets
	private UnityEngine.UI.Button BtnAttack;
	private UnityEngine.RectTransform NodeAttack;
	private UnityEngine.RectTransform NodeSkill1;
	private UnityEngine.RectTransform NodeSkill2;
	private UnityEngine.RectTransform NodeSkill3;
	private ETCJoystick JoystickLeft;
	private UnityEngine.RectTransform HPHuds;
	private UnityEngine.RectTransform GridLayout;
	//externals
	private UnityEngine.GameObject MobaSkillItem;
	private UnityEngine.GameObject CheatButtonItem;
	protected override void BindView()
	{
		var UIHierarchy = this.transform.GetComponent<UIHierarchy>();
		//widgets
		BtnAttack = (UnityEngine.UI.Button)UIHierarchy.widgets[0].item;
		NodeAttack = (UnityEngine.RectTransform)UIHierarchy.widgets[1].item;
		NodeSkill1 = (UnityEngine.RectTransform)UIHierarchy.widgets[2].item;
		NodeSkill2 = (UnityEngine.RectTransform)UIHierarchy.widgets[3].item;
		NodeSkill3 = (UnityEngine.RectTransform)UIHierarchy.widgets[4].item;
		JoystickLeft = (ETCJoystick)UIHierarchy.widgets[5].item;
		HPHuds = (UnityEngine.RectTransform)UIHierarchy.widgets[6].item;
		GridLayout = (UnityEngine.RectTransform)UIHierarchy.widgets[7].item;
		//externals
		MobaSkillItem = (UnityEngine.GameObject)UIHierarchy.externals[0].item;
		CheatButtonItem = (UnityEngine.GameObject)UIHierarchy.externals[1].item;
	}
}
