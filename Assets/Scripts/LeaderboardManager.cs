using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System;

public class LeaderboardManager : MonoBehaviour
{
    // Singleton instance
    public static LeaderboardManager Instance { get; private set; }

    // Replace with your actual API endpoint
    [SerializeField] private string apiUrl = "http://yuhao.app/api/leaderboard";

    private void Awake()
    {
        // Ensure only one instance of this manager exists
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// Call this method to post a new score to the leaderboard.
    /// </summary>
    /// <param name="name">Player name</param>
    /// <param name="score">Player score</param>
    public void InsertScore(string name, int score)
    {
        StartCoroutine(InsertScoreCoroutine(name, score));
    }

    /// <summary>
    /// Fetch all scores from the server, sorted by highest score. 
    /// The onSuccess callback receives an array of ScoreRecord.
    /// </summary>
    public void GetScores(Action<ScoreRecord[]> onSuccess, Action<string> onError = null)
    {
        StartCoroutine(GetScoresCoroutine(onSuccess, onError));
    }

    // --- Private Coroutines ---

    private IEnumerator InsertScoreCoroutine(string playerName, int playerScore)
    {
        // Create a payload object
        ScoreRecord payload = new ScoreRecord { name = playerName, score = playerScore };
        // Convert to JSON
        string jsonPayload = JsonUtility.ToJson(payload);

        using (UnityWebRequest request = new UnityWebRequest(apiUrl, "POST"))
        {
            // Prepare the request
            byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(jsonPayload);
            request.uploadHandler   = new UploadHandlerRaw(jsonToSend);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            // Send the request and wait for response
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError 
                || request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError($"InsertScore Error: {request.error}");
            }
            else
            {
                Debug.Log("Score inserted successfully: " + request.downloadHandler.text);
            }
        }
    }

    private IEnumerator GetScoresCoroutine(Action<ScoreRecord[]> onSuccess, Action<string> onError)
    {
        using (UnityWebRequest request = UnityWebRequest.Get(apiUrl))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError 
                || request.result == UnityWebRequest.Result.ProtocolError)
            {
                string errorMsg = $"GetScores Error: {request.error}";
                Debug.LogError(errorMsg);
                onError?.Invoke(errorMsg);
            }
            else
            {
                string json = request.downloadHandler.text;
                // The API returns an array of records. We can parse directly if it's a valid JSON array.
                // If it’s not a simple JSON array, you may need a helper. For plain arrays, JsonUtility is enough
                // if we wrap the array in an object. Instead, let's create a little helper:

                ScoreRecord[] records = JsonHelper.FromJson<ScoreRecord>(json);
                onSuccess?.Invoke(records);
            }
        }
    }
}

/// <summary>
/// Represents a single score record.
/// Match your PocketBase fields (id, created, updated, etc.) if you want.
/// </summary>
[Serializable]
public class ScoreRecord
{
    public string id;       // if returned by PocketBase
    public string name;
    public int score;
    // Add other fields if needed (created, updated, etc.)
}

/// <summary>
/// Unity's JsonUtility cannot directly parse a top-level JSON array:
/// e.g. [ {...}, {...}, ... ] needs to be wrapped. 
/// This helper solves that by wrapping/unwrapping.
/// </summary>
public static class JsonHelper
{
    [Serializable]
    private class Wrapper<T>
    {
        public T[] Items;
    }

    public static T[] FromJson<T>(string json)
    {
        // We wrap the array into an object so JsonUtility can parse it.
        string newJson = "{\"Items\":" + json + "}";
        Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(newJson);
        return wrapper.Items;
    }
}