using System;
using DG.Tweening;
using Enemies;
using UnityEngine;
using UnityEngine.Events;

namespace Singletons
{
    public class GameManager : Singleton<GameManager>
    {
        public bool Debug = true;
        public float timeScale = 1;

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
            DOTween.SetTweensCapacity(500, 50);
            InitializePlayerScores();
            OnEnemyDie.AddListener(OnEnemyDieCallback);
        }

        private void Update()
        {
            Time.timeScale = timeScale;

            if (Input.GetKeyDown(KeyCode.Alpha0))
            {
                timeScale = 0;
            }
            else if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                timeScale = 1;
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                timeScale = 2;
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                timeScale = 3;
            }
            else if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                timeScale = 4;
            }
            else if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                timeScale = 5;
            }
            else if (Input.GetKeyDown(KeyCode.Alpha6))
            {
                timeScale = 6;
            }
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