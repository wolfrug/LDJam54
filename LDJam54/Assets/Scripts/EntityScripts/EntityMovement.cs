using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class EntityMovement : EntityComponent {

    public District m_currentDistrict;

    public List<EntityActionData> m_movementActions = new List<EntityActionData> { };
    private List<EntityActionData> m_remainingActions = new List<EntityActionData> { };
    private List<EntityActionData> m_usedActions = new List<EntityActionData> { };
    protected override void Init () {
        base.Init ();
        //Vector3Int currenLocation = GridManager.instance.WorldSpaceToGrid (Entity.transform.position);
        //m_currentDistrict = DistrictManager.instance.GetDistrict (currenLocation);
        m_movementActions = new List<EntityActionData> (Entity.m_data.m_entityMovementActions);
        m_remainingActions = new List<EntityActionData> (m_movementActions);
        //MoveTo (m_currentDistrict.m_gridLocation);
    }

    public bool MoveTo (Vector3Int targetLocation) {
        District targetDistrict = null;
        if (GridManager.instance.HasTile (targetLocation)) { // Tile available check
            targetDistrict = DistrictManager.instance.GetDistrict (targetLocation);
            if (DistrictManager.instance.CanMoveEntity (new MovementArgs (Entity, targetDistrict, m_currentDistrict))) {
                if (targetDistrict != null) { // Just in case, although this shouldn't be a problem
                    Vector3 targetPos = targetDistrict.MoveEntity (new MovementArgs (Entity, targetDistrict, m_currentDistrict));
                    transform.DOMove (targetPos, 1);
                    return true;
                }
            }
        }
        GlobalEvents.InvokeOnDistrictMoveFailed (new DistrictEventArgs (m_currentDistrict, targetDistrict, Entity));
        return false;
    }
    public bool MoveTo (GridLocations location) {
        if (location != GridLocations.None) {
            if (DistrictManager.instance.GetDistrict (location) != null) {
                return MoveTo (DistrictManager.instance.GetDistrict (location).m_gridPosition);
            }
        }
        return false;
    }

    public void MoveInDirection (MovementDirections direction, int squares = 1) {
        District targetDistrict = DistrictManager.instance.GetDistrictInDirection (m_currentDistrict.m_gridLocation, direction);
        if (targetDistrict != m_currentDistrict) {
            if (squares == 1) {
                MoveTo (targetDistrict.m_gridLocation);
            } else {
                Vector3 targetPos = targetDistrict.MoveEntity (new MovementArgs (Entity, targetDistrict, m_currentDistrict));
                m_currentDistrict = targetDistrict;
                transform.DOMove (targetPos, 1).OnComplete (() => MoveInDirection (direction, squares - 1)).SetEase (Ease.Linear);
            }
        }
    }

    [NaughtyAttributes.Button]
    void TestLongMovement () {
        MoveInDirection (MovementDirections.RIGHT, 3);
    }

    [NaughtyAttributes.Button]
    public void PerformRandomMovementAction (bool discardWhenDone = true) {

        EntityActionData randomAction = GetRandomMovementAction ();
        if (randomAction != null) {
            ActionResultArgs reply = randomAction.Perform (new ActionArgs (Entity, null));
            if (reply.stringVal == "Shuffle") {
                ReshuffleMovementDeck ();
                PerformRandomMovementAction (discardWhenDone);
            } else {
                if (discardWhenDone) {
                    m_remainingActions.Remove (randomAction);
                    m_usedActions.Add (randomAction);
                }
            }
            Debug.Log ("[EntityMovement] Performed action " + reply.performedAction.ID + " with result " + reply.stringVal);
        } else {
            ReshuffleMovementDeck ();
            PerformRandomMovementAction (discardWhenDone);
        }
    }
    public EntityActionData GetRandomMovementAction () {
        if (m_movementActions.Count > 0) {
            if (m_remainingActions.Count > 0) {
                EntityActionData randomAction = m_remainingActions[Random.Range (0, m_remainingActions.Count - 1)];
                if (randomAction != null) {
                    Debug.Log ("[EntityMovement] Pulled random action " + randomAction.ID);
                }
                GlobalEvents.InvokeOnMonsterMovementCardDrawn (new ActionResultArgs (new ActionArgs (), Entity, null, randomAction));
                return randomAction;
            }
        }
        return null;
    }
    public void ReshuffleMovementDeck () {
        Debug.Log ("[EntityMovement] Shuffling movement action deck!");
        m_remainingActions.Clear ();
        m_remainingActions.AddRange (m_movementActions);
    }

}