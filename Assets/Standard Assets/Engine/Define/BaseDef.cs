/// <summary>
/// 基础定义文件，仅限engine目录下的内容使用
/// </summary>
using UnityEngine;

public class BaseDef
{
    public static string GameName = "Aver3";
    public const string MANIFEST_NAME = "data";         // unity 生成的清单，按文件夹名称生成
    public static string MD5_FILENAME = "md5";          // md5 文件名
    public static string CONFIG = "configData";
    public const string AB_SUFFIX = "unity3d";
    public const string ATLAS_NAME = "atlas_tp";
    public const string MUSIC_SUFFIX = "ogg";                   // 音乐后缀
    public const string SOUND_SUFFIX = "ogg";                   // 音乐后缀
    public const string FONT_SUFFIX = "ttf";                    // 字体后缀
}

public class ABFileInfo
{
    public string filename;
    public string md5;
    public int rawSize;         // 压缩前的文件大小
    public int compressSize;		// 压缩后的文件大小
}

public class GameNewVersionInfo
{
    public int appID;
    public string version;
    public string ip;
    public int port;
}
