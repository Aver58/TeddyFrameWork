// using System.Collections.Generic;
// using System.Threading.Tasks;
// using UnityEngine;
//
// public class SnapShotRecord {
//
//     //--------------------------------录制数据--------------------------------
//     public List<FrameData> frameDatas = new List<FrameData>(3000);
//     private List<MsgData> curFrameMsgDatas = new List<MsgData>(36);//当前帧的消息数据
//     private List<UnitData> unitDatas = new List<UnitData>(128);//全局，只增不减
//     private List<UnitDataSave> unitDataSaves = new List<UnitDataSave>(36);//TODO:活跃的，与上面的数据 unitDatas 同步，不过可以做个定期将不用的清理
//
//     /// <summary>
//     /// 录制快照数据
//     /// </summary>
//     public void Record(int index) {
//         FrameData data = new FrameData();
//         data.index = index;
//
//         //保存当前帧的状态数据
//         for (int i = 0; i < unitDataSaves.Count; i++) {
//             if (unitDataSaves[i].unitData.isActive && unitDataSaves[i].go != null) {
//                 int id = unitDataSaves[i].unitData.id;
//                 unitDatas[id] = unitDataSaves[i].Save();
//             }
//         }
//
//         data.unitDatas = new List<UnitData>(36);
//         for (int i = 0; i < unitDatas.Count; i++) {
//             data.unitDatas.Add(unitDatas[i]);//需要clone吗？
//         }
//
//         data.msgDatas = new List<MsgData>(36);
//         for (int i = 0; i < curFrameMsgDatas.Count; i++) {
//             data.msgDatas.Add(curFrameMsgDatas[i]);
//         }
//         curFrameMsgDatas.Clear();
//
//         frameDatas.Add(data);
//     }
//
//     public void Clear() {
//         frameDatas.Clear();
//         curFrameMsgDatas.Clear();
//         unitDatas.Clear();
//         unitDataSaves.Clear();
//     }
//
//     #region 创建预制体
//
//     public async Task<GameObject> LoadPrefab(string path) {
//         GameObject go = await AssetManager.LoadPrefab(path);
//         //后面读取的时候，生成 UnitData 解析的时候自动创建
//         UnitData unitData = new UnitData();
//         unitData.id = unitDatas.Count;
//         unitData.isActive = true;
//         unitData.pos = Vector3.zero;
//         unitData.path = path;
//         unitDatas.Add(unitData);
//
//         UnitDataSave unitDataSave = new UnitDataSave();
//         unitDataSave.unitData = unitData;
//         unitDataSave.go = go;
//         unitDataSaves.Add(unitDataSave);
//
//         return go;
//     }
//
//     public void ReleasePrefab(GameObject go, int id) {
//         AssetManager.UnLoadPrefab(go);
//
//         //------如果上一帧存在，下一帧不存在，说明他消失了，所有此段代码注释------
//         //记录一个释放预制体的消息
//         //MsgData msgData = new MsgData();
//         //msgData.msgType = MsgType.ReleasePrefab;
//         //msgData.data = new object[] { id };
//         //curFrameMsgDatas.Add(msgData);
//
//         unitDatas[id].SetActive(false);
//     }
//
//     #endregion
//
//     #region 动画
//
//     public void PlayAnimation(uint id, string name, float time) {
//         //记录一个播放动画的消息
//         MsgData msgData = new MsgData();
//         msgData.msgType = MsgType.PlayAnimation;
//         msgData.data = new object[] { id, name, time };
//         curFrameMsgDatas.Add(msgData);
//     }
//
//     #endregion
//
//     #region 通用消息，用来处理本地表现的 （虽然我们的怪兽也是通过消息的来创建的，但是不需要这个，因为创建prefab本身是同步的）
//
//     public void AddMsg(string eventName)
//     {
//         EventSystemManager.Instance.Dispatch(eventName);
//         MsgData msgData = new MsgData();
//         msgData.msgType = MsgType.EventMsg;
//         msgData.data = new object[] { eventName };
//         curFrameMsgDatas.Add(msgData);
//     }
//
//     public void AddMsg<T>(string eventName, T t)
//     {
//         EventSystemManager.Instance.Dispatch(eventName, t);
//         MsgData msgData = new MsgData();
//         msgData.msgType = MsgType.EventMsg;
//         msgData.data = new object[] { eventName, t };
//         curFrameMsgDatas.Add(msgData);
//     }
//
//     public void AddMsg<T1, T2>(string eventName, T1 t1, T2 t2)
//     {
//         EventSystemManager.Instance.Dispatch(eventName, t1, t2);
//         MsgData msgData = new MsgData();
//         msgData.msgType = MsgType.EventMsg;
//         msgData.data = new object[] { eventName, t1, t2 };
//         curFrameMsgDatas.Add(msgData);
//     }
//
//     public void AddMsg<T1, T2, T3>(string eventName, T1 t1, T2 t2, T3 t3)
//     {
//         EventSystemManager.Instance.Dispatch(eventName, t1, t2, t3);
//         MsgData msgData = new MsgData();
//         msgData.msgType = MsgType.EventMsg;
//         msgData.data = new object[] { eventName, t1, t2, t3 };
//         curFrameMsgDatas.Add(msgData);
//     }
//
//     public void AddMsg<T1, T2, T3, T4>(string eventName, T1 t1, T2 t2, T3 t3, T4 t4)
//     {
//         EventSystemManager.Instance.Dispatch(eventName, t1, t2, t3, t4);
//         MsgData msgData = new MsgData();
//         msgData.msgType = MsgType.EventMsg;
//         msgData.data = new object[] { eventName, t1, t2, t3, t4 };
//         curFrameMsgDatas.Add(msgData);
//     }
//
//     #endregion
// }
