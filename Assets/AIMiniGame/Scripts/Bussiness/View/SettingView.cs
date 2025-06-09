using AIMiniGame.Scripts.Bussiness.Controller;
using AIMiniGame.Scripts.Framework.UI;

using Kirurobo;

using UnityEngine;
using UnityEngine.UI;

public class SettingView : UIViewBase{
    public Toggle isWindowsTop;
    public Toggle isClickThrough;
    public Dropdown languageDropdown;
    public Button btnClose;

    private UniWindowController uniWindowController;
    
    protected override void OnInit() {
        isWindowsTop.isOn = PlayerPrefs.GetInt(SettingController.IsWindowsTopMost, 0) == 1;
        isWindowsTop.onValueChanged.AddListener(OnToggleIsTopChanged);
        isClickThrough.isOn = PlayerPrefs.GetInt(SettingController.IsClickThrough, 0) == 1;
        isClickThrough.onValueChanged.AddListener(OnToggleClickThroughChanged);

        languageDropdown.captionText.text = LocalizationFeature.GetLanguageName(LocalizationFeature.CurrentLanguage);
        languageDropdown.ClearOptions();
        languageDropdown.options.Add(new Dropdown.OptionData(){text = "中文"});
        languageDropdown.options.Add(new Dropdown.OptionData(){text="English"});
        languageDropdown.onValueChanged.AddListener(OnLanguageChanged);

        uniWindowController = UniWindowController.current;

        btnClose.onClick.AddListener(() => {
            ControllerManager.Instance.Close<SettingController>();
        });
    }

    protected override void OnClear() { }
    protected override void OnOpen() { }
    protected override void OnClose() { }

    // 显示在其他应用上
    private void OnToggleIsTopChanged(bool isOn) {
        uniWindowController.isTopmost = isOn;
        PlayerPrefs.SetInt(SettingController.IsWindowsTopMost, isOn ? 1 : 0);
    }

    private void OnToggleClickThroughChanged(bool isOn) {
        uniWindowController.isClickThrough = isOn;
        PlayerPrefs.SetInt(SettingController.IsClickThrough, isOn ? 1 : 0);
    }

    private void OnLanguageChanged(int index) {
        switch (index) {
            case 0:
                LocalizationFeature.CurrentLanguage = SystemLanguage.Chinese;
                break;
            case 1:
                LocalizationFeature.CurrentLanguage = SystemLanguage.English;
                break;
        }

        LocalizationFeature.SetCurrentLanguage(LocalizationFeature.CurrentLanguage);
    }
}
