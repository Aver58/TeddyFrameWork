//using System;
//using System.Collections.Generic;
//using UnityEngine;
//using Stardom.Core.Model;
//using Stardom.Core.XProto;
//using Stardom;
//using Net;
//using Google.Protobuf;
//using CUI.View;

//public class LogView : MonoBehaviour
//{
//    public static GameObject logView;
//    private static GUIStyle labelStyle;
//    private static GUIStyle buttonStyle;
//    public static bool isShow = false;
//    private bool isDown;

//    private float[] touchDounw = new float[2];
//    private float[] touchPosition = new float[2];


//    public static void Init()
//    {
//        if(logView == null)
//        {
//            labelStyle = new GUIStyle();
//            labelStyle.fontSize = Screen.height / 0x2d;
//            labelStyle.normal = new GUIStyleState();
//            labelStyle.normal.textColor = Color.white;

//            logView = new GameObject("LogView");
//            logView.isStatic = true;
//            logView.AddComponent<LogView>();
//            logView.transform.parent = CorePlugins.Instance.pluginRoot;
//        }
//    }

//    private void Awake()
//    {
//        CorePlugins.Instance.netManager.AddHandler(typeof(SC_GM), ScGMHandler);
//    }

//    private void ScGMHandler(object data)
//    {
//        gmRspContent = "";
//        gmContent.Clear();
//        SC_GM dat = data as SC_GM;
//        gmContent.Add(dat.ResultMsg);
//    }


//    private Vector2 scrollPosition = Vector2.zero;
//    private string[] btnInfos = new string[] { "GM Set", "Current Log", "PhoneInfo", "Close" };
//    private static int currentSelect = 1;

//    private void OnGUI()
//    {
//        if(!GameSet.ShowLogView)
//            return;
//        if(!isShow) return;
//        GUI.skin.verticalScrollbar.fixedWidth = 70;
//        GUI.skin.verticalScrollbarThumb.fixedWidth = 70;

//        buttonStyle = GUI.skin.button;
//        buttonStyle.fontSize = Screen.height / 30;
//        buttonStyle.fixedHeight = 60;

//        GUI.Box(new Rect(0, 0, Screen.width, Screen.height), "");
//        GUI.Box(new Rect(0, 0, Screen.width, Screen.height), "");
//        GUI.Box(new Rect(0, 0, Screen.width, Screen.height), "");
//        GUI.Box(new Rect(0, 0, Screen.width, Screen.height), "");

//        GUILayout.BeginHorizontal(GUILayout.Width(Screen.width), GUILayout.Height(70));
//        int select = GUILayout.SelectionGrid(currentSelect, btnInfos, btnInfos.Length, buttonStyle);
//        GUILayout.EndHorizontal();

//        if(select != currentSelect)
//        {
//            currentSelect = select;
//        }

//        if(currentSelect == 0)
//        {
//            GUIGMSet();
//        }
//        else if(currentSelect == 1)
//        {
//            GUILog();
//        }
//        else if(currentSelect == 2)
//        {
//            GUIPhone();
//        }
//        else if(currentSelect == 3)
//        {
//            Hide();
//        }
//        return;
//    }

//    private Vector2 gmLogPos = Vector2.zero;
//    private string gmCMD = "";
//    private string gmRspContent = "";
//    private List<string> gmContent = new List<string>();
//    private void GUIGMSet()
//    {
//        int fontSize = GUI.skin.textField.fontSize;
//        Color color = GUI.skin.label.normal.textColor;
//        GUI.skin.textField.fontSize = 40;
//        GUI.skin.label.fontSize = 40;

//        GUILayout.Label("输入GM指令,点击发送按钮", GUILayout.Height(50));
//        gmCMD = GUILayout.TextField(gmCMD, GUILayout.Height(50));
//        GUILayout.BeginHorizontal();

//        if(GUILayout.Button("发送到Game server", GUILayout.Height(60)) && !string.IsNullOrEmpty(gmCMD))
//        {
//            gmRspContent = "正在发送GM指令...\n";

//            CS_GM sendGM = new CS_GM();
//            sendGM.Command = gmCMD;

//            CorePlugins.Instance.netManager.SendMessage(sendGM);
//        }
//        GUILayout.EndHorizontal();
//        GUILayout.BeginHorizontal();
//        if(GUILayout.Button("清除本地数据", GUILayout.Height(60)))
//        {

//        }
//        if(GUILayout.Button("打印机器persistentDataPath路径", GUILayout.Height(60)))
//        {
//            Log.Info(Application.persistentDataPath);
//        }
//        GUILayout.EndHorizontal();


//        GUI.skin.label.normal.textColor = color;
//        gmLogPos = GUILayout.BeginScrollView(gmLogPos);
//        GUI.skin.textArea.fontSize = 30;
//        GUI.skin.label.fontSize = 30;

//        string showStr = "";
//        showStr += gmRspContent;

//        for(int i = 0; i < gmContent.Count; i++)
//        {
//            showStr += gmContent[i];
//            showStr += "\n";
//        }
//        showStr = GUILayout.TextArea(showStr, GUILayout.Height(1000));
//        GUILayout.EndScrollView();

//        GUI.skin.label.normal.textColor = color;
//        GUI.skin.textArea.fontSize = fontSize;
//        GUI.skin.label.fontSize = fontSize;
//    }

//    private bool IsShowLog = true;
//    private bool IsShownNetLog = false;
//    private bool IsShowError = false;

//    private void GUILog()
//    {
//        GUILayout.BeginHorizontal(GUILayout.Width(Screen.width), GUILayout.Height(70));

//        GUI.color = IsShowLog ? Color.yellow : GUI.contentColor;
//        if(GUILayout.Button("Info log"))
//        {
//            IsShowLog = !IsShowLog;
//        }

