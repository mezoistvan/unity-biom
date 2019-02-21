using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class AudioLoader : MonoBehaviour {
    string nextSceneName = "first-scene";
    string loadedBankName = "Master Bank";

    void Start() {
        FMODUnity.RuntimeManager.LoadBank("Master Bank.strings");
        FMODUnity.RuntimeManager.LoadBank(loadedBankName);
    }
    void Update() {
        if (FMODUnity.RuntimeManager.HasBankLoaded(loadedBankName) && FMODUnity.RuntimeManager.HasBankLoaded("Master Bank.strings"))
        {
            Debug.Log("Master Bank Loaded");
            SceneManager.LoadScene(nextSceneName, LoadSceneMode.Single);
        }
    }
}