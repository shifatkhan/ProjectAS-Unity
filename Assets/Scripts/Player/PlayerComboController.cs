using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Player))]
public class PlayerComboController : MonoBehaviour
{
    public int maxCombo = 3; // Number of times we can click for combos.
    public int noOfClicks = 0;
    private float lastClickedTime = 0f;
    public float maxComboDelay = 0.9f;

    private Player player;
    private Animator animator;

    void Start()
    {
        player = GetComponent<Player>();
        animator = GetComponent<Animator>();
    }
    
    void Update()
    {
        if(Time.time - lastClickedTime > maxComboDelay)
        {
            // Reset combo.
            noOfClicks = 0;
        }

        if (Input.GetButtonDown("Attack1"))
        {
            lastClickedTime = Time.time;
            noOfClicks++;

            if(noOfClicks == 1)
            {
                animator.SetBool("attack1", true);
            }

            noOfClicks = Mathf.Clamp(noOfClicks, 0, maxCombo);
        }
    }

    public void return1()
    {
        if(noOfClicks >= 2)
        {
            animator.SetBool("attack2", true);
        }
        else
        {
            animator.SetBool("attack1", false);
            noOfClicks = 0;
        }
    }

    public void return2()
    {
        if (noOfClicks >= 2)
        {
            animator.SetBool("attack3", true);
        }
        else
        {
            animator.SetBool("attack2", false);
            noOfClicks = 0;
        }
    }

    public void return3()
    {
        animator.SetBool("attack1", false);
        animator.SetBool("attack2", false);
        animator.SetBool("attack3", false);
        noOfClicks = 0;
    }
}
