using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class WinManager : MonoBehaviour
{

    [Header("Settings")]
    [SerializeField] private GameObject winPopup;
    [SerializeField] private TMP_Text winHighscoreText;
    [SerializeField] private GameObject gameLoadFadePanel;
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private MusicManager musicManager;
    [SerializeField] private ScoreManager scoreManager;
    [SerializeField] private float winPauseTime;
    [SerializeField] private float fadeTime;
    PlayerController player;
    private Fade gameLoadFade;
    private String winHighscoreMessage;
    private Animator _animator;

    void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            _animator.SetBool("isPlayerHere", true);
            player = collision.GetComponent<PlayerController>();
            player.PausePlayer();
            player.GetHurtWithoutAnim();
            if (scoreManager.CheckHighscore())
            {
                scoreManager.SetHighscore();
                winHighscoreMessage = "New Highscore: " + scoreManager.GetScore() + "!";
            }
            else
            {
                winHighscoreMessage = "";
            }
            StartCoroutine(WinCoroutine(winPauseTime));
        }
    }

    private IEnumerator WinCoroutine(float time)
    {
        musicManager.PlayWinMusic();
        yield return new WaitForSeconds(time);
        gameLoadFade = gameLoadFadePanel.GetComponent<Fade>();
        gameLoadFade.FadeIn(fadeTime, 0.4f);
        yield return new WaitForSeconds(fadeTime);

        winPopup.SetActive(true);
        winHighscoreText.text = winHighscoreMessage;
    }

    public void OnMainMenuClicked()
    {
        mainMenuButton.interactable = false;
        StartCoroutine(OnMainMenuClickedCoroutine(fadeTime));
    }

    private IEnumerator OnMainMenuClickedCoroutine(float time)
    {
        yield return new WaitForSeconds(0.1f);
        gameLoadFade.FadeIn(time, 1f, 0.4f);
        yield return new WaitForSeconds(time);
        DataPersistenceManager.instance.SaveGame();
        SceneManager.LoadSceneAsync("MainMenu");
    }


}
