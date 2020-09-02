using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/** 
 * @author ShifatKhan
 */
public class QuestManager : MonoBehaviour
{
    public Quest quest;

    public GameObject questScrollView;
    private RectTransform scrollTransform;
    public GameObject buttonPrefab;

    QuestEvent final; // Test for quest completion.

    public GameObject A;
    public GameObject B;
    public GameObject C;
    public GameObject D;
    public GameObject E;

    private void Awake()
    {
        scrollTransform = questScrollView.GetComponent<RectTransform>();
    }

    void Start()
    {
        quest = new Quest();

        // TEST. Create each event.
        QuestEvent a = quest.AddQuestEvent("test1", "description 1");
        QuestEvent b = quest.AddQuestEvent("test2", "description 2");
        QuestEvent c = quest.AddQuestEvent("test3", "description 3");
        QuestEvent d = quest.AddQuestEvent("test4", "description 4");
        QuestEvent e = quest.AddQuestEvent("test5", "description 5");

        //QuestEvent f = quest.AddQuestEvent("test6", "description 6");
        //scrollTransform.sizeDelta = new Vector2(scrollTransform.sizeDelta.x, scrollTransform.sizeDelta.y + 50);

        // TEST. Define paths between events (the order they must be completed)
        quest.AddPath(a.GetId(), b.GetId());
        quest.AddPath(b.GetId(), c.GetId());
        quest.AddPath(b.GetId(), d.GetId());
        quest.AddPath(c.GetId(), e.GetId());
        quest.AddPath(d.GetId(), e.GetId());
        //quest.AddPath(e.GetId(), f.GetId());

        quest.AssignOrderToNodesBFS(a.GetId());

        QuestButton button = CreateButton(a).GetComponent<QuestButton>();
        A.GetComponent<QuestLocation>().Setup(this, a, button);

        button = CreateButton(b).GetComponent<QuestButton>();
        B.GetComponent<QuestLocation>().Setup(this, b, button);

        button = CreateButton(c).GetComponent<QuestButton>();
        C.GetComponent<QuestLocation>().Setup(this, c, button);

        button = CreateButton(d).GetComponent<QuestButton>();
        D.GetComponent<QuestLocation>().Setup(this, d, button);

        button = CreateButton(e).GetComponent<QuestButton>();
        E.GetComponent<QuestLocation>().Setup(this, e, button);

        //button = CreateButton(f).GetComponent<QuestButton>();
        //A.GetComponent<QuestLocation>().Setup(this, a, button);

        final = e;

        quest.PrintPath();
    }

    GameObject CreateButton(QuestEvent e)
    {
        GameObject b = Instantiate(buttonPrefab);
        b.GetComponent<QuestButton>().Setup(e, questScrollView);

        if (e.order == 1)
        {
            b.GetComponent<QuestButton>().UpdateButton(EventStatus.CURRENT);
            e.status = EventStatus.CURRENT;
        }

        return b;
    }

    internal void UpdateQuestOnCompletion(QuestEvent questEvent)
    {
        // Check if quest is completed.
        if(questEvent == final)
        {
            Debug.Log("QUEST COMPLETED");
            return;
        }

        foreach(QuestEvent n in quest.questEvents)
        {
            // Check if event is next
            if(n.order == (questEvent.order + 1))
            {
                // Make next available for completion.
                n.SetStatus(EventStatus.CURRENT);
            }
        }
    }

    // TODO: Add function to add quests.
}
