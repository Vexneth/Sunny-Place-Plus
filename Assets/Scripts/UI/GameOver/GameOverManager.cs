using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverManager : MonoBehaviour
{
    [Header("Menu Buttons")]
    [SerializeField] private Button newGameButton;
    [SerializeField] private Button mainMenuButton;

    [Header("Panels")]
    [SerializeField] private GameObject gameOverFadePanel;
    [SerializeField] private GameObject gameLoadFadePanel;
    [SerializeField] private GameObject gameOverPopup;
    [SerializeField] private GameObject gameOverText;

    [Header("Settings")]
    [SerializeField] private float fadeTime;
    [SerializeField] private float fadeWaitTime;

    private Fade gameOverFade;
    private Fade gameLoadFade;

    void Start()
    {
        gameOverFade = gameOverFadePanel.GetComponent<Fade>();
        gameLoadFade = gameLoadFadePanel.GetComponent<Fade>();
        gameLoadFade.FadeOut(fadeTime, 1f);
        GameEventsManager.instance.OnPlayerDeath += ShowGameOverPopup;
    }

    public void OnNewGameClicked()
    {
        DisableAllButtons();
        StartCoroutine(OnNewGameClickedCoroutine(fadeTime));
    }

    public void OnMainMenuClicked()
    {
        DisableAllButtons();
        StartCoroutine(OnMainMenuClickedCoroutine(fadeTime));
    }

    private IEnumerator OnNewGameClickedCoroutine(float time)
    {
        float timePadding = 0.6f;
        HideAllMenuButtons(true);
        yield return new WaitForSeconds(0.1f);
        gameLoadFade.FadeIn(time, 1f);
        gameOverFade.FadeOut(time + timePadding - 0.1f, 0.8f);
        yield return new WaitForSeconds(time + timePadding);
        DataPersistenceManager.instance.NewGame();
        SceneManager.LoadSceneAsync("FirstLevel");
    }

    private IEnumerator OnMainMenuClickedCoroutine(float time)
    {
        float timePadding = 0.6f;
        HideAllMenuButtons(true);
        yield return new WaitForSeconds(0.1f);
        gameLoadFade.FadeIn(time, 1f);
        gameOverFade.FadeOut(time + timePadding - 0.1f, 0.8f);
        yield return new WaitForSeconds(time + timePadding);
        DataPersistenceManager.instance.SaveGame();
        SceneManager.LoadSceneAsync("MainMenu");
    }



    private void DisableAllButtons()
    {
        newGameButton.interactable = false;
        mainMenuButton.interactable = false;
    }

    private void ShowGameOverPopup()
    {
        StartCoroutine(ShowGameOverPopupCoroutine(fadeTime));
    }

    private IEnumerator ShowGameOverPopupCoroutine(float time)
    {
        yield return new WaitForSeconds(fadeWaitTime);
        gameOverFade.FadeIn(time, 0.8f);
        yield return new WaitForSeconds(time + 0.5f);
        if (gameOverPopup != null)
        {
            gameOverPopup.SetActive(true);
        }
    }

    private void HideAllMenuButtons(bool hide)
    {
        newGameButton.gameObject.SetActive(!hide);
        mainMenuButton.gameObject.SetActive(!hide);
        gameOverText.gameObject.SetActive(!hide);
    }

    void OnDestroy()
    {
        if (GameEventsManager.instance != null)
        {
            GameEventsManager.instance.OnPlayerDeath -= ShowGameOverPopup;
        }
    }
}
