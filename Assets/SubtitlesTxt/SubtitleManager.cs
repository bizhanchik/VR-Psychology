using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SubtitleManager : MonoBehaviour
{
    [Header("Настройки содержания")]
    [Tooltip("Текст для субтитров")]
    public string[] subtitles;

    [Tooltip("Проигрываемые фразы")]
    public AudioClip[] audioClips;

    [Tooltip("Пауза между фразами (секунды)")]
    public float pauseBetweenClips = 0.5f;

    [Tooltip("UI-элемент для вывода субтитров")]
    public TMP_Text subtitleText;

    [Tooltip("Аудиоисточник для воспроизведения озвучки")]
    public AudioSource audioSource;

    [Tooltip("Текст предупреждения до триггера")]
    public string warningText = "Повторяй за мной";

    public float testTime = 0.3f; // Время ожидания перед началом воспроизведения
    private Coroutine subtitleCoroutine;

    public float fadeDuration = 0.3f; // Длительность fade in/out

    private void Start()
    {
        // Показываем предупреждение до входа в триггер
        subtitleText.text = warningText;
        SetTextAlpha(1f);
    }

    // Вызов метода из триггера
    public void StartSubtitlesWithAudio()
    {
        Debug.Log("StartSubtitlesWithAudio вызван");
        if (subtitleCoroutine != null)
        {
            Debug.Log("subtitleCoroutine не null, останавливаем корутину");
            StopCoroutine(subtitleCoroutine);
        }

        subtitleCoroutine = StartCoroutine(PlaySubtitles());
    }

    private IEnumerator PlaySubtitles()
    {
        Debug.Log("PlaySubtitles корутина запущена");
        // for (int i = 0; i < Mathf.Min(subtitles.Length, audioClips.Length); i++)
        for (int i = 0; i < subtitles.Length; i++)
        {
            Debug.Log($"Показываем субтитр: {subtitles[i]}");
            subtitleText.text = subtitles[i];
            yield return StartCoroutine(FadeTextAlpha(0f, 1f, fadeDuration)); // Fade in

            // audioSource.clip = audioClips[i];
            // audioSource.Play();
            // yield return new WaitForSeconds(audioClips[i].length);

            yield return new WaitForSeconds(testTime);

            yield return StartCoroutine(FadeTextAlpha(1f, 0f, fadeDuration)); // Fade out
            subtitleText.text = "";
            yield return new WaitForSeconds(pauseBetweenClips);
        }
    }

    // Вспомогательный метод для плавного изменения альфа-канала текста
    private IEnumerator FadeTextAlpha(float from, float to, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            float alpha = Mathf.Lerp(from, to, elapsed / duration);
            SetTextAlpha(alpha);
            elapsed += Time.deltaTime;
            yield return null;
        }
        SetTextAlpha(to);
    }

    // Установка альфа-канала текста
    private void SetTextAlpha(float alpha)
    {
        Color c = subtitleText.color;
        c.a = alpha;
        subtitleText.color = c;
    }
}
