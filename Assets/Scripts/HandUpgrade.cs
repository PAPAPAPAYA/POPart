using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandUpgrade : MonoBehaviour
{
    #region SINGLETON
    public static HandUpgrade me;
    private void Awake()
    {
        me = this;
    }
    #endregion
    public int boxHandLevel;
    public int lineHandLevel;
    public int xxHandLevel;
    public int boxExplodePercentLevel=1;
    public int lineExplodePercentLevel=1;
    public int thornFanPercentLevel=1;

    public List<BubbleScript> selectedBubbles = new List<BubbleScript>();

    void Start()
    {
        boxHandLevel = 1;
        lineHandLevel = 1;
        xxHandLevel = 1;
        BubbleMakerScript.me.SetPercentage("box",lineExplodePercentLevel);
        BubbleMakerScript.me.SetPercentage("line",lineExplodePercentLevel);
        BubbleMakerScript.me.SetPercentage("thornFan",lineExplodePercentLevel);
    }

    void Update()
    {

        if (Input.GetMouseButtonDown(0))
        {
            foreach (var bubble in selectedBubbles)
            {
                bubble.mouseDown = true;
            }
            Debug.Log("Down");
        }
        if (Input.GetMouseButtonUp(0))
        {
            foreach (var bubble in selectedBubbles)
            {
                bubble.ResetBubble();

            }
            selectedBubbles.Clear();
            Debug.Log("Up");
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            BoxExplodePercentUpgrade();
        }
    }

    public void SqueezeSpdUp()
    {
        foreach (var bubble in BubbleMakerScript.me.bubbles)
        {
            BubbleScript bs = bubble.GetComponentInChildren<BubbleScript>();
            bs.squeezeTime -= 0.1f;
        }

    }

    public void LineHand(int rowNumber, int colNumber)
    {

        foreach (var bubble in BubbleMakerScript.me.bubbles)
        {
            BubbleScript bs = bubble.GetComponentInChildren<BubbleScript>();
            if (lineHandLevel == 1)
            {
                if (bs.rowNumber == rowNumber && (Mathf.Abs(bs.colNumber - colNumber) <= 2) && bs.colNumber != colNumber)
                {
                    selectedBubbles.Add(bs);
                }
            }
            else if (lineHandLevel > 1)
            {
                
                if ((bs.rowNumber == rowNumber && (Mathf.Abs(bs.colNumber - colNumber) <= (lineHandLevel - 1) * 2) && bs.colNumber != colNumber) ||
                    (bs.colNumber == colNumber && (Mathf.Abs(bs.rowNumber - rowNumber) <= (lineHandLevel - 1) * 2) && bs.rowNumber != rowNumber))
                {
                    selectedBubbles.Add(bs);
                }
            }
        }
    }
    public void XXHand(int rowNumber, int colNumber)
    {
        foreach (var bubble in BubbleMakerScript.me.bubbles)
        {
            BubbleScript bs = bubble.GetComponentInChildren<BubbleScript>();
            if ((Mathf.Abs(bs.colNumber - colNumber) == Mathf.Abs(bs.rowNumber - rowNumber)) &&
                (Mathf.Abs(bs.colNumber - colNumber) <= xxHandLevel))
            {
                selectedBubbles.Add(bs);
            }
        }
    }

    public void BoxHand(int rowNumber, int colNumber)
    {
        //add the coordinate of all bubble around according to the box explode level
        List<(int, int)> offsets = new List<(int, int)>();
        for (int i = 1; i <= boxHandLevel; i++)
        {
            for (int j = 0; j <= boxHandLevel + 1; j++)
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
            if (offsets.Contains((bs.rowNumber - rowNumber, bs.colNumber - colNumber)))
            {
                //int levelNum = Mathf.Max(Mathf.Abs(bs.rowNumber - rowNumber), Mathf.Abs(bs.colNumber - colNumber));
                if (bs.active == true)
                {
                    selectedBubbles.Add(bs);

                }

            }
        }
    }

    public void BoxExplodePercentUpgrade()
    {
        boxExplodePercentLevel++;
        BubbleMakerScript.me.SetPercentage("box",boxExplodePercentLevel);
    }

    public void LineExplodePercentUpgrade()
    {
        lineExplodePercentLevel++;
        BubbleMakerScript.me.SetPercentage("line",lineExplodePercentLevel);
    }

    public void ThornFanPercentUpgrade()
    {
        thornFanPercentLevel++;
        BubbleMakerScript.me.SetPercentage("thornFan",thornFanPercentLevel);
    }
}
