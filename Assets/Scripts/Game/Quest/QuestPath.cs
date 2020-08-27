using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/** This class represents the edges of a quest graph.
 * 
 * @author ShifatKhan
 */
public class QuestPath
{
    public QuestEvent startEvent;
    public QuestEvent endEvent;

    public QuestPath(QuestEvent from, QuestEvent to)
    {
        this.startEvent = from;
        this.endEvent = to;
    }
}
