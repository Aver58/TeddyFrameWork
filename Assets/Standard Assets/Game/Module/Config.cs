#region Copyright © 2020 Aver. All rights reserved.
/*
=====================================================
 AverFrameWork v1.0
 Filename:    Config.cs
 Author:      Zeng Zhiwei
 Time:        2020/12/14 11:42:38
=====================================================
*/
#endregion

[System.Serializable]
public class Config
{
    public static Config Instance { get; private set; }
    public bool UseAssetBundle = false;

    public void Init()
    {
        Instance = this;

    }
}