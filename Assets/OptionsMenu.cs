using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour {
    [SerializeField] private Slider _sensitivitySlider;
    [SerializeField] private Slider _fovSlider;
    [SerializeField] private Slider _masterVolumeSlider;


    public void OnEnable() {
        ReloadValues();
    }


    public void ReloadValues() {
        StartCoroutine(LoadValues());
    }

    private IEnumerator LoadValues() {
        if(Options.Loaded == false) {
            Options.LoadSettings();
            while (Options.Loaded == false) { yield return null; }
        }

        _sensitivitySlider.value = Options.Sensitivity;
        _fovSlider.value = Options.FOV;
        _masterVolumeSlider.value = Options.MasterVolume;
    }


    public void ResetValues() {
        Options.Sensitivity = Options.Def_Sensitivity;
        Options.FOV = Options.Def_FOV;
        Options.MasterVolume = Options.Def_MasterVolume;

        ReloadValues();
    }

    public void AdjustSensitivity(float value) {
        Options.Sensitivity = value;
    }

    public void AdjustFOV(float value) {
        Options.FOV = value;
    }

    public void AdjustMasterVolume(float value) {
        Options.MasterVolume = value;
    }
}
