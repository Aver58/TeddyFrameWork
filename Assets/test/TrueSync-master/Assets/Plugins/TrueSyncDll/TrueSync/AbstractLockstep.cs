using System;
using System.Collections.Generic;
using UnityEngine;

namespace TrueSync
{
	public abstract class AbstractLockstep
	{
		private enum SimulationState // ģ��״̬
		{
            NOT_STARTED, // δ����
			WAITING_PLAYERS, // �ȴ����
			RUNNING, // ������
			PAUSED, // ��ͣ
			ENDED // ����
		}
        /*
		private const int INITIAL_PLAYERS_CAPACITY = 4; // ��ʼ�������

		private const byte SYNCED_GAME_START_CODE = 196; // ��ʼͬ��

		private const byte SIMULATION_CODE = 197; // ģ��֡

		private const byte CHECKSUM_CODE = 198; // У��

		private const byte SEND_CODE = 199;

		private const byte SIMULATION_EVENT_PAUSE = 0;

		private const byte SIMULATION_EVENT_RUN = 1;

		private const byte SIMULATION_EVENT_END = 3;

		private const int MAX_PANIC_BEFORE_END_GAME = 5;

		private const int SYNCED_INFO_BUFFER_WINDOW = 3;
        */
		internal Dictionary<byte, TSPlayer> players;

		internal List<TSPlayer> activePlayers;

		internal List<SyncedData> auxPlayersSyncedData;

		internal List<InputDataBase> auxPlayersInputData;

        private List<int> playersIdsAux = new List<int>(); // �Ǳ�����ҵĻ�Ծ���ID�б�
		internal int[] auxActivePlayersIds; // �Ǳ�����ҵĻ�Ծ���ID����

		internal TSPlayer localPlayer; // �������

		protected TrueSyncUpdateCallback StepUpdate;

		private TrueSyncInputCallback GetLocalData; // ��ȡ��������

		internal TrueSyncInputDataProvider InputDataProvider;

		private TrueSyncEventCallback OnGameStarted;

		private TrueSyncEventCallback OnGamePaused;

		private TrueSyncEventCallback OnGameUnPaused;

		private TrueSyncEventCallback OnGameEnded;

		private TrueSyncPlayerDisconnectionCallback OnPlayerDisconnection;

		public TrueSyncIsReady GameIsReady;

		protected int ticks;

		private int panicWindow;

		protected int syncWindow;

		private int elapsedPanicTicks;

		private AbstractLockstep.SimulationState simulationState;

		internal int rollbackWindow; // �ع�������

		internal ICommunicator communicator;

		protected IPhysicsManagerBase physicsManager;

		private GenericBufferWindow<SyncedInfo> bufferSyncedInfo;

		protected int totalWindow;

		public bool checksumOk;

		public CompoundStats compoundStats;

		public float deltaTime;

		public int _lastSafeTick = 0;

		protected Dictionary<int, List<IBody>> bodiesToDestroy;

		protected Dictionary<int, List<byte>> playersDisconnect;

		private ReplayMode replayMode; // �ط�ģʽ

		private ReplayRecord replayRecord; // �طż�¼

		internal static AbstractLockstep instance;

		private SyncedData[] _syncedDataCacheDrop = new SyncedData[1];

		private SyncedData[] _syncedDataCacheUpdateData = new SyncedData[1];

        #region ����
        public List<TSPlayer> ActivePlayers
		{
			get
			{
				return this.activePlayers;
			}
		}

		public IDictionary<byte, TSPlayer> Players
		{
			get
			{
				return this.players;
			}
		}

		public TSPlayer LocalPlayer
		{
			get
			{
				return this.localPlayer;
			}
		}

		public int Ticks
		{
			get
			{
				return this.GetSimulatedTick(this.GetSyncedDataTick()) - 1;
			}
		}

