using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SubtitleManager : MonoBehaviour
{
    [Header("��������� ����������")]
    [Tooltip("����� ��� ���������")]
    public string[] subtitles;

    [Tooltip("������������� �����")]
    public AudioClip[] audioClips;

    [Tooltip("����� ����� ������� (�������)")]
    public float pauseBetweenClips = 0.5f;

    [Tooltip("UI-������� ��� ������ ���������")]
    public TMP_Text subtitleText;

    [Tooltip("������������� ��� ��������������� �������")]
    public AudioSource audioSource;

    [Tooltip("����� �������������� �� ��������")]
    public string warningText = "�������� �� ����";

    public float testTime = 0.3f; // ����� �������� ����� ������� ���������������
    private Coroutine subtitleCoroutine;

    public float fadeDuration = 0.3f; // ������������ fade in/out

    private void Start()
    {
        // ���������� �������������� �� ����� � �������
        subtitleText.text = warningText;
        SetTextAlpha(1f);
    }

    // ����� ������ �� ��������
    public void StartSubtitlesWithAudio()
    {
        Debug.Log("StartSubtitlesWithAudio ������");
        if (subtitleCoroutine != null)
        {
            Debug.Log("subtitleCoroutine �� null, ������������� ��������");
            StopCoroutine(subtitleCoroutine);
        }

        subtitleCoroutine = StartCoroutine(PlaySubtitles());
    }

    private IEnumerator PlaySubtitles()
    {   
        yield return new WaitForSeconds(2);
        Debug.Log("PlaySubtitles �������� ��������");
        // for (int i = 0; i < Mathf.Min(subtitles.Length, audioClips.Length); i++)
        for (int i = 0; i < subtitles.Length; i++)
        {
            Debug.Log($"���������� �������: {subtitles[i]}");
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

    // ��������������� ����� ��� �������� ��������� �����-������ ������
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

    // ��������� �����-������ ������
    private void SetTextAlpha(float alpha)
    {
        Color c = subtitleText.color;
        c.a = alpha;
        subtitleText.color = c;
    }
}
