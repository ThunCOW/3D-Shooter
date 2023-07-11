using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Audio Config", menuName = "Guns/Audio Config", order = 6)]
public class GunAudioConfigScriptableObject : ScriptableObject
{
    [Range(0f, 1f)]
    public float volume = 1f;
    public AudioClip[] FireClips;
    public AudioClip EmptyClip;

    public void PlayShootingClip(AudioSource AudioSource)
    {
        AudioSource.PlayOneShot(FireClips[Random.Range(0, FireClips.Length)], volume);
    }

    public void PlayOutOfAmmoClip(AudioSource AudioSource)
    {
        AudioSource.PlayOneShot(EmptyClip, volume);
    }
}
