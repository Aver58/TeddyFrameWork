using UnityEngine;
using UnityEngine.AI;

public class ClientPlayer : MonoBehaviour {
    private ServerPlayer ServerPlayer;
    public Vector3 Destination;
    private NavMeshAgent navMeshAgent;

    public void SetPlayer(ServerPlayer serverPlayer) {
        ServerPlayer = serverPlayer;
        ServerPlayer.OnPlayerDataChanged += OnPlayerDataChanged;
    }

    private void OnPlayerDataChanged(Vector3 position) {
        Destination = position;
    }

    void Start() {
        navMeshAgent = GetComponent<NavMeshAgent>();
    }

    private void Update() {
        navMeshAgent.SetDestination(Destination);
    }
}