		public int LastSafeTick
		{
			get
			{
				bool flag = this._lastSafeTick < 0;
				int result;
				if (flag)
				{
					result = -1;
				}
				else
				{
					result = this._lastSafeTick - 1;
				}
				return result;
			}
		}

		private ReplayMode ReplayMode
		{
			set
			{
				this.replayMode = value;
				bool flag = this.replayMode == ReplayMode.RECORD_REPLAY;
				if (flag)
				{
					this.replayRecord = new ReplayRecord();
				}
			}
		}

		public ReplayRecord ReplayRecord
		{
			set
			{
				this.replayRecord = value;
				bool flag = this.replayRecord != null;
				if (flag)
				{
					this.replayMode = ReplayMode.LOAD_REPLAY;
					this.replayRecord.ApplyRecord(this);
				}
			}
		}
        #endregion ����

        public static AbstractLockstep NewInstance(float deltaTime, ICommunicator communicator, IPhysicsManagerBase physicsManager, int syncWindow, int panicWindow, int rollbackWindow, TrueSyncEventCallback OnGameStarted, TrueSyncEventCallback OnGamePaused, TrueSyncEventCallback OnGameUnPaused, TrueSyncEventCallback OnGameEnded, TrueSyncPlayerDisconnectionCallback OnPlayerDisconnection, TrueSyncUpdateCallback OnStepUpdate, TrueSyncInputCallback GetLocalData, TrueSyncInputDataProvider InputDataProvider)
		{
			bool flag = rollbackWindow <= 0 || communicator == null;
			AbstractLockstep result;
			if (flag)
			{
				result = new DefaultLockstep(deltaTime, communicator, physicsManager, syncWindow, panicWindow, rollbackWindow, OnGameStarted, OnGamePaused, OnGameUnPaused, OnGameEnded, OnPlayerDisconnection, OnStepUpdate, GetLocalData, InputDataProvider);
			}
			else
			{
				result = new RollbackLockstep(deltaTime, communicator, physicsManager, syncWindow, panicWindow, rollbackWindow, OnGameStarted, OnGamePaused, OnGameUnPaused, OnGameEnded, OnPlayerDisconnection, OnStepUpdate, GetLocalData, InputDataProvider);
			}
			return result;
		}

		public AbstractLockstep(float deltaTime, ICommunicator communicator, IPhysicsManagerBase physicsManager, int syncWindow, int panicWindow, int rollbackWindow, TrueSyncEventCallback OnGameStarted, TrueSyncEventCallback OnGamePaused, TrueSyncEventCallback OnGameUnPaused, TrueSyncEventCallback OnGameEnded, TrueSyncPlayerDisconnectionCallback OnPlayerDisconnection, TrueSyncUpdateCallback OnStepUpdate, TrueSyncInputCallback GetLocalData, TrueSyncInputDataProvider InputDataProvider)
		{
			AbstractLockstep.instance = this;
			this.deltaTime = deltaTime;
			this.syncWindow = syncWindow;
			this.panicWindow = panicWindow;
			this.rollbackWindow = rollbackWindow;
			this.totalWindow = syncWindow + rollbackWindow;
			this.StepUpdate = OnStepUpdate;
			this.OnGameStarted = OnGameStarted;
			this.OnGamePaused = OnGamePaused;
			this.OnGameUnPaused = OnGameUnPaused;
			this.OnGameEnded = OnGameEnded;
			this.OnPlayerDisconnection = OnPlayerDisconnection;
			this.GetLocalData = GetLocalData;
			this.InputDataProvider = InputDataProvider;
			this.ticks = 0;
			this.players = new Dictionary<byte, TSPlayer>(4);
			this.activePlayers = new List<TSPlayer>(4);
			this.auxPlayersSyncedData = new List<SyncedData>(4);
			this.auxPlayersInputData = new List<InputDataBase>(4);
			this.communicator = communicator;
			bool flag = communicator != null;
			if (flag)
			{
				this.communicator.AddEventListener(new OnEventReceived(this.OnEventDataReceived));
			}
			this.physicsManager = physicsManager;
			this.compoundStats = new CompoundStats();
			this.bufferSyncedInfo = new GenericBufferWindow<SyncedInfo>(3);
			this.checksumOk = true;
			this.simulationState = AbstractLockstep.SimulationState.NOT_STARTED;
			this.bodiesToDestroy = new Dictionary<int, List<IBody>>();
			this.playersDisconnect = new Dictionary<int, List<byte>>();
			this.ReplayMode = ReplayRecord.replayMode;
		}

