using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TUA_AudioManager : MonoBehaviour
{

    public AudioSource audioSource_sfx;
    public AudioSource audioSource_music;

    public AudioSource audioSource_loop;

    [Header("UI Sounds")]
    public AudioClip buttonSound;

    //-------------------------------------------------------------------------------------------------------------------------
    public static TUA_AudioManager Instance { get; private set; } = null;

    void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("Multiple TUA_GameManager Instances exist!");
        }

        Instance = this;

    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayAudioSFX(AudioClip clip) {
        audioSource_sfx.PlayOneShot(clip);
    }

    public void PlayAudioMusic(AudioClip clip)
    {
        audioSource_music.clip = clip;
        audioSource_music.Play();
    }

    public void PlayButtonSound()
    {
        audioSource_sfx.PlayOneShot(buttonSound);
    }

    public void PlayAudioLoop(AudioClip clip) {
        audioSource_loop.clip = clip;
        audioSource_loop.Play();
    }

    public void StopAudioLoop() {
        audioSource_loop.Stop();
    }

    public void AudioLoopAdjustVolume(float adjustValue) {
        audioSource_loop.volume = adjustValue;
    }

    public void AudioLoopAdjustPitch(float adjustValue)
    {
        audioSource_loop.pitch = adjustValue + 0.2f;
    }
}
