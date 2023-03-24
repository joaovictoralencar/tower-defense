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

    public UnityEvent<float, bool> OnChangeMaxHealth;

    private float _currentHealth;
    private float _maxHealth = 10;

    public float MaxHealth => _maxHealth;

    private void Start()
    {
        Initialize();
    }

    void Initialize()
    {
        _maxHealth = _initialHealth;
        ChangeHealth(_maxHealth);
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

    public void ChangeMaxHealth(float newMaxHealth, bool healToMax = false)
    {
        float deltaChange = newMaxHealth - _maxHealth;
        _maxHealth = newMaxHealth;
        OnChangeMaxHealth.Invoke(newMaxHealth, healToMax);
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
                Debug.Log(name + " took " + changeValue + " damage", gameObject);
        }
        else if (changeValue > 0) //Heal
        {
            if (GameManager.Instance.Debug)
                Debug.Log(name + " healed " + changeValue + " points", gameObject);
        }
        else return;

        //Change current health
        _currentHealth += changeValue;

        //Avoid negative number and number above max health
        _currentHealth = Mathf.Clamp(_currentHealth, 0, _maxHealth);
        
        //Call OnChangeHealth event
        OnChangeHealth.Invoke(changeValue, _currentHealth);


        //Handle Death
        if (_currentHealth <= 0)
        {
            if (GameManager.Instance.Debug)
                Debug.Log(name + " died ", gameObject);
            OnDie.Invoke(gameObject);
        }
    }
}