		protected int GetSyncedDataTick()
		{
			return this.ticks - this.syncWindow;
		}

		protected abstract int GetRefTick(int syncedDataTick);

		protected virtual void BeforeStepUpdate(int syncedDataTick, int referenceTick)
		{
		}

		protected virtual void AfterStepUpdate(int syncedDataTick, int referenceTick)
		{
			int i = 0;
			int count = this.activePlayers.Count;
			while (i < count)
			{
				this.activePlayers[i].RemoveData(referenceTick);
				i++;
			}
		}

		protected abstract bool IsStepReady(int syncedDataTick);

		protected abstract void OnSyncedDataReceived(TSPlayer player, List<SyncedData> data);

		protected abstract string GetChecksumForSyncedInfo();

		protected abstract int GetSimulatedTick(int syncedDataTick);

		private void Pause()
		{
			bool flag = this.simulationState == AbstractLockstep.SimulationState.RUNNING;
			if (flag)
			{
				this.OnGamePaused();
				this.simulationState = AbstractLockstep.SimulationState.PAUSED;
			}
		}

		private void End()
		{
			bool flag = this.simulationState != AbstractLockstep.SimulationState.ENDED;
			if (flag)
			{
				this.OnGameEnded();
				bool flag2 = this.replayMode == ReplayMode.RECORD_REPLAY;
				if (flag2)
				{
					ReplayRecord.SaveRecord(this.replayRecord);
				}
				this.simulationState = AbstractLockstep.SimulationState.ENDED;
			}
		}

        #region �������ڷ���
        #region Update ���
        // ����
		public void Update()
		{
			bool flag = this.simulationState == AbstractLockstep.SimulationState.WAITING_PLAYERS;
			if (flag) // �ȴ����
			{
				this.CheckGameStart();
			}
			else
			{
				bool flag2 = this.simulationState == AbstractLockstep.SimulationState.RUNNING;
				if (flag2)
				{
					this.compoundStats.UpdateTime(this.deltaTime);
					bool flag3 = this.communicator != null;
					if (flag3)
					{
						this.compoundStats.AddValue("ping", (long)this.communicator.RoundTripTime());
					}
					bool flag4 = this.syncWindow == 0;
					if (flag4)
					{
						this.UpdateData();
					}
					int i = 0;
					int num = this.activePlayers.Count;
					while (i < num)
					{
						bool flag5 = this.CheckDrop(this.activePlayers[i]);
						if (flag5)
						{
							i--;
							num--;
						}
						i++;
					}
					int syncedDataTick = this.GetSyncedDataTick();
					bool flag6 = this.CheckGameIsReady() && this.IsStepReady(syncedDataTick);
					bool flag7 = flag6;
					if (flag7)
					{
						this.compoundStats.Increment("simulated_frames");
						this.UpdateData();
						this.elapsedPanicTicks = 0;
						int refTick = this.GetRefTick(syncedDataTick);
						bool flag8 = refTick > 1 && refTick % 100 == 0;
						if (flag8)
						{
							this.SendInfoChecksum(refTick);
						}
						this._lastSafeTick = refTick;
						this.BeforeStepUpdate(syncedDataTick, refTick);
						List<SyncedData> tickData = this.GetTickData(syncedDataTick);
						this.ExecutePhysicsStep(tickData, syncedDataTick);
						bool flag9 = this.replayMode == ReplayMode.RECORD_REPLAY;
						if (flag9)
						{
							this.replayRecord.AddSyncedData(this.GetTickData(refTick));
						}
						this.AfterStepUpdate(syncedDataTick, refTick);
						this.ticks++;
					}
					else
					{
						bool flag10 = this.ticks >= this.totalWindow;
						if (flag10)
						{
							bool flag11 = this.replayMode == ReplayMode.LOAD_REPLAY;
							if (flag11)
							{
								this.End();
							}
							else
							{
								this.compoundStats.Increment("missed_frames");
								this.elapsedPanicTicks++;
								bool flag12 = this.elapsedPanicTicks > this.panicWindow;
								if (flag12)
								{
									this.compoundStats.Increment("panic");
									bool flag13 = this.compoundStats.globalStats.GetInfo("panic").count >= 5L;
									if (flag13)
									{
										this.End();
									}
									else
									{
										this.elapsedPanicTicks = 0;
										this.DropLagPlayers();
									}
								}
							}
						}
						else
						{
							this.compoundStats.Increment("simulated_frames");
							this.physicsManager.UpdateStep();
							this.UpdateData();
							this.ticks++;
						}
					}
				}
			}
		}

