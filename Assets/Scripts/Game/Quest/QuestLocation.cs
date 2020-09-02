using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/** This script is attached on box Colliders placed in certain locations where
 * the player need to go to.
 * 
 * @author ShifatKhan
 */
public class QuestLocation : MonoBehaviour
{
    public QuestManager questManager;
    public QuestEvent questEvent;
    public QuestButton questButton;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag != "Player") return;

        // No need to worry about current quests.
        if (questEvent.status != EventStatus.CURRENT) return;

        questEvent.SetStatus(EventStatus.DONE);
        questButton.UpdateButton(EventStatus.DONE);
        questManager.UpdateQuestOnCompletion(questEvent);
    }

    public void Setup(QuestManager qm, QuestEvent qe, QuestButton qb)
    {
        questManager = qm;
        questEvent = qe;
        questButton = qb;

        // Link button and event.
        qe.button = questButton;
    }
}
