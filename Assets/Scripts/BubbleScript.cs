using System.Collections;
using System.Collections.Generic;
using UnityEditor.Timeline;
using UnityEngine;
using MilkShake;

public class BubbleScript : MonoBehaviour
{
	[Header("VFXs")]
	public GameObject PSprefab_burst;
	public GameObject PS_squeeze;
	private Shaker shaker;
	public ShakePreset SP_squeeze;
	private ShakeInstance shakeInstance;
	public GameObject BubbleCircle;//Find Prefab Bubble's child "Circle", try to change color of it if it's a bomb
	[Header("UPGRADE BOOLs")]
	public bool lineExplosion = false;
	public bool boxExplosion = false;
	public bool thornFan = false;
	[Header("BASICs")]
	public int hp = 0;
	public int rowNumber;
	public int colNumber;
	[Header("POP")]
	public float squeezeTime;
	private float squeezeTimer;
	private bool mouseDown;
	[Header("PUMP")]
    public float size_baseline; // when size baseline is reached, this bubble is active
    public bool active = false; // if active, it can be pop
	public bool pumping = true; // if not pumping, it can be pump
	public Vector3 size_bursted; // the size when popped
	public float pumpSpd; // how fast it's pumped

    private void Start()
    {
		// initialize squeezeTimer
		squeezeTimer = squeezeTime;
		shaker = GetComponent<Shaker>();
        shakeInstance = shaker.Shake(SP_squeeze);
		shakeInstance.Stop(0, false);
    }

    private void Update()
	{
		// if size reached baseline, it's active
		if(transform.localScale.x >= size_baseline)
		{
			active = true;
		}
        // if popped, call OnBurst()
        if (hp <= 0)
		{
			OnBurst();
			hp = 1;
		}
		// pumping is set in BubbleMaker, when pumping is true, pump()
		if (pumping)
		{
			Pump();
		}
		if (mouseDown)
		{
			if (active)
			{
                if (squeezeTimer > 0)
                {
					if (!PS_squeeze.GetComponent<ParticleSystem>().isPlaying)
					{
                        PS_squeeze.GetComponent<ParticleSystem>().Play();
                    }
                    shakeInstance.Start(SP_squeeze.FadeIn);
                    squeezeTimer -= Time.deltaTime;
                }
                else if (squeezeTimer <= 0)
                {
                    squeezeTimer = squeezeTime;
                    hp--;
                }
            }
		}
	}
    private void OnMouseDown()
    {
		mouseDown = true;
		if (active)
		{
			// start playing ps_squeeze
            PS_squeeze.GetComponent<ParticleSystem>().Play();
			// start shaking
			shakeInstance.Start(SP_squeeze.FadeIn);
        }
    }
    private void OnMouseUp()
    {
		mouseDown = false;
        // stop playing ps_squeeze
        PS_squeeze.GetComponent<ParticleSystem>().Stop();
        // stop shaking
        shakeInstance.Stop(SP_squeeze.FadeOut, false);
		// reset squeeze timer
		squeezeTime = squeezeTimer;
    }
    private void OnMouseDrag()
    {
  //      if (active)
		//{
		//	if (squeezeTimer > 0)
		//	{
  //              PS_squeeze.GetComponent<ParticleSystem>().Play();
  //              squeezeTimer -= Time.deltaTime;
  //          }
		//	else if (squeezeTimer <= 0)
		//	{
		//		squeezeTimer = squeezeTime;
  //              hp--;
		//	}
		//}
    }
    private void OnBurst()
	{
        if (lineExplosion)
		{
			BubbleUpgrade.me.LineExplode(rowNumber, colNumber);
		}
		if (boxExplosion)
		{
			BubbleUpgrade.me.BoxExplode(rowNumber, colNumber);
		}
		if (thornFan)
		{
			BubbleUpgrade.me.ThornFan(5);
		}
		active = false;
		pumping = false;
		transform.localScale = size_bursted;
		Instantiate(PSprefab_burst, transform.position, Quaternion.identity);
		// stop shaking
		shakeInstance.Stop(0, false);
		// stop playing ps_squeeze
        PS_squeeze.GetComponent<ParticleSystem>().Stop();
    }
    public void Pump()
	{
		if (transform.localScale.x < size_baseline - 0.1f)
		{
			transform.localScale = new Vector3(Mathf.Lerp(transform.localScale.x, size_baseline, pumpSpd * Time.deltaTime),
                Mathf.Lerp(transform.localScale.y, size_baseline, pumpSpd * Time.deltaTime),
				1);
		}
		else if (transform.localScale.x < size_baseline)
		{
			transform.localScale = new Vector3(transform.localScale.x + 0.25f * Time.deltaTime,
				transform.localScale.y + 0.25f * Time.deltaTime,
				1);
		}
		else
		{
			transform.localScale = new Vector3(size_baseline, size_baseline, 1);
        }
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
