using UnityEngine;
using UnityEngine.AI;

public class ClientPlayer : MonoBehaviour {
    private PlayerNet playerNet;
    public Vector3 Destination;
    private NavMeshAgent navMeshAgent;

    public void SetPlayer(PlayerNet playerNet) {
        this.playerNet = playerNet;
        this.playerNet.OnPlayerDataChanged += OnPlayerNetDataChanged;
    }

    private void OnPlayerNetDataChanged(Vector3 position) {
        Destination = position;
    }

    void Start() {
        navMeshAgent = GetComponent<NavMeshAgent>();
    }

    private void Update() {
        navMeshAgent.SetDestination(Destination);
    }
}
