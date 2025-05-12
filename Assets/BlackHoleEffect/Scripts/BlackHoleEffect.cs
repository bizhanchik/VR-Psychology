using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackHoleEffect : MonoBehaviour
{
    [Header("Основные настройки")]
    [Tooltip("Сфера, представляющая черную дыру. Должна иметь коллайдер с триггером")]
    [SerializeField] private Transform blackHoleSphere;
    
    [Tooltip("Материал для визуализации черной дыры")]
    [SerializeField] private Material blackHoleMaterial;

    [Header("Настройки FOV (поле зрения камеры)")]
    [Tooltip("Максимальное значение FOV при приближении к черной дыре")]
    [Range(30f, 180f)]
    [SerializeField] private float maxFOV = 120f;
    
    [Tooltip("Минимальное значение FOV при удалении от черной дыры")]
    [Range(1f, 60f)]
    [SerializeField] private float minFOV = 30f;
    
    [Tooltip("Скорость изменения FOV")]
    [Range(0.1f, 20f)]
    [SerializeField] private float effectSpeed = 5f;
    
    [Tooltip("Расстояние, на котором начинается изменение FOV")]
    [Range(0.1f, 10f)]
    [SerializeField] private float fovChangeDistance = 2f;

    [Header("Настройки визуальных эффектов")]
    [Tooltip("Сила искажения пространства")]
    [Range(0f, 1f)]
    [SerializeField] private float distortionAmount = 0.5f;
    
    [Tooltip("Радиус горизонта событий (внутренняя часть черной дыры)")]
    [Range(0.1f, 1f)]
    [SerializeField] private float eventHorizonRadius = 0.3f;
    
    [Tooltip("Цвет свечения по краям черной дыры")]
    [SerializeField] private Color rimColor = new Color(1f, 0.5f, 0f, 1f);
    
    [Tooltip("Сила свечения по краям")]
    [Range(0.1f, 10f)]
    [SerializeField] private float rimPower = 3f;

    [Header("Настройки гравитационного линзирования")]
    [Tooltip("Сила эффекта гравитационного линзирования")]
    [Range(0f, 2f)]
    [SerializeField] private float gravitationalLensing = 1f;
    
    [Tooltip("Эффект замедления времени")]
    [Range(0f, 1f)]
    [SerializeField] private float timeWarp = 0.5f;

    [Header("Настройки расстояния")]
    [Tooltip("Максимальное расстояние, на котором эффект активен")]
    [Range(1f, 100f)]
    [SerializeField] private float maxDistance = 10f;

    private Camera activeCamera;
    private bool isInEffect = false;
    private bool isExitingTrigger = false;
    private float originalFOV;
    private float currentFOV;
    private Material distortionMaterial;
    private float distanceToBlackHole;
    private Renderer blackHoleRenderer;
    private Vector3 lastPosition;
    private float movementProgress = 0f;
    private float exitStartFOV;

    /// <summary>
    /// Инициализация компонентов при старте
    /// </summary>
    void Start()
    {
        // Создаем материал для искажения
        distortionMaterial = new Material(Shader.Find("Hidden/Internal-DepthNormalsTexture"));
        
        // Настраиваем рендерер черной дыры
        if (blackHoleSphere != null)
        {
            blackHoleRenderer = blackHoleSphere.GetComponent<Renderer>();
            if (blackHoleRenderer != null && blackHoleMaterial != null)
            {
                blackHoleRenderer.material = blackHoleMaterial;
                UpdateMaterialProperties();
            }
        }
    }

    /// <summary>
    /// Обработка входа камеры в триггер черной дыры
    /// </summary>
    void OnTriggerEnter(Collider other)
    {
        blackHoleSphere.gameObject.SetActive(true);
        // Проверяем, является ли объект камерой
        Camera cam = other.GetComponentInChildren<Camera>();
        if (cam != null)
        {
            activeCamera = cam;
            Debug.Log("Камера найдена, FOV: " + activeCamera.fieldOfView);
            isInEffect = true;
            isExitingTrigger = false;
            originalFOV = activeCamera.fieldOfView;
            currentFOV = originalFOV;
            lastPosition = activeCamera.transform.position;
            movementProgress = 0f;
        }
    }

    /// <summary>
    /// Обработка выхода камеры из триггера черной дыры
    /// </summary>
    void OnTriggerExit(Collider other)
    {
        Camera cam = other.GetComponentInChildren<Camera>();
        if (cam != null && cam == activeCamera)
        {
            isInEffect = false;
            isExitingTrigger = true;
            exitStartFOV = currentFOV;
            movementProgress = 0f;
            lastPosition = activeCamera.transform.position;
        }
    }

    /// <summary>
    /// Основной метод обновления, вызывается каждый кадр
    /// </summary>
    void Update()
    {
        if (activeCamera != null)
        {
            if (isInEffect)
            {
                // Расширение FOV при входе в триггер
                distanceToBlackHole = Vector3.Distance(activeCamera.transform.position, transform.position);
                float distanceMoved = Vector3.Distance(activeCamera.transform.position, lastPosition);
                
                // Вычисляем прогресс движения
                movementProgress = Mathf.Clamp01(movementProgress + (distanceMoved / fovChangeDistance) * effectSpeed);
                
                // Применяем плавное изменение FOV
                float progressPower = Mathf.Pow(movementProgress, 0.5f);
                float targetFOV = Mathf.Lerp(originalFOV, maxFOV, progressPower);
                currentFOV = Mathf.Lerp(currentFOV, targetFOV, Time.deltaTime * effectSpeed * 2f);
                
                activeCamera.fieldOfView = currentFOV;
                Debug.Log($"FOV: {currentFOV:F2}, Прогресс: {movementProgress:F2}, Дистанция: {distanceToBlackHole:F2}");

                // Обновляем визуальные эффекты
                if (blackHoleRenderer != null && blackHoleMaterial != null)
                {
                    UpdateMaterialProperties();
                }

                ApplyDistortionEffect();
                lastPosition = activeCamera.transform.position;
            }
            else if (isExitingTrigger)
            {
                // Сужение FOV при выходе из триггера
                float distanceMoved = Vector3.Distance(activeCamera.transform.position, lastPosition);
                movementProgress = Mathf.Clamp01(movementProgress + (distanceMoved / fovChangeDistance) * effectSpeed);
                
                float progressPower = Mathf.Pow(movementProgress, 0.5f);
                float targetFOV = Mathf.Lerp(exitStartFOV, minFOV, progressPower);
                currentFOV = Mathf.Lerp(currentFOV, targetFOV, Time.deltaTime * effectSpeed * 2f);
                
                activeCamera.fieldOfView = currentFOV;
                Debug.Log($"Exiting - FOV: {currentFOV:F2}, Progress: {progressPower:F2}, Distance Moved: {distanceMoved:F2}");

                // Увеличиваем искажение при выходе
                if (blackHoleRenderer != null && blackHoleMaterial != null)
                {
                    blackHoleMaterial.SetFloat("_Distortion", progressPower * distortionAmount * 2f);
                }

                lastPosition = activeCamera.transform.position;
            }
        }
    }

    /// <summary>
    /// Обновление свойств материала черной дыры
    /// </summary>
    void UpdateMaterialProperties()
    {
        float normalizedDistance = Mathf.Clamp01(1 - (distanceToBlackHole / maxDistance));
        blackHoleMaterial.SetFloat("_Distortion", normalizedDistance * distortionAmount);
        blackHoleMaterial.SetFloat("_EventHorizonRadius", eventHorizonRadius);
        blackHoleMaterial.SetColor("_RimColor", rimColor);
        blackHoleMaterial.SetFloat("_RimPower", rimPower);
        blackHoleMaterial.SetFloat("_GravitationalLensing", gravitationalLensing);
        blackHoleMaterial.SetFloat("_TimeWarp", timeWarp);
    }

    /// <summary>
    /// Применение эффекта искажения к изображению
    /// </summary>
    void ApplyDistortionEffect()
    {
        if (distortionMaterial != null)
        {
            float distortion = Mathf.Lerp(0f, distortionAmount, 1 - (distanceToBlackHole / maxDistance));
            distortionMaterial.SetFloat("_Distortion", distortion);
        }
    }

    /// <summary>
    /// Обработка пост-эффектов рендеринга
    /// </summary>
    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if ((isInEffect || isExitingTrigger) && distortionMaterial != null)
        {
            Graphics.Blit(source, destination, distortionMaterial);
        }
        else
        {
            Graphics.Blit(source, destination);
        }
    }
}
