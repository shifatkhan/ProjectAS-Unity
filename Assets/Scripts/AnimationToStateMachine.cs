using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/** This class will be the bridge between animations and the state machine.
 * Since the animation events can't call the state machine's functions directly, 
 * this class will be a proxy for it.
 * 
 * TODO: Move this into EntityAI.cs when it is created.
 */
public class AnimationToStateMachine : MonoBehaviour
{
    public AttackState attackState;

    private void TriggerAttack()
    {
        attackState.TriggerAttack();
    }

    private void FinishAttack()
    {
        attackState.FinishAttack();
    }
}
