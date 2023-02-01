using UnityEngine;
using System.Collections.Generic;
using System;
using System.Threading;
using System.Linq;

/*
    解决 Internal_Create  can only be called from the main thread.
    其他transform.position，Texture.Apply()等等都不能在分线程里运行。
    
    有了Loom这个工具类，在很多涉及UnityEngine对象的耗时计算还是可以得到一个解决方法的：
               如在场景中用A*算法进行大量的数据计算
               变形网格中操作大量的顶点
               持续的要运行上传数据到服务器
               二维码识别等图像处理
 */
public class Loom : MonoBehaviour {
    public static int maxThreads = 8;
    static int numThreads;

    private static Loom _current;
    private int _count;

    public static Loom Current {
        get {
            Initialize();
            return _current;
        }
    }

    void Awake() {
        _current = this;
        initialized = true;
    }

    static bool initialized;

    static void Initialize() {
        if (!initialized) {
            if (!Application.isPlaying)
                return;
            initialized = true;
            var g = new GameObject("Loom");
            _current = g.AddComponent<Loom>();
        }
    }

    private List<Action> _actions = new List<Action>();

    public struct DelayedQueueItem {
        public float time;
        public Action action;
    }

    private List<DelayedQueueItem> _delayed = new List<DelayedQueueItem>();

    List<DelayedQueueItem> _currentDelayed = new List<DelayedQueueItem>();

    public static void QueueOnMainThread(Action action) {
        QueueOnMainThread(action, 0f);
    }

    public static void QueueOnMainThread(Action action, float time) {
        if (time != 0) {
            lock (Current._delayed) {
                Current._delayed.Add(new DelayedQueueItem {time = Time.time + time, action = action});
            }
        } else {
            lock (Current._actions) {
                Current._actions.Add(action);
            }
        }
    }

    public static Thread RunAsync(Action a) {
        Initialize();
        while (numThreads >= maxThreads) {
            Thread.Sleep(1);
        }

        Interlocked.Increment(ref numThreads);
        ThreadPool.QueueUserWorkItem(RunAction, a);
        return null;
    }

    private static void RunAction(object action) {
        try {
            ((Action)action)();
        } catch {
        } finally {
            Interlocked.Decrement(ref numThreads);
        }
    }


    void OnDisable() {
        if (_current == this) {
            _current = null;
        }
    }


    // Use this for initialization  
    void Start() {
    }

    List<Action> _currentActions = new List<Action>();

    // Update is called once per frame  
    void Update() {
        lock (_actions) {
            _currentActions.Clear();
            _currentActions.AddRange(_actions);
            _actions.Clear();
        }

        foreach (var a in _currentActions) {
            a();
        }

        lock (_delayed) {
            _currentDelayed.Clear();
            _currentDelayed.AddRange(_delayed.Where(d => d.time <= Time.time));
            foreach (var item in _currentDelayed)
                _delayed.Remove(item);
        }

        foreach (var delayed in _currentDelayed) {
            delayed.action();
        }
    }
}