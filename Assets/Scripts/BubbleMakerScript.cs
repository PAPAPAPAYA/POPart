using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Video;

public class BubbleMakerScript : MonoBehaviour
{
	#region SINGLETON
	public static BubbleMakerScript me;
	private void Awake()
	{
		me = this;
	}
	#endregion
	[Header("Bubble Prefabs")]
	public GameObject prefab_bubble;
	public List<GameObject> bubbles = new();
	[Header("For Spawning Inactive Bubbles")]
	public int amount_layer;
	public float offset_row_x;
	public float offset_row_y;
	public float offset_col_x;
	public float offset_col_y;
    public int bubbleHp;
	public int amount_initialPump;
	[Header("For Activating Bubbles")]
	public float activateInterval;
	private float activateTimer;
	public float activateInterval_decrease_Interval;
	private float activateInterval_decrease_timer;
	public float activateInterval_decrease_Mult;
	[Header("For Upgrades")]
	public float percentage_lineExplosion;
	public float percentage_boxExplosion;
	public float percentage_thornFan;
	public float percentage_fastSqueeze;
	public Material matChest;

    private void Start()
	{
		// initialize activateTimer
		activateTimer = activateInterval;
		
		// spawn all the bubbles
		MakeBubbles2();

		// pump a few bubbles at start
		InitialPump(amount_initialPump);

		activateInterval_decrease_timer = activateInterval_decrease_Interval;
    }
    private void Update()
    {
		if (activateTimer > 0 &&
			bubbles.Count == Mathf.Pow(amount_layer * 2 - 1, 2))
		{
			activateTimer -= Time.deltaTime;
		}
		else if (activateTimer <= 0)
		{
			activateTimer = activateInterval;
			ActivateABubble();
			GameManager.me.IfFail();
		}
		SpeedUpBubbleActivationRateOverTime();
    }
	private void SpeedUpBubbleActivationRateOverTime()
	{
		if (activateInterval_decrease_timer > 0)
		{
			activateInterval_decrease_timer -= Time.deltaTime;
		}
		else
		{
            activateInterval_decrease_timer = activateInterval_decrease_Interval;
            activateInterval *= activateInterval_decrease_Mult;
		}
	}
    // used to initializing bubbles
    private void MakeBubbles2()
	{
		for (int i = 0; i < amount_layer; i++)
		{
			GameObject bubble1 = Instantiate(prefab_bubble);
            Debug.Log("jj");
            if (i == 0)
			{
                bubble1.transform.position = new(0,
                    0,
                    0);
				BubbleScript bs = bubble1.GetComponentInChildren<BubbleScript>();
				bs.rowNumber = amount_layer;
				bs.colNumber = amount_layer;
                bubbles.Add(bubble1);
            }
			else
			{
                bubble1.transform.position = bubbles[^1].transform.position + 
					new Vector3(offset_col_x,
                    -offset_col_y,
                    -offset_col_y);
                BubbleScript bs = bubble1.GetComponentInChildren<BubbleScript>();
                bs.rowNumber = amount_layer - i + 1;
                bs.colNumber = amount_layer + i;
                bubbles.Add(bubble1);
            }
			for (int j = 0; j < i * 2 - 1; j++)
			{
				GameObject bubble2 = Instantiate(prefab_bubble);
				bubble2.transform.position = bubbles[^1].transform.position +
					new Vector3(offset_row_x,
					offset_row_y,
					offset_row_y);
                BubbleScript bs = bubble2.GetComponentInChildren<BubbleScript>();
				BubbleScript bs_last = bubbles[^1].GetComponentInChildren<BubbleScript>();
                bs.rowNumber = bs_last.rowNumber + 1;
                bs.colNumber = bs_last.colNumber;
                bubbles.Add(bubble2);
            }
			for (int k = 1; k < i * 2 + 1; k++)
			{
				GameObject bubble3 = Instantiate(prefab_bubble);
				bubble3.transform.position = bubbles[^1].transform.position + 
					new Vector3(-offset_col_x,
                    offset_col_y,
                    offset_col_y);
                BubbleScript bs = bubble3.GetComponentInChildren<BubbleScript>();
                BubbleScript bs_last = bubbles[^1].GetComponentInChildren<BubbleScript>();
                bs.rowNumber = bs_last.rowNumber;
                bs.colNumber = bs_last.colNumber - 1;
                bubbles.Add(bubble3);
            }
			for (int m = 1; m < i * 2 + 1; m++)
			{
				GameObject bubble4 = Instantiate(prefab_bubble);
				bubble4.transform.position = bubbles[^1].transform.position + 
					new Vector3(-offset_row_x,
					-offset_row_y,
					-offset_row_y);
                BubbleScript bs = bubble4.GetComponentInChildren<BubbleScript>();
                BubbleScript bs_last = bubbles[^1].GetComponentInChildren<BubbleScript>();
                bs.rowNumber = bs_last.rowNumber - 1;
                bs.colNumber = bs_last.colNumber;
                bubbles.Add(bubble4);
            }
			for (int n = 1; n < i * 2 + 1; n++)
			{
				GameObject bubble5 = Instantiate(prefab_bubble);
				bubble5.transform.position = bubbles[^1].transform.position + 
					new Vector3(offset_col_x,
					-offset_col_y,
					-offset_col_y);
                BubbleScript bs = bubble5.GetComponentInChildren<BubbleScript>();
                BubbleScript bs_last = bubbles[^1].GetComponentInChildren<BubbleScript>();
                bs.rowNumber = bs_last.rowNumber;
                bs.colNumber = bs_last.colNumber + 1;
                bubbles.Add(bubble5);
            }
		}
	}
	
