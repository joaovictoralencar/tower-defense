using System;
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using Singletons;
using UnityEngine.Events;

public class LeaderboardManager : MonoBehaviour
{
    [SerializeField] int _maxScores = 5; // maximum number of scores to keep track of
    public List<ScoreEntry> _scoreList = new List<ScoreEntry>();

    // path to the JSON file to save and load the leaderboard data
    private string _filePath = "/leaderboard.json";

    public UnityEvent<List<ScoreEntry>> OnUpdateScoreList;


    public List<ScoreEntry> ScoreList => _scoreList;

    private void OnPlayerDie(GameObject playerObj)
    {
        AddScore("Player", GameManager.Instance.Points.CurrentValue);
    }

    void Start()
    {
        // load the leaderboard data from the JSON file
        LoadLeaderboard();
        GameManager.Instance.OnPlayerDie.AddListener(OnPlayerDie);
    }

    // adds a new score to the leaderboard
    public void AddScore(string playerName, float score)
    {
        if (_scoreList.Count > 1 && _scoreList[_scoreList.Count - 1].score > score)
            return;

        ScoreEntry newScore = new ScoreEntry();
        newScore.playerName = playerName;
        newScore.score = score;

        // add the new score to the list and sort it by score (highest to lowest)
        _scoreList.Add(newScore);
        _scoreList.Sort((x, y) => y.score.CompareTo(x.score));

        // remove any scores above the maximum allowed
        if (_scoreList.Count > _maxScores)
        {
            _scoreList.RemoveAt(_maxScores);
        }

        // save the updated leaderboard data to the JSON file
        SaveLeaderboard();

        OnUpdateScoreList.Invoke(_scoreList);
    }

    // saves the current leaderboard data to the JSON file
    private void SaveLeaderboard()
    {
        
        SaveData data = new SaveData();
        for (int i = 0; i < _scoreList.Count; i++)
        {
            data.ScoreList.Add(_scoreList[i]);
        }
        string json = JsonUtility.ToJson(data);
        File.WriteAllText(Application.persistentDataPath + _filePath, json);
    }

    // loads the leaderboard data from the JSON file
    private void LoadLeaderboard()
    {
        _scoreList = new List<ScoreEntry>();
        if (File.Exists(Application.persistentDataPath + _filePath))
        {
            string json = File.ReadAllText(Application.persistentDataPath + _filePath);
            SaveData data = JsonUtility.FromJson<SaveData>(json);

            foreach (var scoreEntry in data.ScoreList)
            {
                _scoreList.Add(scoreEntry);
            }
        }

        OnUpdateScoreList.Invoke(_scoreList);
    }

    // displays the leaderboard in the console
    public void DisplayLeaderboard()
    {
        Debug.Log("LEADERBOARD:");
        for (int i = 0; i < _scoreList.Count; i++)
        {
            Debug.Log((i + 1) + ". " + _scoreList[i].playerName + ": " + _scoreList[i].score);
        }
    }
    [System.Serializable]
    public class SaveData
    {
        public List<ScoreEntry> ScoreList = new List<ScoreEntry>();
    }
    // class to hold the score data
    [System.Serializable]
    public class ScoreEntry
    {
        public string playerName;
        public float score;
    }
}