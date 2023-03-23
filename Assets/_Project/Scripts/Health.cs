using System;
using Singletons;
using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    [SerializeField] private float _initialHealth = 10;

    public UnityEvent<float> OnTakeDamage;
    public UnityEvent<GameObject> OnDie;

    /// <summary>
    /// float deltaChange, float healthBefore
    /// </summary>
    public UnityEvent<float, float> OnChangeHealth;

    private float _currentHealth;
    private float _maxHealth = 10;

    private void Start()
    {
        OnTakeDamage.AddListener(TakeDamage);
        Initialize();
    }

    void Initialize()
    {
        _maxHealth = _initialHealth;
        ChangeHealth(_maxHealth);
    }

    void TakeDamage(float damage)
    {
        ChangeHealth(-Math.Abs(damage));
    }

    void ChangeHealth(float changeValue)
    {
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

        //Call OnChangeHealth event
        OnChangeHealth.Invoke(changeValue, _currentHealth);

        //Avoid negative number and number above max health
        _currentHealth = Mathf.Clamp(_currentHealth, 0, _maxHealth);

        //Handle Death
        if (_currentHealth <= 0)
        {
            if (GameManager.Instance.Debug)
                Debug.Log(name + " died ", gameObject);
            OnDie.Invoke(gameObject);
        }
    }
}