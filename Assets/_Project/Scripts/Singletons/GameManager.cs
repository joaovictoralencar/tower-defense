using System;
using Enemies;
using UnityEngine;
using UnityEngine.Events;

namespace Singletons
{
    public class GameManager : Singleton<GameManager>
    {
        public bool Debug = true;

        public UnityEvent<GameObject> OnPlayerDie = new UnityEvent<GameObject>();
        public UnityEvent<Enemy> OnEnemyDie = new UnityEvent<Enemy>();

        /// <summary>
        /// startPosition, endposition
        /// </summary>
        public UnityEvent<Vector3, Vector3> OnGenerateGrid = new UnityEvent<Vector3, Vector3>();

        [SerializeField] private PlayerScore _points;
        [SerializeField] private PlayerScore _coins;
        [SerializeField] private PlayerScore _defeated;

        public PlayerScore Points => _points;

        private void Start()
        {
            InitializePlayerScores();
            OnEnemyDie.AddListener(OnEnemyDieCallback);
        }

        private void InitializePlayerScores()
        {
            InitializePlayerScore(_points);
            InitializePlayerScore(_coins);
            InitializePlayerScore(_defeated);
        }

        private void InitializePlayerScore(PlayerScore score)
        {
            if (score == null) return;
            score.Initialize();
        }

        private void OnEnemyDieCallback(Enemy enemy)
        {
            _points.AddScore(enemy.PointsToGive);
            _coins.AddScore(enemy.CoinsToGive);
            _defeated.AddScore(1);
        }
    }
}