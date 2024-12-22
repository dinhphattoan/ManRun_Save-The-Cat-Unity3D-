using System.Collections.Generic;
using UnityEngine;

public class SoundController : MonoBehaviour
{
    [SerializeField] private SO_GameDatabase gameDatabase;
    [SerializeField] private AudioSource backgroundAudioSource;
    [SerializeField] private List<AudioSource> activeAudioSource = new();
    List<AudioSource> sfxPoolAudioSource = new();
    [SerializeField] Transform audioPoolHolder;
    [SerializeField] int audioPoolSize = 5;
    public void Initialize()
    {
        backgroundAudioSource.clip = gameDatabase.NextBackgroundAudioClip();
        backgroundAudioSource.Play();

    }
    private void Update()
    {
        HandleBackgroundAudio();
    }
    public void HandleBackgroundAudio()
    {
        if (!backgroundAudioSource.isPlaying)
        {
            backgroundAudioSource.clip = gameDatabase.NextBackgroundAudioClip();
            backgroundAudioSource.Play();
        }

    }

    public void PlayNextBackgroundMusic()
    {
        backgroundAudioSource.clip = gameDatabase.NextBackgroundAudioClip();
    }

    public void PlayAudioClip(AudioClip audioClip, float distanceToListener)
    {
        AudioSource audioSource = GetPooledObject();
        if (audioSource != null && distanceToListener <=100)
        {
            audioSource.volume = 100 - distanceToListener;
            audioSource.clip = audioClip;
            audioSource.Play();

        }

    }
    public AudioSource GetPooledObject()
    {
        for (int i = 0; i < sfxPoolAudioSource.Count; i++)
        {
            if (!sfxPoolAudioSource[i].isPlaying)
            {
                return sfxPoolAudioSource[i];
            }
        }
        //If all is active
        if (sfxPoolAudioSource.Count < audioPoolSize)
        {
            GameObject obj = new GameObject();
            obj.transform.parent = audioPoolHolder;
            obj.name = "AudioSource";
            obj.AddComponent<AudioSource>();
            sfxPoolAudioSource.Add(obj.GetComponent<AudioSource>());
            return obj.GetComponent<AudioSource>();
        }
        return null;
    }
}
