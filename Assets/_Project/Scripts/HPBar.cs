using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class HPBar : MonoBehaviour
{
    [Header("Main bar")] [SerializeField] private Image _mainBarImage;
    [SerializeField] private Color _mainBarColor = Color.green;
    [SerializeField] private float _mainAnimTime = .05f;

    [Header("Secondary bar")] [SerializeField]
    private Image _secondaryBarImage;

    [SerializeField] private Color _secondaryBarColor = Color.red;
    [SerializeField] private float _secondaryAnimTime = .15f;

    protected float maxHealth = 100;
    private Tween _barTween;

    private Camera _cam;

    private bool _worldSpace;
    [SerializeField] private bool _faceCamera;

    private void Start()
    {
        _worldSpace = GetComponent<Canvas>().renderMode == RenderMode.WorldSpace && _faceCamera;

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
        if (_secondaryBarImage) _secondaryBarImage.color = _secondaryBarColor;
        else Debug.LogWarning("Secondary health bar image is missing", gameObject);
    }

    private void Update()
    {
        if (!_worldSpace) return;
        if (_cam == null)
        {
            _cam = Camera.main;
        }

        transform.LookAt(transform.position + _cam.transform.rotation * Vector3.forward,
            _cam.transform.rotation * Vector3.up);
    }

    public void Initialize(float maxHealth, Camera cam = default)
    {
        _mainBarImage.fillAmount = 1;
        _secondaryBarImage.fillAmount = 1;
        this.maxHealth = maxHealth;
        if (cam != default) _cam = cam;
    }

    public void ChangeMaxHealth(float newMaxHealth, float current, bool healToMax)
    {
        float deltaChange = newMaxHealth - current;
        maxHealth = newMaxHealth;
        if (healToMax)
            ChangeHealthBar(deltaChange, newMaxHealth);
    }

    public void ChangeHealthBar(float changeValue, float newHealth)
    {
        float healthPercent = newHealth / maxHealth;
        if (_barTween != null && _barTween.IsPlaying()) _barTween.Complete();

        //heal
        if (changeValue > 0)
        {
            _barTween = _mainBarImage.DOFillAmount(healthPercent, _mainAnimTime).OnComplete(() =>
            {
                if (_secondaryBarImage)
                    _secondaryBarImage.fillAmount = healthPercent;
            }).SetAutoKill(false);
        }
        //damage
        else if (changeValue < 0)
        {
            _barTween = _mainBarImage.DOFillAmount(healthPercent, _mainAnimTime).OnComplete(() =>
            {
                if (_secondaryBarImage)
                    _secondaryBarImage.DOFillAmount(healthPercent, _secondaryAnimTime);
            }).SetAutoKill(false);
        }
    }
}