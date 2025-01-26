using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    #region SINGLETON
    public static ScoreManager me;
    private void Awake()
    {
        me = this;
    }
    #endregion
    [SerializeField]
    private TextMeshProUGUI chestCount;
    [SerializeField]
    private TextMeshProUGUI chestCountShadow;
    [SerializeField]
    private TextMeshProUGUI chestCountMax;
    [SerializeField]
    private TextMeshProUGUI chestCountMaxShadow;
    [SerializeField]
    private TextMeshProUGUI score;
    [SerializeField]
    private TextMeshProUGUI scoreShadow;

    void Start()
    {
        chestCountMax.text = "/" + GameManager.me.chestCountMax;
        chestCountMaxShadow.text = "/" + GameManager.me.chestCountMax;
        UpdateChestCount();
        UpdateScore();
    }

    void Update()
    {
        UpdateChestCount();
        UpdateScore();
    }

    public void UpdateChestCount()
    {
        chestCount.text = "" + GameManager.me.chestCount;
        chestCountShadow.text = "" + GameManager.me.chestCount;
    }
    public void UpdateScore()
    {
        score.text = "" + GameManager.me.score;
        scoreShadow.text = "" + GameManager.me.score;
    }
    public void UpdateChestCountMax()
    {
        chestCountMax.text = "/" + GameManager.me.chestCountMax;
        chestCountMaxShadow.text = "/" + GameManager.me.chestCountMax;
    }
}
