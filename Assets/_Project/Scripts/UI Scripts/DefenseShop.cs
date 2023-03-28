using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Singletons;
using UnityEngine;
using UnityEngine.UI;

public class DefenseShop : MonoBehaviour
{
    [SerializeField] private AllDefensesData _allDefenseData;
    [SerializeField] private DefenseUIData _shopDataBuyPrefab;
    [SerializeField] private RectTransform _shopDefenseHolder;
    [SerializeField] private BuyDefensePreview _buyDefensePreviewPrefab;
    [SerializeField] private RectTransform _isBuyingText;
    [SerializeField] private RectTransform _shopScrollRect;

    private RectTransform _rt;
    private float _startYAnchorPos;
    private bool _buyingDefense;

    private void Start()
    {
        _rt = GetComponent<RectTransform>();
        _startYAnchorPos = _rt.anchoredPosition.y - 30f;
        InitializeStore();
        HideIsBuyingText();
    }

    void InitializeStore()
    {
        foreach (var defenseData in _allDefenseData.allDefenses)
        {
            DefenseUIData defensedUI = Instantiate(_shopDataBuyPrefab, _shopDefenseHolder);
            defensedUI.UpdateUI(defenseData);
            defensedUI.OnSelect.AddListener(OnSelectDefense);
        }

        Canvas.ForceUpdateCanvases();
        LayoutRebuilder.ForceRebuildLayoutImmediate(_shopDefenseHolder);
    }

    private void OnSelectDefense(DefenseData defenseData)
    {
        BuyDefensePreview preview = Instantiate(_buyDefensePreviewPrefab);
        preview.Initialize(defenseData);
        preview.OnBuy.AddListener((tower) => OnBuyTower(tower, defenseData));
        preview.OnCancel.AddListener(OnCancelBuy);
        ShowIsBuyingText();
    }

    private void OnCancelBuy(Tower arg0)
    {
        HideIsBuyingText();
    }

    private void OnBuyTower(Tower tower, DefenseData defenseData)
    {
        HideIsBuyingText();
        GameManager.Instance.ReduceCoin(defenseData.defenseCost);
    }


    void ShowIsBuyingText()
    {
        _isBuyingText.gameObject.SetActive(true);
        _shopScrollRect.gameObject.SetActive(false);
    }

    void HideIsBuyingText()
    {
        _shopScrollRect.gameObject.SetActive(true);
        _isBuyingText.gameObject.SetActive(false);
    }

    public void CloseButton()
    {
        _rt.DOAnchorPosY(-_rt.sizeDelta.y - _startYAnchorPos, .25f);
    }
}