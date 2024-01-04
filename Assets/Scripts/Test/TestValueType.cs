using System.Collections.Generic;
using UnityEngine;

public class VideoSliceInfo {
    public int Start;
    public int End;
}

public class TestValueType : MonoBehaviour{
    private void Start() {
        var list = new List<VideoSliceInfo>();
        list.Add(new VideoSliceInfo() {
            Start = 111,
            End = 222,
        });

        var item = list[0];
        item.Start = 110;

        Debug.Log(list[0].Start);
        Debug.Log(item.GetType());
    }
}