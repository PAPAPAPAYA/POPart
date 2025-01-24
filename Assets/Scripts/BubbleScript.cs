using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubbleScript : MonoBehaviour
{
	public GameObject PSprefab_burst;
	public GameObject BubbleCircle;//Find Prefab Bubble's child "Circle", try to change color of it if it's a bomb
	public bool bomb = false;
	public int hp = 0;
	public int rowNumber;
	public int colNumber;
	private void Update()
	{
		if (hp <= 0)
		{
			OnBurst();
		}
	}
	private void OnMouseDown()
	{
		SpawnHurtPS();
		hp -= 1;
		//CameraZoomScript.me.FitCamera();
	}
	private void OnBurst()
	{
		if (bomb)
		{
			Explode();
		}
		BubbleMasterScript.me.bubbles.Remove(gameObject);
		gameObject.SetActive(false);
    }
	
	
	#region FOR UPGRADES
	private void Explode() // destroy neighbour bubbles
	{
		foreach (var bubble in BubbleMasterScript.me.bubbles)
		{
			BubbleScript bs = bubble.GetComponent<BubbleScript>();
			if ((bs.rowNumber == rowNumber && bs.colNumber == colNumber - 1) ||
			(bs.rowNumber == rowNumber && bs.colNumber == colNumber + 1) ||
			(bs.rowNumber == rowNumber - 1 && bs.colNumber == colNumber) ||
			(bs.rowNumber == rowNumber + 1 && bs.colNumber == colNumber))
			{
				bs.DelayedDMGCaller(0.25f, 1);
			}
			if ((bs.rowNumber == rowNumber && bs.colNumber == colNumber - 2) ||
			(bs.rowNumber == rowNumber && bs.colNumber == colNumber + 2) ||
			(bs.rowNumber == rowNumber - 2&& bs.colNumber == colNumber) ||
			(bs.rowNumber == rowNumber + 2 && bs.colNumber == colNumber))
			{
				bs.DelayedDMGCaller(0.5f, 1);
			}
		}
	}
	public void DelayedDMGCaller(float delayDuration, int dmgAmount) // used to start coroutine from another script
	{
		StartCoroutine(DelayedDMG(delayDuration, dmgAmount));
	}
	private IEnumerator DelayedDMG(float delayDuration, int dmgAmount) // create time differences between bursts
	{
		yield return new WaitForSeconds(delayDuration);
		SpawnHurtPS();
		hp -= dmgAmount;
	}
	#endregion

	#region VFX
	private void SpawnHurtPS()
	{
		Vector3 pos = new(transform.position.x, transform.position.y, transform.position.z - 0.2f);
		Instantiate(PSprefab_burst, pos, Quaternion.identity);
	}
	#endregion
}
