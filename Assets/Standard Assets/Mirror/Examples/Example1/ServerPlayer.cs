using Mirror;
using UnityEngine;

public class ServerPlayer : NetworkBehaviour {
   public GameObject ClientPlayer;
   private GameObject aiPlayerInstance;

   public event System.Action<Vector3> OnPlayerDataChanged;
   public Vector3 PlayerPosition;

   public override void OnStartClient() {
      base.OnStartClient();

      aiPlayerInstance = Instantiate(ClientPlayer, transform.position, Quaternion.identity);
      aiPlayerInstance.GetComponent<ClientPlayer>().SetPlayer(this);
      aiPlayerInstance.name = "ClientPlayer" + netId;

      if (OnPlayerDataChanged != null)
         OnPlayerDataChanged.Invoke(PlayerPosition);
   }

   public override void OnStopClient() {
      Destroy(aiPlayerInstance);
   }

   private void Update() {
      if (Input.GetMouseButtonDown(0)) {
         Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
         RaycastHit hit;
         if (Physics.Raycast(ray, out hit)) {
            CmdPositionChanged(netId, hit.point);
         }
      }
   }

   [Command]
   private void CmdPositionChanged(uint id, Vector3 position) {
      PlayerPosition = position;
      ClientRpcPlayerPositionChanged(id, position);
   }

   [ClientRpc]
   private void ClientRpcPlayerPositionChanged(uint id, Vector3 newPosition) {
      if (id == netId) {
         if (OnPlayerDataChanged != null)
            OnPlayerDataChanged.Invoke(newPosition);
      }
   }
}
