using System;
using UnityEngine;

namespace Enemies
{
    [RequireComponent(typeof(NavMeshMovement))]
    public class Enemy : MonoBehaviour
    {
        [SerializeField] private EnemyType _type = EnemyType.Basic;
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

        private void Awake()
        {
            _movement = GetComponent<NavMeshMovement>();
            _health = GetComponent<Health>();
        }
    }

    public enum EnemyType
    {
        Basic, Tank, Runner, Boss
    }
}