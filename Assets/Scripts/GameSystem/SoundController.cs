using System.Collections.Generic;
using UnityEngine;

public class SoundController : MonoBehaviour
{
    [SerializeField] private SO_GameDatabase gameDatabase;
    [SerializeField] private AudioSource backgroundAudioSource;
    List<AudioSource> sfxPoolAudioSource = new();
    [SerializeField] Transform audioPoolHolder;
    [SerializeField] int audioPoolSize = 5;
    public void Initialize()
    {
        backgroundAudioSource.clip = gameDatabase.PlayBackgroundClip();
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
            backgroundAudioSource.clip = gameDatabase.NextMusicClip(backgroundAudioSource.clip);
            backgroundAudioSource.Play();
        }

    }

    public void PlayInGameMusic()
    {
        backgroundAudioSource.clip = gameDatabase.PlayInGameClip();
        backgroundAudioSource.Play();
    }
    public void PlayBackgroundMusic()
    {
        backgroundAudioSource.clip = gameDatabase.PlayBackgroundClip();
        backgroundAudioSource.Play();
    }
    public void NextMusicClip()
    {
        backgroundAudioSource.clip = gameDatabase.NextMusicClip(backgroundAudioSource.clip);
        backgroundAudioSource.Play();
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
