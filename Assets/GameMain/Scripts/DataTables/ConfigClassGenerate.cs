using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;

public class ConfigClassGenerate {
    private const string retract1 = "\t";
    private const string retract2 = "\t\t";
    private const string retract3 = "\t\t\t";
    private const string retract4 = "\t\t\t\t";
    private const string retract5 = "\t\t\t\t\t";
    private static string configClassPath = "Assets/Script/Config";
    private static string templatePath = "Assets/Editor/Template/ConfigTemplate.txt";
    private const string CSharpCodeTemplateFileName = "Assets/GameMain/Scripts/DataTables/DataTableGenerator/ConfigTemplate.txt";
    private const string CSharpCodePath = "Assets/GameMain/Scripts/DataTables/DataTableGenerator/";

    [MenuItem("Assets/Generate DataTables")]
    public static void GenerateConfigClass() {
        if (Selection.objects != null) {
            foreach (var o in Selection.objects) {
                var path = AssetDatabase.GetAssetPath(o.GetInstanceID());
                var fileName = Path.GetFileName(path);
                if (path.EndsWith(".txt") || path.EndsWith(".TXT")) {
                    CreateConfigClass(new FileInfo(path));
                }
            }

            AssetDatabase.Refresh();
        }
    }

    public static string filedContent = string.Empty;
    public static string readContent = string.Empty;
    public static string propertyContent = string.Empty;
    public static void CreateConfigClass(FileInfo fileInfo) {
        filedContent = string.Empty;
        readContent = string.Empty;
        propertyContent = string.Empty;
        var lines = File.ReadAllLines(fileInfo.FullName);
        if (lines.Length > 2) {
            var typeLine = lines[0];
            var fieldLine = lines[1];
            var types = typeLine.Split('\t');
            var fields = fieldLine.Split('\t');
            var min = Math.Min(types.Length, fields.Length);
            var fieldFulls = new List<string>();
            var readFulls = new List<string>();
            var propertyFulls = new List<string>();

            int index = 0;
            for (int j = 0; j < min; j++) {
                var type = types[j];
                var field = fields[j];
                var fieldString = GetField(type, field);
                var readString = GetRead(type, field, index, j);
                var propertyString = GetProperty(type, field, j);
                if (!string.IsNullOrEmpty(fieldString)) {
                    fieldFulls.Add(fieldString);
                }

                if (!string.IsNullOrEmpty(readString)) {
                    index++;
                    readFulls.Add(readString);
                }

                // if (!string.IsNullOrEmpty(propertyString)) {
                //     propertyFulls.Add(propertyString);
                // }
            }

            filedContent = string.Join("\r\n\t", fieldFulls.ToArray());
            readContent = string.Join("\r\n\t\t\t", readFulls.ToArray());
            propertyContent = string.Join("\r\n\t", propertyFulls.ToArray());
            CreatNewConfigClass(fileInfo.Name.Substring(0, fileInfo.Name.IndexOf('.')));
        }
    }
    
    private static string GetField(string type, string field) {
        field = field.Replace(" ", "");
        if (type.Contains("int[]")) {
            return Concat("public readonly int[] ", field.Trim(), ";");
        } else if (type.Contains("Int2[]")) {
            return Concat("public readonly Int2[] ", field.Trim(), ";");
        } else if (type.Contains("Int3[]")) {
            return Concat("public readonly Int3[] ", field.Trim(), ";");
        } else if (type.Contains("float[]")) {
            return Concat("public readonly float[] ", field.Trim(), ";");
        } else if (type.Contains("string[]")) {
            return Concat("public readonly string[] ", field.Trim(), ";");
        } else if (type.Contains("Vector3[]")) {
            return Concat("public readonly Vector3[] ", field.Trim(), ";");
        } else if (type.Contains("int")) {
            return Concat("public readonly int ", field.Trim(), ";");
        } else if (type.Contains("ushort")) {
            return Concat("public readonly ushort ", field.Trim(), ";");
        } else if (type.Contains("long")) {
            return Concat("public readonly long ", field.Trim(), ";");
        } else if (type.Contains("long[]")) {
            return Concat("public readonly long[] ", field.Trim(), ";");
        } else if (type.Contains("float")) {
            return Concat("public readonly float ", field.Trim(), ";");
        } else if (type.Contains("string")) {
            return Concat("public readonly string ", field, ";");
        } else if (type.Contains("Vector3")) {
            return Concat("public readonly Vector3 ", field.Trim(), ";");
        } else if (type.Contains("bool")) {
            return Concat("public readonly bool ", field.Trim(), ";");
        } else if (type.Contains("Int2")) {
            return Concat("public readonly Int2 ", field.Trim(), ";");
        } else if (type.Contains("Int3")) {
            return Concat("public readonly Int3 ", field.Trim(), ";");
        }else if (type.Contains("MagicFloat")) {
            return Concat("public readonly MagicType.MagicFloat ", field.Trim(), ";");
        } else if (type.Contains("MagicInt")) {
            return Concat("public readonly MagicType.MagicInt ", field.Trim(), ";");
        } else {
            return string.Empty;
        }
    }

