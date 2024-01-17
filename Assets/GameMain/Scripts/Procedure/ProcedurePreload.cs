//------------------------------------------------------------
// Game Framework
// Copyright © 2013-2021 Jiang Yin. All rights reserved.
// Homepage: https://gameframework.cn/
// Feedback: mailto:ellan@gameframework.cn
//------------------------------------------------------------

using GameFramework;
using GameFramework.Event;
using GameFramework.Resource;
using System.Collections.Generic;
using UnityEngine;
using UnityGameFramework.Runtime;
using ProcedureOwner = GameFramework.Fsm.IFsm<GameFramework.Procedure.IProcedureManager>;
using GameFramework.Procedure;
using StarForce;

public class ProcedurePreload : ProcedureBase
    {
        public static readonly string[] DataTableNames = new string[]
        {
            "UIForm",
        };

        private Dictionary<string, bool> m_LoadedFlag = new Dictionary<string, bool>();

        protected override void OnEnter(ProcedureOwner procedureOwner)
        {
            base.OnEnter(procedureOwner);
            GameEntry.Event.Subscribe(LoadDataTableSuccessEventArgs.EventId, OnLoadDataTableSuccess);


            m_LoadedFlag.Clear();
        }

        protected override void OnLeave(ProcedureOwner procedureOwner, bool isShutdown) {
            GameEntry.Event.Unsubscribe(LoadDataTableSuccessEventArgs.EventId, OnLoadDataTableSuccess);
            base.OnLeave(procedureOwner, isShutdown);
        }

        private void PreloadResources()
        {
            // Preload configs
            // LoadConfig("DefaultConfig");

            // Preload data tables
            foreach (string dataTableName in DataTableNames)
            {
                LoadDataTable(dataTableName);
            }
        }

        private void LoadConfig(string configName)
        {
            // string configAssetName = AssetUtility.GetConfigAsset(configName, false);
            // m_LoadedFlag.Add(configAssetName, false);
            // GameEntry.Config.ReadData(configAssetName, this);
        }

        private void LoadDataTable(string dataTableName)
        {
            string dataTableAssetName = AssetUtility.GetDataTableAsset(dataTableName, false);
            m_LoadedFlag.Add(dataTableAssetName, false);
            GameEntry.DataTable.LoadDataTable(dataTableName, dataTableAssetName, this);
        }

        private void OnLoadDataTableSuccess(object sender, GameEventArgs e)
        {
            LoadDataTableSuccessEventArgs ne = (LoadDataTableSuccessEventArgs)e;
            if (ne.UserData != this)
            {
                return;
            }

            m_LoadedFlag[ne.DataTableAssetName] = true;
            Log.Info("Load data table '{0}' OK.", ne.DataTableAssetName);
        }

        protected override void OnUpdate(ProcedureOwner procedureOwner, float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(procedureOwner, elapseSeconds, realElapseSeconds);
            foreach (KeyValuePair<string, bool> loadedFlag in m_LoadedFlag)
            {
                if (!loadedFlag.Value)
                {
                    return;
                }
            }

            ChangeState<ProcedureMain>(procedureOwner);
        }
    }
