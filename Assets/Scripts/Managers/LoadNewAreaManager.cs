using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadNewAreaManager : MonoBehaviour
{
    [Header("Settings:")]
    [SerializeField] String newLevelName;
    [SerializeField] private GameObject gameLoadFadePanel;
    [SerializeField] private float fadeTime;

    PlayerController player;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            player = collision.GetComponent<PlayerController>();
            player.ChangeLevel(newLevelName);

            StartCoroutine(OnNewLevelLoadCoroutine(fadeTime));
        }
    }

    private IEnumerator OnNewLevelLoadCoroutine(float time)
    {
        player.PausePlayer();
        var levelFade = gameLoadFadePanel.GetComponent<Fade>();
        yield return new WaitForSeconds(0.1f);
        levelFade.FadeIn(time, 1f);
        yield return new WaitForSeconds(time);
        DataPersistenceManager.instance.SaveGame();
        player.PausePlayer();
        SceneManager.LoadSceneAsync(newLevelName);
    }
}
