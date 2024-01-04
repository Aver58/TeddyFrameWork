using System.Collections.Generic;
using UnityEngine;

public class TestSerializableMonoBehavior : MonoBehaviour{
    private struct PositionInfo {
        public float x;
        public float y;
        public float radius;
    }

    private struct MyStruct {
        // public Vector3 test;
        public List<PositionInfo> wulinInns; // 武林客栈位置参数
    }

    public enum HighlightType {
        LowHpKill = 1,
        ContinuousKill = 2,
        CarKill = 3,
        KillALot = 4,
    }

    // 一个视频片段
    public struct VideoSlice {
        public int Start;
        public int End;
    }

    // 一个视频集合
    public struct OneVideo {
        public int Priority;
        public HighlightType Type;
        public List<VideoSlice> SliceList;
    }


    private void Start() {
        var myStruct = new MyStruct {
            // test = Vector3.one,
            wulinInns = new List<PositionInfo>() {
                new PositionInfo {
                    x = 1,
                    y = 2,
                    radius = 3
                },
                new PositionInfo {
                    x = 4,
                    y = 5,
                    radius = 6
                }
            }
        };

        var json = Newtonsoft.Json.JsonConvert.SerializeObject(myStruct);
        // var json = MiniJSON.jsonEncode(myStruct);
        // Debug.LogError(json);

        var test = new List<OneVideo>() {
            new OneVideo() {
                Priority = 3,
                SliceList = new List<VideoSlice>() {
                    new VideoSlice() {
                        Start = 0,
                        End = 10,
                    },
                    new VideoSlice() {
                        Start = 21,
                        End = 30,
                    },
                }
            },
            {
                new OneVideo() {
                    Priority = 3,
                    SliceList = new List<VideoSlice>() {
                        new VideoSlice() {
                            Start = 11,
                            End = 20,
                        }
                    }
                }
            }
        };
        var jsonString = LitJson.JsonMapper.ToJson(test);
        Debug.LogError(jsonString);
    }
}