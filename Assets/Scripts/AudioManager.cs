using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    public AudioSource bgmSource;
    public AudioSource sfxSource;
    public AudioSource chargingSource; // Separate AudioSource for charging sound
    public AudioMixer audioMixer;

    public AudioClip[] popSounds;
    public AudioClip[] chargingSounds; // Array of charging sounds
    public AudioClip[] rechargeSounds; // Array of recharge sounds
    public AudioClip[] popDenySounds; // Array of pop deny sounds
    public AudioClip bgm; // Single BGM for all scenes

    private Coroutine chargingCoroutine;
    private Coroutine muffleCoroutine;

    private int maxSimultaneousSounds = 10; // Maximum number of simultaneous sounds
    private List<AudioSource> activeSources = new List<AudioSource>();

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
        SceneManager.sceneLoaded += OnSceneLoaded; // Register the event handler
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded; // Unregister the event handler
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "LeaderBoardScene")
        {
            ApplyMuffleEffect(true); // Apply the muffle effect when in LeaderBoardScene
        }
        else
        {
            ApplyMuffleEffect(false); // Unapply the muffle effect in other scenes
        }
    }

    // Play a sound effect with volume attenuation
    public void PlaySound(AudioClip clip)
    {
        if (activeSources.Count >= maxSimultaneousSounds)
        {
            AudioSource oldestSource = activeSources[0];
            activeSources.RemoveAt(0);
            Destroy(oldestSource.gameObject);
        }

        AudioSource newSource = CreateNewAudioSource();
        newSource.clip = clip;
        newSource.volume = Mathf.Clamp(1.0f / (activeSources.Count + 1), 0.1f, 1.0f); // Attenuate volume
        newSource.Play();
        activeSources.Add(newSource);
        StartCoroutine(RemoveSourceAfterPlay(newSource));
    }

    // Create a new AudioSource
    private AudioSource CreateNewAudioSource()
    {
        GameObject newAudioSourceObject = new GameObject("AudioSource");
        newAudioSourceObject.transform.SetParent(transform);
        return newAudioSourceObject.AddComponent<AudioSource>();
    }

    // Remove AudioSource after it finishes playing
    private IEnumerator RemoveSourceAfterPlay(AudioSource source)
    {
        yield return new WaitWhile(() => source != null && source.isPlaying);
        if (source != null)
        {
            activeSources.Remove(source);
            Destroy(source.gameObject);
        }
    }

    // Play a random pop sound with pitch variation, reverb, and echo
    public void PlayPopSound()
    {
        int index = Random.Range(0, popSounds.Length);
        sfxSource.pitch = Random.Range(0.9f, 1.1f);
        PlaySound(popSounds[index]);
    }

    public void PlayPopDenySound()
    {
        int index = Random.Range(0, popDenySounds.Length);
        sfxSource.pitch = Random.Range(0.8f, 1.2f);
        PlaySound(popDenySounds[index]);
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
        chargingSource.clip = clip;
        chargingSource.pitch = chargingSource.clip.length / length; // Adjust pitch to fit the desired length
        chargingSource.Play();
        yield return new WaitForSeconds(length);
        chargingSource.Stop();
        chargingSource.pitch = 1.0f; // Reset pitch to normal after playing
    }

    // Terminate charging sound
    public void TerminateChargingSound()
    {
        if (chargingCoroutine != null)
        {
            StopCoroutine(chargingCoroutine);
            chargingSource.Stop();
        }
    }

    // Play recharge sound with adjustable length
    public void PlayRechargeSound()
    {
        int index = Random.Range(0, rechargeSounds.Length);
        AudioClip selectedClip = rechargeSounds[index];
        //sfxSource.pitch = selectedClip.length / length; // Adjust pitch to fit the desired length
        PlaySound(selectedClip);
    }

    // Apply muffle effect with smooth transition
    public void ApplyMuffleEffect(bool apply)
    {
        if (muffleCoroutine != null)
        {
            StopCoroutine(muffleCoroutine);
        }
        float targetCutoff = apply ? 300f : 22000f;
        float targetVolume = apply ? -30f : -20f; // Example values, adjust as needed
        float targetPitch = apply ? 0.8f : 1.0f; // Example values, adjust as needed
        muffleCoroutine = StartCoroutine(SmoothTransition(targetCutoff, targetVolume, targetPitch));
    }

    private IEnumerator SmoothTransition(float targetCutoff, float targetVolume, float targetPitch)
    {
        float currentCutoff;
        float currentVolume;
        float currentPitch = bgmSource.pitch;
        audioMixer.GetFloat("LowPassCutoff", out currentCutoff);
        audioMixer.GetFloat("BGMVolume", out currentVolume);
        float duration = 0.5f; // Duration of the transition in seconds
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime; // Use unscaledDeltaTime instead of deltaTime
            float t = elapsed / duration;
            float easedT = EaseOutExpo(t);
            float newCutoff = Mathf.Lerp(currentCutoff, targetCutoff, easedT);
            float newVolume = Mathf.Lerp(currentVolume, targetVolume, easedT);
            float newPitch = Mathf.Lerp(currentPitch, targetPitch, easedT);
            audioMixer.SetFloat("LowPassCutoff", newCutoff);
            audioMixer.SetFloat("BGMVolume", newVolume);
            bgmSource.pitch = newPitch;
            yield return null;
        }

        audioMixer.SetFloat("LowPassCutoff", targetCutoff);
        audioMixer.SetFloat("BGMVolume", targetVolume);
        bgmSource.pitch = targetPitch;
    }

    // Exponential easing function for smooth transition
    private float EaseOutExpo(float t)
    {
        return t == 1 ? 1 : 1 - Mathf.Pow(2, -10 * t);
    }

    // Set master volume
    public void SetMasterVolume(float volume)
    {
        audioMixer.SetFloat("MasterVolume", volume);
    }

    //when spawn source, assign it to audio mixer
}