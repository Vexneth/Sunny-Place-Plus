using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenuManager : MonoBehaviour
{
    [Header("Menu Buttons")]
    [SerializeField] private Button continueButton;
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private Button quitButton;

    [Header("Panels")]
    [SerializeField] private GameObject gameLoadFadePanel;
    [SerializeField] private GameObject pauseMenuPopup;
    private Fade gameLoadFade;


    [Header("Settings")]
    [SerializeField] private float fadeTime;
    [SerializeField] private float pauseFadeTime;

    private bool inAnimation = false;
    private bool isMenuOpen = false;

    void Start()
    {
        GameEventsManager.instance.OnPauseKeyPressed += TogglePauseMenu;
    }


    private void TogglePauseMenu()
    {
        if (!inAnimation)
        {
            if (isMenuOpen)
            {
                StartCoroutine(TogglePauseMenuCoroutine(false));
            }
            else
            {
                StartCoroutine(TogglePauseMenuCoroutine(true));
            }
        }
    }

    private IEnumerator TogglePauseMenuCoroutine(bool toggle)
    {

        inAnimation = true;
        gameLoadFade = gameLoadFadePanel.GetComponent<Fade>();
        if (toggle)
        {
            GameEventsManager.instance.GamePaused();
            EnableAllButtons();
            gameLoadFade.FadeIn(pauseFadeTime, 0.4f);
            yield return new WaitForSeconds(pauseFadeTime);
            pauseMenuPopup.SetActive(true);
            isMenuOpen = true;
        }
        else
        {
            DisableAllButtons();
            pauseMenuPopup.SetActive(false);
            gameLoadFade.FadeOut(pauseFadeTime, 0.4f);
            yield return new WaitForSeconds(pauseFadeTime);
            isMenuOpen = false;
            GameEventsManager.instance.GamePaused();
        }
        inAnimation = false;
    }


    public void OnContinueGameClicked()
    {
        DisableAllButtons();
        StartCoroutine(TogglePauseMenuCoroutine(false));
    }



    public void OnMainMenuClicked()
    {
        DisableAllButtons();
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

    public void OnQuitClicked()
    {
        DisableAllButtons();
        StartCoroutine(OnQuitClickedCoroutine(fadeTime));
    }

    private IEnumerator OnQuitClickedCoroutine(float time)
    {
        yield return new WaitForSeconds(0.1f);
        gameLoadFade.FadeIn(time / 2, 1f, 0.4f);
        yield return new WaitForSeconds(time / 2);
        Application.Quit();
    }

    private void DisableAllButtons()
    {
        continueButton.interactable = false;
        mainMenuButton.interactable = false;
        quitButton.interactable = false;
    }

    private void EnableAllButtons()
    {
        continueButton.interactable = true;
        mainMenuButton.interactable = true;
        quitButton.interactable = true;
    }

    void OnDestroy()
    {
        if (GameEventsManager.instance != null)
        {
            GameEventsManager.instance.OnPauseKeyPressed -= TogglePauseMenu;
        }
    }
}
