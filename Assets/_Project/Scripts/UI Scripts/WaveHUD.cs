using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class WaveHUD : MonoBehaviour
{
    [SerializeField] private PlayerScore _enemyPerWave;
    [SerializeField] private TextMeshProUGUI _wavesProgressText;


    [Header("Main bar")] [SerializeField] private Image _mainBarImage;
    [SerializeField] private Color _mainBarColor = Color.green;

    private float _maxEnemyPerWave;
    private float _maxBarValue;

    private void Awake()
    {
        if (_enemyPerWave == null) return;
        _maxEnemyPerWave = _enemyPerWave.MaxValue;

        //On Change
        _enemyPerWave.OnChangeValue.AddListener(OnChangeValue); //text

        //On Change Max
        _enemyPerWave.OnChangeMaxValue.AddListener(OnChangeMaxWaves); //text
    }
    
    public void ChangeBarMaxValue(float newMaxValue, bool changeToMax)
    {
        _maxBarValue = newMaxValue;
        if (changeToMax)
            ChangeBar(newMaxValue);
    }
    private void Start()
    {
        SetColors();
    }

    private void OnValidate()
    {
        SetColors();
    }

    void SetColors()
    {
        if (_mainBarImage) _mainBarImage.color = _mainBarColor;
        else Debug.LogError("Main health bar image is missing", gameObject);
    }

    private void OnChangeMaxWaves(float newMaxWave, float current, bool changeToMax)
    {
        _maxEnemyPerWave = newMaxWave;
        _wavesProgressText.text = current + "/" + _maxEnemyPerWave;
    }

    private void OnChangeValue(float changeValue, float newValue)
    {
        _wavesProgressText.text = newValue + "/" + _maxEnemyPerWave;
    }

    public void ChangeBar(float newValue)
    {
        float valuePercent = newValue / _maxBarValue;
        _mainBarImage.fillAmount = valuePercent;
    }
}