using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneControler : MonoBehaviour
{
    public int sceneOffset = 1; // Насколько продвигаемся вперёд по buildIndex
    public Image fadeImage; // Ссылка на UI Image для эффекта затухания
    public float fadeDuration = 1f; // Длительность эффекта затухания

    private void Start()
    {
        // Начинаем с полностью прозрачного экрана
        if (fadeImage != null)
        {
            fadeImage.color = new Color(0, 0, 0, 0);
            StartCoroutine(FadeIn());
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Проверим, что объект игрока входит в триггер (например, по тэгу)
        if (other.CompareTag("Player"))
        {
            StartCoroutine(ChangeSceneWithFade());
        }
    }

    private IEnumerator ChangeSceneWithFade()
    {
        // Затухание экрана
        yield return StartCoroutine(FadeOut());

        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = currentSceneIndex + sceneOffset;

        // Убедимся, что следующая сцена существует 
        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(nextSceneIndex);
            Debug.Log("Scene changed");
        }
        else
        {
            Debug.LogWarning("Нет следующей сцены с таким индексом в Build Settings.");
        }
    }

    private IEnumerator FadeOut()
    {
        float elapsedTime = 0f;
        Color startColor = new Color(0, 0, 0, 0);
        Color endColor = new Color(0, 0, 0, 1);

        while (elapsedTime < fadeDuration)
        {
            fadeImage.color = Color.Lerp(startColor, endColor, elapsedTime / fadeDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        fadeImage.color = endColor;
    }

    private IEnumerator FadeIn()
    {
        float elapsedTime = 0f;
        Color startColor = new Color(0, 0, 0, 1);
        Color endColor = new Color(0, 0, 0, 0);

        while (elapsedTime < fadeDuration)
        {
            fadeImage.color = Color.Lerp(startColor, endColor, elapsedTime / fadeDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        fadeImage.color = endColor;
    }
}

