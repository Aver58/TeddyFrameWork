using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace MTAssets.EasyMinimapSystem
{
    public class AntsScript : MonoBehaviour
    {
        //Cache variables
        private Vector3 thisStartingPosition = Vector3.zero;
        private NavMeshAgent antNavMeshAgent;
        private Vector3 currentDestinationPosition = Vector3.zero;
        private bool alreadyDefinedFirstDestinationPosition = false;

        void Start()
        {
            //Get components
            thisStartingPosition = this.gameObject.transform.position;
            antNavMeshAgent = this.gameObject.GetComponent<NavMeshAgent>();
        }

        void Update()
        {
            //If not defined the first destination position or is closer to current destination
            if (Vector3.Distance(new Vector3(this.transform.position.x, 0, this.transform.position.z), new Vector3(currentDestinationPosition.x, 0, currentDestinationPosition.z)) <= 4.0f || alreadyDefinedFirstDestinationPosition == false)
            {
                //Calculate next current destination position
                currentDestinationPosition = new Vector3(thisStartingPosition.x + (Random.Range(-8.0f, 8.0f)), 0, thisStartingPosition.z + (Random.Range(-8.0f, 8.0f)));
                //Set on cache
                alreadyDefinedFirstDestinationPosition = true;
            }

            //Start to move to destination point
            antNavMeshAgent.SetDestination(new Vector3(currentDestinationPosition.x, this.gameObject.transform.position.y, currentDestinationPosition.z));
        }
    }
}