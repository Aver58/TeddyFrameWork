///[[Notice:This file is auto generate by ExportPanelHierarchy，don't modify it manually! it will be call OnLoaded--]]

public partial class CheatButtonItem
{
	//widgets
	private UnityEngine.UI.Button Button;
	private UnityEngine.UI.Text Text;
	//externals
	protected override void BindView()
	{
		var UIHierarchy = this.transform.GetComponent<UIHierarchy>();
		//widgets
		Button = (UnityEngine.UI.Button)UIHierarchy.widgets[0].item;
		Text = (UnityEngine.UI.Text)UIHierarchy.widgets[1].item;
		//externals
	}
}
