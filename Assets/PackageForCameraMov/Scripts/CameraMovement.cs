using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CameraMovement : MonoBehaviour
{
    [Header("Настройки движения")]
    [Tooltip("Точки пути для движения камеры")]
    public Transform[] waypoints;
    
    [Tooltip("Точки, куда будет смотреть камера в каждой позиции")]
    public Transform[] lookAtPoints;
    
    [Tooltip("Скорость движения между точками")]
    public float moveSpeed = 1f;
    
    [Tooltip("Скорость поворота камеры")]
    public float rotationSpeed = 1.5f;
    
    [Tooltip("Время ожидания в каждой точке (в секундах)")]
    public float waitTime = 0.5f;
    
    [Tooltip("Зациклить движение")]
    public bool loopPath = false;
    
    [Tooltip("Плавность движения (0-1)")]
    [Range(0, 1)]
    public float smoothness = 0.7f;

    [Tooltip("Плавность поворота (0-1)")]
    [Range(0, 1)]
    public float rotationSmoothness = 0.75f;

    [Tooltip("Максимальная скорость движения камеры")]
    public float maxMoveSpeed = 0.5f;

    [Tooltip("Максимальная скоростьв вращения камеры")]
    public float maxRotationSpeed = 0.3f;

    private int currentWaypointIndex = 0;
    private bool isMoving = true;
    private Coroutine moveCoroutine;
    private Vector3 currentVelocity;
    private Vector3 currentRotationVelocity;


    // Start is called before the first frame update
    void Start()
    {
        if (waypoints.Length > 0)
        {
            StartCoroutine(FollowPath());
        }
        else
        {
            Debug.LogWarning("Не заданы точки пути для движения камеры!");
        }
    }

    IEnumerator FollowPath()
    {
        while (true)
        {
            if (waypoints.Length == 0) yield break;

            Transform targetWaypoint = waypoints[currentWaypointIndex];
            Transform currentLookAt = null;
            Transform nextLookAt = null;
            
            // Определяем точки взгляда для текущего и следующего сегмента пути
            if (lookAtPoints.Length > currentWaypointIndex)
            {
                currentLookAt = lookAtPoints[currentWaypointIndex];
            }
            
            // Определяем следующую точку взгляда, если она существует
            int nextWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
            if (lookAtPoints.Length > nextWaypointIndex)
            {
                nextLookAt = lookAtPoints[nextWaypointIndex];
            }

            if (waitTime > 0)
            {
                // Режим с ожиданием в точках
                while (Vector3.Distance(transform.position, targetWaypoint.position) > 0.01f)
                {
                    if (isMoving)
                    {
                        // Плавное движение к точке с использованием SmoothDamp
                        transform.position = Vector3.SmoothDamp(
                            transform.position,
                            targetWaypoint.position,
                            ref currentVelocity,
                            moveSpeed * (1 - smoothness),
                            maxMoveSpeed
                        );

                        // Если есть точка для взгляда, плавно поворачиваем камеру к ней
                        if (currentLookAt != null)
                        {
                            Vector3 lookDirection = (currentLookAt.position - transform.position).normalized;
                            if (lookDirection != Vector3.zero)
                            {
                                Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
                                transform.rotation = Quaternion.Slerp(
                                    transform.rotation,
                                    targetRotation,
                                    rotationSpeed * Time.deltaTime * (1 - rotationSmoothness)
                                );
                            }
                        }
                    }
                    yield return null;
                }

                // Плавное замедление перед остановкой
                float slowdownTime = 0.5f;
                float elapsedTime = 0f;
                while (elapsedTime < slowdownTime)
                {
                    transform.position = Vector3.SmoothDamp(
                        transform.position,
                        targetWaypoint.position,
                        ref currentVelocity,
                        moveSpeed * (1 - smoothness),
                        maxMoveSpeed
                    );
                    elapsedTime += Time.deltaTime;
                    yield return null;
                }

                // Ждем указанное время в точке
                yield return new WaitForSeconds(waitTime);
            }
            else
            {
                // Режим без ожидания - непрерывное движение
                Vector3 startPosition = transform.position;
                Quaternion startRotation = transform.rotation;
                float startTime = Time.time;
                float journeyLength = Vector3.Distance(startPosition, targetWaypoint.position);
                
                while (Vector3.Distance(transform.position, targetWaypoint.position) > 0.01f)
                {
                    if (isMoving)
                    {
                        // Плавное движение к точке
                        float distCovered = (Time.time - startTime) * moveSpeed * 0.5f;
                        float fractionOfJourney = Mathf.Clamp01(distCovered / journeyLength);
                        
                        transform.position = Vector3.Lerp(
                            startPosition,
                            targetWaypoint.position,
                            fractionOfJourney
                        );

                        // Если есть точка для взгляда, плавно поворачиваем камеру к ней
                        if (currentLookAt != null)
                        {
                            Vector3 lookDirection = (currentLookAt.position - transform.position).normalized;
                            if (lookDirection != Vector3.zero)
                            {
                                Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
                                
                                // Рассчитываем угол между текущим и целевым поворотом
                                float angle = Quaternion.Angle(transform.rotation, targetRotation);
                                
                                // Определяем скорость поворота в зависимости от пройденного пути
                                float rotationProgress = Mathf.Clamp01(fractionOfJourney * 2f); // Ускоряем начало поворота
                                float currentRotationSpeed = rotationSpeed * (1 - rotationSmoothness) * rotationProgress;
                                
                                // Плавный поворот с учетом пройденного пути
                                transform.rotation = Quaternion.Slerp(
                                    transform.rotation,
                                    targetRotation,
                                    currentRotationSpeed * Time.deltaTime
                                );
                            }
                        }
                    }
                    yield return null;
                }
            }

            // Переходим к следующей точке
            if (currentWaypointIndex < waypoints.Length - 1)
            {
                currentWaypointIndex++;
            }
            else if (loopPath)
            {
                currentWaypointIndex = 0;
            }
            else
            {
                yield break;
            }
        }
    }

    public void PauseMovement()
    {
        isMoving = false;
    }

    public void ResumeMovement()
    {
        isMoving = true;
    }

    public void RestartPath()
    {
        currentWaypointIndex = 0;
        if (moveCoroutine != null)
        {
            StopCoroutine(moveCoroutine);
        }
        moveCoroutine = StartCoroutine(FollowPath());
    }
    private void LateUpdate()
    {
        Vector3 euler = transform.eulerAngles;
        euler.z = 0f;
        transform.rotation = Quaternion.Euler(euler);
    }

}