        // �����Ϸ�Ƿ���Կ�ʼ
        private void CheckGameStart()
        {
            bool flag = this.replayMode == ReplayMode.LOAD_REPLAY;
            if (flag) // ���ػط�ģʽ
            {
                this.RunSimulation(false);
            }
            else // �Ǽ��ػط�ģʽ
            {
                bool flag2 = true;
                int i = 0;
                int count = this.activePlayers.Count;
                while (i < count)
                {
                    flag2 &= this.activePlayers[i].sentSyncedStart;
                    i++;
                }
                bool flag3 = flag2;
                if (flag3)
                {
                    this.RunSimulation(false);
                    SyncedData.pool.FillStack(this.activePlayers.Count * (this.syncWindow + this.rollbackWindow));
                }
                else
                {
                    this.RaiseEvent(196, SyncedInfo.Encode(new SyncedInfo
                    {
                        playerId = this.localPlayer.ID
                    }));
                }
            }
        }
        #endregion Update ���
        #endregion �������ڷ���

        private bool CheckGameIsReady()
		{
			bool flag = this.GameIsReady != null;
			bool result;
			if (flag)
			{
				Delegate[] invocationList = this.GameIsReady.GetInvocationList();
				for (int i = 0; i < invocationList.Length; i++)
				{
					Delegate @delegate = invocationList[i];
					bool flag2 = (bool)@delegate.DynamicInvoke(new object[0]);
					bool flag3 = !flag2;
					if (flag3)
					{
						result = false;
						return result;
					}
				}
			}
			result = true;
			return result;
		}

		protected void ExecutePhysicsStep(List<SyncedData> data, int syncedDataTick)
		{
			this.ExecuteDelegates(syncedDataTick);
			this.SyncedArrayToInputArray(data);
			this.StepUpdate(this.auxPlayersInputData);
			this.physicsManager.UpdateStep();
		}

		private void ExecuteDelegates(int syncedDataTick)
		{
			syncedDataTick++;
			bool flag = this.playersDisconnect.ContainsKey(syncedDataTick);
			if (flag)
			{
				List<byte> list = this.playersDisconnect[syncedDataTick];
				int i = 0;
				int count = list.Count;
				while (i < count)
				{
					this.OnPlayerDisconnection(list[i]);
					i++;
				}
			}
		}

        #region �ڲ�����
        // ���� �Ǳ�����ҵĻ�Ծ���ID ����
        internal void UpdateActivePlayers()
		{
			this.playersIdsAux.Clear();
			int i = 0;
			int count = this.activePlayers.Count;
			while (i < count)
			{
				bool flag = this.localPlayer == null || this.localPlayer.ID != this.activePlayers[i].ID;
				if (flag) // �ѱ�������Ҽ����б���
				{   
					this.playersIdsAux.Add((int)this.activePlayers[i].ID);
				}
				i++;
			}
			this.auxActivePlayersIds = this.playersIdsAux.ToArray();
		}
        #endregion �ڲ�����

        

