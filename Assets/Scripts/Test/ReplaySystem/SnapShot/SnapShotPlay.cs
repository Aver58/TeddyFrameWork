// using System.Collections.Generic;
// using UnityEngine;
//
// public class SnapShotPlay {
//
//     //--------------------------------播放数据--------------------------------
//     private List<UnitDataSave> runtimeUnitDatas = new List<UnitDataSave>(128);
//
//     /// <summary>
//     /// 解析快照数据
//     /// </summary>
//     public void Play(FrameData frameData) {
//
//         for (int i = 0; i < frameData.unitDatas.Count; i++) {
//             UnitData one = frameData.unitDatas[i];
//
//             if (one.isActive) {
//                 //---------------------------创建新物体---------------------------
//                 //完全不存在的
//                 if (one.id >= runtimeUnitDatas.Count) {
//                     RuntimeCreateUnitPrefab(one, true);
//                 } else {
//                     //已经存在的，但是曾经被销毁了
//                     int id = one.id;
//                     if (!runtimeUnitDatas[id].unitData.isActive) {
//                         runtimeUnitDatas[id].unitData.SetActive(true);
//                         RuntimeCreateUnitPrefab(one, false);
//                     } else {
//                         //已经存在，那么更新位置
//                         runtimeUnitDatas[id].go.transform.position = one.pos;
//                     }
//                 }
//             } else {
//                 //---------------------------销毁游戏物体---------------------------
//                 int id = one.id;
//                 if (runtimeUnitDatas[id].unitData.isActive) {
//                     runtimeUnitDatas[id].unitData.SetActive(false);
//                     AssetManager.UnLoadPrefab(runtimeUnitDatas[id].go);
//                 }
//             }
//         }
//
//         //TODO:还原游戏事件
//
//     }
//
//     //只为 Play 状态运作
//     private async void RuntimeCreateUnitPrefab(UnitData data, bool isNew) {
//         GameObject go = await AssetManager.LoadPrefab(data.path);
//         if (isNew) {
//             UnitDataSave unitDataSave = new UnitDataSave();
//             unitDataSave.unitData = data;
//             unitDataSave.go = go;
//             runtimeUnitDatas.Add(unitDataSave);
//         }
//     }
//
//     public void Clear() {
//         runtimeUnitDatas.Clear();
//     }
// }
