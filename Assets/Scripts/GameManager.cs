using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

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
    public int CCMUpgradeCount; //record how many times the chest max number has been upgraded
    public int upgradedCount; //record how many times you have already upgraded
    [SerializeField]
    private List<int> chestCountMaxPresets = new List<int>();

    private float lastScore;
    private float scoreCheckInterval = 0.2f; // Time interval to check score burst
    private float scoreBurstThreshold = 10.0f; // Score burst threshold
    private float screenShakeCooldown = 0.2f; // Cooldown time in seconds
    private float lastScreenShakeTime = -5.0f; // Initialize to allow immediate shake

    private float startTime;
    private float currentTime;
    public float gameLength;
    private int timeLeft;

    public GameObject UI_timer;
    public GameObject UI_timer_shadow;

    void Start()
    {
        failBubbleNum = (int)Mathf.Pow(2 * BubbleMakerScript.me.amount_layer - 1f, 2.0f);
        hasFailed = false;
        StartCoroutine(CheckScoreBurst());
        startTime = Time.time;
    }

    void Update()
    {
        Pause();
        SlowDown();
        currentTime = Time.time;
        timeLeft = (int)(gameLength - currentTime + startTime);
        print(timeLeft);
        if (timeLeft <= 0)
        {
            hasFailed = true;
            Fail();
        }
        UI_timer.GetComponent<TextMeshProUGUI>().text = ""+timeLeft;
        UI_timer_shadow.GetComponent<TextMeshProUGUI>().text = "" + timeLeft;
    }

    private IEnumerator CheckScoreBurst()
    {
        while (true)
        {
            yield return new WaitForSeconds(scoreCheckInterval);

            float scoreIncrement = score - lastScore;
            lastScore = score;

            if (scoreIncrement > scoreBurstThreshold && Time.time - lastScreenShakeTime > screenShakeCooldown)
            {
                ApplyScreenShake();
                lastScreenShakeTime = Time.time;
            }
        }
    }

    private void ApplyScreenShake()
    {
        // Implement your screen shake logic here
        Debug.Log("Screen Shake Applied!");
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
        //Time.timeScale = 0f;
        MenuManager.me.SendMessage("OpenFailMenu");
        AudioManager.Instance.ApplyMuffleEffect(true);
    }

    private void Pause()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!isPaused)
            {
                isPaused = true;
            }
            else
            {
                isPaused = false;
            }
            MenuManager.me.SendMessage("ChangePauseMenuState");

            AudioManager.Instance.ApplyMuffleEffect(isPaused);
            AudioManager.Instance.SetMasterVolume(isPaused ? -80 : 0);
        }
    }
    public void ChestCountUp()
    {
        //if (chestCount < chestCountMax)
        //{
        //    chestCount += 1;
        //}
        //else
        //{
        //    chestCount = 0 + 1;
        //}
        chestCount++;
    }
    public void ChestCountMaxUp()
    {
        if (CCMUpgradeCount < chestCountMaxPresets.Count)
        {
            chestCountMax = chestCountMaxPresets[CCMUpgradeCount];
        }
        else if (CCMUpgradeCount >= chestCountMaxPresets.Count)
        {
            chestCountMax *= chestCountMaxFactor;
            chestCountMax = (int)chestCountMax;
        }
        CCMUpgradeCount++;
    }
    public void ResetChestCount()
    {
        chestCount = 0;
    }
    private void SlowDown()
    {
        if (UpgradeInteractionManagerScript.me.showingButtons)
        {
            Time.timeScale = 0.1f;
        }
        else
        {
            Time.timeScale = 1f;
        }
    }
}
