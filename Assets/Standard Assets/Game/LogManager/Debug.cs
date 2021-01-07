using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 系统日志模块
/// </summary>
public static class Debug
{
    public static bool EnableLog = true;    // 是否启用日志，仅可控制普通级别的日志的启用与关闭，LogError和LogWarn都是始终启用的。
    public static bool EnableSave = false;  // 是否允许保存日志，即把日志写入到文件中

    public static bool IsOpenNetPackageLog = false;// 是否开放网络日志
    public static string LogFileDir = null;
    public static string LogFileName = "";
    public static string Prefix = "> ";     // 用于与Unity默认的系统日志做区分。本日志系统输出的日志头部都会带上这个标记。
    public static StreamWriter LogFileWriter = null;

    /// <summary>
    /// 日志输出最大行数
    /// </summary>
    private static readonly int LineCount = 100;
    //日志列表
    private static List<KeyValuePair<int, string>> ListBugs = new List<KeyValuePair<int, string>>();

    //第一次执行打印log
    private static bool FirstLogTag = true;

    private static string GetLogTime()
    {
        string str = "";
        str = DateTime.Now.ToString("HH:mm:ss.fff") + " ";

        return str;
    }
    private static void AddLogToList(int key, string str)
    {
        if(ListBugs.Count > LineCount)
            ListBugs.RemoveAt(0);

        ListBugs.Add(new KeyValuePair<int, string>(key, str));
    }

    public static void Log(string message, params object[] args)
    {
        if(!EnableLog)
            return;

        if(args != null && args.Length > 0)
            message = string.Format(message, args);
        string str = Prefix + GetLogTime() + message;
        AddLogToList(1,str);
        UnityEngine.Debug.Log(str, null);
        LogToFile("[I]" + str, false);
    }

    public static void RawLog(object message)
    {
        UnityEngine.Debug.Log(message);
    }

    public static void LogException(Exception message)
    {
        string str = GetLogTime() + message.Message;
        AddLogToList(2, str);
        UnityEngine.Debug.LogException(message);
        LogToFile("[E]" + str, true);
    }
    
    public static void LogError(string message, params object[] args)
    {
        if(args != null && args.Length > 0)
        {
            message = string.Format(message, args);
        }
        string str = Prefix + GetLogTime() + message;
        AddLogToList(3, str);

        UnityEngine.Debug.LogError(str, null);
        LogToFile("[E]" + str, true);
    }

    public static void LogWarning(string message, params object[] args)
    {
        if(args != null && args.Length > 0)
        {
            message = string.Format(message, args);
        }
        string str = Prefix + GetLogTime() + message;
        AddLogToList(4, str);
        UnityEngine.Debug.LogWarning(str, null);
        LogToFile("[W]" + str, true);
    }

    private static void Warning(object message)
    {
        string str = Prefix + message;
        AddLogToList(5, str);
        UnityEngine.Debug.LogWarning(str, null);
        LogToFile("[W]" + str, false);
    }

    #region 战斗

    #endregion
    
    #region Format

    public static void LogFormat(string message, params object[] args)
    {
        Log(message, args);
    }

    public static void LogErrorFormat(string message, params object[] args)
    {
        LogError(message, args);
    }

    public static void LogWarningFormat(string message, params object[] args)
    {
        LogWarning(message, args);
    }

    #endregion

