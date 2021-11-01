using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SfxManager : MonoBehaviour
{
    AudioSource audio;
    public AudioClip collect_sound;
    public AudioClip timetravel_sound;
    public AudioClip ramBlock_sound;
    public AudioClip speed_sound;
    // Start is called before the first frame update
    public enum Clip {
        collect,
        timeTravel,
        ramBlock,
        speedUp,
    }

    private void Start() {
        audio = GetComponent<AudioSource>();
    }

    public void PlaySound(Clip clip) {
        switch (clip) {
            case Clip.collect:
                audio.clip = collect_sound;
                break;
            case Clip.timeTravel:
                audio.clip = timetravel_sound;
                break;
            case Clip.ramBlock:
                audio.clip = ramBlock_sound;
                break;
            case Clip.speedUp:
                audio.clip = speed_sound;
                break;
            default:
                break;
        }
        audio.Play();
    }
}
