using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ObjectPool;
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

    private void Awake()
    {
        _rangeCollider = GetComponent<SphereCollider>();
        _rangeCollider.isTrigger = true;
    }

    //Lifecycle
    private void Start()
    {
        InitializeTower();
    }

    private void Update()
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
        projectile.Setup(_target.position, _damage, _projectilePool);
        _fireRateCooldown = _fireRate;
        //TODO Play sound fx, instantiate MUZZLE vfx
    }

    private void InitializeTower()
    {
        UpdateRange(_attackRange);
        _projectilePool = new ObjectPool<Projectile>(_shootPrefab, 20, transform);
    }

    private void UpdateRange(float newAttackRange)
    {
        _attackRange = newAttackRange;
        _rangeCollider.radius = newAttackRange;
    }

    private void SetTarget(Transform target)
    {
        _target = target;
        Health health = target.GetComponentInParent<Health>();
        if (health)
        {
            health.OnDie.AddListener(ResetTarget);
        }
    }

    private void ResetTarget()
    {
        Health health = _target.GetComponentInParent<Health>();
        if (health)
        {
            health.OnDie.RemoveListener(ResetTarget);
        }

        _target = null;
    }

    //Event Functions
    private void OnTriggerEnter(Collider other)
    {
        if (_target != null) return;
        if (Util.IsInLayer(other.gameObject, _targetLayer))
        {
            SetTarget(other.transform);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (_target == other.transform)
        {
            ResetTarget();
        }
    }
}