using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MilkShake;
using System.Diagnostics.Tracing;
using UnityEngine.SceneManagement;

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
	public bool fastSqueeze = false;
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

	[Header("ANIM")]
	public Animator bubbleAnimator;
	public Color ogColor;
	public Color inActiveColor;

	private LeaderboardTester leaderboardTester;

    protected virtual void Start()
    {
		// initialize squeezeTimer
		squeezeTimer = squeezeTime;
		shaker = GetComponent<Shaker>();
        shakeInstance = shaker.Shake(SP_squeeze);
		shakeInstance.Stop(0, false);
		ogMat = bubbleImg.GetComponent<SpriteRenderer>().material;
		ogColor = bubbleImg.GetComponent<SpriteRenderer>().color;

		leaderboardTester = FindObjectOfType<LeaderboardTester>();
    }

    private void Update()
	{
		// if size reached baseline, it's active
		/*if(transform.localScale.x >= size_baseline)
		{
			active = true;
            bubbleImg.GetComponent<SpriteRenderer>().color = ogColor;
        }*/
		if (active)
		{
            bubbleAnimator.Play("Active");
            bubbleImg.GetComponent<SpriteRenderer>().color = ogColor;

        }
		if (!active)
		{
            bubbleImg.GetComponent<SpriteRenderer>().color = inActiveColor;
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
			if (active && // if bubble is pumped
				!UpgradeInteractionManagerScript.me.showingButtons) // if not showing upgrade buttons
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
	public void setActive()
	{
		Debug.Log("setactive");
		active = true;
		pumping = false;
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
    protected virtual void OnBurst()
	{
        Debug.Log("b");
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
			BubbleUpgrade.me.ThornFan(BubbleUpgrade.me.thornFanLevel * 2, transform.position);
		}
		if (fastSqueeze)
		{
			BubbleUpgrade.me.FastSqueeze(rowNumber, colNumber);
		}
		if (containUpgrade)
		{
            UpgradeInteractionManagerScript.me.showButtonStack++;
        }
		active = false;
		pumping = false;

        bubbleAnimator.Play("Flat");
        //transform.localScale = size_bursted;
		
		Instantiate(PSprefab_burst, transform.position, Quaternion.identity);


		if (GameManager.me != null)
		{
			// Score up
			GameManager.me.score += 1;
			// Chest Count Up
			GameManager.me.ChestCountUp();
		}

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
		fastSqueeze = false;

		// Call the OnBubbleBurst method from LeaderboardTester
        if (leaderboardTester != null && SceneManager.GetActiveScene().name == "LeaderBoardScene")
        {
            leaderboardTester.OnBubbleBurst();
        }
		
		// Find the PumpThoseFuckers instance and call IncrementBurstCount
        PumpThoseFuckers pumpManager = FindObjectOfType<PumpThoseFuckers>();
        if (pumpManager != null)
        {
            pumpManager.IncrementBurstCount();
        }

    }
    protected virtual void Pump()
    {

		bubbleAnimator.Play("Pump");

		/*
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
		*/
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