	// used to start pumping a single bubble
	private void ActivateABubble()
	{
		for(int q = 0; q < bubbles.Count; q++)
		{
			BubbleScript bs = bubbles[q].GetComponentInChildren<BubbleScript>();
			if (!bs.active)
			{
				bs.hp = bubbleHp;
				bs.pumping = true;
                if (GameManager.me.chestCount >= GameManager.me.chestCountMax)
				{
					bs.containUpgrade = true;
					bs.bubbleImg.GetComponent<SpriteRenderer>().material = matChest; // set bubble mat to yellow
					GameManager.me.ResetChestCount(); // reset chest count to zero
					GameManager.me.ChestCountMaxUp(); // increase chest count max
					ScoreManager.me.UpdateChestCountMax(); // update chest count max UI
                }
				if (BubbleUpgrade.me.lineExplosion &&
                    CheckPercentage(percentage_lineExplosion))
				{
					bs.lineExplosion = true;
					bs.bubbleImg.GetComponent<SpriteRenderer>().material = BubbleUpgrade.me.prefab_lineExplosion.GetComponent<UpgradeHolderScript>().mat_upgrade;

                }
				if (BubbleUpgrade.me.boxExplosion &&
                    CheckPercentage(percentage_boxExplosion))
				{
					bs.boxExplosion = true;
                    bs.bubbleImg.GetComponent<SpriteRenderer>().material = BubbleUpgrade.me.prefab_boxExplosion.GetComponent<UpgradeHolderScript>().mat_upgrade;
                }
				if (BubbleUpgrade.me.thornFan &&
                    CheckPercentage(percentage_thornFan))
				{
					bs.thornFan = true;
                    bs.bubbleImg.GetComponent<SpriteRenderer>().material = BubbleUpgrade.me.prefab_thornFan.GetComponent<UpgradeHolderScript>().mat_upgrade;
                }
				if(BubbleUpgrade.me.fastSqueeze &&
                    CheckPercentage(percentage_fastSqueeze))
				{
					bs.fastSqueeze = true;
					bs.bubbleImg.GetComponent<SpriteRenderer>().material = BubbleUpgrade.me.prefab_fastSqueeze.GetComponent<UpgradeHolderScript>().mat_upgrade;
				}
                //CameraZoomScript.me.FitCamera();
                break;
			}
		}
	}

	// used to start pumping a few bubbles at start
	private void InitialPump(int amount)
	{
		for(int q = 0;q < amount; q++)
		{
            BubbleScript bs = bubbles[q].GetComponentInChildren<BubbleScript>();
            bs.pumping = true;
            //bs.transform.localScale = new(bs.size_baseline, bs.size_baseline);
        }
	}

	private bool CheckPercentage(float percentage)
	{
		float randNum = Random.Range(0f, 1f);
        print("rolled " + randNum + " vs " + percentage);
        if (randNum <= percentage)
		{
			return true;
		}
		else
		{
			return false;
		}
	}

	public void SetPercentage(string bombClass, int level)
	{
		switch (bombClass)
		{
			case "box":
				percentage_boxExplosion = level * HandUpgrade.me.moreBomb_percentageIncreasePerLevel;
				break;
			case "line":
				percentage_lineExplosion = level * HandUpgrade.me.moreBomb_percentageIncreasePerLevel;
				break;
			case "thornFan":
				percentage_thornFan = level * HandUpgrade.me.moreBomb_percentageIncreasePerLevel;
				break;
			default:
				break;
		}
	}
}
