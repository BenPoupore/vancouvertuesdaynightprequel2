using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace VipExtraction
{
    [RequireComponent(typeof(NavMeshAgent))]
    public sealed class WaypointPatrol : MonoBehaviour
    {
        [SerializeField] private List<Transform> waypoints = new List<Transform>();
        [Min(0f), SerializeField] private float waitAtPoint = 2f;
        private NavMeshAgent agent;
        private int index;
        private Coroutine patrolRoutine;

        private void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
        }

        private void OnEnable()
        {
            patrolRoutine = StartCoroutine(Patrol());
        }

        private void OnDisable()
        {
            if (patrolRoutine != null)
            {
                StopCoroutine(patrolRoutine);
                patrolRoutine = null;
            }
        }

        private IEnumerator Patrol()
        {
            while (waypoints.Count > 0)
            {
                Transform destination = waypoints[index];
                if (destination != null)
                {
                    agent.SetDestination(destination.position);
                    while (agent.pathPending || agent.remainingDistance > agent.stoppingDistance)
                    {
                        yield return null;
                    }

                    yield return new WaitForSeconds(waitAtPoint);
                }

                index = (index + 1) % waypoints.Count;
            }
        }
    }
}

