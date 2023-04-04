using Enemies;
using Singletons;
using UnityEngine;
using UnityEngine.Serialization;

public class WaveManager : MonoBehaviour
{
    [SerializeField] private PlayerScore _enemiesPerWave;
    [SerializeField] private PlayerScore _waveNumber;
    [SerializeField] private WaveHUD _waveHUD;

    [SerializeField] private EnemyManager _enemyManager;
    [SerializeField] private bool _spawnEnemies = true;
    [SerializeField] private float _timeToSpawn = 1;

    [Space(10)] [Header("Waves")] [SerializeField]
    private float _intensity = 1.0f; // Intensity of the growth curve

    [SerializeField] private int _initialEnemies = 10; // Initial number of enemies


    private int _currentEnemies; // Current number of enemies
    private float _timeElapsed; // Time elapsed since the start of the game
    private float _spawnCooldown;
    private int _numEnemiesToSpawn;
    private int _numSpawnedEnemies;
    private int _defeatedPerWave;
    private int _undefeatedCount;

    private void Start()
    {
        GameManager.Instance.OnPlayerDie.AddListener(OnPlayerDie);
        GameManager.Instance.OnEnemyDie.AddListener(OnEnemyDie);
        if (_enemiesPerWave)
            _enemiesPerWave.Initialize();
        if (_waveNumber)
            _waveNumber.Initialize();
    }

    private void OnEnemyDie(Enemy enemy)
    {
        _undefeatedCount--;
        _defeatedPerWave++;
        _enemiesPerWave.SetScore(_defeatedPerWave);
    }

    private void Update()
    {
        SpawnEnemiesOverTime();

        //Debug only
        if (Input.GetKeyDown(KeyCode.T))
        {
            _spawnCooldown = 0;
        }
    }

    void OnPlayerDie(GameObject gameObj)
    {
        _spawnEnemies = false;
    }

    void OnStartWave()
    {
        _waveNumber.AddScore(1);
        _defeatedPerWave = 0;
        _enemiesPerWave.ChangeMaxValue(_numSpawnedEnemies + _undefeatedCount);
        _enemiesPerWave.SetScore(_defeatedPerWave);
        _waveHUD.ChangeBarMaxValue(_spawnCooldown, false);
    }

    void OnEndWave()
    {
        _numSpawnedEnemies = 0;
    }

    private void SpawnEnemiesOverTime()
    {
        if (!_spawnEnemies) return;
        _timeElapsed += Time.deltaTime;

        if (_spawnCooldown <= 0)
        {
            _spawnCooldown = _timeToSpawn;
            OnEndWave();
            SpawnEnemies();
        }
        else _spawnCooldown -= Time.deltaTime;

        _waveHUD.ChangeBar(_spawnCooldown);
    }

    private void SpawnEnemies()
    {
        _numEnemiesToSpawn =
            Mathf.RoundToInt(_initialEnemies * Mathf.Pow(1 + _intensity, _timeElapsed / _spawnCooldown));


        for (int i = 0; i < _numEnemiesToSpawn; i++)
        {
            if (_enemyManager.SpawnRandomEnemy())
            {
                _numSpawnedEnemies++;
            }
        }

        OnStartWave();
        _undefeatedCount += _numSpawnedEnemies;
    }
}