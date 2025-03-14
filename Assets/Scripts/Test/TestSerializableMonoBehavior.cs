using System;
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

    [Serializable]
    public class TurnBasedMode1v1Record {
        public bool IsWin;
        public int BlueScore;
        public int RedScore;
        public int GradeType;
        public int ContinueWinTimes;
        public long PlayerId;

        public override string ToString() {
            return $"IsWin {IsWin} BlueScore {BlueScore} RedScore {RedScore} GradeType {GradeType} " +
                   $"ContinueWinTimes {ContinueWinTimes} PlayerId {PlayerId}";
        }
    }

    public class MyClass {
        public List<TurnBasedMode1v1Record> SaveData;
    }

    public struct MyStruct1 {
        public bool IsWin;
    }

    private void Start() {
        var struct1 = new MyStruct1() {
        };
        Debug.LogError(struct1.IsWin);

        MyClass c = new MyClass();
        c.SaveData = new List<TurnBasedMode1v1Record>();
        c.SaveData.Add(new TurnBasedMode1v1Record() {
            IsWin = true,
            BlueScore = 1,
            RedScore = 2,
            GradeType = 3,
            ContinueWinTimes = 4,
            PlayerId = 5
        });
        var newJson = JsonUtility.ToJson(c);
        var records2 = LitJson.JsonMapper.ToObject<MyClass>(newJson);
        Debug.LogError($"newJson: {newJson}");
        Debug.LogError($"newJson: {records2.SaveData[0].BlueScore}");

        var records = new List<TurnBasedMode1v1Record>();
        records.Add(new TurnBasedMode1v1Record() {
            IsWin = true,
            BlueScore = 1,
            RedScore = 2,
            GradeType = 3,
            ContinueWinTimes = 4,
            PlayerId = 5
        });

        newJson = LitJson.JsonMapper.ToJson(records);
        var records3 = LitJson.JsonMapper.ToObject<List<TurnBasedMode1v1Record>>(newJson);
        Debug.LogError($"newJson: {newJson}");
        Debug.LogError($"newJson: {records3[0].BlueScore}");


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

        // var json = Newtonsoft.Json.JsonConvert.SerializeObject(myStruct);
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