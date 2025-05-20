using UnityEngine;

public class KidneyScript : MonoBehaviour
{
    public Transform kidneyObject;
    public Animator kidneyAnimator;

    private void Start()
    {
        if (kidneyObject != null)
        {
            kidneyAnimator.SetBool("isNeedMove", false);
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (kidneyAnimator != null)
            {
                kidneyAnimator.SetBool("isNeedMove", true);
            }
        }
    }
    public void KidneyBay()
    {
        if (kidneyObject != null)
        {
            kidneyObject.gameObject.SetActive(false);
        }
    }
}
