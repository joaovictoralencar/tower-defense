using System;
using System.Collections;
using System.Collections.Generic;
using Singletons;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class DefenseUIData : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _nameText;
    [SerializeField] private TextMeshProUGUI _descriptionText;
    [SerializeField] private TextMeshProUGUI _valueText;
    [SerializeField] private Button _buyButton;
    [HideInInspector] public UnityEvent<DefenseData> OnSelect;
    [HideInInspector] public UnityEvent<DefenseData> OnBuy;

    private DefenseData _defenseData;
    private float _buyingCost;
    public DefenseData Data => _defenseData;

    private void Awake()
    {
        _buyButton.onClick.AddListener(OnSelectCallback);
        OnBuy.AddListener(OnBuyDefense);
    }


    private void OnBuyDefense(DefenseData defenseData)
    {
        _buyingCost *= defenseData.defenseCostMultiplier;
        UpdateCost();
    }

    private void Start()
    {
        GameManager.Instance.Coins.OnChangeValue.AddListener(UpdateBuyButton);
        UpdateBuyButton(0, GameManager.Instance.Coins.CurrentValue);
    }

    public void UpdateBuyingCost()
    {
        _buyingCost = _defenseData.defenseCost;
        UpdateCost();
    }

    private void UpdateBuyButton(float changeValue, float currentMoney)
    {
        if (_defenseData && _buyButton)
            _buyButton.interactable = currentMoney >= _defenseData.defenseCost;
    }

    private void OnSelectCallback()
    {
        OnSelect.Invoke(_defenseData);
    }

    void UpdateCost()
    {
        _valueText.text = _buyingCost.ToString();
    }

    public void UpdateUI(DefenseData defenseData)
    {
        _nameText.text = defenseData.defenseName;
        _descriptionText.text = defenseData.defenseDescription;
        _defenseData = defenseData;
        //_buyingCost = defenseData.defenseCost;
        _valueText.text = _buyingCost.ToString();
    }
}