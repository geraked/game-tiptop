using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public static bool IsGameOver;
    public static bool IsGamePaused;

    public GameObject PlayingUI;
    public GameObject PauseMenuUI;
    public GameObject GameOverUI;
    public Text PlayingScoreText;
    public Text GameOverScoreText;
    public Text GameOverBestScoreText;

    private int score;
    private int bestScore;
    private static float playingBackgroungMusicTime;
    private AudioSource playingBackgroungMusic;

    public int Score
    {
        get { return score; }
        set
        {
            if (value >= 0)
            {
                score = value;
            }
            else
            {
                score = 0;
            }
            PlayingScoreText.text = "Score: " + score;
        }
    }

    public int BestScore
    {
        get { return bestScore; }
        set
        {
            if (value >= 0)
            {
                bestScore = value;
            }
            else
            {
                bestScore = 0;
            }
            GameOverBestScoreText.text = "Best Score: " + bestScore;
        }
    }

    void Awake()
    {
        IsGameOver = false;
        IsGamePaused = false;
        playingBackgroungMusic = GetComponent<AudioSource>();
        playingBackgroungMusic.time = playingBackgroungMusicTime;
        Score = 0;
        BestScore = PlayerPrefs.GetInt(MainController.Prefs_BestScore_Key, MainController.Prefs_BestScore_DefaultValue);
        PlayingUI.SetActive(true);
        GameOverUI.SetActive(false);
        PauseMenuUI.SetActive(false);
    }

    void Start()
    {
        Time.timeScale = 1;
        TapsellStandardBanner.Hide();
        InvokeRepeating("addScore", 1f, 1f);
    }

    void Update()
    {
        if (!IsGameOver && Input.GetKeyDown(KeyCode.Escape))
        {
            PauseMenuToggle();
        }
    }

    private void addScore()
    {
        Score++;
    }

    public void GameOver()
    {
        IsGameOver = true;
        Time.timeScale = 0;
        TapsellStandardBanner.Show();
        CancelInvoke("AddScore");
        if (Score > BestScore)
        {
            BestScore = Score;
            PlayerPrefs.SetInt(MainController.Prefs_BestScore_Key, BestScore);
        }
        ColorEffect.ColorIndex++;
        PlayerPrefs.SetInt(MainController.Prefs_ColorIndex_Key, ColorEffect.ColorIndex);
        ColorEffect.ColorIndex--;
        PlayingUI.SetActive(false);
        GameOverScoreText.text = "SCORE\n" + score;
        GameOverBestScoreText.text = "BEST SCORE\n" + bestScore;
        GameOverUI.SetActive(true);
        playingBackgroungMusic.Pause();
        playingBackgroungMusicTime = playingBackgroungMusic.time;
        PlayerPrefs.Save();
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void MainMenu()
    {
        SceneManager.LoadScene(1);
        IsGameOver = false;
        IsGamePaused = false;
    }

    public void PauseMenuToggle()
    {
        if (!PauseMenuUI.activeSelf)
        {
            IsGamePaused = true;
            Time.timeScale = 0;
            playingBackgroungMusic.Pause();
            PauseMenuUI.SetActive(true);
        }
        else
        {
            IsGamePaused = false;
            PauseMenuUI.SetActive(false);
            if (PlayerPrefs.GetInt("sounds", 1) == 1)
            {
                playingBackgroungMusic.UnPause();
            }
            Time.timeScale = 1;
        }
    }
}
