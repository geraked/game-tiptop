using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ColorEffect : MonoBehaviour
{
    public float ChangeDelay;
    public float ChangeDuration;

    public static readonly Color[][] ThemeColors =
    {
        new Color[] {new Color(0.956f, 0.262f, 0.211f), new Color(1f, 0.921f, 0.933f)},     // 1
        new Color[] {new Color(0.913f, 0.117f, 0.388f), new Color(0.988f, 0.894f, 0.925f)}, // 2
        new Color[] {new Color(0.611f, 0.152f, 0.690f), new Color(0.952f, 0.898f, 0.960f)}, // 3
        new Color[] {new Color(0.403f, 0.227f, 0.717f), new Color(0.929f, 0.905f, 0.964f)}, // 4
        new Color[] {new Color(0.247f, 0.317f, 0.709f), new Color(0.909f, 0.917f, 0.964f)}, // 5
        new Color[] {new Color(0.129f, 0.588f, 0.952f), new Color(0.890f, 0.949f, 0.992f)}, // 6
        new Color[] {new Color(0.011f, 0.662f, 0.956f), new Color(0.882f, 0.960f, 0.996f)}, // 7
        new Color[] {new Color(0f, 0.737f, 0.831f), new Color(0.878f, 0.968f, 0.980f)},     // 8
        new Color[] {new Color(0f, 0.588f, 0.533f), new Color(0.878f, 0.949f, 0.945f)},     // 9
        new Color[] {new Color(0.298f, 0.686f, 0.313f), new Color(0.909f, 0.960f, 0.913f)}, // 10
        new Color[] {new Color(0.545f, 0.764f, 0.290f), new Color(0.945f, 0.972f, 0.913f)}, // 11
        new Color[] {new Color(0.803f, 0.862f, 0.223f), new Color(0.976f, 0.984f, 0.905f)}, // 12
        new Color[] {new Color(1f, 0.921f, 0.231f), new Color(1f, 0.992f, 0.905f)},         // 13
        new Color[] {new Color(1f, 0.756f, 0.027f), new Color(1f, 0.972f, 0.882f)},         // 14
        new Color[] {new Color(1f, 0.596f, 0f), new Color(1f, 0.952f, 0.878f)},             // 15
        new Color[] {new Color(1f, 0.341f, 0.133f), new Color(0.984f, 0.913f, 0.905f)},     // 16
        new Color[] {new Color(0.474f, 0.333f, 0.282f), new Color(0.937f, 0.921f, 0.913f)}, // 17
        new Color[] {new Color(0.619f, 0.619f, 0.619f), new Color(0.980f, 0.980f, 0.980f)}, // 18
        new Color[] {new Color(0.376f, 0.490f, 0.545f), new Color(0.925f, 0.937f, 0.945f)}, // 19
    };
    private static int colorIndex;

    public static int ColorIndex
    {
        get
        {
            return colorIndex;
        }
        set
        {
            if (value >= ThemeColors.Length || value < 0)
            {
                colorIndex = 0;
            }
            else
            {
                colorIndex = value;
            }
        }
    }

    void Awake()
    {
        colorIndex = PlayerPrefs.GetInt(MainController.Prefs_ColorIndex_Key, MainController.Prefs_ColorIndex_DefaultValue);
        Camera.main.backgroundColor = ThemeColors[colorIndex][1];
        ObstacleColor.BaseColor = ThemeColors[colorIndex][0];
    }

    void Start()
    {
        if (SceneManager.GetActiveScene().name == "Play")
        {
            InvokeRepeating("cahngeColorIndex", ChangeDelay, ChangeDelay);
        }
    }

    void Update()
    {
        if (!ObstacleColor.BaseColor.Equals(ThemeColors[ColorIndex][0]))
        {
            ObstacleColor.BaseColor = Color.Lerp(ObstacleColor.BaseColor, ThemeColors[ColorIndex][0], ChangeDuration);
        }
    }

    void LateUpdate()
    {
        if (!Camera.main.backgroundColor.Equals(ThemeColors[ColorIndex][1]))
        {
            Camera.main.backgroundColor = Color.Lerp(Camera.main.backgroundColor, ThemeColors[ColorIndex][1], ChangeDuration);
        }
    }

    private void cahngeColorIndex()
    {
        ColorIndex++;
    }
}
