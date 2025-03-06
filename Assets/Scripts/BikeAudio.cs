using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BikeAudio : MonoBehaviour
{
    [Header("Bike grounded audio")]
    [SerializeField]
    private AudioSource _bikeGroundedAudioSource;

    [Header("Bike speed audio")]
    [SerializeField]
    private AudioSource _bikeLoopFastAudioSource;
    [SerializeField]
    private AudioSource _bikeLoopSlowAudioSource;

    [Header("Braking audio")]
    [SerializeField]
    private AudioSource _bikeBrakingAudioSource;
    [SerializeField]
    private AudioSource _bikeBrakingLoopAudioSource;

    [Header("Wind audio")]
    [SerializeField]
    private AudioSource _windAudioSource;


    [Header("Audio Clips")]
    [SerializeField]
    private AudioClip _fastRunningClip;
    [SerializeField]
    private AudioClip _slowRunningClip;
    [SerializeField]
    private AudioClip _brakingClip;
    [SerializeField]
    private AudioClip _windClip;


    private Coroutine _crossFadeCoroutine;

    [Header("Crash Audio")]
    [SerializeField]
    private AudioSource _crashAudioSource;
    [SerializeField]
    private List<AudioClip> _crashAudioClips;


    public void PlayFastRunningAudio()
    {
        _bikeLoopFastAudioSource.clip = _fastRunningClip;
        _bikeLoopFastAudioSource.loop = true;
        _bikeLoopFastAudioSource.Play();
    } 
    
    public void PlaySlowRunningAudio()
    {
        _bikeLoopSlowAudioSource.clip = _slowRunningClip;
        _bikeLoopSlowAudioSource.loop = true;
        _bikeLoopSlowAudioSource.Play();
    }

    public void PlayBrakingClip()
    {
        if (!_bikeBrakingAudioSource.isPlaying)
        {
            _bikeBrakingAudioSource.clip = _brakingClip;
            _bikeBrakingAudioSource.loop = false;
            _bikeBrakingAudioSource.pitch = Random.Range(0.9f,1.1f);
            _bikeBrakingAudioSource.Play();
        }

    }

    public void StopBrakingClip()
    {
        if (_bikeBrakingAudioSource.isPlaying)
        {
            _bikeBrakingAudioSource.Stop();
        }

    }

    public void StopAllClips()
    {
        if(_bikeLoopFastAudioSource.isPlaying)
            _bikeLoopFastAudioSource.Stop();
        if(_bikeLoopSlowAudioSource.isPlaying)
            _bikeLoopSlowAudioSource.Stop();
    }

    public void SetFastRunningAudioSpeed(float speed)
    {
        float pitchVariation = Mathf.Clamp( speed * .05f , .5f, 2f); 
        _bikeLoopFastAudioSource.outputAudioMixerGroup.audioMixer.SetFloat("fastRunningPitchFrequency", 1 / (pitchVariation));
        _bikeLoopFastAudioSource.pitch = pitchVariation;
    }

    public void SetSlowRunningAudioSpeed(float speed)
    {
        float pitchVariation = Mathf.Clamp(speed * .2f, 1f, 2f);
        _bikeLoopSlowAudioSource.outputAudioMixerGroup.audioMixer.SetFloat("slowRunningPitchFrequency", 1 / (pitchVariation));
        _bikeLoopSlowAudioSource.pitch = pitchVariation;
    }


    public IEnumerator FadeInRunngingClip()
    {
        while(_bikeLoopFastAudioSource.volume < 1f)
        {
            yield return new WaitForSeconds(.01f);
            _bikeLoopFastAudioSource.volume += .01f;
        }
    }

    public void StartRunningClip()
    {
        if(_bikeLoopFastAudioSource.volume == 0)
        {
            StartCoroutine(FadeInRunngingClip());
        }
    }

    public IEnumerator FadeOutRunngingClip()
    {
        while (_bikeLoopFastAudioSource.volume > 0f)
        {
            yield return new WaitForSeconds(.1f);
            _bikeLoopFastAudioSource.volume -= .1f;
        }
    }

    public bool IsFastRunningClipPlaying()
    {
        return _bikeLoopFastAudioSource.isPlaying;
    }  
    
    public bool IsSlowRunningClipPlaying()
    {
        return _bikeLoopSlowAudioSource.isPlaying;

    }

    private IEnumerator RunningBikeCrossFade(AudioSource fadeOut, AudioSource fadeIn, float fadeTime)
    {
        float t = 0;

        if(!fadeIn.isPlaying)
            fadeIn.Play();

        float ratio = 0;

        while(t < fadeTime)
        {
            t += Time.deltaTime;
            ratio = Mathf.Lerp(0, 1, t);


            fadeOut.volume = Mathf.Lerp(1f, 0f, ratio);

            fadeIn.volume = Mathf.Lerp(0f, 1f , ratio);


            //Debug.Log("T : " + t);
            //Debug.Log("RATIO : " + ratio);
            //Debug.Log("IN VOLUME : " + fadeIn.volume);
            //Debug.Log("OUT VOLUME : " + fadeOut.volume);

            yield return null;
        }

        if(fadeOut.volume == 0)
            fadeOut.Stop();

        _crossFadeCoroutine = null;
        yield break;
    }

    public void SwitchToHigherGear()
    {
        if(_crossFadeCoroutine == null)
            _crossFadeCoroutine = StartCoroutine(RunningBikeCrossFade(_bikeLoopSlowAudioSource, _bikeLoopFastAudioSource, 1f));
    }  
    
    public void SwitchToLowerGear()
    {
        if (_crossFadeCoroutine == null)
            _crossFadeCoroutine = StartCoroutine(RunningBikeCrossFade(_bikeLoopFastAudioSource, _bikeLoopSlowAudioSource, 1f));
    }

    public IEnumerator AudioFadeIn(AudioSource audioSource, float maxVolume, float time)
    {
        audioSource.Play();
        while (audioSource.volume < maxVolume)
        {
            audioSource.volume += .01f;
            yield return new WaitForSeconds(time / 100f);
        }
    }

    public IEnumerator AudioFadeOut(AudioSource audioSource, float time)
    {

        while (audioSource.volume > 0)
        {
            audioSource.volume -= .01f;
            yield return new WaitForSeconds(time / 100f);
        }
        audioSource.Stop();
    }

    public void StartWindAudio()
    {
        if(!_windAudioSource.isPlaying)
        {
            StartCoroutine(AudioFadeIn(_windAudioSource, .2f, 2f));
        }
    }

    public void StopWindAudio()
    {
        if (_windAudioSource.isPlaying)
        {
            StartCoroutine(AudioFadeOut(_windAudioSource, 2f));
        }
    }


    public void StartGroundedAudio()
    {
        if (!_bikeGroundedAudioSource.isPlaying)
        {
            StartCoroutine(AudioFadeIn(_bikeGroundedAudioSource, .85f, .5f));
        }
    }

    public void StopGroundedAudio()
    {
        if (_bikeGroundedAudioSource.isPlaying)
        {
            StartCoroutine(AudioFadeOut(_bikeGroundedAudioSource, .5f));
        }
    }

    public void StartBrakingLoopAudio()
    {
        if (!_bikeBrakingLoopAudioSource.isPlaying)
        {
            PlayBrakingClip();
            StartCoroutine(AudioFadeIn(_bikeBrakingLoopAudioSource, .7f, .5f));
        }
    }

    public void StopBrakingLoopAudio()
    {
        if (_bikeBrakingLoopAudioSource.isPlaying)
        {
            StartCoroutine(AudioFadeOut(_bikeBrakingLoopAudioSource, .5f));
        }
    }

    public void PlayCrashAudio()
    {
        _crashAudioSource.clip = _crashAudioClips[Random.Range(0, _crashAudioClips.Count)];
        _crashAudioSource.pitch = Random.Range(.8f, 1.2f);
        _crashAudioSource.Play();
    }

}
