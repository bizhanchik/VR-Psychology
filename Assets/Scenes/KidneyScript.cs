using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KidneyScript : MonoBehaviour
{
    public Transform kidneyObject;

    public void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            kidneyObject.gameObject.SetActive(false);
        }
    }
}
