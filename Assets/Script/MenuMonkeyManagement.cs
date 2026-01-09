using UnityEngine;

public class MenuMonkeyManagement : MonoBehaviour
{
    private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();

        animator.Play("walking");

    }



}
