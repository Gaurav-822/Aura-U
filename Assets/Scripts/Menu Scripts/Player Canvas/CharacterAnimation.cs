using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimation : MonoBehaviour
{
    private Animator animator;

    // Start is called before the first frame update
    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void idleCharacter()
    {
        animator.SetBool("isMoving", false);
    }

    public void moveCharacter()
    {
        animator.SetBool("isMoving", true);
    }
}
