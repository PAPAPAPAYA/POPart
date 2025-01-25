using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    public AudioSource bgmSource;
    public AudioSource sfxSource;
    public AudioMixer audioMixer;

    public AudioClip[] popSounds;
    public AudioClip[] chargingSounds; // Array of charging sounds
    public AudioClip[] rechargeSounds; // Array of recharge sounds
    public AudioClip bgm; // Single BGM for all scenes

    private Coroutine chargingCoroutine;
    private Coroutine muffleCoroutine;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        PlayBGM(bgm); // Play the single BGM on start
    }

    // Play a sound effect
    public void PlaySound(AudioClip clip)
    {
        sfxSource.PlayOneShot(clip);
    }

    // Play a random pop sound with pitch variation, reverb, and echo
    public void PlayPopSound()
    {
        int index = Random.Range(0, popSounds.Length);
        sfxSource.pitch = Random.Range(0.9f, 1.1f);

        // // Set reverb and echo strengths
        // SetReverbStrength(reverbStrength);
        // SetEchoStrength(echoStrength);

        sfxSource.PlayOneShot(popSounds[index]);
    }

    // Play background music
    public void PlayBGM(AudioClip clip)
    {
        bgmSource.clip = clip;
        bgmSource.Play();
    }

    // Play charging sound with adjustable length
    public void PlayChargingSound(float length)
    {
        if (chargingCoroutine != null)
        {
            StopCoroutine(chargingCoroutine);
        }
        int index = Random.Range(0, chargingSounds.Length);
        AudioClip selectedClip = chargingSounds[index];
        chargingCoroutine = StartCoroutine(PlayChargingSoundCoroutine(selectedClip, length));
    }

    private IEnumerator PlayChargingSoundCoroutine(AudioClip clip, float length)
    {
        sfxSource.clip = clip;
        sfxSource.pitch = sfxSource.clip.length / length; // Adjust pitch to fit the desired length
        sfxSource.Play();
        yield return new WaitForSeconds(length);
        sfxSource.Stop();
        sfxSource.pitch = 1.0f; // Reset pitch to normal after playing
    }

    // Terminate charging sound
    public void TerminateChargingSound()
    {
        if (chargingCoroutine != null)
        {
            StopCoroutine(chargingCoroutine);
            sfxSource.Stop();
        }
    }

    // Play recharge sound with randomized pitch
    public void PlayRechargeSound()
    {
        int index = Random.Range(0, rechargeSounds.Length);
        sfxSource.pitch = Random.Range(0.9f, 1.1f);
        sfxSource.PlayOneShot(rechargeSounds[index]);
        sfxSource.pitch = 1.0f; // Reset pitch to normal after playing
    }

    // Apply muffle effect with smooth transition
    public void ApplyMuffleEffect(bool apply)
    {
        if (muffleCoroutine != null)
        {
            StopCoroutine(muffleCoroutine);
        }
        float targetCutoff = apply ? 300f : 22000f;
        float targetVolume = apply ? -20f : 0f; // Example values, adjust as needed
        muffleCoroutine = StartCoroutine(SmoothTransition(targetCutoff, targetVolume));
    }

    private IEnumerator SmoothTransition(float targetCutoff, float targetVolume)
    {
        float currentCutoff;
        float currentVolume;
        audioMixer.GetFloat("LowPassCutoff", out currentCutoff);
        audioMixer.GetFloat("SFXVolume", out currentVolume);
        float duration = 1.0f; // Duration of the transition in seconds
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            float easedT = EaseOutExpo(t);
            float newCutoff = Mathf.Lerp(currentCutoff, targetCutoff, easedT);
            float newVolume = Mathf.Lerp(currentVolume, targetVolume, easedT);
            audioMixer.SetFloat("LowPassCutoff", newCutoff);
            audioMixer.SetFloat("SFXVolume", newVolume);
            yield return null;
        }

        audioMixer.SetFloat("LowPassCutoff", targetCutoff);
        audioMixer.SetFloat("SFXVolume", targetVolume);
    }

    // Exponential easing function for smooth transition
    private float EaseOutExpo(float t)
    {
        return t == 1 ? 1 : 1 - Mathf.Pow(2, -10 * t);
    }

    // Set reverb strength
    // private void SetReverbStrength(float strength)
    // {
    //     audioMixer.SetFloat("ReverbLevel", Mathf.Lerp(-10000, 0, strength)); // Adjust range as needed
    // }

    // // Set echo strength
    // private void SetEchoStrength(float strength)
    // {
    //     audioMixer.SetFloat("EchoDelay", Mathf.Lerp(10, 500, strength)); // Adjust range as needed
    //     audioMixer.SetFloat("EchoDecayRatio", Mathf.Lerp(0.1f, 1.0f, strength)); // Adjust range as needed
    // }
}