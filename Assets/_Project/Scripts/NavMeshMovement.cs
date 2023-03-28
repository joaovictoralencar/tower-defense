using System;
using System.Collections;
using System.Collections.Generic;
using Singletons;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class NavMeshMovement : MonoBehaviour
{
    private Vector3 _targetPoint;
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

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    public void StartEnemyRecoil(float freezeTime)
    {
        if (!_agent.isOnNavMesh) return;
        if (!gameObject.activeInHierarchy) return;
        _agent.isStopped = true;
        StartCoroutine(EndEnemyRecoilCoroutine(freezeTime));
    }

    IEnumerator EndEnemyRecoilCoroutine(float freezeTime)
    {
        yield return new WaitForSeconds(freezeTime);
        _agent.isStopped = false;
    }

    public void SetDestination(Vector3 targetPoint)
    {
        _agent.Warp(transform.position);
        if (!_agent.isOnNavMesh) return;
        _targetPoint = targetPoint;
        _agent.destination = _targetPoint;
        _agent.isStopped = false;
    }

    public void ResetDestination()
    {
        if (!_agent.isOnNavMesh) return;
        _agent.destination = default;
        _agent.isStopped = true;
    }
}