using System;
using System.Collections;
using System.Collections.Generic;
using Singletons;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class NavMeshMovement : MonoBehaviour
{
    private Transform _target;
    private NavMeshAgent _agent;

    void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
    }

    private void Start()
    {
        GameManager.Instance.OnPlayerDie.AddListener(gameObj =>
        {
            ResetDestination();
        });
    }

    public void SetDestination(Vector3 targetPoint)
    {
        _agent.Warp(transform.position);
        if (!_agent.isOnNavMesh) return;
        _agent.destination = targetPoint;
        _agent.isStopped = false;
    }

    public void ResetDestination()
    {
        if (!_agent.isOnNavMesh) return;
        _agent.destination = default;
        _agent.isStopped = true;
    }
}