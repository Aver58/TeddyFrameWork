using Mirror;
using UnityEngine;

public class PlayerNet : NetworkBehaviour {
   public GameObject ClientPlayer;
   private GameObject aiPlayerInstance;

   public event System.Action<Vector3> OnPlayerDataChanged;
   [SyncVar(hook = nameof(OnPlayerPositionChange))]
   public Vector3 PlayerPosition;

   public SyncList<IdCardSkinData> TestInts = new SyncList<IdCardSkinData>();

   public struct IdCardSkinData {
      public long ItemId;
      public long SkinId;
      public int SkinIndex;
   }

   public override void OnStartClient() {
      base.OnStartClient();

      // 断线重连 要么在 OnStartClient 对每个字段进行初始化，要么用 SYNC 连上同步，要么自己写个协议，在连上服务端进行同步
      aiPlayerInstance = Instantiate(ClientPlayer, PlayerPosition, transform.rotation);
      aiPlayerInstance.GetComponent<ClientPlayer>().SetPlayer(this);
      aiPlayerInstance.name = "ClientPlayer" + netId;

      TestInts.Callback += TestCallback;

      if (OnPlayerDataChanged != null)
         OnPlayerDataChanged.Invoke(PlayerPosition);
   }

   private void TestCallback(SyncList<IdCardSkinData>.Operation op, int itemIndex, IdCardSkinData oldItem, IdCardSkinData newItem) {
      Debug.LogError("op: " + op + " itemIndex: " + itemIndex + " oldItem: " + oldItem.ItemId + " newItem: " + newItem.ItemId);
   }

   public override void OnStopClient() {
      Destroy(aiPlayerInstance);
   }

   private void Update() {
      if (Input.GetMouseButtonDown(0)) {
         Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
         RaycastHit hit;
         if (Physics.Raycast(ray, out hit)) {
            transform.position = hit.point;
            CmdPositionChanged(netId, hit.point);
         }
      }
   }

   [Command]
   private void CmdPositionChanged(uint id, Vector3 position) {
      PlayerPosition = position;
      ClientRpcPlayerPositionChanged(id, position);

      TestInts.Clear();
      TestInts.Add(new IdCardSkinData() {
         ItemId = 1,
         SkinId = 2,
         SkinIndex = 3
      });
   }

   [ClientRpc]
   private void ClientRpcPlayerPositionChanged(uint id, Vector3 newPosition) {
      if (id == netId) {
         if (OnPlayerDataChanged != null)
            OnPlayerDataChanged.Invoke(newPosition);
      }

      foreach (var item in TestInts) {
         Debug.LogError(item.ItemId);
         Debug.LogError(item.SkinId);
         Debug.LogError(item.SkinIndex);
      }
   }

   private void OnPlayerPositionChange(Vector3 old, Vector3 @new) {
      // Debug.LogError("Server Client 都走：" + old + $"new {@new}");
   }
}