//        GUI.color = IsShownNetLog ? Color.yellow : GUI.contentColor;
//        if(GUILayout.Button("Net log"))
//        {
//            IsShownNetLog = !IsShownNetLog;
//        }

//        GUI.color = IsShowError ? Color.yellow : GUI.contentColor;
//        if(GUILayout.Button("Error log"))
//        {
//            IsShowError = !IsShowError;
//        }

//        GUI.color = GUI.contentColor;
//        if(GUILayout.Button("清除日志"))
//        {
//            Log.ListBugs.Clear();
//        }
//        GUILayout.EndHorizontal();
//        scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.Width(Screen.width - 20), GUILayout.Height(Screen.height - 150));


//        for(int i = 0; i < Log.ListBugs.Count; i++)
//        {
//            KeyValuePair<int, string> kv = Log.ListBugs[i];
//            int type = kv.Key;
//            string str = kv.Value;
//            if(type == 1 && IsShowLog)
//            {
//                GUI.color = Color.white;
//                GUILayout.Label(str, labelStyle);
//            }
//            if(type == 2 && IsShownNetLog)
//            {
//                GUI.color = Color.yellow;
//                GUILayout.Label(str, labelStyle);
//            }
//            if(type == 3 && IsShowError)
//            {
//                GUI.color = Color.red;
//                GUILayout.Label(str, labelStyle);
//            }
//        }
//        GUILayout.EndScrollView();
//        GUI.color = Color.white;
//    }


//    private void GUIPhone()
//    {
//        scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.Width(Screen.width), GUILayout.Height(Screen.height - 80));
//        GUILayout.Label("操作系统:  " + SystemInfo.operatingSystem, labelStyle);
//        GUILayout.Label("系统内存大小:  " + SystemInfo.systemMemorySize, labelStyle);
//        GUILayout.Label("设备模型:  " + SystemInfo.deviceModel, labelStyle);
//        GUILayout.Label("设备唯一标识符:  " + SystemInfo.deviceUniqueIdentifier, labelStyle);
//        GUILayout.Label("处理器数量:  " + SystemInfo.processorCount, labelStyle);
//        GUILayout.Label("主频:" + SystemInfo.processorFrequency, labelStyle);
//        GUILayout.Label("处理器类型:  " + SystemInfo.processorType, labelStyle);
//        GUILayout.Label("显卡标识符:  " + SystemInfo.graphicsDeviceID, labelStyle);
//        GUILayout.Label("显卡名称:  " + SystemInfo.graphicsDeviceName, labelStyle);
//        GUILayout.Label("显卡标识符:  " + SystemInfo.graphicsDeviceVendorID, labelStyle);
//        GUILayout.Label("显卡厂商:  " + SystemInfo.graphicsDeviceVendor, labelStyle);
//        GUILayout.Label("显卡版本:  " + SystemInfo.graphicsDeviceVersion, labelStyle);
//        GUILayout.Label("显存大小:  " + SystemInfo.graphicsMemorySize, labelStyle);
//        GUILayout.Label("显卡着色器级别:  " + SystemInfo.graphicsShaderLevel, labelStyle);
//        GUILayout.Label("是否图像效果:  " + SystemInfo.supportsImageEffects, labelStyle);
//        GUILayout.Label("是否支持内置阴影:  " + SystemInfo.supportsShadows, labelStyle);
//        GUILayout.Label("gameVer:  ", labelStyle);

//#if UNITY_IPHONE
//        GUILayout.Label("iphone generationId:  " + (int)(UnityEngine.iOS.Device.generation), labelStyle);
//#endif

//        GUILayout.EndScrollView();
//    }

//    private BaseForms GMToolsMaskTop;

//    private void Update()
//    {
//        if(GameSet.ShowLogView)
//        {
//            if(Input.GetKeyDown(KeyCode.Escape))
//            {
//                if(isShow)
//                {
//                    Hide();
//                }
//                else
//                {
//                    Show();
//                }
//                return;
//            }
//            if(Input.touchCount == 2)
//            {
//                if(!this.isDown)
//                {
//                    this.isDown = true;
//                    this.touchDounw[0] = Input.GetTouch(0).position.y;
//                    this.touchDounw[1] = Input.GetTouch(1).position.y;
//                }
//                this.touchPosition[0] = Input.GetTouch(0).position.y;
//                this.touchPosition[1] = Input.GetTouch(1).position.y;
//            }
//            if(Input.touchCount == 0 && this.isDown)
//            {
//                this.isDown = false;
//                float y1 = this.touchDounw[0] - this.touchPosition[0];
//                float y2 = this.touchDounw[1] - this.touchPosition[1];

//                if(y1 > Screen.height / 2 && y2 > Screen.height / 2)
//                {
//                    Show();
//                }
//                else
//                {
//                    Hide();
//                }
//            }
//        }
//    }

//    private void Show()
//    {
//        isShow = true;
//        //一个顶层UI遮罩 避免这个gui击穿触发下层的事件
//        if(GMToolsMaskTop == null)
//        {
//            GMToolsMaskTop = UIManager.Instance.OpenUI(UIConst.GMToolsMaskTop);
//            return;
//        }
//        GMToolsMaskTop.Show(true);
//    }

//    private void Hide()
//    {
//        currentSelect = 1;
//        isDown = false;
//        isShow = false;
//        if(GMToolsMaskTop != null)
//        {
//            GMToolsMaskTop.Show(false);
//        }
//    }

//    void OnDestroy()
//    {
//        logView = null;
//        labelStyle = null;
//        buttonStyle = null;
//        isDown = false;
//    }
//}
