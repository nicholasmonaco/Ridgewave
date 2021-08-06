using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Options {

    public static bool Loaded = false;
    public static bool DoingUpdate { get; private set; } = false;

    public static readonly float Def_Sensitivity = 100;
    public static readonly float Def_FOV = 60;
    public static readonly float Def_MasterVolume = 50;

    private static float _sensitivity;
    private static float _fov;
    private static float _masterVolume;


    public static float Sensitivity {
        get => _sensitivity;
        set {
            _sensitivity = value;
            DoingUpdate = true;
        }
    }

    public static float FOV {
        get => _fov;
        set {
            _fov = value;
            DoingUpdate = true;
        }
    }

    public static float MasterVolume {
        get => _masterVolume;
        set {
            _masterVolume = value;
            DoingUpdate = true;
        }
    }

    public static System.Action UpdateActions = () => { DoingUpdate = false; };


    public static void LoadSettings() {
        // Do loading
        Sensitivity = PlayerPrefs.GetFloat("Sensitivity", Def_Sensitivity);
        FOV = PlayerPrefs.GetFloat("FOV", Def_FOV);
        MasterVolume = PlayerPrefs.GetFloat("Volume_Master", Def_MasterVolume);
        //

        Loaded = true;
    }

    public static void SaveSettings() {
        PlayerPrefs.SetFloat("Sensitivity", Sensitivity);
        PlayerPrefs.SetFloat("FOV", FOV);
        PlayerPrefs.SetFloat("Volume_Master", MasterVolume);

        PlayerPrefs.Save();
    }

}
