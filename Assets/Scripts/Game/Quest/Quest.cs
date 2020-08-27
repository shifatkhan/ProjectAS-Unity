using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/** The quest graph holding the QuestEvents (nodes) and
 * the Quest Paths (edges).
 * 
 * @author ShifatKhan
 */
public class Quest
{
    public List<QuestEvent> questEvents = new List<QuestEvent>();
    List<QuestEvent> pathList = new List<QuestEvent>();

    public Quest() { }

    /** Main function where we will create quests and add it to the quest system.
     */
    public QuestEvent AddQuestEvent(string name, string description)
    {
        QuestEvent questEvent = new QuestEvent(name, description);
        questEvents.Add(questEvent);
        return questEvent;
    }

    public void AddPath(string fromQuestEvent, string toQuestEvent)
    {
        // Check if the quests exists before adding the path.
        QuestEvent from = FindQuestEvent(fromQuestEvent);
        QuestEvent to = FindQuestEvent(toQuestEvent);

        if(from != null && to != null)
        {
            QuestPath path = new QuestPath(from, to);
            from.pathList.Add(path);
        }
    }

    QuestEvent FindQuestEvent(string id)
    {
        foreach(QuestEvent qe in questEvents)
        {
            if(qe.GetId() == id)
            {
                return qe;
            }
        }

        return null;
    }

    /** Use Breadth First Search traversal to assign correct orderNumber to each QuestEvent nodes.
     * We know a node has not been assigned a number if it's -1.
     */
    public void AssignOrderToNodesBFS(string id, int orderNumber = 1)
    {
        QuestEvent thisEvent = FindQuestEvent(id);
        thisEvent.order = orderNumber;

        foreach(QuestPath qp in thisEvent.pathList)
        {
            if(qp.endEvent.order == -1)
            {
                AssignOrderToNodesBFS(qp.endEvent.GetId(), orderNumber + 1); // Recursion.
            }
        }
    }

    /** Debug: Print all of our quests.
     */
    public void PrintPath()
    {
        Debug.Log("QuestEvents:");
        foreach (QuestEvent qe in questEvents)
        {
            Debug.Log($"\t{qe.name}, {qe.order}");
        }
    }
}
