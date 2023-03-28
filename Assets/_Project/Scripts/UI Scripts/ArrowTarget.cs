using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class ArrowTarget : MonoBehaviour
{
    // Start is called before the first frame update
    private Vector2 _startPos;
    private Vector2 _endPos;
    [SerializeField] private RectTransform _targetImage;

    void Start()
    {
        // _startPos = _targetImage.anchoredPosition;
        // _endPos = _startPos + (Vector2.down * .25f);
        //
        // _targetImage.DOPunchAnchorPos(_endPos, .5f, 2, 0).SetLoops(-1);
    }
    
}