    private static string GetRead(string type, string field, int index, int dataIndex) {
        field = field.Replace(" ", "");
        if (type.Contains("int[]")) {
            var line1 = Concat("string[] ", field, "StringArray", " = ", "tables", "[", index, "]", ".Trim().Split(DataTableExtension.splitSeparator,StringSplitOptions.RemoveEmptyEntries);", "\n");
            var line2 = Concat(retract3, field, " = ", "new int", "[", field, "StringArray.Length]", ";", "\n");
            var line3 = Concat(retract3, "for (int i=0;i<", field, "StringArray", ".Length", ";", "i++", ")", "\n");
            var line4 = Concat(retract3, "{\n");
            var line5 = Concat(retract4, " int.TryParse(", field, "StringArray", "[i]", ",", "out ", field, "[i]", ")", ";", "\n");
            var line6 = Concat(retract3, "}");

            return Concat(line1, line2, line3, line4, line5, line6);
        } else if (type.Contains("Int2[]")) {
            var line1 = Concat("string[] ", field, "StringArray", " = ", "tables", "[", index, "]", ".Trim().Split(DataTableExtension.splitSeparator,StringSplitOptions.RemoveEmptyEntries);", "\n");
            var line2 = Concat(retract3, field, " = ", "new Int2", "[", field, "StringArray.Length]", ";", "\n");
            var line3 = Concat(retract3, "for (int i=0;i<", field, "StringArray", ".Length", ";", "i++", ")", "\n");
            var line4 = Concat(retract3, "{\n");
            var line5 = Concat(retract4, " Int2.TryParse(", field, "StringArray", "[i]", ",", "out ", field, "[i]", ")", ";", "\n");
            var line6 = Concat(retract3, "}");

            return Concat(line1, line2, line3, line4, line5, line6);
        } else if (type.Contains("Int3[]")) {
            var line1 = Concat("string[] ", field, "StringArray", " = ", "tables", "[", index, "]", ".Trim().Split(DataTableExtension.splitSeparator,StringSplitOptions.RemoveEmptyEntries);", "\n");
            var line2 = Concat(retract3, field, " = ", "new Int3", "[", field, "StringArray.Length]", ";", "\n");
            var line3 = Concat(retract3, "for (int i=0;i<", field, "StringArray", ".Length", ";", "i++", ")", "\n");
            var line4 = Concat(retract3, "{\n");
            var line5 = Concat(retract4, " Int3.TryParse(", field, "StringArray", "[i]", ",", "out ", field, "[i]", ")", ";", "\n");
            var line6 = Concat(retract3, "}");

            return Concat(line1, line2, line3, line4, line5, line6);
        } else if (type.Contains("float[]")) {
            var line1 = Concat("string[] ", field, "StringArray", " = ", "tables", "[", index, "]", ".Trim().Split(DataTableExtension.splitSeparator,StringSplitOptions.RemoveEmptyEntries);", "\n");
            var line2 = Concat(retract3, field, " = ", "new float", "[", field, "StringArray.Length", "]", ";", "\n");
            var line3 = Concat(retract3, "for (int i=0;i<", field, "StringArray", ".Length", ";", "i++", ")", "\n");
            var line4 = Concat(retract3, "{\n");
            var line5 = Concat(retract4, " float.TryParse(", field, "StringArray", "[i]", ",", "out ", field, "[i]", ")", ";", "\n");
            var line6 = Concat(retract3, "}");

            return Concat(line1, line2, line3, line4, line5, line6);
        } else if (type.Contains("string[]")) {
            var line1 = Concat(field, " = ", "tables", "[", index, "]", ".Trim().Split(DataTableExtension.splitSeparator,StringSplitOptions.RemoveEmptyEntries);");
            return line1;
        } else if (type.Contains("Vector3[]")) {
            var line1 = Concat("string[] ", field, "StringArray", " = ", "tables", "[", index, "]", ".Trim().Split(DataTableExtension.splitSeparator,StringSplitOptions.RemoveEmptyEntries);", "\n");
            var line2 = Concat(retract3, field, " = ", "new Vector3", "[", field, "StringArray.Length", "]", ";", "\n");
            var line3 = Concat(retract3, "for (int i=0;i<", field, "StringArray", ".Length", ";", "i++", ")", "\n");
            var line4 = Concat(retract3, "{\n");
            var line5 = Concat(retract4, field, "[i]", "=", field, "StringArray", "[i]", ".Vector3Parse()", ";", "\n");
            var line6 = Concat(retract3, "}");

            return Concat(line1, line2, line3, line4, line5, line6);
        } else if (type.Contains("int")) {
            return Concat("int.TryParse(tables", "[", index, "]", ",", "out ", field, ")", "; ");
        } else if (type.Contains("ushort")) {
            return Concat("ushort.TryParse(tables", "[", index, "]", ",", "out ", field, ")", "; ");
        } else if (type.Contains("float")) {
            return Concat("float.TryParse(tables", "[", index, "]", ",", "out ", field, ")", "; ");
        } else if (type.Contains("string")) {
            return Concat(field, " = ", "tables", "[", index, "]", ";");
        } else if (type.Contains("Vector3")) {
            return Concat(field, "=", "tables", "[", index, "]", ".Vector3Parse()", ";");
        } else if (type.Contains("bool")) {
            var line1 = Concat("var ", field, "Temp", " = 0", ";", "\n");
            var line2 = Concat(retract3, "int.TryParse(tables", "[", index, "]", ",", "out ", field, "Temp", ")", "; ", "\n");
            var line3 = Concat(retract3, field, "=", field, "Temp", "!=0", ";");
            return Concat(line1, line2, line3);
        } else if (type.Contains("Int2")) {
            return Concat("Int2.TryParse(tables", "[", index, "]", ",", "out ", field, ")", "; ");
        } else if (type.Contains("Int3")) {
            return Concat("Int3.TryParse(tables", "[", index, "]", ",", "out ", field, ")", "; ");
        } else if (type.Contains("MagicFloat")) {
            return Concat("MagicType.MagicFloat.TryParse(tables", "[", index, "]", ",", "out ", field, ")", "; ");
        } else if (type.Contains("MagicInt")) {
            return Concat("MagicType.MagicInt.TryParse(tables", "[", index, "]", ",", "out ", field, ")", "; ");
        }else if (type.Contains("long")) {
            return Concat("long.TryParse(tables", "[", index, "]", ",", "out ", field, ")", "; ");
        } else {
            return string.Empty;
        }
    }

