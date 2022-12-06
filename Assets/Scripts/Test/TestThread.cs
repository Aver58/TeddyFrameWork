using System.Diagnostics;
using System.Threading;
using UnityEngine;
using UnityEngine.Profiling;
using Debug = UnityEngine.Debug;

struct TestStruct {
    public int i;
}

public class TestThread : MonoBehaviour {
    private TestStruct s;
    void Start() {
        s = new TestStruct();
        s.i = 0;

        // var name = Process.GetCurrentProcess().ProcessName;
        // var ramCounter =  new PerformanceCounter("Process", "Working Set", name);
        // var ram = ramCounter.NextValue();
        Profiler.BeginSample("TestThread");
        // 测试一下，结构体在多线程中使用，是否会复制内存
        for (int i = 0; i < 100; i++) {
            ThreadPool.QueueUserWorkItem((object state) => {
                s.i++;
                // Debug.Log(s.i);
            });
        }
        
        Profiler.EndSample();

        // Debug.Log("final:"+s.i);
    }

    void Update()
    {
        
    }
}
