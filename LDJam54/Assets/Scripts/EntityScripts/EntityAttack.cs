using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityAttack : EntityComponent {
    public List<EntityActionData> m_attackActions = new List<EntityActionData> { };
    private List<EntityActionData> m_remainingActions = new List<EntityActionData> { };
    private List<EntityActionData> m_usedActions = new List<EntityActionData> { };

    protected override void Init () {
        base.Init ();
        m_attackActions = new List<EntityActionData> (Entity.m_data.m_entityAttackActions);
        m_remainingActions = new List<EntityActionData> (m_attackActions);
    }

    [NaughtyAttributes.Button]
    public void PerformRandomAttackAction (bool discardWhenDone = true) {
        if (m_attackActions.Count > 0) {
            if (m_remainingActions.Count > 0) {
                EntityActionData randomAction = m_remainingActions[Random.Range (0, m_remainingActions.Count - 1)];
                ActionResultArgs reply = randomAction.Perform (new ActionArgs (Entity, null));
                if (reply.stringVal == "Shuffle") {
                    ReshuffleAttackDeck ();
                    PerformRandomAttackAction (discardWhenDone);
                } else {
                    if (discardWhenDone) {
                        m_remainingActions.Remove (randomAction);
                        m_usedActions.Add (randomAction);
                    }
                }
                Debug.Log ("[EntityAttack] Performed action " + reply.performedAction.ID + " with result " + reply.stringVal);
            } else {
                ReshuffleAttackDeck ();
                PerformRandomAttackAction (discardWhenDone);
            }
        }
    }
    void ReshuffleAttackDeck () {
        Debug.Log ("[EntityAttack] Shuffling attack action deck!");
        m_remainingActions.Clear ();
        m_remainingActions.AddRange (m_attackActions);
    }
}