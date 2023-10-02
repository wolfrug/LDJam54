using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityAttack : EntityComponent {
    public List<EntityActionData> m_attackActions = new List<EntityActionData> { };
    private List<EntityActionData> m_remainingActions = new List<EntityActionData> { };
    private List<EntityActionData> m_usedActions = new List<EntityActionData> { };

    private int m_attacksLeft = 1;

    protected override void Init () {
        base.Init ();
        m_attackActions = new List<EntityActionData> (Entity.m_data.m_entityAttackActions);
        m_remainingActions = new List<EntityActionData> (m_attackActions);
        GlobalEvents.OnPlayerClickAttack.AddListener (PlayerAttackInput);
    }

    public void PlayerAttackInput (MovementArgs args) {
        if (args.owner == Entity) {
            if (m_attacksLeft > 0) {
                Entity foundEntity = GetAttackableEntity (args.direction);
                if (foundEntity != null) {
                    foundEntity.AttackEntity (new ActionResultArgs (new ActionArgs (), Entity, foundEntity, null, "", 1), 0);
                }
                AttacksLeft--;
            }
        }
    }

    public bool AutoAttackRandomAdjacentEnemy () {
        if (AttacksLeft > 0) {
            foreach (MovementDirections dir in GetPermittedAttackDirections ()) {
                Entity foundEntity = GetAttackableEntity (dir);
                if (foundEntity != null) {
                    foundEntity.AttackEntity (new ActionResultArgs (new ActionArgs (), Entity, foundEntity, null, "", 1), 0);
                    AttacksLeft--;
                    return true;
                }
            }
        }
        return false;
    }

    public int AttacksLeft {
        get {
            return m_attacksLeft;
        }
        set {
            m_attacksLeft = value;
        }
    }

    public void ResetAttacksLeft () {
        m_attacksLeft = 1;
    }

    [NaughtyAttributes.Button]
    public void PerformRandomAttackAction (bool discardWhenDone = true) {
        if (m_attackActions.Count > 0) {
            if (m_remainingActions.Count > 0) {
                EntityActionData randomAction = GetRandomAttackAction ();
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

    public EntityActionData GetRandomAttackAction () {
        if (m_attackActions.Count > 0) {
            if (m_remainingActions.Count > 0) {
                EntityActionData randomAction = m_remainingActions[Random.Range (0, m_remainingActions.Count - 1)];
                if (randomAction != null) {
                    Debug.Log ("[EntityAttack] Pulled random action " + randomAction.ID);
                }
                GlobalEvents.InvokeOnMonsterAttackCardDrawn (new ActionResultArgs (new ActionArgs (), Entity, null, randomAction));
                return randomAction;
            }
        }
        return null;
    }

    public List<MovementDirections> GetPermittedAttackDirections () {
        List<MovementDirections> returnList = new List<MovementDirections> { };
        foreach (MovementDirections dir in new List<MovementDirections> { MovementDirections.LEFT, MovementDirections.RIGHT, MovementDirections.UP, MovementDirections.DOWN, MovementDirections.DIAGONAL_UP_RIGHT, MovementDirections.DIAGONAL_UP_LEFT, MovementDirections.DIAGONAL_DOWN_RIGHT, MovementDirections.DIAGONAL_DOWN_LEFT }) {
            District targetDistrict = DistrictManager.instance.GetDistrictInDirection (Entity.Location, dir, true);
            if (targetDistrict.m_gridLocation != Entity.Location) {
                if (GetAttackableEntity (targetDistrict) != null) {
                    returnList.Add (dir);
                }
            }
        }
        Debug.Log ("[EntityAttack] Received permitted attack directions: " + string.Join ("\n", returnList));
        return returnList;
    }

    public Entity GetAttackableEntity (District targetDistrict) {
        foreach (Entity targetEntity in targetDistrict.m_entitiesContained) {
            if (targetEntity.m_data.m_faction != Entity.m_data.m_faction) {
                return targetEntity;
            }
        }
        return null;
    }
    public Entity GetAttackableEntity (MovementDirections direction) {
        District targetDistrict = DistrictManager.instance.GetDistrictInDirection (Entity.Location, direction);
        if (targetDistrict.m_gridLocation != Entity.Location) {
            return GetAttackableEntity (targetDistrict);
        }
        return null;
    }

    void ReshuffleAttackDeck () {
        Debug.Log ("[EntityAttack] Shuffling attack action deck!");
        m_remainingActions.Clear ();
        m_remainingActions.AddRange (m_attackActions);
    }
}