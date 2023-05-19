// using System.Collections.Generic;
// using UnityEngine;
//
// public enum PlayMode {
//     None,
//     Record,//录制
//     Play,//播放录像
//     Invert,//倒放
// }
//
// /// <summary>
// /// 【增量快照同步】
// /// 1.数据帧和渲染帧分离，主同步数据帧
// /// 2.增量快照同步
// /// 3.Checkpoint：存档点，即一个完整的世界快照（类似单机游戏中的存档），通过这个快照可以完全的回复当时的游戏状态。
// /// 4.倒放问题：生成是销毁，销毁是生成
// /// 5.本地处理与同步，有些东西不是给同步用的，比如自己的血量状态，这种只是为了回放处理，感觉可以通过录制本地消息来处理，UI需要是通过消息发生事件来驱动显示的，所以我们可以来录制这个消息
// /// 6.高延迟问题怎么处理？一顿一顿的问题
// /// 7.快照录像可以本地录制，但是如果是快照同步，需要服务器录制
// /// </summary>
// public class SnapShotManager : IFixedUpdate {
//
//     public int index;
//     public PlayMode playMode;
//     public SnapShotPlay snapShotPlay;
//     public SnapShotRecord snapShotRecord;
//
//     public SnapShotManager() {
//         playMode = PlayMode.None;
//         snapShotRecord = new SnapShotRecord();
//         snapShotPlay = new SnapShotPlay();
//     }
//
//     public void FixedUpdate() {
//         if (playMode == PlayMode.Record) {
//             snapShotRecord.Record(index);
//             index++;
//         } else if (playMode == PlayMode.Play) {
//             if (index < snapShotRecord.frameDatas.Count) {
//                 snapShotPlay.Play(snapShotRecord.frameDatas[index]);
//                 index++;
//             } else {
//                 Debug.Log("录像播放结束");
//                 playMode = PlayMode.None;
//             }
//         } else if (playMode == PlayMode.Invert) {
//             if (index > 0) {
//                 index--;
//                 snapShotPlay.Play(snapShotRecord.frameDatas[index]);
//             } else {
//                 Debug.Log("录像倒放结束");
//                 playMode = PlayMode.None;
//             }
//         }
//     }
//
//     //录制
//     public void StartRecord() {
//         Debug.Log("开始录像");
//         index = 0;
//         snapShotRecord.Clear();
//         playMode = PlayMode.Record;
//     }
//
//     //播放
//     public void PlayRecord() {
//         index = 0;
//         snapShotPlay.Clear();
//         playMode = PlayMode.Play;
//     }
//
//     //倒放 （此处的倒放是指倒放录像，不是倒放录制，因为两者数据源不一样。流程：录制——>播放录像——>倒放）
//     public void InvertRecord() {
//         playMode = PlayMode.Invert;
//     }
// }
//
// //每帧数据
// public struct FrameData {
//     public int index;
//     //游戏物体
//     public List<UnitData> unitDatas;
//     //消息
//     public List<MsgData> msgDatas;
// }
//
// //存储的录像消息
// public struct MsgData {
//     public MsgType msgType;
//     public object[] data;//注意存储的数据是可以序列化的
// }
//
// //需要同步的游戏物体
// public struct UnitData {
//     public int id;//只增加不删减，id 就是 index
//     public bool isActive;//创建物体的时候设置为 true ，释放的时候设置为 false，但是不会在 unitDatas 中删除，方便做倒放
//     public string path;
//     public Vector3 pos;
//     public Vector3 rot;
//     public Vector3 scale;
//
//     public void SetActive(bool v) {
//         this.isActive = v;
//     }
// }
//
// //中间数据，不需要序列化，不会存储，他会和 GameObject 做强关联，他是每帧将 GameObject 的数据存成 UnitData
// public struct UnitDataSave {
//     public GameObject go;
//     public UnitData unitData;
//
//     public UnitData Save() {
//         UnitData data = new UnitData();
//         data.id = unitData.id;
//         data.isActive = unitData.isActive;
//         data.path = unitData.path;
//         data.pos = go.transform.position;
//         data.rot = go.transform.eulerAngles;
//         data.scale = go.transform.localScale;
//
//         return data;
//     }
// }
//
// public enum MsgType {
//     None,
//     EventMsg,
//     ReleasePrefab,//不需要创建的消息，创建直接 new UnitData
//     PlayAnimation,
// }