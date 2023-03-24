using System;
using UnityEngine;

namespace Enemies
{
    [RequireComponent(typeof(NavMeshMovement))]
    public class Enemy : MonoBehaviour
    {
        [SerializeField] private float _damage = 1;
        [SerializeField] private EnemyType _type = EnemyType.Basic;
        [SerializeField] private HPBar _hpBar;
        private NavMeshMovement _movement;
        private Health _health;

        public NavMeshMovement Movement
        {
            get
            {
                if (_movement == null)
                    _movement = GetComponent<NavMeshMovement>();
                return _movement;
            }
        }

        public Health Health
        {
            get
            {
                if (_health == null)
                    _health = GetComponent<Health>();
                return _health;
            }
        }

        public EnemyType Type => _type;
        public float Damage => _damage;

        private void Awake()
        {
            _movement = GetComponent<NavMeshMovement>();
            _health = GetComponent<Health>();
            InitializeHPBar();
        }

        private void OnEnable()
        {
            InitializeHPBar();
        }

        private void InitializeHPBar()
        {
            if (_hpBar == null) _hpBar = GetComponentInChildren<HPBar>();
            if (_health)
            {
                _hpBar.Initialize(_health.MaxHealth);
                _health.OnChangeHealth.AddListener(_hpBar.ChangeHealthBar);
                _health.OnChangeMaxHealth.AddListener(_hpBar.ChangeMaxHealth);
            }
            else Debug.LogWarning("Could not initialize HPBar because health is missing");
        }
    }

    public enum EnemyType
    {
        Basic,
        Tank,
        Runner,
        Boss
    }
}