    private static string GetProperty(string type, string field, int dataIndex) {
        if (type.Contains("string")) {
            var propertyField = Concat(field.Substring(0, 1).ToUpper(), field.Substring(1));
            var line1 = Concat("public string ", propertyField, $" => {field};\n");
            return line1;
        }
        return string.Empty;
    }

    private static readonly object lockObject = new object();
    private static readonly StringBuilder stringBuilder = new StringBuilder();
    private static string Concat(params object[] objects) {
        if (objects == null) {
            return string.Empty;
        }
        
        lock (lockObject) {
            stringBuilder.Length = 0;
            foreach (var item in objects) {
                if (item != null) {
                    stringBuilder.Append(item);
                }
            }
            return stringBuilder.ToString();
        }
    }
    
    internal static UnityEngine.Object CreateScriptAssetFromTemplate(string pathName, string resourceFile) {
        var fullPath = Path.GetFullPath(pathName);

        var streamReader = new StreamReader(resourceFile);
        var text = streamReader.ReadToEnd();
        streamReader.Close();
        var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(pathName);
        text = Regex.Replace(text, "#ClassName*#", fileNameWithoutExtension);
        text = Regex.Replace(text, "#DateTime#", System.DateTime.Now.ToLongDateString());
        text = Regex.Replace(text, "#Field#", filedContent);
        text = Regex.Replace(text, "#Read#", readContent);
        text = Regex.Replace(text, "#Property#", propertyContent);
        text = Regex.Replace(text, "#FileName#", fileNameWithoutExtension.Substring(0, fileNameWithoutExtension.Length - 6));

        var encoderShouldEmitUTF8Identifier = true;
        var throwOnInvalidBytes = false;
        var encoding = new UTF8Encoding(encoderShouldEmitUTF8Identifier, throwOnInvalidBytes);
        var append = false;
        var streamWriter = new StreamWriter(fullPath, append, encoding);
        streamWriter.Write(text);
        streamWriter.Close();
        AssetDatabase.ImportAsset(pathName);
        return AssetDatabase.LoadAssetAtPath(pathName, typeof(UnityEngine.Object));
    }
    
    private static void CreatNewConfigClass(string name) {
        var newConfigPath = $"{CSharpCodePath}/{name}.cs";
        AssetDatabase.DeleteAsset(newConfigPath);
        UnityEngine.Object o = CreateScriptAssetFromTemplate(newConfigPath, CSharpCodeTemplateFileName);
        ProjectWindowUtil.ShowCreatedAsset(o);
    }
}
