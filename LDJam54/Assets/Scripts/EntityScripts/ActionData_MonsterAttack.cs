using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (fileName = "Data", menuName = "Actions/Monster Attack Data", order = 1)]
public class ActionData_MonsterAttack : EntityActionData {

    public List<MovementDirections> m_attackDirections = new List<MovementDirections> { };

    [Tooltip ("Attempts to wreck the district if set to true")]
    public bool m_attackDistrict = false;
    [Tooltip ("Attacks any entities in the district if set to true")]
    public List<EntityType> m_entityTypesTargeted = new List<EntityType> { };
    public int m_knockBackSquares = 0;
    public bool m_shuffleAttackDeckOnDraw = false;
    public int m_baseDamage = 1;
    public int m_baseRange = 1;

    // For charge attack
    public bool m_isChargeAttack = false;
    public override ActionResultArgs Perform (ActionArgs args) {

        if (m_isChargeAttack) {
            m_resultArgs = ChargeAttack (args);
            return base.Perform (args);
        }

        List<District> attackedDistricts = new List<District> { };
        string stringVal = "Success";

        foreach (MovementDirections direction in m_attackDirections) {
            District targetDistrict = DistrictManager.instance.GetDistrictInDirection (args.owner.Location, direction);
            if (targetDistrict != args.owner.entityMovement.m_currentDistrict) {
                attackedDistricts.Add (targetDistrict);
                if (m_baseRange > 1) {
                    for (int i = 1; i < m_baseRange; i++) {
                        District nextDistrict = DistrictManager.instance.GetDistrictInDirection (targetDistrict.m_gridLocation, direction);
                        if (nextDistrict != targetDistrict) {
                            attackedDistricts.Add (nextDistrict);
                            targetDistrict = nextDistrict;
                        } else {
                            break;
                        }
                    }
                }
            }
            if (direction == MovementDirections.NONE) {
                attackedDistricts.Add (args.owner.entityMovement.m_currentDistrict);
            }
        }
        if (attackedDistricts.Count > 0) {
            foreach (District distr in attackedDistricts) {
                if (m_attackDistrict) {
                    bool success = distr.AttemptWreckDistrict ();
                    if (!success) {
                        stringVal = "Fail";
                    }
                }
                if (m_entityTypesTargeted.Count > 0) {
                    foreach (Entity entity in new List<Entity> (distr.m_entitiesContained)) {
                        if (m_entityTypesTargeted.Contains (entity.m_data.m_type)) {
                            entity.AttackEntity (new ActionResultArgs (args, args.owner, entity, this, stringVal, m_baseDamage), m_knockBackSquares);
                        }
                    }
                }
            }
        }
        if (m_shuffleAttackDeckOnDraw) {
            stringVal = "Shuffle";
        }
        m_resultArgs = new ActionResultArgs (args, args.owner, args.target, this, stringVal, m_baseDamage);
        return base.Perform (args);
    }

    ActionResultArgs ChargeAttack (ActionArgs args) {
        ActionData_MonsterMovement movement = args.owner.entityMovement.GetRandomMovementAction () as ActionData_MonsterMovement;
        District targetDistrict = DistrictManager.instance.GetDistrictInDirection (args.owner.Location, movement.m_direction);
        string stringVal = "Charge Attack Failed";
        foreach (Entity entity in new List<Entity> (targetDistrict.m_entitiesContained)) {
            if (m_entityTypesTargeted.Contains (entity.m_data.m_type)) {
                entity.AttackEntity (new ActionResultArgs (args, args.owner, entity, this, "Charge Attack", m_baseDamage), m_knockBackSquares);
                stringVal = "Charge Attack Succeeded";
            }
        }
        bool success = targetDistrict.AttemptWreckDistrict ();
        ActionResultArgs reply = movement.Perform (new ActionArgs (args.owner, null));
        if (reply.stringVal == "Shuffle") {
            args.owner.entityMovement.ReshuffleMovementDeck ();
        }
        return new ActionResultArgs (args, args.owner, args.target, this, stringVal, m_baseDamage);
    }
}