		protected void SyncedArrayToInputArray(List<SyncedData> data)
		{
			this.auxPlayersInputData.Clear();
			int i = 0;
			int count = data.Count;
			while (i < count)
			{
				this.auxPlayersInputData.Add(data[i].inputData);
				i++;
			}
		}

		public void PauseSimulation()
		{
			this.Pause();
			this.RaiseEvent(197, new byte[1], true, this.auxActivePlayersIds);
		}

        // ִ��ģ��
		public void RunSimulation(bool firstRun)
		{
			this.Run();
			bool flag = !firstRun;
			if (flag) // ���ǵ�һ������
			{
				this.RaiseEvent(197, new byte[]
				{
					1
				}, true, this.auxActivePlayersIds);
			}
		}

		public void EndSimulation()
		{
			this.End();
			this.RaiseEvent(197, new byte[]
			{
				3
			}, true, this.auxActivePlayersIds);
		}

		public void Destroy(IBody rigidBody)
		{
			rigidBody.TSDisabled = true;
			int key = this.GetSimulatedTick(this.GetSyncedDataTick()) + 1;
			bool flag = !this.bodiesToDestroy.ContainsKey(key);
			if (flag)
			{
				this.bodiesToDestroy[key] = new List<IBody>();
			}
			this.bodiesToDestroy[key].Add(rigidBody);
		}

		protected void CheckSafeRemotion(int refTick)
		{
			bool flag = this.bodiesToDestroy.ContainsKey(refTick);
			if (flag)
			{
				List<IBody> list = this.bodiesToDestroy[refTick];
				foreach (IBody current in list)
				{
					bool tSDisabled = current.TSDisabled;
					if (tSDisabled)
					{
						this.physicsManager.RemoveBody(current);
					}
				}
				this.bodiesToDestroy.Remove(refTick);
			}
			bool flag2 = this.playersDisconnect.ContainsKey(refTick);
			if (flag2)
			{
				this.playersDisconnect.Remove(refTick);
			}
		}

		private void DropLagPlayers()
		{
			List<TSPlayer> list = new List<TSPlayer>();
			int refTick = this.GetRefTick(this.GetSyncedDataTick());
			bool flag = refTick >= 0;
			if (flag)
			{
				int i = 0;
				int count = this.activePlayers.Count;
				while (i < count)
				{
					TSPlayer tSPlayer = this.activePlayers[i];
					bool flag2 = !tSPlayer.IsDataReady(refTick);
					if (flag2)
					{
						tSPlayer.dropCount++;
						list.Add(tSPlayer);
					}
					i++;
				}
			}
			int j = 0;
			int count2 = list.Count;
			while (j < count2)
			{
				TSPlayer p = list[j];
				this.CheckDrop(p);
				bool sendDataForDrop = list[j].GetSendDataForDrop(this.localPlayer.ID, this._syncedDataCacheDrop);
				if (sendDataForDrop)
				{
					this.communicator.OpRaiseEvent(199, SyncedData.Encode(this._syncedDataCacheDrop), true, null);
					SyncedData.pool.GiveBack(this._syncedDataCacheDrop[0]);
				}
				j++;
			}
		}

