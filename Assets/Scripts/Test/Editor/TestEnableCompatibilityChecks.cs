using System.Reflection;
using UnityEditor;
using UnityEngine;

public class TestEnableCompatibilityChecks {
       [MenuItem("Assets/TestEnableCompatibilityChecks")]
       public static void Test() {
              // 打开文件窗口
              var file = EditorUtility.OpenFilePanel("Select File", "", "");
              var request = AssetBundle.LoadFromFileAsync(file);
              var method = typeof(AssetBundleCreateRequest).GetMethod("SetEnableCompatibilityChecks", BindingFlags.Instance | BindingFlags.NonPublic);
              method.Invoke(request, new object[] { false });
              // var config = request.assetBundle.LoadAsset<SOGunFightModeSetting>("Assets/ToBundle/ScriptableObject/Screen/ModeSetting/SOGunFightModeSetting.asset");
              // Debug.Log(config.RoundPickItems[0].equipInfo.Length);
       }
}