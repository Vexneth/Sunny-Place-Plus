using System.Collections;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    [SerializeField] AudioSource backgroundMusic;
    [SerializeField] AudioSource deathMusic;
    [SerializeField] AudioSource deathBackgroundMusic;
    [SerializeField] AudioSource WinEffect;
    [SerializeField] AudioSource WinBackgroundMusic;


    private PlayerController playerController;
    private bool isBGStopped = false;


    void Start()
    {
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        backgroundMusic.Play();
    }

    void Update()
    {
        if (!isBGStopped && playerController.IsPlayerHurt())
        {
            backgroundMusic.Stop();
            isBGStopped = true;
            StartCoroutine(DeathMusicCoroutine());
        }
    }

    private IEnumerator DeathMusicCoroutine()
    {
        deathMusic.Play();
        yield return new WaitForSeconds(1f);
        deathMusic.Stop();
        yield return new WaitForSeconds(0.5f);
        deathBackgroundMusic.Play();
    }

    public void PlayWinMusic()
    {
        backgroundMusic.Stop();
        isBGStopped = true;
        StartCoroutine(WinMusicCoroutine());
    }

    private IEnumerator WinMusicCoroutine()
    {
        WinEffect.Play();
        yield return new WaitForSeconds(1.5f);
        WinBackgroundMusic.Play();
        yield return new WaitForSeconds(1f);
        WinEffect.Stop();

    }
}
