using UnityEngine;

public class KidneyScript : MonoBehaviour
{
    public Transform kidneyObject;
    private Animator kidneyAnimator;

    private void Start()
    {
        if (kidneyObject != null)
        {
            kidneyAnimator = kidneyObject.GetComponent<Animator>();
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
