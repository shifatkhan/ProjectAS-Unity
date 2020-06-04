using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/** Subject.
 * @Special thanks to Ryan Hipple (https://unity.com/how-to/architect-game-code-scriptable-objects)
 */
[CreateAssetMenu]
public class GameEvent : ScriptableObject
{
    private List<GameEventListener> listeners =
        new List<GameEventListener>();

    /** Signal all the listeners (observers)
     */
    public void Raise()
    {
        for (int i = listeners.Count - 1; i >= 0; i--)
            listeners[i].OnEventRaised();
    }

    public void RegisterListener(GameEventListener listener)
    { listeners.Add(listener); }

    public void UnregisterListener(GameEventListener listener)
    { listeners.Remove(listener); }
}
