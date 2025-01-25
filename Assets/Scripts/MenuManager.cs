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
        //if (failMenu == null) failMenu = transform.Find("FailMenu").gameObject;
        pauseMenu.SetActive(false);
        //failMenu.SetActive(false);
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

    public void RestartGame()
    {
        SceneManager.LoadScene(1);
    }
}
