using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO: Change name to QuestStatus
public enum EventStatus { WAITING, CURRENT, DONE }
// WAITING = quest not yet active (maybe there's a prerequisite event). Can't be worked on.
// CURRENT = quest is active, player should try and achieve this.
// DONE = has been achieved.

/** Nodes of a quest graph.
 * 
 * @author ShifatKhan
 */
public class QuestEvent
{
    [Header("Info")]
    public string name;
    [TextArea]
    public string description;
    public string id;
    public int order = -1; // Specifies order in the quest graph.
    public EventStatus status;
    public QuestButton button;

    [Header("Path")]
    public List<QuestPath> pathList = new List<QuestPath>();

    public QuestEvent(string name, string description)
    {
        this.name = name;
        this.description = description;
        id = Guid.NewGuid().ToString(); // Create a unique id.
        status = EventStatus.WAITING;
    }

    public void SetStatus(EventStatus status)
    {
        this.status = status;
        button.UpdateButton(status);
    }

    public string GetId()
    {
        return id;
    }
}
