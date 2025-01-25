using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class BubbleMasterScript : MonoBehaviour
{
	#region SINGLETON
	public static BubbleMasterScript me;
	private void Awake()
	{
		me = this;
	}
	#endregion
	public List<GameObject> bubbles;
	private List<GameObject> shuffledList;
	private bool bubblesPopulated;
	private bool bubblesReady;
	
	public int amount_bomb;
	private void Update()
	{
		// check if list populated
		if (!bubblesPopulated &&
			bubbles.Count == BubbleMakerScript.me.amount_col * BubbleMakerScript.me.amount_row)
		{
			bubblesPopulated = true;
		}
		// when list populated, put in items once
		if (bubblesPopulated && !bubblesReady)
		{
			//PutInBombs();
			bubblesReady = true;
		}
	}
	private void PutInBombs()
	{
		// shuffle list
		shuffledList =  UtilityFunctions.me.ShuffleList(bubbles);
		for (int i = 0; i < amount_bomb; i++)
		{
			shuffledList[i].GetComponent<BubbleScript>().bomb = true;
			shuffledList[i].GetComponent<BubbleScript>().BubbleCircle.GetComponent<SpriteRenderer>().color = Color.red;//Change Circle's color when it's a bomb

        }
	}
}
