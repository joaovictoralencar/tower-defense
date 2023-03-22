using System;
using UnityEngine;

namespace Enemies
{
    [RequireComponent(typeof(NavMeshMovement))]
    public class Enemy : MonoBehaviour
    {
        private NavMeshMovement _movement;

        public NavMeshMovement Movement
        {
            get
            {
                if (_movement == null)
                    _movement = GetComponent<NavMeshMovement>();
                return _movement;
            }
        }

        private void Awake()
        {
            _movement = GetComponent<NavMeshMovement>();
        }
    }
}