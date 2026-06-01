using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [Header("Menu Buttons")]
    [SerializeField] private Button newGameButton;
    [SerializeField] private Button continueGameButton;
    [SerializeField] private Button controlsButton;
    [SerializeField] private Button quitButton;
    [SerializeField] private TMP_Text highScoreText;

    [Header("Panels")]
    [SerializeField] private GameObject menuFadePanel;
    [SerializeField] private GameObject controlsPopup;

    [Header("Settings")]
    [SerializeField] private float fadeTime;

    private String levelName;

    void Start()
    {
        if (!DataPersistenceManager.instance.HasGameData() || DataPersistenceManager.instance.WasPlayerDead())
        {
            continueGameButton.interactable = false;
        }
        else
        {
            levelName = DataPersistenceManager.instance.GetLevelName();
        }
        var hscr = PlayerPrefs.GetInt("Highscore");
        highScoreText.text = "Highest Score: " + hscr;

        
    }

    public void OnNewGameClicked()
    {
        DisableAllButtons();
        StartCoroutine(OnNewGameClickedCoroutine(fadeTime));
    }
    public void OnContinueGameClicked()
    {
        DisableAllButtons();
        StartCoroutine(OnContinueGameClickedCoroutine(fadeTime));
    }

    public void OnControlsClicked()
    {
        HideAllMenuButtons(true);
        controlsPopup.SetActive(true);

    }

    public void OnControlsContinueClicked()
    {
        HideAllMenuButtons(false);
        controlsPopup.SetActive(false);
    }

    public void OnQuitClicked()
    {
        DisableAllButtons();
        StartCoroutine(OnQuitClickedCoroutine(fadeTime));
    }

    private IEnumerator OnQuitClickedCoroutine(float time)
    {
        menuFadePanel.SetActive(true);
        var menuFade = menuFadePanel.GetComponent<Fade>();
        yield return new WaitForSeconds(0.1f);
        menuFade.FadeIn(time / 2, 1f);
        yield return new WaitForSeconds(time);
        Application.Quit();
    }

    private IEnumerator OnContinueGameClickedCoroutine(float time)
    {
        menuFadePanel.SetActive(true);
        var menuFade = menuFadePanel.GetComponent<Fade>();
        yield return new WaitForSeconds(0.1f);
        menuFade.FadeIn(time, 1f);
        yield return new WaitForSeconds(time);
        DataPersistenceManager.instance.SaveGame();
        SceneManager.LoadSceneAsync(levelName);
    }

    private IEnumerator OnNewGameClickedCoroutine(float time)
    {
        menuFadePanel.SetActive(true);
        var menuFade = menuFadePanel.GetComponent<Fade>();
        yield return new WaitForSeconds(0.1f);
        menuFade.FadeIn(time, 1f);
        yield return new WaitForSeconds(time);
        DataPersistenceManager.instance.NewGame();
        SceneManager.LoadSceneAsync("FirstLevel");
    }

    private void DisableAllButtons()
    {
        newGameButton.interactable = false;
        continueGameButton.interactable = false;
        controlsButton.interactable = false;
        quitButton.interactable = false;
    }

    private void HideAllMenuButtons(bool hide)
    {
        newGameButton.gameObject.SetActive(!hide);
        continueGameButton.gameObject.SetActive(!hide);
        controlsButton.gameObject.SetActive(!hide);
        quitButton.gameObject.SetActive(!hide);
        highScoreText.gameObject.SetActive(!hide);
    }



}
