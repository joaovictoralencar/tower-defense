using System;
using System.Collections.Generic;
using ObjectPool;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Enemies
{
    public class EnemyManager : MonoBehaviour
    {
        [SerializeField] private Transform _playerMainTower;
        [Space(10)]
        [Header("Enemy Spawn")]
        [SerializeField] private Enemy[] _enemiesPrefabs;
        [SerializeField] private int _initialEnemyPoolSize;
        [SerializeField] private Transform _enemiesHolder;
        
        
        private Dictionary<Enemy, ObjectPool<Enemy>> _enemyPools;

        private void Awake()
        {
            InitializeEnemyPools();
        }

        void InitializeEnemyPools()
        {
            //Instantiate new pool dictionary
            _enemyPools = new Dictionary<Enemy, ObjectPool<Enemy>>();
            
            foreach (var enemiesPrefab in _enemiesPrefabs)
            {
                //Avoid existing pools
                if (_enemyPools.ContainsKey(enemiesPrefab)) continue;

                //Create pool from current type of enemy
                ObjectPool<Enemy> enemyPool =
                    new ObjectPool<Enemy>(enemiesPrefab, _initialEnemyPoolSize, _enemiesHolder);

                enemyPool.OnObjectActivate += OnActivateBasicEnemy;
                //Add pool to the pool dictionary
                _enemyPools.Add(enemiesPrefab, enemyPool);
            }
        }

        private void Update()
        {
            //Debug only
            if (Input.GetKeyDown(KeyCode.T))
            {
                SpawnRandomEnemy();
            }
        }

        private void SpawnRandomEnemy()
        {
            if (_enemiesPrefabs.Length == 0)
            {
                Debug.LogWarning("Enemies prefabs list is empty, can't spawn");
                return;
            }
            int randomIndex = Random.Range(0, _enemiesPrefabs.Length);
            _enemyPools[_enemiesPrefabs[randomIndex]].ActivateObject(Vector3.zero, Quaternion.identity);
        }

        private void OnActivateBasicEnemy(Enemy enemy)
        {
            enemy.Movement.MoveToLocation(_playerMainTower.position);
        }
    }
}