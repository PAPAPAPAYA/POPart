using System.Collections;
using System.Collections.Generic;
using UnityEditor.Timeline;
using UnityEngine;
using MilkShake;
using System.Diagnostics.Tracing;

public class BubbleScript : MonoBehaviour
{
	[Header("VFXs")]
	public GameObject PSprefab_burst;
	public GameObject PS_squeeze;
	private Shaker shaker;
	public ShakePreset SP_squeeze;
	public ShakePreset shortShakePreset;
	private ShakeInstance shakeInstance;
	public GameObject BubbleCircle;//Find Prefab Bubble's child "Circle", try to change color of it if it's a bomb
	public GameObject bubbleImg;
	[Header("UPGRADE BOOLs")]
	public bool containUpgrade = false;
	public bool lineExplosion = false;
	public bool boxExplosion = false;
	public bool thornFan = false;
	[Header("BASICs")]
	public int hp = 0;
	public int rowNumber;
	public int colNumber;
	private Material ogMat;
	[Header("POP")]
    public bool mouseDown;
    public float squeezeTime;
    public float squeezeTimer;
	//private bool hasBurst = false;

	[Header("PUMP")]
    public float size_baseline; // when size baseline is reached, this bubble is active
    public bool active = false; // if active, it can be pop
	public bool pumping = true; // if not pumping, it can be pump
	public Vector3 size_bursted; // the size when popped
	public float pumpSpd; // how fast it's pumped

    public bool hasPlayedRechargeSound = true; // Flag to check if the recharge sound has been played

    private void Start()
    {
		// initialize squeezeTimer
		squeezeTimer = squeezeTime;
		shaker = GetComponent<Shaker>();
        shakeInstance = shaker.Shake(SP_squeeze);
		shakeInstance.Stop(0, false);
		ogMat = bubbleImg.GetComponent<SpriteRenderer>().material;
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
			
			AudioManager.Instance.PlayChargingSound(squeezeTime);
        }
		else{
			shaker.Shake(shortShakePreset);
			AudioManager.Instance.PlayPopDenySound();
			// if not active, play a sound
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
		squeezeTimer = squeezeTime;
    }
    public void ResetBubble()
    {
        mouseDown = false;
        // stop playing ps_squeeze
        PS_squeeze.GetComponent<ParticleSystem>().Stop();
        // stop shaking
        shakeInstance.Stop(SP_squeeze.FadeOut, false);
        // reset squeeze timer
        squeezeTimer = squeezeTime;

		AudioManager.Instance.TerminateChargingSound();
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
			BubbleUpgrade.me.ThornFan(BubbleUpgrade.me.thornFanLevel * 2);
		}
		if (containUpgrade)
		{
            UpgradeInteractionManagerScript.me.ShowButtons();
        }
		active = false;
		pumping = false;
		transform.localScale = size_bursted;
		Instantiate(PSprefab_burst, transform.position, Quaternion.identity);
		//Score up
		GameManager.me.score += 1;
		// stop shaking
		shakeInstance.Stop(0, false);
		// stop playing ps_squeeze
        PS_squeeze.GetComponent<ParticleSystem>().Stop();

		AudioManager.Instance.PlayPopSound();

		// reset mat
		bubbleImg.GetComponent<SpriteRenderer>().material = ogMat;
		// reset upgrade
		lineExplosion = false;
		boxExplosion = false;
		thornFan = false;
		containUpgrade = false;
    }
    public void Pump()
    {
        if (transform.localScale.x < size_baseline - 0.1f)
        {
            transform.localScale = new Vector3(Mathf.Lerp(transform.localScale.x, size_baseline, pumpSpd * Time.deltaTime),
                Mathf.Lerp(transform.localScale.y, size_baseline, pumpSpd * Time.deltaTime),
                1);
            hasPlayedRechargeSound = false; // Reset the flag when pumping
        }
        else if (transform.localScale.x < size_baseline)
        {
            transform.localScale = new Vector3(transform.localScale.x + 0.25f * Time.deltaTime,
                transform.localScale.y + 0.25f * Time.deltaTime,
                1);
            hasPlayedRechargeSound = false; // Reset the flag when pumping
        }
        else
        {
            transform.localScale = new Vector3(size_baseline, size_baseline, 1);
            // Play recharge sound once when the bubble reaches the baseline size
            if (!hasPlayedRechargeSound)
            {
                AudioManager.Instance.PlayRechargeSound();
                hasPlayedRechargeSound = true; // Set the flag to true
            }
        }
    }
	
	
	#region FOR UPGRADES
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
