using TMPro;
using UnityEngine;

public class PlayerHPBar : HPBar
{
    [SerializeField] private Health _health;
    [SerializeField] private TextMeshProUGUI _healthText;
    
    private void Awake()
    {
        if (_health == null) return;
        maxHealth = _health.MaxHealth;

        //On Change
        _health.OnChangeHealth.AddListener(ChangeHealthBar); //bar
        _health.OnChangeHealth.AddListener(OnChangeHealthBar); //text

        //On Change Max
        _health.OnChangeMaxHealth.AddListener(ChangeMaxHealth); //bar
        _health.OnChangeMaxHealth.AddListener(OnChangeMaxHealth); //text
    }

    private void OnChangeMaxHealth(float newMaxHealth, float current, bool changeToMax)
    {
        maxHealth = newMaxHealth;
        _healthText.text = current + "/" + maxHealth;
    }

    private void OnChangeHealthBar(float changeValue, float newHealth)
    {
        _healthText.text = newHealth + "/" + maxHealth;
    }
}