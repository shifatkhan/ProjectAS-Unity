using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/** 
 * @author ShifatKhan
 */
public class QuestManager : MonoBehaviour
{
    public Quest quest;

    void Start()
    {
        quest = new Quest();

        // TEST. Create each event.
        QuestEvent a = quest.AddQuestEvent("test1", "description 1");
        QuestEvent b = quest.AddQuestEvent("test2", "description 2");
        QuestEvent c = quest.AddQuestEvent("test3", "description 3");
        QuestEvent d = quest.AddQuestEvent("test4", "description 4");
        QuestEvent e = quest.AddQuestEvent("test5", "description 5");

        // TEST. Define paths between events (the order they must be completed)
        quest.AddPath(a.GetId(), b.GetId());
        quest.AddPath(b.GetId(), c.GetId());
        quest.AddPath(b.GetId(), d.GetId());
        quest.AddPath(c.GetId(), e.GetId());
        quest.AddPath(d.GetId(), e.GetId());

        quest.AssignOrderToNodesBFS(a.GetId());

        quest.PrintPath();
    }
}
