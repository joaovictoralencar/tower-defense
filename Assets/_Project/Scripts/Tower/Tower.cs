using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ObjectPool;
using Singletons;
using Utils;

[RequireComponent(typeof(SphereCollider))]
public class Tower : MonoBehaviour
{
    [Header("Stats")] [SerializeField] private float _attackRange = 5;
    [SerializeField] private float _damage = 1;
    [SerializeField] private float _fireRate = 1;
    [Header("Shoot")] [SerializeField] private Transform _shootOrigin;
    [SerializeField] private LayerMask _targetLayer;
    [SerializeField] private Projectile _shootPrefab;
    [SerializeField] private SphereCollider _rangeCollider;
    [Header("Debug")] [SerializeField] bool _canAttack;

    private Transform _target;
    private float _fireRateCooldown;
    private ObjectPool<Projectile> _projectilePool;
    private Vector3 _targetPos;

    private List<Transform> _allTargets = new List<Transform>();

    protected virtual void Awake()
    {
        _rangeCollider = GetComponent<SphereCollider>();
        _rangeCollider.isTrigger = true;
    }

    //Lifecycle
    protected virtual void Start()
    {
        InitializeTower();
    }

    protected virtual void Update()
    {
        HandleShoot();
    }

    //Main methods
    private void HandleShoot()
    {
        if (!_canAttack || _target == null) return;
        if (_fireRateCooldown <= 0)
        {
            Shoot();
        }
        else
        {
            _fireRateCooldown -= Time.deltaTime;
        }
    }

    void Shoot()
    {
        if (_projectilePool == null)
        {
            Debug.LogError("Projectile Pool Not Initialized", gameObject);
            return;
        }

        Projectile projectile = _projectilePool.ActivateObject(_shootOrigin.position, Quaternion.identity);

        _targetPos = _target.position + Vector3.up;
        projectile.Setup(_targetPos, _damage, _projectilePool);
        _fireRateCooldown = _fireRate;
        //TODO Play sound fx, instantiate MUZZLE vfx
    }


    private void InitializeTower()
    {
        UpdateRange(_attackRange);
        _projectilePool = new ObjectPool<Projectile>(_shootPrefab, 20, _shootOrigin);
    }

    private void UpdateRange(float newAttackRange)
    {
        _attackRange = newAttackRange;
        _rangeCollider.radius = newAttackRange;
    }

    private void TryToSetTarget()
    {
        //Only if target is null
        if (_target != null) return;

        //Try to get closest target
        _target = GetClosestTarget();
        if (_target == null) return; //if couldn't find target, just return

        Health health = _target.GetComponentInParent<Health>();
        if (health)
        {
            health.OnDie.AddListener((gameObj) => { TryToRemoveTargetFromList(gameObj.transform); });
        }
    }

    private Transform GetClosestTarget()
    {
        Transform closest = null;

        float closestDistance = Mathf.Infinity;
        foreach (Transform target in _allTargets)
        {
            if (!target.gameObject.activeInHierarchy) continue;
            float dist = Vector3.Distance(transform.position, target.position);
            if (dist < closestDistance)
            {
                closest = target;
                closestDistance = dist;
            }
        }

        return closest;
    }

    private void ResetTarget(GameObject obj)
    {
        Health health = obj.GetComponentInParent<Health>();
        if (health)
        {
            health.OnDie.RemoveListener((gameObj) => { TryToRemoveTargetFromList(gameObj.transform); });
        }

        _target = null;

        //When, for any reason, loses a target, try to get a new one
        TryToSetTarget();
    }

    //Event Functions
    protected void OnTriggerEnter(Collider other)
    {
        if (!Util.IsInLayer(other.gameObject, _targetLayer)) return;

        TryToAddTargetToList(other.transform);
        TryToSetTarget();
    }

    private void TryToAddTargetToList(Transform targetTransform)
    {
        if (_allTargets.Contains(targetTransform)) return;
        _allTargets.Add(targetTransform);
    }

    private void TryToRemoveTargetFromList(Transform targetTransform)
    {
        if (!_allTargets.Contains(targetTransform)) return;
        _allTargets.Remove(targetTransform);

        if (targetTransform == _target)
            ResetTarget(targetTransform.gameObject);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!Util.IsInLayer(other.gameObject, _targetLayer)) return;

        TryToRemoveTargetFromList(other.transform);
    }

    private void OnDrawGizmos()
    {
        if (!GameManager.Instance.Debug) return;

        Gizmos.color = new Color(1, 1, 0, .65f);
        Gizmos.DrawWireSphere(transform.position, _attackRange);

        if (!_target) return;
        Gizmos.color = new Color(1, 0, 0, .5f);
        Gizmos.DrawSphere(_target.position, 1);
        Vector3 shootDirection = (_targetPos - _shootOrigin.position);
        Gizmos.color = new Color(0, 1, 0, 1f);

        Gizmos.DrawRay(_shootOrigin.position, shootDirection);
    }
}