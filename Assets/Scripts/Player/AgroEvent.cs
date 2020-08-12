using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Event System/AgroEvent")]
public class AgroEvent : GameEvent
{
    public Transform target;

    public void SetTarget(Transform target)
    {
        this.target = target;
    }
}
