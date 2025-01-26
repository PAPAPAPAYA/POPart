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
        Time.timeScale = 1.0f;
	}
	#endregion

    private int failBubbleNum;

    public bool wudi;//Wudi
    public bool hasFailed = false;
    public bool isPaused = false;

    public int score = 0;
    public int chestCount = 0; // when a bubble is popped, +1
    public float chestCountMax; // when chest count reached chest count max, next bubble is a chest
    public float chestCountMaxFactor; // when a chest bubble is spawned, multiply chest count max with chest count max factor

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
        if (!wudi)
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
                hasFailed = true;
                Fail();
            }
        }
    }

    public void Fail()
    {
        Time.timeScale = 0f;
        MenuManager.me.SendMessage("OpenFailMenu");
        AudioManager.Instance.ApplyMuffleEffect(true);
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
            
            AudioManager.Instance.ApplyMuffleEffect(isPaused);
        }
    }
    public void ChestCountUp()
    {
        if (chestCount < chestCountMax)
        {
            chestCount += 1;
        }
        else
        {
            chestCount = 0 + 1;
        }
    }
    public void ChestCountMaxUp()
    {
        chestCountMax *= chestCountMaxFactor;
        chestCountMax = (int)chestCountMax;
    }
    public void ResetChestCount()
    {
        chestCount = 0;
    }
}
