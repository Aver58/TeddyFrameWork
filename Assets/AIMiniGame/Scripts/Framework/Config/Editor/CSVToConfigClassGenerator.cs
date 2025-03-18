using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Generic;

public class CSVToConfigClassGenerator : Editor {
    [MenuItem("Assets/Generate Config Class From CSV", true)]
    private static bool ValidateGenerateConfigClass() {
        var selected = Selection.activeObject;
        if (selected != null) {
            string path = AssetDatabase.GetAssetPath(selected);
            return path.EndsWith(".csv");
        }
        return false;
    }

    [MenuItem("Assets/Generate Config Class From CSV")]
    private static void GenerateConfigClass() {
        var selected = Selection.activeObject;
        if (selected == null)
            return;

        string csvPath = AssetDatabase.GetAssetPath(selected);
        if (!csvPath.EndsWith(".csv")) {
            Debug.LogError("Selected file is not a CSV.");
            return;
        }

        string fileName = Path.GetFileNameWithoutExtension(csvPath);
        string className = ToPascalCase(fileName) + "Config";

        string[] lines = File.ReadAllLines(csvPath);
        if (lines.Length < 4) {
            Debug.LogError("CSV file does not have enough data.");
            return;
        }

        string[] headers = lines[0].Split(',');
        string[] types = lines[1].Split(',');
        // Skipping descriptions on line 2
        string[] firstDataLine = lines[3].Split(',');

        StringBuilder sb = new StringBuilder();
        sb.AppendLine("using System;");
        sb.AppendLine("using System.Collections.Generic;");
        sb.AppendLine();
        sb.AppendLine("[System.Serializable]");
        sb.AppendLine($"public class {className} : BaseConfig {{");

        for (int i = 0; i < headers.Length; i++) {
            string fieldName = ToCamelCase(headers[i]);
            string fieldType = types[i];
            sb.AppendLine($"    public {fieldType} {fieldName};");
        }

        sb.AppendLine();
        sb.AppendLine("    public override void Parse(string[] values, string[] headers) {");
        sb.AppendLine("        for (int i = 0; i < headers.Length; i++) {");
        sb.AppendLine("            var header = headers[i].Replace(\"\\r\", \"\");");
        sb.AppendLine("            switch (header) {");

        for (int i = 0; i < headers.Length; i++) {
            string fieldName = ToCamelCase(headers[i]);
            string fieldType = types[i];
            sb.AppendLine($"                case \"{headers[i]}\":");
            if (fieldType.EndsWith("[]")) {
                string elementType = fieldType.Substring(0, fieldType.Length - 2);
                if (elementType == "string") {
                    sb.AppendLine($"                    {fieldName} = values[i].Split(';');");
                } else {
                    sb.AppendLine($"                    {fieldName} = Array.ConvertAll(values[i].Split(';'), {elementType}.Parse);");
                }
            } else if (fieldType == "string") {
                sb.AppendLine($"                    {fieldName} = values[i];");
            } else {
                sb.AppendLine($"                    {fieldName} = {fieldType}.Parse(values[i]);");
            }
            sb.AppendLine("                    break;");
        }

        sb.AppendLine("            }");
        sb.AppendLine("        }");
        sb.AppendLine("    }");
        sb.AppendLine();
        sb.AppendLine($"    private static Dictionary<string, {className}> cachedConfigs;");
        sb.AppendLine($"    public static {className} Get(string key) {{");
        sb.AppendLine("        if (cachedConfigs == null) {");
        sb.AppendLine($"            cachedConfigs = ConfigManager.Instance.LoadConfig<{className}>(\"{fileName}.csv\");");
        sb.AppendLine("        }");
        sb.AppendLine();
        sb.AppendLine($"        {className} config = null;");
        sb.AppendLine("        if (cachedConfigs != null) {");
        sb.AppendLine("            cachedConfigs.TryGetValue(key, out config);");
        sb.AppendLine("        }");
        sb.AppendLine();
        sb.AppendLine("        return config;");
        sb.AppendLine("    }");
        sb.AppendLine();
        sb.AppendLine("    public static List<string> GetKeys() {");
        sb.AppendLine("        if (cachedConfigs == null) {");
        sb.AppendLine($"            cachedConfigs = ConfigManager.Instance.LoadConfig<{className}>(\"{fileName}.csv\");");
        sb.AppendLine("        }");
        sb.AppendLine();
        sb.AppendLine("        return cachedConfigs != null ? new List<string>(cachedConfigs.Keys) : new List<string>();");
        sb.AppendLine("    }");
        sb.AppendLine("}");

        string directoryPath = Application.dataPath + @"\AIMiniGame\Scripts\Framework\Config\GeneratedConfigs\";
        string outputPath = Path.Combine(directoryPath, className + ".cs");
        File.WriteAllText(outputPath, sb.ToString());

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log($"Generated config class: {outputPath}");
    }

    private static string ToPascalCase(string str) {
        return Regex.Replace(str, "(^|_)([a-z])", match => match.Groups[2].Value.ToUpper());
    }

    private static string ToCamelCase(string str) {
        str = ToPascalCase(str);
        return char.ToLower(str[0]) + str.Substring(1);
    }
}
