using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public class PlayerScoreChangeUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _text;
    [SerializeField] private CanvasGroup _container;
    [SerializeField] Vector3 startPos = new Vector3(0, 0, 0);
    [SerializeField] Vector3 endPos = new Vector3(2, 0, 0);
    [SerializeField] private float jumpPower = 1;
    [SerializeField] private float duration = 1;
    [HideInInspector] public UnityEvent<PlayerScoreChangeUI> OnComplete;


    private void OnEnable()
    {
        //_sequence.SetAutoKill(false);
        //Animate();
    }

    public void SetText(string text, Color color = default)
    {
        if (color != default)
            _text.color = color;
        _text.text = text;
    }

    private Sequence _sequence;

    public void Animate(bool right = true)
    {
        if (!Application.isPlaying) return;
        _container.transform.localPosition = startPos;
        _container.alpha = 1;
        Vector3 newEndPos = endPos;
        newEndPos.y = Random.Range(newEndPos.y, newEndPos.y * 1.5f);
        newEndPos.z = Random.Range(newEndPos.z, newEndPos.z * 1.5f);

        if (!right)
            endPos.x *= -1;

        _sequence = _container.transform.DOLocalJump(newEndPos, jumpPower, 1, duration).OnComplete(OnCompleteAnimation);
        _sequence.Insert(0, _container.transform.DOPunchScale(new Vector3(1.1f, 1.1f, 1.1f), duration, 0, 0));
        //_sequence.PrependInterval(duration / 2);
        _sequence.Insert(duration / 2, _container.DOFade(0, duration / 2));
        _sequence.OnComplete(OnCompleteAnimation);
    }

    private void OnCompleteAnimation()
    {
        OnComplete.Invoke(this);

        // if (_sequence != null && _sequence.IsPlaying())
        // {
        //     _sequence.Complete();
        // }
        //
        // _container.transform.localPosition = Vector3.zero;
        // _container.alpha = 1;
    }
}