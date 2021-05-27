///[[Notice:This file is auto generate by ExportPanelHierarchy，don't modify it manually! it will be call OnLoaded--]]

public partial class MobaSkillItem
{
	//widgets
	private ImageEx ImgIcon;
	private ETCJoystick Joystick;
	private UnityEngine.UI.Image ImgCDMask;
	private TMPro.TextMeshProUGUI TxtCD;
	//externals
	protected override void BindView()
	{
		var UIHierarchy = this.transform.GetComponent<UIHierarchy>();
		//widgets
		ImgIcon = (ImageEx)UIHierarchy.widgets[0].item;
		Joystick = (ETCJoystick)UIHierarchy.widgets[1].item;
		ImgCDMask = (UnityEngine.UI.Image)UIHierarchy.widgets[2].item;
		TxtCD = (TMPro.TextMeshProUGUI)UIHierarchy.widgets[3].item;
		//externals
	}
}
