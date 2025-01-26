using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


public class BubbleUpgrade : MonoBehaviour
{
    #region SINGLETON
    public static BubbleUpgrade me;
    private void Awake()
    {
        me = this;
    }
    #endregion
    public enum Upgrades
    {
        lineExplode,
        boxExplode,
        thornFan,
        fastSqueeze
    };
    [Header("UPGRADE LEVELs")]
    //level for upgrade one bubble function
    public int lineExplodeLevel;
    public int boxExplodeLevel;
    public int thornFanLevel;
    public int fastSqueezeLevel;
    [Header("UPGRADE VARIABLEs")]
    public float ExplosionDelay = 0.25f;
    public GameObject prefab_thorn;
    public float fastSqueezeTime = 0.1f;
    [Header("ACTIVATED UPGRADEs")]
    public bool lineExplosion;
    public GameObject prefab_lineExplosion;
    public bool boxExplosion;
    public GameObject prefab_boxExplosion;
    public bool thornFan;
    public GameObject prefab_thornFan;
    public bool fastSqueeze;
    public GameObject prefab_fastSqueeze;

    private void Start()
    {
        //default is set to 1
        lineExplodeLevel = 1;
        boxExplodeLevel = 1;
        thornFanLevel = 1;
        fastSqueezeLevel = 1;
    }

    public void ThornFan(int amount, Vector3 spawnPos)
    {
        for (int i = 0; i < amount; i++)
        {
            GameObject thorn = Instantiate(prefab_thorn);
            thorn.transform.position = spawnPos;
        }
    }

    //explode in horizontal and vertical line, each layer explode with different delay
    public void LineExplode(int rowNumber, int colNumber)
    {
        foreach (var bubble in BubbleMakerScript.me.bubbles)
        {
            BubbleScript bs = bubble.GetComponentInChildren<BubbleScript>();
            if ((bs.rowNumber == rowNumber && (Mathf.Abs(bs.colNumber - colNumber) <= lineExplodeLevel) && bs.colNumber != colNumber) ||
                (bs.colNumber == colNumber && (Mathf.Abs(bs.rowNumber - rowNumber) <= lineExplodeLevel) && bs.rowNumber != rowNumber))
            {
                if (bs.active)
                {
                    int levelNum = Mathf.Max(Mathf.Abs(bs.rowNumber - rowNumber), Mathf.Abs(bs.colNumber - colNumber));
                    bs.DelayedDMGCaller(ExplosionDelay * levelNum, 1);
                }
            }
        }
    }

    //explode in box around, level1 - 3x3, level2 - 5x5, level3 - 7x7...
    public void BoxExplode(int rowNumber, int colNumber)
    {
        //add the coordinate of all bubble around according to the box explode level
        List<(int, int)> offsets = new List<(int, int)>();
        for (int i = 1; i <= boxExplodeLevel; i++)
        {
            for (int j = 0; j <= boxExplodeLevel + 1; j++)
            {
                offsets.Add((-1 * i, -1 * i + j));
                offsets.Add((1 * i - j, -1 * i));
                offsets.Add((-1 * i + j, 1 * i));
                offsets.Add((1 * i, 1 * i - j));
            }
        }
        //for each bubble in the offset boundary, explode with different delay
        foreach (var bubble in BubbleMakerScript.me.bubbles)
        {
            BubbleScript bs = bubble.GetComponentInChildren<BubbleScript>();
            //int levelNum = Mathf.Max(Mathf.Abs(bs.rowNumber - rowNumber), Mathf.Abs(bs.colNumber - colNumber));
            //if (levelNum <= boxExplodeLevel)
            //{
                //bs.DelayedDMGCaller(ExplosionDelay * levelNum, 1);
            //}
            if (offsets.Contains((bs.rowNumber - rowNumber, bs.colNumber - colNumber)))
            {
                if (bs.active)
                {
                    int levelNum = Mathf.Max(Mathf.Abs(bs.rowNumber - rowNumber), Mathf.Abs(bs.colNumber - colNumber));
                    bs.DelayedDMGCaller(ExplosionDelay * levelNum, 1);
                }
            }
        }
    }

    //make squeeze time lower for bubbles around this one, lv1 - 33, lv2 - 55, lv3 - 77
    public void FastSqueeze(int rowNumber, int colNumber)
    {
        List<(int, int)> offsets = new List<(int, int)>();
        for (int i = 1; i <= boxExplodeLevel; i++)
        {
            for (int j = 0; j <= boxExplodeLevel + 1; j++)
            {
                offsets.Add((-1 * i, -1 * i + j));
                offsets.Add((1 * i - j, -1 * i));
                offsets.Add((-1 * i + j, 1 * i));
                offsets.Add((1 * i, 1 * i - j));
            }
        }
        foreach (var bubble in BubbleMakerScript.me.bubbles)
        {
            BubbleScript bs = bubble.GetComponentInChildren<BubbleScript>();
            if (offsets.Contains((bs.rowNumber - rowNumber, bs.colNumber - colNumber)))
            {
                bs.squeezeTimer = fastSqueezeTime;
            }
        }
    }
}
