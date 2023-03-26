using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LeaderboardScorePrefab : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _rankText;
    [SerializeField] private TextMeshProUGUI _pointsText;

    public void SetScore(int rank, float points)
    {
        _rankText.text = rank + ":";
        _pointsText.text = points + " points";
    }
}