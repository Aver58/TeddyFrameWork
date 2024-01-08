using System.Collections.Generic;
using UnityEngine;

public class TestReferenceValuePass : MonoBehaviour
{
    public enum HighlightType {
        LowHpKill = 1,
        ContinuousKill = 2,
        CarrierKill = 3,
        KillALot = 4,
    }

    public struct OneVideo {
        public HighlightType type;
        public List<VideoSlice> clips;
    }

    public struct VideoSlice {
        public int s;
        public int e;
    }

    private Dictionary<int, Dictionary<HighlightType, OneVideo>> roleHighlightDataMap;

    // Start is called before the first frame update
    void Start()
    {
        roleHighlightDataMap = new Dictionary<int, Dictionary<HighlightType, OneVideo>>();
        roleHighlightDataMap.Add(0, new Dictionary<HighlightType, OneVideo>() {
            {HighlightType.LowHpKill, new OneVideo() {
                type = HighlightType.LowHpKill,
                clips = new List<VideoSlice>() {
                    { new VideoSlice() {
                        s = 0,
                        e = 1,
                    }}
                }
            }}
        });

        Debug.Log(roleHighlightDataMap.Count);
        Test(roleHighlightDataMap);
        Debug.Log(roleHighlightDataMap.Count);

        List<VideoSlice> a = new List<VideoSlice>() {
            {new VideoSlice() { s = 0, e = 1, } },
            {new VideoSlice() { s = 1, e = 2, } },

        };

        List<VideoSlice> b = new List<VideoSlice>();        b.AddRange(a);

    }

    void Test(Dictionary<int, Dictionary<HighlightType, OneVideo>> map)
    {
        // map.Clear(); 有效的
        map = null;// 无效的
    }
}
