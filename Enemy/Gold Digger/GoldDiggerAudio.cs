using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoldDiggerAudio : MonoBehaviour
{
    [SerializeField] private AudioSource _EffectSource;

    [SerializeField] private AudioClip _GroundImpact;
    [SerializeField] private AudioClip _Grinding;
    [SerializeField] private AudioClip _Walking;

    public void PlayGroundImpact()
    {
        //_EffectSource.loop = false;
        _EffectSource.PlayOneShot(_GroundImpact);
    }

    public void PlayGrinding()
    {
        //_EffectSource.loop = true;

        _EffectSource.clip = _Grinding;
        _EffectSource.Play();
    }

    public void PlayWalking()
    {
        //_EffectSource.loop = true;
        _EffectSource.clip = _Walking;
        _EffectSource.Play();
    }

    public void StopSound()
    {
        _EffectSource.Stop();
        Debug.Log("Stop");
    }
}
