using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerForSubtitle : MonoBehaviour
{
    public SubtitleManager subtitleManager;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // ��������� �������� � �������
            subtitleManager.StartSubtitlesWithAudio();
            Debug.Log("Trigger activated: " + other.name);
        }
    }
}
