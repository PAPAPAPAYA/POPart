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
	public GameObject prefab_bubble;
	private Vector3 startPos;
	public int amount_row;
	public int amount_col;
	public float offset_row_x;
	public float offset_row_y;
	public float offset_col_x;
	public float offset_col_y;
	
	public int bubbleHp;



	private void Start()
	{
		startPos = new Vector3(
			-amount_row / 2f * offset_row_x - amount_col / 2f * offset_col_x + offset_col_x, 
			-amount_row / 2f * offset_row_y + amount_col / 2f * offset_col_y, 
			0);
		MakeBubbles(amount_row, amount_col);
	}
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
}
