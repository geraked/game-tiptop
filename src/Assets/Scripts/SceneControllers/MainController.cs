using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainController : MonoBehaviour
{
    // PlayerPrefs constants
    public const string Prefs_BestScore_Key = "best_score";
    public const int Prefs_BestScore_DefaultValue = 0;

    public const string Prefs_ColorIndex_Key = "color_index";
    public const int Prefs_ColorIndex_DefaultValue = 1;

    public const string Prefs_Sounds_Key = "sounds";
    public const int Prefs_Sounds_DefaultValue = 1;

    void Awake()
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        Application.targetFrameRate = 60;
    }
}
