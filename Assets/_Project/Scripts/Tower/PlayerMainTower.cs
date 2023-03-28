using Singletons;
using UnityEngine;

[RequireComponent(typeof(Health))]
public class PlayerMainTower : Tower
{
    [SerializeField] private float _initialUpgradeCost; 
    private Health _health;
    public Transform ScoreGainPostion;

    public Health Health
    {
        get
        {
            if (_health == null)
                _health = GetComponent<Health>();
            return _health;
        }
    }

    protected override void Awake()
    {
        base.Awake();
        _health = GetComponent<Health>();
        _health.OnDie.AddListener(OnDie);
        UpgradeCost = _initialUpgradeCost;
    }

    protected override void Start()
    {
        base.Start();
        GameManager.Instance.OnGenerateGrid.AddListener((start, end) => SetMainTowerPosition(end));
    }

    void SetMainTowerPosition(Vector3 pos)
    {
        transform.position = pos;
    }

    private void OnDie(GameObject gameObj)
    {
        GameManager.Instance.OnPlayerDie.Invoke(gameObj);
        Destroy(gameObj);
    }
}