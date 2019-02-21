using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class MusicPlayer: MonoBehaviour {

    [FMODUnity.EventRef]
    public string selectSound;
    FMOD.Studio.EventInstance soundEvent;

    void Start() {
        // soundEvent = FMODUnity.RuntimeManager.CreateInstance(selectSound);
        // FMODUnity.RuntimeManager.AttachInstanceToGameObject(soundEvent, GetComponent<Transform>(), GetComponent<Rigidbody>());
    }
    void Update() {
        // FMODUnity.RuntimeManager.PlayOneShot(selectSound, transform.position);
        // PlaySound();
    }

    // void PlaySound() {
    //     FMOD.Studio.PLAYBACK_STATE fmodPBState;
    //     soundEvent.getPlaybackState(out fmodPBState);
    //     if (fmodPBState != FMOD.Studio.PLAYBACK_STATE.PLAYING) {
    //         soundEvent.start();
    //     }
    // }
}