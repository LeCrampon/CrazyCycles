using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BikeAudio : MonoBehaviour
{
    private AudioSource _audioSource;

    [SerializeField]
    private AudioClip _runningClip;
    [SerializeField]
    private AudioClip _brakingClip;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    public void PlayRunningClip()
    {
        _audioSource.Stop();
        _audioSource.clip = _runningClip;
        _audioSource.loop = true;
        _audioSource.Play();
    }

    public void PlayBrakingClip()
    {
        _audioSource.Stop();
        _audioSource.clip = _brakingClip;
        _audioSource.loop = false;
        _audioSource.Play();
    }

    public void StopAllClips()
    {
        _audioSource.Stop();
    }
}
