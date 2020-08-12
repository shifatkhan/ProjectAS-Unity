using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Player))]
public class PlayerCombatController : MonoBehaviour
{
    [SerializeField] private bool combatEnabled;
    [SerializeField] private float attackSpeed = 0.5f; // TODO: Implement attack cd

    // Check if player pressed attack button too early.
    private bool gotInputBuffer;
    private bool isAttacking;

    private float lastInputTime;
    [SerializeField] private float inputTimer;

    private Player player;

    private void Start()
    {
        player = GetComponent<Player>();
        player.animator.SetBool("canAttack", combatEnabled);
    }

    private void Update()
    {
        CheckCombatInput();
        CheckAttacks();
    }

    private void CheckCombatInput()
    {
        if (Input.GetButtonDown("Attack1"))
        {
            // Attempt combat
            gotInputBuffer = true;
            lastInputTime = Time.time;
        }
    }

    private void CheckAttacks()
    {
        if (gotInputBuffer)
        {
            // Perform attack1
            if (!isAttacking)
            {
                gotInputBuffer = false;
                isAttacking = true;
                player.animator.SetBool("attack1", true);
                player.animator.SetBool("attacking", isAttacking);
            }
        }

        if(Time.time >= lastInputTime + inputTimer)
        {
            // Wait for new input
            gotInputBuffer = false;
        }
    }

    private void FinishAttack1()
    {
        isAttacking = false;
        player.animator.SetBool("attacking", isAttacking);
        player.animator.SetBool("attack1", false);
    }
}
