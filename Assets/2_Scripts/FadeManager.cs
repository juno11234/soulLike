using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;


public class FadeManager : MonoBehaviour
{
    public static FadeManager Instance;
    
    [SerializeField] private float fadeDuration = 1.5f;
    [SerializeField] private float waitTime = 0.5f;

    private void Awake()
    {
        Instance = this;
    }

    public void StartFade(Action callback, Image image)
    {
        StartCoroutine(FadeSequence(callback, image));
    }

    private IEnumerator FadeSequence(Action callBack, Image image)
    {
        yield return new WaitForSeconds(waitTime);
        yield return Fade(true, image);
        yield return new WaitForSeconds(waitTime);
        callBack?.Invoke();
        yield return Fade(false, image);
        yield return new WaitForSeconds(fadeDuration);
    }

    private IEnumerator Fade(bool isout, Image image)
    {
        float timer = 0f;

        Color color = image.color;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            float t = timer / fadeDuration;

            if (isout)
            {
                color.a = Mathf.Lerp(0f, 1f, t); // 점점 어둡게
            }
            else
            {
                color.a = Mathf.Lerp(1f, 0f, t);
            }

            image.color = color;

            yield return null;
        }

        if (isout)
        {
            color.a = 1f;
        }
        else
        {
            color.a = 0f;
        }

        image.color = color;
    }
}