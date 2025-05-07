using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneControler : MonoBehaviour
{
    public int sceneOffset = 1; // Насколько продвигаемся вперёд по buildIndex

    private void OnTriggerEnter(Collider other)
    {
        // Проверим, что объект игрока входит в триггер (например, по тэгу)
        if (other.CompareTag("Player"))
        {
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
    }
}

