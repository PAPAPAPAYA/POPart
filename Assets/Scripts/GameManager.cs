using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    #region SINGLETON
    public static GameManager me;
    private void Awake()
	{
		me = this;
	}
	#endregion

    private int failBubbleNum;
    
    public bool hasFailed = false;
    public bool isPaused = false;

    public int score = 0;

    void Start()
    {
        failBubbleNum = (int)Mathf.Pow(2*BubbleMakerScript.me.amount_layer - 1f, 2.0f); 
    }

    void Update()
    {
        Pause();
    }

    public void IfFail()
    {
        int activecount = 0;
        foreach (var bubble in BubbleMakerScript.me.bubbles)
        {
            if (bubble.GetComponentInChildren<BubbleScript>().active)
            {
                activecount++;
            }
        }
        if (activecount >= failBubbleNum)
        {
            print("You failed");
            Fail();
            hasFailed = true;
        }
    }

    public void Fail()
    {
        Time.timeScale = 0f;
    }

    private void Pause()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if (!isPaused)
            {
                Time.timeScale = 0f;
                isPaused = true;
            }
            else
            {
                Time.timeScale = 1f;
                isPaused = false;
            }
            MenuManager.me.SendMessage("ChangePauseMenuState");
        }
    }

}
