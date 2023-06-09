using System.Collections.Generic;
using ObjectPool;
using Singletons;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Enemies
{
    public class EnemyManager : MonoBehaviour
    {
        [SerializeField] private Transform _playerMainTower;
        
        [Space(10)] [Header("Enemy Spawn")] [SerializeField]
        private Enemy[] _enemiesPrefabs;

        [SerializeField] private int _initialEnemyPoolSize;
        [SerializeField] private Transform _enemiesSpawnTransform;
        
        private Dictionary<EnemyType, ObjectPool<Enemy>> _enemyPools;


        private void Awake()
        {
            InitializeEnemyPools();
        }

        private void Start()
        {
            GameManager.Instance.OnGenerateGrid.AddListener(SetSpawnOrigin);
        }

        void SetSpawnOrigin(Vector3 startPos, Vector3 endPos)
        {
            _enemiesSpawnTransform.position = startPos;
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
                    new ObjectPool<Enemy>(enemiesPrefab, _initialEnemyPoolSize, _enemiesSpawnTransform);

                enemyPool.OnObjectActivate += OnActivateBasicEnemy;
                enemyPool.OnObjectDeactivate += OnDeactivateBasicEnemy;
                //Add pool to the pool dictionary
                _enemyPools.Add(enemiesPrefab.Type, enemyPool);
            }
        }
        
        public Enemy SpawnRandomEnemy()
        {
            if (_enemiesPrefabs.Length == 0)
            {
                Debug.LogWarning("Enemies prefabs list is empty, can't spawn");
                return null;
            }

            int randomIndex = Random.Range(0, _enemiesPrefabs.Length);
            EnemyType type = _enemiesPrefabs[randomIndex].Type;
            return _enemyPools[type].ActivateObject(_enemiesSpawnTransform.position, Quaternion.identity);
        }

        private void OnEnemyDie(GameObject obj)
        {
            Enemy enemy = obj.GetComponentInParent<Enemy>();

            if (enemy == null) return;

            _enemyPools[enemy.Type].DeactivateObject(enemy);
            GameManager.Instance.OnEnemyDie.Invoke(enemy);
        }

        private void OnActivateBasicEnemy(Enemy enemy)
        {
            enemy.Movement.SetDestination(_playerMainTower.position);
            enemy.Health.OnDie.AddListener(OnEnemyDie);
        }

        private void OnDeactivateBasicEnemy(Enemy enemy)
        {
            enemy.Movement.ResetDestination();
            enemy.Health.OnDie.RemoveListener(OnEnemyDie);
        }
    }
}