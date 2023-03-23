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

        [Header("QA Enemy Spawn")] [SerializeField]
        private int _numEnemiesToSpawn = 1;

        [Space(10)] [Header("Enemy Spawn")] [SerializeField]
        private Enemy[] _enemiesPrefabs;

        [SerializeField] private int _initialEnemyPoolSize;
        [SerializeField] private Transform _enemiesHolder;


        private Dictionary<EnemyType, ObjectPool<Enemy>> _enemyPools;

        private void Awake()
        {
            InitializeEnemyPools();
        }

        void InitializeEnemyPools()
        {
            //Instantiate new pool dictionary
            _enemyPools = new Dictionary<EnemyType, ObjectPool<Enemy>>();

            foreach (var enemiesPrefab in _enemiesPrefabs)
            {
                //Avoid existing pools
                if (_enemyPools.ContainsKey(enemiesPrefab.Type)) continue;

                //Create pool from current type of enemy
                ObjectPool<Enemy> enemyPool =
                    new ObjectPool<Enemy>(enemiesPrefab, _initialEnemyPoolSize, _enemiesHolder);

                enemyPool.OnObjectActivate += OnActivateBasicEnemy;
                enemyPool.OnObjectDeactivate += OnDeactivateBasicEnemy;
                //Add pool to the pool dictionary
                _enemyPools.Add(enemiesPrefab.Type, enemyPool);
            }
        }

        private void Update()
        {
            //Debug only
            if (Input.GetKeyDown(KeyCode.T))
            {
                SpawnEnemies();
            }
        }

        private void SpawnEnemies()
        {
            for (int i = 0; i < _numEnemiesToSpawn; i++)
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
            EnemyType type = _enemiesPrefabs[randomIndex].Type;
            _enemyPools[type].ActivateObject(Vector3.zero, Quaternion.identity);
        }

        private void OnEnemyDie(GameObject obj)
        {
            Enemy enemy = obj.GetComponentInParent<Enemy>();
            
            if (enemy == null) return;
            
            _enemyPools[enemy.Type].DeactivateObject(enemy);
        }

        private void OnActivateBasicEnemy(Enemy enemy)
        {
            enemy.Movement.MoveToLocation(_playerMainTower.position);
            enemy.Health.OnDie.AddListener(OnEnemyDie);
        }

        private void OnDeactivateBasicEnemy(Enemy enemy)
        {
            enemy.Movement.ResetDestination();
            enemy.Health.OnDie.RemoveListener(OnEnemyDie);
        }
    }
}