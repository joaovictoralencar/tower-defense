using System;
using Cinemachine;
using DG.Tweening;
using Enemies;
using ObjectPool;
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

        [Header("UI Change Text Prefab")] [SerializeField]
        private PlayerScoreChangeUI _changePointsText;

        [SerializeField] private PlayerScoreChangeUI _changeCoinsText;
        [SerializeField] private PlayerScoreChangeUI _changeDefeatedText;
        [SerializeField] private PlayerScoreChangeUI _changeDamageText;
        public PlayerScore Points => _points;

        public ObjectPool<PlayerScoreChangeUI> PointsTextPool => _pointsTextPool;

        public ObjectPool<PlayerScoreChangeUI> CoinsTextPool => _coinsTextPool;

        public ObjectPool<PlayerScoreChangeUI> DefeatedTextPool => _defeatedTextPool;

        public ObjectPool<PlayerScoreChangeUI> DamageTextPool => _damageTextPool;

        public CinemachineVirtualCamera MainVirtualCamera => _mainVirtualCamera;

        public PlayerScore Coins => _coins;

        [SerializeField] CinemachineVirtualCamera _mainVirtualCamera;


        private ObjectPool<PlayerScoreChangeUI> _pointsTextPool;
        private ObjectPool<PlayerScoreChangeUI> _coinsTextPool;
        private ObjectPool<PlayerScoreChangeUI> _defeatedTextPool;
        private ObjectPool<PlayerScoreChangeUI> _damageTextPool;

        [SerializeField] private PlayerMainTower _playerMainTower;

        private void Start()
        {
            DOTween.SetTweensCapacity(500, 400);
            InitializePlayerScores();
            OnEnemyDie.AddListener(OnEnemyDieCallback);
        }

        public void CloseGame()
        {
            Application.Quit();
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
            _pointsTextPool =
                InitializePlayerScore(_points, _changePointsText);
            _coinsTextPool = InitializePlayerScore(_coins, _changeCoinsText);
            _defeatedTextPool = InitializePlayerScore(_defeated, _changeDefeatedText);

            _damageTextPool = new ObjectPool<PlayerScoreChangeUI>(_changeDamageText, 50, transform);
        }

        private ObjectPool<PlayerScoreChangeUI> InitializePlayerScore(PlayerScore score,
            PlayerScoreChangeUI playerScoreChangeUI)
        {
            if (score == null) return null;
            score.Initialize();
            ObjectPool<PlayerScoreChangeUI> pool =
                new ObjectPool<PlayerScoreChangeUI>(playerScoreChangeUI, 50, transform);

            pool.OnObjectActivate += (scoreChangeUI) =>
            {
                scoreChangeUI.OnComplete.AddListener(pool.DeactivateObject);
            };
            return pool;
        }

        private void OnEnemyDieCallback(Enemy enemy)
        {
            AddPoints(enemy);
            AddCoin(enemy);
            AddDefeated(enemy);
        }

        void AddPoints(Enemy enemy)
        {
            _points.AddScore(enemy.PointsToGive);
            PlayerScoreChangeUI obj = _pointsTextPool.ActivateObject(enemy.ScoreUIPosition.position);
            obj.SetText("+" + enemy.PointsToGive);
            obj.Animate(false);
        }

        void AddCoin(Enemy enemy)
        {
            _coins.AddScore(enemy.CoinsToGive);
            PlayerScoreChangeUI obj = _coinsTextPool.ActivateObject(enemy.ScoreUIPosition.position);
            obj.SetText("+" + enemy.CoinsToGive);
            obj.Animate();
        }

        public void ReduceCoin(float reduceAmount)
        {
            _coins.ReduceScore(reduceAmount);
            PlayerScoreChangeUI obj = _coinsTextPool.ActivateObject(_playerMainTower.ScoreGainPostion.position);
            obj.SetText("-" + reduceAmount);
            obj.Animate();
        }

        void AddDefeated(Enemy enemy)
        {
            _defeated.AddScore(1);
            PlayerScoreChangeUI obj = _defeatedTextPool.ActivateObject(_playerMainTower.ScoreGainPostion.position);
            obj.SetText("+" + 1);
            obj.Animate();
        }
    }
}