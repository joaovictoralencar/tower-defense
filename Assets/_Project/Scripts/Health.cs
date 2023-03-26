using System;
using Singletons;
using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    [SerializeField] private float _initialHealth = 10;

    public UnityEvent<GameObject> OnDie;

    /// <summary>
    /// float deltaChange, float healthBefore
    /// </summary>
    public UnityEvent<float, float> OnChangeHealth;
    public UnityEvent<Health> OnInitialize;

    public UnityEvent<float, float, bool> OnChangeMaxHealth;

    private float _currentHealth;
    private float _maxHealth = 10;

    public float MaxHealth => _maxHealth;

    public float CurrentHealth => _currentHealth;

    private void OnEnable()
    {
        Initialize();
    }

    void Initialize()
    {
        _maxHealth = _initialHealth;
        _currentHealth = _initialHealth;
        ChangeMaxHealth(_initialHealth, _currentHealth, true);
        OnInitialize.Invoke(this);
    }

    public void Die()
    {
        TakeDamage(_maxHealth);
    }

    public void Heal(float healAmount)
    {
        ChangeHealth(Math.Abs(healAmount));
    }

    public void TakeDamage(float damage)
    {
        ChangeHealth(-Math.Abs(damage));
    }

    public void ChangeMaxHealth(float newMaxHealth, float current, bool healToMax = false)
    {
        float deltaChange = newMaxHealth - current;
        _maxHealth = newMaxHealth;
        OnChangeMaxHealth.Invoke(newMaxHealth, current, healToMax);
        if (healToMax)
            ChangeHealth(deltaChange);
    }

    private void ChangeHealth(float changeValue)
    {
        if (!Application.isPlaying) return;
        //Damage
        if (changeValue < 0)
        {
            if (GameManager.Instance.Debug)
                Debug.Log(name + " took " + changeValue + " damage. ", gameObject);
        }
        else if (changeValue > 0) //Heal
        {
            if (GameManager.Instance.Debug)
                Debug.Log(name + " healed " + changeValue + " points", gameObject);
        } else return;

        //Change current health
        _currentHealth += changeValue;

        //Avoid negative number and number above max health
        _currentHealth = Mathf.Clamp(_currentHealth, 0, _maxHealth);

        //Call OnChangeHealth event
        OnChangeHealth.Invoke(changeValue, _currentHealth);
        
        if (GameManager.Instance.Debug)
            Debug.Log(name + " health: " + _currentHealth, gameObject);

        //Handle Death
        if (_currentHealth <= 0)
        {
            if (GameManager.Instance.Debug)
                Debug.Log(name + " died ", gameObject);
            OnDie.Invoke(gameObject);
        }
    }
}