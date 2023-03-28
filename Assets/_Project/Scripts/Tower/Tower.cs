using System;
using System.Collections;
using System.Collections.Generic;
using Enemies;
using UnityEngine;
using ObjectPool;
using Singletons;
using TMPro;
using UnityEngine.Events;
using UnityEngine.UI;
using Utils;

[RequireComponent(typeof(SphereCollider))]
public class Tower : MonoBehaviour
{
    [Header("Reference Assign")] public GameObject GFX;
    [SerializeField] private Transform _rangeTransform;
    [SerializeField] private Button _upgradeButton;
    [SerializeField] private TextMeshProUGUI _upgradeCostText;
    [SerializeField] private TextMeshProUGUI _upgradeLevelText;

    [Header("Upgrade")] [SerializeField] private float _rangeUpgradeMultiplier = 1.15f;
    [SerializeField] private float _damageUpgradeMultiplier = 1.25f;
    [SerializeField] private float _fireRateUpgradeMultiplier = 1.25f;
    [SerializeField] private float _costUpgradeMultiplier = 1.8f;

    [Header("Stats")] [SerializeField] private float _attackRange = 5;
    [SerializeField] private float _damage = 1;
    [SerializeField] private float _fireRate = 1;
    [SerializeField] private float _rotationSpeed = 1;
    [Header("Shoot")] [SerializeField] private Transform _shootOrigin;
    [SerializeField] private LayerMask _targetLayer;
    [SerializeField] private Projectile _shootPrefab;
    [SerializeField] private SphereCollider _rangeCollider;
    [Header("Debug")] [SerializeField] bool _canAttack;

    private Transform _target;
    private float _fireRateCooldown;
    private ObjectPool<Projectile> _projectilePool;
    private Vector3 _targetPos;
    public UnityEvent<Enemy, Projectile> OnShoot;
    private List<Transform> _allTargets = new List<Transform>();
    private int _level = 1;
    public float UpgradeCost { get; set; }

    public float AttackRange => _attackRange;

    protected virtual void Awake()
    {
        _rangeCollider = GetComponent<SphereCollider>();
        _rangeCollider.isTrigger = true;
    }

    //Lifecycle
    protected virtual void Start()
    {
        InitializeTower();
        StartCoroutine(CheckTargetCoroutine());
    }

    protected virtual void Update()
    {
        HandleShoot();
    }

    private void FixedUpdate()
    {
        FaceTarget();
    }

    public float tolerance = 0.1f; // tolerance value for comparison

    private void FaceTarget()
    {
        if (_target)
        {
            Vector3 faceDirection = (_target.position - transform.position).normalized;
            faceDirection.y = 0;
            transform.forward = Vector3.Slerp(transform.forward, faceDirection, _rotationSpeed * Time.deltaTime);
            if (Vector3.Dot(transform.forward, faceDirection) >= 1.0f - tolerance) _canAttack = true;
            else _canAttack = false;
        }
    }

    public void UpgradeTower()
    {
        GameManager.Instance.Coins.ReduceScore(UpgradeCost);
        UpdateRange(_attackRange *= _rangeUpgradeMultiplier);
        _damage *= _damageUpgradeMultiplier;
        _fireRate *= _fireRateUpgradeMultiplier;

        UpgradeCost *= _costUpgradeMultiplier;
        _upgradeCostText.text = UpgradeCost.ToString();
        _level++;
        _upgradeLevelText.text = "Lv " + _level + " Upgrade";
    }

    private void UpdateUpgradeButton(float changeValue, float currentMoney)
    {
        if (_upgradeButton == null) return;
        //Disable button
        _upgradeButton.interactable = currentMoney >= UpgradeCost;


        if (_upgradeCostText)
            _upgradeCostText.text = UpgradeCost.ToString();
        if (_upgradeLevelText)
            _upgradeLevelText.text = "Lv " + _level + " Upgrade";

        LayoutRebuilder.ForceRebuildLayoutImmediate(_upgradeCostText.rectTransform);
        LayoutRebuilder.ForceRebuildLayoutImmediate(_upgradeLevelText.rectTransform);
    }

    private IEnumerator CheckTargetCoroutine()
    {
        while (true)
        {
            TryToSetTarget();
            yield return new WaitForSeconds(1);
        }
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

        _targetPos = _target.position + (Vector3.up * .75f);
        projectile.Setup(_targetPos, _damage, _projectilePool);
        _fireRateCooldown = _fireRate;
        OnShoot.Invoke(_target.GetComponent<Enemy>(), projectile);
        //TODO Play sound fx, instantiate MUZZLE vfx
    }


    private void InitializeTower()
    {
        UpdateRange(_attackRange);
        _projectilePool = new ObjectPool<Projectile>(_shootPrefab, 20, _shootOrigin);
        GameManager.Instance.Coins.OnChangeValue.AddListener(UpdateUpgradeButton);
        UpdateUpgradeButton(0, GameManager.Instance.Coins.CurrentValue);
    }

    private void UpdateRange(float newAttackRange)
    {
        _attackRange = newAttackRange;
        _rangeCollider.radius = newAttackRange;
        if (_rangeTransform)
            _rangeTransform.localScale = new Vector3(_attackRange, 1, _attackRange);
    }


    private void TryToSetTarget()
    {
        //Only if target is null
        //if (_target != null) return;

        //Try to get closest target
        Transform lastTarget = _target;
        _target = GetClosestTarget();
        if (_target == null) return; //if couldn't find target, just return

        if (lastTarget != null && lastTarget != _target)
        {
            SetTargetVisual(lastTarget, false);
        }

        SetTargetVisual(_target, true);

        Health health = _target.GetComponentInParent<Health>();
        if (health)
        {
            health.OnDie.AddListener((gameObj) => { TryToRemoveTargetFromList(gameObj.transform); });
        }
    }

    void SetTargetVisual(Transform target, bool showVisual)
    {
        EnemyTargetVisual visual = target.GetComponentInParent<EnemyTargetVisual>();
        if (visual)
        {
            visual.SetTargetVisualActive(showVisual);
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

        SetTargetVisual(_target, false);

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

    private void OnValidate()
    {
        if (_rangeCollider)
        {
            _rangeCollider.radius = _attackRange;
            UpdateRange(_attackRange);
        }
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