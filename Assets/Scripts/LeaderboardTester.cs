using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Linq;

public class LeaderboardTester : MonoBehaviour
{
    public TMP_InputField inputField; // Reference to the TMP_InputField
    public Button submitButton; // Reference to the submit Button
    public Transform leaderboardContent; // Reference to the parent transform for leaderboard entries
    public GameObject bubble; // Reference to the bubble GameObject

    // References to the TMP_Text elements for rank, name, and score
    public TMP_Text[] rankTexts = new TMP_Text[6];
    public TMP_Text[] nameTexts = new TMP_Text[6];
    public TMP_Text[] scoreTexts = new TMP_Text[6];

    private const string PlayerNameKey = "PlayerName";
    private bool isLeaderboardDisplayed = false; // Flag to check if the leaderboard is displayed

    private static int scoreToPass = 1;

    private void Awake()
    {
        // // Ensure only one instance of LeaderboardTester exists
        // if (FindObjectsOfType<LeaderboardTester>().Length > 1)
        // {
        //     Destroy(gameObject);
        // }
        // else
        // {
        //     DontDestroyOnLoad(gameObject);
        // }
    }

    public static void SetScoreToPass(int score)
    {
        scoreToPass = score;
    }

    private void Start()
    {
        // Use the scoreToPass value as needed
        Debug.Log("Score passed to LeaderboardTester: " + scoreToPass);

        // Add a listener to the submit button
        submitButton.onClick.AddListener(OnSubmit);

        // Limit the input field to 10 characters
        inputField.characterLimit = 10;

        // Hide the leaderboard content initially
        leaderboardContent.gameObject.SetActive(false);

        // Check if the player's name is already saved
        if (PlayerPrefs.HasKey(PlayerNameKey))
        {
            // If the name is saved, hide the input field and button, and show the leaderboard
            string savedName = PlayerPrefs.GetString(PlayerNameKey);
            inputField.gameObject.SetActive(false);
            submitButton.gameObject.SetActive(false);
            LeaderboardManager.Instance.InsertScore(savedName, scoreToPass, () => FetchAndDisplayLeaderboard(savedName));
        }
        else
        {
            // If the name is not saved, show the input field and button
            inputField.gameObject.SetActive(true);
            submitButton.gameObject.SetActive(true);
        }

        // Clear all text fields when the scene loads
        ClearTextFields();
    }

    // Update is called once per frame
    void Update()
    {
        // Check if the space bar is pressed and the leaderboard is displayed
        if (isLeaderboardDisplayed && Input.GetKeyDown(KeyCode.Space))
        {
            LoadMainScene();
        }
    }

    // Method to handle the submit action
    void OnSubmit()
    {
        string inputValue = inputField.text; // Get the value from the input field
        Debug.Log("Submitted value: " + inputValue);

        // Save the player's name
        PlayerPrefs.SetString(PlayerNameKey, inputValue);
        PlayerPrefs.Save();

        // Hide the input field and button
        inputField.gameObject.SetActive(false);
        submitButton.gameObject.SetActive(false);

        // Insert the score and fetch the leaderboard
        LeaderboardManager.Instance.InsertScore(inputValue, scoreToPass, () => FetchAndDisplayLeaderboard(inputValue));
    }

    // Method to fetch and display the leaderboard information
    void FetchAndDisplayLeaderboard(string playerName)
    {
        LeaderboardManager.Instance.GetScores(
            (scores) =>
            {
                // Success callback
                Debug.Log("Fetched scores: ");
                DisplayScores(new List<ScoreRecord>(scores), playerName); // Convert array to list
            },
            (error) =>
            {
                // Error callback
                Debug.LogError(error);
            }
        );
    }

    // Method to display the scores in the leaderboardContent
    void DisplayScores(List<ScoreRecord> scores, string playerName)
    {
        // Sort the scores by descending order
        scores = scores.OrderByDescending(s => s.score).ToList();

        
        // Clear existing leaderboard entries
        ClearTextFields();

        // Display the leaderboard content
        leaderboardContent.gameObject.SetActive(true);
        isLeaderboardDisplayed = true; // Set the flag to true

        // Show the bubble
        if (bubble != null)
        {
            bubble.SetActive(true);
        }
        
               // Print all scores
        Debug.Log("Leaderboard Scores:");
        foreach (var score in scores)
        {
            Debug.Log($"Name: {score.name}, Score: {score.score}");
        }

        // Display the top 5 scores
        for (int i = 0; i < Mathf.Min(5, scores.Count); i++)
        {
            rankTexts[i].text = (i + 1).ToString();
            nameTexts[i].text = scores[i].name;
            scoreTexts[i].text = scores[i].score.ToString();
        }

        // Find the user's score
        var userScore = scores.FirstOrDefault(s => s.name == playerName && s.score == scoreToPass);
        if (userScore != null)
        {
            int userRank = scores.IndexOf(userScore) + 1;
            rankTexts[5].text = userRank.ToString();
            nameTexts[5].text = userScore.name;
            scoreTexts[5].text = userScore.score.ToString();
        }
        else
        {
            // If the user's score is not found, add it manually
            int userRank = scores.Count + 1;
            rankTexts[5].text = userRank.ToString();
            nameTexts[5].text = playerName;
            scoreTexts[5].text = scoreToPass.ToString();
        }
    }

    // Method to clear all text fields
    void ClearTextFields()
    {
        for (int i = 0; i < 6; i++)
        {
            rankTexts[i].text = "";
            nameTexts[i].text = "";
            scoreTexts[i].text = "";
        }
    }

    // Method to handle bubble burst
    public void OnBubbleBurst()
    {
        StartCoroutine(LoadMainSceneWithDelay());
    }

    // Coroutine to load the main scene with a delay
    private IEnumerator LoadMainSceneWithDelay()
    {
        yield return new WaitForSeconds(.2f); // Adjust the delay as needed
        SceneManager.LoadScene("MainScene"); // Replace "MainScene" with the actual name of your main scene
    }

    // Method to load the main scene
    void LoadMainScene()
    {
        SceneManager.LoadScene("MainScene"); // Replace "MainScene" with the actual name of your main scene
    }
}