    /// <summary>
    /// 将日志写入到文件中
    /// </summary>
    /// <param name="message"></param>
    /// <param name="EnableStack"></param>
    private static void LogToFile(string message, bool EnableStack = false)
    {
        if(!EnableSave)
            return;

        if(LogFileWriter == null)
        {
            LogFileName = DateTime.Now.GetDateTimeFormats('s')[0].ToString();
            LogFileName = LogFileName.Replace("-", "_");
            LogFileName = LogFileName.Replace(":", "_");
            LogFileName = LogFileName.Replace(" ", "");
            LogFileName = LogFileName + ".log";
            if(string.IsNullOrEmpty(LogFileDir))
            {
                try
                {
#if UNITY_EDITOR
                    if(!Directory.Exists("d:/UnityLog"))
                    {
                        Directory.CreateDirectory("d:/UnityLog");
                    }
                    LogFileDir = "d:/UnityLog/";
#else
                        if ((Application.platform == RuntimePlatform.Android) || (Application.platform == RuntimePlatform.IPhonePlayer))
                        {
                            LogFileDir = Application.persistentDataPath + "/DebuggerLog/";
                        }
#endif
                }
                catch(Exception exception)
                {
                    UnityEngine.Debug.Log(Prefix + "获取 Application.persistentDataPath 报错！" + exception.Message, null);
                    return;
                }
            }
            string path = LogFileDir + LogFileName;
            try
            {
                if(!Directory.Exists(LogFileDir))
                {
                    Directory.CreateDirectory(LogFileDir);
                }
                LogFileWriter = File.AppendText(path);
                LogFileWriter.AutoFlush = true;
            }
            catch(Exception exception2)
            {
                LogFileWriter = null;
                UnityEngine.Debug.Log("LogToCache() " + exception2.Message + exception2.StackTrace, null);
                return;
            }
        }
        if(LogFileWriter != null)
        {
            try
            {
                if(FirstLogTag)
                {
                    FirstLogTag = false;
                    PhoneSystemInfo(LogFileWriter);
                }
                LogFileWriter.WriteLine(message);
                if(EnableStack)
                {
                    LogFileWriter.WriteLine(StackTraceUtility.ExtractStackTrace());
                }
            }
            catch(Exception)
            {
            }
        }
    }

    private static void PhoneSystemInfo(StreamWriter sw)
    {
        sw.WriteLine("*********************************************************************************************************start");
        sw.WriteLine("By " + SystemInfo.deviceName);
        DateTime now = DateTime.Now;
        sw.WriteLine(string.Concat(new object[] { now.Year.ToString(), "年", now.Month.ToString(), "月", now.Day, "日  ", now.Hour.ToString(), ":", now.Minute.ToString(), ":", now.Second.ToString() }));
        sw.WriteLine();
        sw.WriteLine("操作系统:  " + SystemInfo.operatingSystem);
        sw.WriteLine("系统内存大小:  " + SystemInfo.systemMemorySize);
        sw.WriteLine("设备模型:  " + SystemInfo.deviceModel);
        sw.WriteLine("设备唯一标识符:  " + SystemInfo.deviceUniqueIdentifier);
        sw.WriteLine("处理器数量:  " + SystemInfo.processorCount);
        sw.WriteLine("处理器类型:  " + SystemInfo.processorType);
        sw.WriteLine("显卡标识符:  " + SystemInfo.graphicsDeviceID);
        sw.WriteLine("显卡名称:  " + SystemInfo.graphicsDeviceName);
        sw.WriteLine("显卡标识符:  " + SystemInfo.graphicsDeviceVendorID);
        sw.WriteLine("显卡厂商:  " + SystemInfo.graphicsDeviceVendor);
        sw.WriteLine("显卡版本:  " + SystemInfo.graphicsDeviceVersion);
        sw.WriteLine("显存大小:  " + SystemInfo.graphicsMemorySize);
        sw.WriteLine("显卡着色器级别:  " + SystemInfo.graphicsShaderLevel);
        //sw.WriteLine("是否图像效果:  " + SystemInfo.supportsImageEffects);
        sw.WriteLine("是否支持内置阴影:  " + SystemInfo.supportsShadows);
        sw.WriteLine("*********************************************************************************************************end");
        sw.WriteLine("LogInfo:");
        sw.WriteLine();
    }

    public static void CloseLog()
    {
        if(LogFileWriter != null)
        {
            try
            {
                LogFileWriter.Flush();
                LogFileWriter.Close();
                LogFileWriter.Dispose();
                LogFileWriter = null;
            }
            catch(Exception)
            {
            }
        }
    }

    public enum LogPackageType
    {
        ClientToServer = 0,
        ServerToClient = 1
    }

    public static void ShowPackageInfo(LogPackageType type, string info, int cmd)
    {
        if(IsOpenNetPackageLog)
        {
            if(type == LogPackageType.ClientToServer)
            {
                Debug.Warning(string.Format("{0} <color=#ffff00>{1}</color>  - {2}", info, cmd.ToString(), DateTime.Now.ToString()));
            }
            else
            {
                Debug.Warning(string.Format("{0} <color=#01c00f>{1}</color>  - {2}", info, cmd.ToString(), DateTime.Now.ToString()));
            }
        }
    }
}