		private SyncedData UpdateData()
		{
			bool flag = this.replayMode == ReplayMode.LOAD_REPLAY;
			SyncedData result;
			if (flag) // ���ػط�ģʽ
			{
				result = null;
			}
            else // �Ǽ��ػط�ģʽ
			{
				SyncedData @new = SyncedData.pool.GetNew(); // �ӳ��л�ȡ
				@new.Init(this.localPlayer.ID, this.ticks);
                this.GetLocalData(@new.inputData); // ����Ҳ������ݷ���SyncedDataʵ����
                this.localPlayer.AddData(@new); // ��SyncedDataʵ�����뱾����ҵĲ��������ֵ���

				bool flag2 = this.communicator != null;
				if (flag2) // ��ͨ����
				{
                    this.localPlayer.GetSendData(this.ticks, this._syncedDataCacheUpdateData); // �����µĲ������ݷ���_syncedDataCacheUpdateData��
					this.communicator.OpRaiseEvent(199, SyncedData.Encode(this._syncedDataCacheUpdateData), true, this.auxActivePlayersIds); // ���Ͳ�������
				}
				result = @new;
			}
			return result;
		}

		public InputDataBase GetInputData(int playerId)
		{
			return this.players[(byte)playerId].GetData(this.GetSyncedDataTick()).inputData;
		}

		private void SendInfoChecksum(int tick)
		{
			bool flag = this.replayMode == ReplayMode.LOAD_REPLAY;
			if (!flag)
			{
				SyncedInfo syncedInfo = this.bufferSyncedInfo.Current();
				syncedInfo.playerId = this.localPlayer.ID;
				syncedInfo.tick = tick;
				syncedInfo.checksum = this.GetChecksumForSyncedInfo();
				this.bufferSyncedInfo.MoveNext();
				this.RaiseEvent(198, SyncedInfo.Encode(syncedInfo));
			}
		}

		private void RaiseEvent(byte eventCode, object message)
		{
			this.RaiseEvent(eventCode, message, true, null);
		}

		private void RaiseEvent(byte eventCode, object message, bool reliable, int[] toPlayers)
		{
			bool flag = this.communicator != null;
			if (flag)
			{
				this.communicator.OpRaiseEvent(eventCode, message, reliable, toPlayers);
			}
		}

		private void OnEventDataReceived(byte eventCode, object content)
		{
			bool flag = eventCode == 199;
			if (flag) // ���ղ�������
			{
				byte[] data = content as byte[];
				List<SyncedData> list = SyncedData.Decode(data); // ����
				bool flag2 = list.Count > 0;
				if (flag2) // ������
				{
					TSPlayer tSPlayer = this.players[list[0].inputData.ownerID]; // �ҵ���Ӧ�����
					bool flag3 = !tSPlayer.dropped;
					if (flag3) // �����û�е���
					{
						this.OnSyncedDataReceived(tSPlayer, list); // ����ͬ������
						bool flag4 = list[0].dropPlayer && tSPlayer.ID != this.localPlayer.ID && !this.players[list[0].dropFromPlayerId].dropped;
						if (flag4)
						{
							tSPlayer.dropCount++; // ͳ�Ƶ����������
						}
					}
					else // ����ҵ�����,�������ݶ���
					{
						int i = 0;
						int count = list.Count;
						while (i < count)
						{
							SyncedData.pool.GiveBack(list[i]);
							i++;
						}
					}
					SyncedData.poolList.GiveBack(list);
				}
			}
			else
			{
				bool flag5 = eventCode == 198;
				if (flag5)
				{
					byte[] infoBytes = content as byte[];
					this.OnChecksumReceived(SyncedInfo.Decode(infoBytes));
				}
				else
				{
					bool flag6 = eventCode == 197;
					if (flag6)
					{
						byte[] array = content as byte[];
						bool flag7 = array.Length != 0;
						if (flag7)
						{
							bool flag8 = array[0] == 0;
							if (flag8)
							{
								this.Pause();
							}
							else
							{
								bool flag9 = array[0] == 1;
								if (flag9)
								{
									this.Run();
								}
								else
								{
									bool flag10 = array[0] == 3;
									if (flag10)
									{
										this.End();
									}
								}
							}
						}
					}
					else
					{
						bool flag11 = eventCode == 196;
						if (flag11)
						{
							byte[] infoBytes2 = content as byte[];
							SyncedInfo syncedInfo = SyncedInfo.Decode(infoBytes2);
							this.players[syncedInfo.playerId].sentSyncedStart = true;
						}
					}
				}
			}
		}

