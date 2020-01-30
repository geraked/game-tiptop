using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SoundButton : MonoBehaviour
{
    public AudioSource[] Audios;

    private Button btn;
    private GameObject onImage, offImage;
    private bool sounds;

    void Awake()
    {
        if (PlayerPrefs.GetInt(MainController.Prefs_Sounds_Key, MainController.Prefs_Sounds_DefaultValue) == 0)
        {
            sounds = false;
        }
        else
        {
            sounds = true;
        }
        btn = GetComponent<Button>();
        btn.onClick.AddListener(SoundToogle);
        onImage = btn.transform.GetChild(0).gameObject;
        offImage = btn.transform.GetChild(1).gameObject;
    }

    void Start()
    {
        SetAudiosPauseStatus(!sounds);
        if (sounds)
        {
            offImage.SetActive(false);
            onImage.SetActive(true);
        }
        else
        {
            onImage.SetActive(false);
            offImage.SetActive(true);
        }
    }

    void SoundToogle()
    {
        if (sounds)
        {
            sounds = false;
            SetAudiosPauseStatus(!sounds);
            onImage.SetActive(false);
            offImage.SetActive(true);
            PlayerPrefs.SetInt(MainController.Prefs_Sounds_Key, 0);
        }
        else
        {
            sounds = true;
            if (!GameController.IsGameOver && !GameController.IsGamePaused)
            {
                SetAudiosPauseStatus(!sounds);
            }
            offImage.SetActive(false);
            onImage.SetActive(true);
            PlayerPrefs.SetInt(MainController.Prefs_Sounds_Key, 1);
        }
        PlayerPrefs.Save();
    }

    void SetAudiosPauseStatus(bool status)
    {
        foreach (var item in Audios)
        {
            if (status)
            {
                item.Pause();
            }
            else
            {
                item.UnPause();
            }
        }
    }
}
