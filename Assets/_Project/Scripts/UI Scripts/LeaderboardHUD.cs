using System;
using System.Collections.Generic;
using UnityEngine;

public class LeaderboardHUD : MonoBehaviour
{
    [SerializeField] private LeaderboardManager _leaderboardManager;
    [SerializeField] private LeaderboardScorePrefab[] _scoresList;

    private void Awake()
    {
        _leaderboardManager.OnUpdateScoreList.AddListener(UpdateList);
    }

    private void UpdateList(List<LeaderboardManager.ScoreEntry> scoreList)
    {
        for (int i = 0; i < _scoresList.Length; i++)
        {
            _scoresList[i].SetScore(i + 1, scoreList.Count > i ? scoreList[i].score : 0);
        }
    }
}