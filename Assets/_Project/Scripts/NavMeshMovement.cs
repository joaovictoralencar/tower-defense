using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class NavMeshMovement : MonoBehaviour
{
    private Transform _target;
    private NavMeshAgent _agent;
    void Awake () {
        _agent = GetComponent<NavMeshAgent>();
    }
    
    public void MoveToLocation(Vector3 targetPoint)
    {
        _agent.destination = targetPoint;
        _agent.isStopped = false;
    }
}