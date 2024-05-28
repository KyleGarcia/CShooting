using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class FadeEffect : MonoBehaviour
{
    public Image fadeImage;
    public TextMeshProUGUI gameOverText;

    public float fadeDuration = 1.0f;
    public float gameOverDelay = 1.0f;

    private bool isOverlayActive = false;

    private void Start()
    {
        // Ensure the fade image and "Game Over" text are initially inactive
        fadeImage.gameObject.SetActive(false);
        gameOverText.gameObject.SetActive(false);
    }

    public void StartOverlay()
    {
        // Enable the fade image and start the fade-in effect
        fadeImage.gameObject.SetActive(true);
        StartCoroutine(FadeIn());
    }

    IEnumerator FadeIn()
    {
        float timer = 0f;
        Color color = fadeImage.color;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            color.a = Mathf.Lerp(0f, 1f, timer / fadeDuration);
            fadeImage.color = color;
            yield return null;
        }

        // Ensure the fade image is fully opaque
        color.a = 1f;
        fadeImage.color = color;

        // Display "Game Over" text after a delay
        yield return new WaitForSeconds(gameOverDelay);
        gameOverText.gameObject.SetActive(true);
    }
}
