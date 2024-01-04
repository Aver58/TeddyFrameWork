using UnityEngine;
using UnityEngine.AI;

public class AIPlayer : MonoBehaviour
{
    [SerializeField] private Transform destination;
    private NavMeshAgent _navMeshAgent;

    private void Start()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        _navMeshAgent.SetDestination(destination.position);
    }
}
