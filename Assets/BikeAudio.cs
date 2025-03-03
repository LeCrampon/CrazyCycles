using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BikeAudio : MonoBehaviour
{
    [SerializeField]
    private AudioSource _bikeLoopFastAudioSource;
    [SerializeField]
    private AudioSource _bikeLoopSlowAudioSource;

    [SerializeField]
    private AudioClip _fastRunningClip;
    [SerializeField]
    private AudioClip _slowRunningClip;
    [SerializeField]
    private AudioClip _brakingClip;


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
        Debug.Log("SLOWPLAYIN'");
    }

    public void PlayBrakingClip()
    {
        _bikeLoopFastAudioSource.Stop();
        _bikeLoopFastAudioSource.clip = _brakingClip;
        _bikeLoopFastAudioSource.loop = false;
        _bikeLoopFastAudioSource.Play();
    }

    public void StopAllClips()
    {
        if(_bikeLoopFastAudioSource.isPlaying)
            _bikeLoopFastAudioSource.Stop();
        if(_bikeLoopSlowAudioSource.isPlaying)
            _bikeLoopSlowAudioSource.Stop();
        Debug.Log("STOPPING EVERYTHING");
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
        Debug.Log("STARTING");

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
        Debug.Log("STOPPING");

        yield break;
    }

    public void SwitchToHigherGear()
    {
        StartCoroutine(RunningBikeCrossFade(_bikeLoopSlowAudioSource, _bikeLoopFastAudioSource, 1f));
    }  
    
    public void SwitchToLowerGear()
    {
        StartCoroutine(RunningBikeCrossFade(_bikeLoopFastAudioSource, _bikeLoopSlowAudioSource, 1f));
    }
}
