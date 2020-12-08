using System.Collections.Generic;
using UnityEngine;

public class LoadModule : ModuleBase
{
    public int SyncCount = 6;           // 同步加载并发
    private float m_lastClearTime = 0;	// 上一次垃圾回收时间

    //货单
    AssetBundleManifest m_manifest = null;

    HashSet<string> m_bundleNames = new HashSet<string>();
    // 加载队列
    //private List<Loader> m_syncLoadList = new List<Loader>();   // 同步加载列表
    //private List<Loader> m_asyncLoaderList = new List<Loader>();// 异步加载列表

    //private Dictionary<string, BundleLoader> m_dicAllLoader = new Dictionary<string, BundleLoader>();

    // todo https://github.com/xasset/xasset
    public override void Init()
    {
        base.Init();
        SyncCount = SystemInfo.processorCount;
        if(true)//Config.Instance.UseAssetBundle
        {
            //LoadManifest();
        }
        else
        {
            m_bInit = true;
            Debug.Log("【LoadModule】初始化完成！");
        }
    }

}
