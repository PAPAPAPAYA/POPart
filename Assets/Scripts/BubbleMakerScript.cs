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
	private Vector3 startPos;
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
	[Header("For Putting in Upgrades")]
	public float percentage_containUpgrade;
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
                if (CheckPercentage(percentage_containUpgrade))
				{
					bs.containUpgrade = true;
					bs.bubbleImg.GetComponent<SpriteRenderer>().material = matChest;
                }
				if (CheckPercentage(percentage_lineExplosion) &&
					BubbleUpgrade.me.lineExplosion)
				{
					bs.lineExplosion = true;
					bs.bubbleImg.GetComponent<SpriteRenderer>().material = BubbleUpgrade.me.prefab_lineExplosion.GetComponent<UpgradeHolderScript>().mat_upgrade;

                }
				if (CheckPercentage(percentage_boxExplosion) &&
                    BubbleUpgrade.me.boxExplosion)
				{
					bs.boxExplosion = true;
                    bs.bubbleImg.GetComponent<SpriteRenderer>().material = BubbleUpgrade.me.prefab_boxExplosion.GetComponent<UpgradeHolderScript>().mat_upgrade;
                }
				if (CheckPercentage(percentage_thornFan) &&
                    BubbleUpgrade.me.thornFan)
				{
					bs.thornFan = true;
                    bs.bubbleImg.GetComponent<SpriteRenderer>().material = BubbleUpgrade.me.prefab_thornFan.GetComponent<UpgradeHolderScript>().mat_upgrade;
                }
				if(CheckPercentage(percentage_fastSqueeze) &&
					BubbleUpgrade.me.fastSqueeze)
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
            bs.transform.localScale = new(bs.size_baseline, bs.size_baseline);
        }
	}

	private bool CheckPercentage(float percentage)
	{
		if (Random.Range(0, 1f) <= percentage)
		{
			return true;
		}
		else
		{
			return false;
		}
	}

    // DEPRECATED
    private void MakeBubbles(int x, int y)
    {
        for (int i = 0; i < x; i++)
        {
            for (int j = 0; j < y; j++)
            {
                GameObject bubble = Instantiate(prefab_bubble);
                bubble.transform.position = new Vector3(
                    startPos.x + i * offset_row_x + j * offset_col_x,
                    startPos.y + i * offset_row_y - j * offset_col_y,
                    startPos.z + i * offset_row_y - j * offset_col_y);
                BubbleMasterScript.me.bubbles.Add(bubble);
                BubbleScript bs = bubble.GetComponent<BubbleScript>();
                bs.hp = bubbleHp;
                bs.rowNumber = i;
                bs.colNumber = j;
                bubble.name = bubble.name + " (" + i + ", " + j + ")";
                //CameraZoomScript.me.SaveBubbleWidth(bubble.transform.position.x, bubble.transform.position.x, bubble.transform.position.y, bubble.transform.position.y);
            }
        }
    }

	public void SetPercentage(string bombClass, int level)
	{
		switch (bombClass)
		{
			case "box":
				percentage_boxExplosion = level * 0.05f;
				break;
			case "line":
				percentage_lineExplosion = level * 0.05f;
				break;
			case "thornFan":
				percentage_thornFan = level * 0.05f;
				break;
			default:
				break;
		}
	}
}
