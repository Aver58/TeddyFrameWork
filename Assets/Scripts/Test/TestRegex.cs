using System;
using System.Text.RegularExpressions;
using UnityEngine;

class TestRegex : MonoBehaviour {
    private void Awake() {
        Main();
    }

    static void Main()
    {
        string inputString = "Hello (World)! This is (a) test.";

        // 使用正则表达式替换括号及其内容
        string resultString = RemoveParenthesesAndContent(inputString);

        // 输出结果
        Debug.Log(resultString);
    }

    static string RemoveParenthesesAndContent(string input)
    {
        // 使用正则表达式匹配括号及其内容，并替换为空字符串
        string pattern = @"\([^)]*\)";
        string result = Regex.Replace(input, pattern, "");

        return result;
    }
}