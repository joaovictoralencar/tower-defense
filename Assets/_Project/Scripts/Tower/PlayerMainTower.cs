using Singletons;
using UnityEngine;

[RequireComponent(typeof(Health))]
public class PlayerMainTower : Tower
{
    private Health _health;

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
    }

    private void OnDie(GameObject gameObj)
    {
        GameManager.Instance.OnPlayerDie.Invoke(gameObj);
        Destroy(gameObj);
    }
    
}