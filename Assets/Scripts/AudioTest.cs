using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioTest : MonoBehaviour
{
    private bool isMuffled = false;
    private bool isCharging = false;
    private float chargeTime = 0.5f;
    private float chargeTimer = 0f;

    void Start()
    {
        AudioManager.Instance.PlayBGM(AudioManager.Instance.bgm);
    }

    // Update is called once per frame
    void Update()
    {
        // if (Input.GetKeyDown(KeyCode.Space))
        // {
        //     isCharging = true;
        //     chargeTimer = 0f;
        //     AudioManager.Instance.PlayChargingSound(chargeTime);
        // }

        if (Input.GetKey(KeyCode.Space))
        {
            // if (isCharging)
            // {
            //     chargeTimer += Time.deltaTime;
            //     if (chargeTimer >= chargeTime)
            //     {
            //         isCharging = false;
            //         AudioManager.Instance.TerminateChargingSound();
            //         AudioManager.Instance.PlayPopSound(); // Example reverb and echo strengths
            //     }
            // }
            //AudioManager.Instance.PlayPopSound();
        }

        // if (Input.GetKeyUp(KeyCode.Space))
        // {
        //     if (isCharging)
        //     {
        //         isCharging = false;
        //         AudioManager.Instance.TerminateChargingSound();
        //     }
        // }

        // if (Input.GetKeyDown(KeyCode.R))
        // {
        //     AudioManager.Instance.PlayRechargeSound();
        // }

        // if (Input.GetKeyDown(KeyCode.M))
        // {
        //     isMuffled = !isMuffled; // Toggle the state
        //     AudioManager.Instance.ApplyMuffleEffect(isMuffled);
        // }
        if(Input.GetKeyDown(KeyCode.Space))
        {
            AudioManager.Instance.PlayPopSound();
        }
    }
}