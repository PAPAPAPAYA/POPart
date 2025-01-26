using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    #region SINGLETON
    public static MenuManager me;
    private void Awake()
    {
        me = this;
    }
    #endregion

    private GameObject pauseMenu;
    private GameObject failMenu;

    void Start()
    {
        if (pauseMenu == null) pauseMenu = transform.Find("PauseMenu").gameObject;
        if (failMenu == null) failMenu = transform.Find("FailMenu").gameObject;
        pauseMenu.SetActive(false);
        failMenu.SetActive(false);
    }

    void Update()
    {

    }

    private void ChangePauseMenuState()
    {
        if (GameManager.me.isPaused)
        {
            pauseMenu.transform.Find("Score").gameObject.GetComponent<TextMeshProUGUI>().text = "Score: " + GameManager.me.score;
            pauseMenu.SetActive(true);
        }
        else
        {
            pauseMenu.SetActive(false);
        }
    }
    private void OpenFailMenu()
    {
        if (GameManager.me.hasFailed)
        {
            // failMenu.transform.Find("Score").gameObject.GetComponent<TextMeshProUGUI>().text = "Score: " + GameManager.me.score;
            // failMenu.SetActive(true);
            
            // Set the score to pass to the LeaderboardTester
            LeaderboardTester.SetScoreToPass(GameManager.me.score);

            // Load the LeaderBoardScene
            SceneManager.LoadScene("LeaderBoardScene");
        }
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(1);
    }
    public void BackToTitle()
    {
        SceneManager.LoadScene(0);
    }

    // Function to set the master volume based on the slider value
    public void SetVolume(float volume)
    {
        // Convert the slider value (0-1) to a logarithmic scale (-80dB to 0dB)
        float dB = Mathf.Log10(Mathf.Clamp(volume, 0.0001f, 1f)) * 20;
        AudioManager.Instance.SetMasterVolume(dB);
    }
}
