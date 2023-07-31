using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIPhaseNotification : MonoBehaviour
{
    TMP_Text phaseText;

    AudioSource audioSource;

    public AudioClip PhaseClip;
    public float Volume;
    public float AudioDelay;
    void Start()
    {
        phaseText = GetComponent<TMP_Text>();
        audioSource = GetComponent<AudioSource>();

        phaseText.text = "***Phase " + (SpawnManager.CurrentSpawnPhase + 1).ToString() + "***";
        audioSource.PlayOneShot(PhaseClip, Volume);
        audioSource.PlayDelayed(AudioDelay);
    }

    public void AnimationOver()
    {
        Destroy(transform.parent.gameObject);
    }
}
