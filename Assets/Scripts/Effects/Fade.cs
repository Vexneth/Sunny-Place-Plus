using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Fade : MonoBehaviour
{
    private Image _image;

    void Start()
    {
        _image = GetComponent<Image>();

    }

    public void FadeIn(float time, float alphaEnd)
    {
        StartCoroutine(FadeCoroutine(0f, alphaEnd, time));
    }
    public void FadeIn(float time, float alphaEnd, float alphaStart)
    {
        StartCoroutine(FadeCoroutine(alphaStart, alphaEnd, time));
    }

    public void FadeOut(float time, float alphaStart)
    {
        StartCoroutine(FadeCoroutine(alphaStart, 0f, time));
    }

    private IEnumerator FadeCoroutine(float startAlpha, float alphaEnd, float time)
    {
        float anlikZaman = 0f;
        Color olacakRenk = _image.color;


        while (anlikZaman < time)
        {
            anlikZaman += Time.deltaTime;
            float t = anlikZaman / time;

            olacakRenk.a = Mathf.Lerp(startAlpha, alphaEnd, t);
            _image.color = olacakRenk;

            yield return null;
        }

        olacakRenk.a = alphaEnd;
        _image.color = olacakRenk;
    }

}