		private void OnChecksumReceived(SyncedInfo syncedInfo)
		{
			bool dropped = this.players[syncedInfo.playerId].dropped;
			if (!dropped)
			{
				this.checksumOk = true;
				SyncedInfo[] buffer = this.bufferSyncedInfo.buffer;
				for (int i = 0; i < buffer.Length; i++)
				{
					SyncedInfo syncedInfo2 = buffer[i];
					bool flag = syncedInfo2.tick == syncedInfo.tick && syncedInfo2.checksum != syncedInfo.checksum;
					if (flag)
					{
						this.checksumOk = false;
						break;
					}
				}
			}
		}

		protected List<SyncedData> GetTickData(int tick)
		{
			this.auxPlayersSyncedData.Clear();
			int i = 0;
			int count = this.activePlayers.Count;
			while (i < count)
			{
				this.auxPlayersSyncedData.Add(this.activePlayers[i].GetData(tick));
				i++;
			}
			return this.auxPlayersSyncedData;
		}

        // ������
		public void AddPlayer(byte playerId, string playerName, bool isLocal)
		{
			TSPlayer tSPlayer = new TSPlayer(playerId, playerName);
			this.players.Add(tSPlayer.ID, tSPlayer);
			this.activePlayers.Add(tSPlayer);
			if (isLocal) // �������
			{
				this.localPlayer = tSPlayer;
				this.localPlayer.sentSyncedStart = true;
			}
			this.UpdateActivePlayers();
			bool flag = this.replayMode == ReplayMode.RECORD_REPLAY;
			if (flag)
			{
				this.replayRecord.AddPlayer(tSPlayer);
			}
		}

		private bool CheckDrop(TSPlayer p)
		{
			bool flag = p != this.localPlayer && !p.dropped && p.dropCount > 0;
			bool result;
			if (flag)
			{
				int num = this.activePlayers.Count - 1;
				bool flag2 = p.dropCount >= num;
				if (flag2)
				{
					this.compoundStats.globalStats.GetInfo("panic").count = 0L;
					p.dropped = true;
					this.activePlayers.Remove(p);
					this.UpdateActivePlayers();
					Debug.Log("Player dropped (stopped sending input)");
					int key = this.GetSyncedDataTick() + 1;
					bool flag3 = !this.playersDisconnect.ContainsKey(key);
					if (flag3)
					{
						this.playersDisconnect[key] = new List<byte>();
					}
					this.playersDisconnect[key].Add(p.ID);
					result = true;
					return result;
				}
			}
			result = false;
			return result;
        }

        #region ˽�з���
        // ����
        private void Run()
        {
            bool flag = this.simulationState == AbstractLockstep.SimulationState.NOT_STARTED;
            if (flag) // δ��ʼ
            {
                this.simulationState = AbstractLockstep.SimulationState.WAITING_PLAYERS;
            }
            else // ��NOT_STARTED״̬
            {
                bool flag2 = this.simulationState == AbstractLockstep.SimulationState.WAITING_PLAYERS || this.simulationState == AbstractLockstep.SimulationState.PAUSED;
                if (flag2) // ���ڵȴ���� �� ��ͣ״̬
                {
                    bool flag3 = this.simulationState == AbstractLockstep.SimulationState.WAITING_PLAYERS;
                    if (flag3) // ���ڵȴ����
                    {
                        this.OnGameStarted();
                    }
                    else // ��ͣ״̬
                    {
                        this.OnGameUnPaused();
                    }
                    this.simulationState = AbstractLockstep.SimulationState.RUNNING; // ��Ϊ����״̬
                }
            }
        }
        #endregion ˽�з���
    }
}
