namespace Origins {
	///[[Notice:This file is auto generate by ExportPanelHierarchy，don't modify it manually! it will be call OnLoaded--]]

	public partial class HpSliderView {
		//widgets
		private TMPro.TextMeshProUGUI TxtHp;
		protected override void BindView()
		{
			var UIHierarchy = this.transform.GetComponent<UIHierarchy>();
			TxtHp = (TMPro.TextMeshProUGUI)UIHierarchy.widgets[0].item;
		}
	}
}
