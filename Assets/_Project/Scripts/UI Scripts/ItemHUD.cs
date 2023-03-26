using System;
using TMPro;
using UnityEngine;

public class ItemHUD : MonoBehaviour
{
    [SerializeField] private PlayerScore _score;
    [SerializeField] private string _label = "COINS";
    [SerializeField] private TextMeshProUGUI _text;

    private void OnValidate()
    {
        ChangeLabel(_label);
        if (_score)
            OnChangeValue(0, _score.CurrentValue);
    }

    private void Awake()
    {
        SetScore(_score);
    }

    public void SetScore(PlayerScore score)
    {
        _score = score;
        if (_score == null) return;
        _score.OnChangeValue.AddListener(OnChangeValue);
    }

    public void ChangeLabel(string label)
    {
        _label = label;
    }

    void OnChangeValue(float old, float value)
    {
        _text.text = _label + ": " + value;
    }
}