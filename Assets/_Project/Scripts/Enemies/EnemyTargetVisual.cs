using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTargetVisual : MonoBehaviour
{
    [SerializeField] private GameObject tagetArrow;
    [SerializeField] private GameObject rangeIndicator;

    private void Start()
    {
        SetTargetVisualActive(false);
    }

    private void OnDisable()
    {
        SetTargetVisualActive(false);
    }

    public void SetTargetVisualActive(bool isActive)
    {
        tagetArrow.SetActive(isActive);
        rangeIndicator.SetActive(isActive);
    }
}
