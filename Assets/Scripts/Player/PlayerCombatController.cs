using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Player))]
public class PlayerCombatController : MonoBehaviour
{
    [SerializeField] private bool combatEnabled = true;

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
        animator.SetBool("canAttack", combatEnabled);
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
                animator.SetBool("attacking", true);
            }

            noOfClicks = Mathf.Clamp(noOfClicks, 0, maxCombo);
        }
    }

    public void FinishAttack1()
    {
        animator.SetBool("attack1", false);

        if (noOfClicks >= 2)
        {
            animator.SetBool("attack2", true);
            animator.SetBool("attacking", true);
        }
        else
        {
            animator.SetBool("attacking", false);
            noOfClicks = 0;
        }
    }

    public void FinishAttack2()
    {
        animator.SetBool("attack2", false);

        if (noOfClicks >= 3)
        {
            animator.SetBool("attack3", true);
            animator.SetBool("attacking", true);
        }
        else
        {
            animator.SetBool("attacking", false);
            noOfClicks = 0;
        }
    }

    public void FinishLastAttack()
    {
        animator.SetBool("attack1", false);
        animator.SetBool("attack2", false);
        animator.SetBool("attack3", false);
        animator.SetBool("attacking", false);
        noOfClicks = 0;
    }
}
