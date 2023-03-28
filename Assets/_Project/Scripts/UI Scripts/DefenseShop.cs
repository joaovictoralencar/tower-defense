using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Singletons;
using UnityEngine;
using UnityEngine.UI;

public class DefenseShop : MonoBehaviour
{
    [SerializeField] private Button _shopButton;
    [SerializeField] private AllDefensesData _allDefenseData;
    [SerializeField] private DefenseUIData _shopDataBuyPrefab;
    [SerializeField] private RectTransform _shopDefenseHolder;
    [SerializeField] private BuyDefensePreview _buyDefensePreviewPrefab;
    [SerializeField] private RectTransform _isBuyingText;
    [SerializeField] private RectTransform _shopScrollRect;

    private RectTransform _rt;
    private float _startYAnchorPos;
    private float _startYAnchorPosOffset = 30f;
    private bool _buyingDefense;

    private List<DefenseUIData> _defensedUIList = new List<DefenseUIData>();

    private void Start()
    {
        _rt = GetComponent<RectTransform>();
        _startYAnchorPos = _rt.anchoredPosition.y;
        InitializeStore();
        HideIsBuyingText();
    }

    void InitializeStore()
    {
        foreach (var defenseData in _allDefenseData.allDefenses)
        {
            DefenseUIData defensedUI = Instantiate(_shopDataBuyPrefab, _shopDefenseHolder);
            defensedUI.UpdateUI(defenseData);
            defensedUI.UpdateBuyingCost();
            defensedUI.OnSelect.AddListener(OnSelectDefense);
            _defensedUIList.Add(defensedUI);
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
        {
            for (int i = 0; i < _defensedUIList.Count; i++)
            {
                if (_defensedUIList[i].Data.defenseName == defenseData.defenseName)
                {
                    _defensedUIList[i].OnBuy.Invoke(defenseData);
                }
            }
        }
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
        _rt.DOAnchorPosY(-_rt.sizeDelta.y - _startYAnchorPos - _startYAnchorPosOffset, .25f)
            .OnComplete(() => ShowShopButton(true));
    }

    public void OpenShop()
    {
        ShowShopButton(false);
        _rt.DOAnchorPosY(_startYAnchorPos, .25f);
    }

    void ShowShopButton(bool isActive)
    {
        _shopButton.gameObject.SetActive(isActive);
    }
}