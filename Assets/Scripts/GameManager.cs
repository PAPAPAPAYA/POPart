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
    // Start is called before the first frame update
    void Start()
    {
        failBubbleNum = (int)Mathf.Pow(2*BubbleMakerScript.me.amount_layer - 1f, 2.0f); 
    }

    // Update is called once per frame
    void Update()
    {
        //if (!hasFailed)
        //{
        //    IfFail();
        //}
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
            hasFailed = true;
            Fail();
        }
    }

    public void Fail()
    {
        Time.timeScale = 0f;
    }

}
