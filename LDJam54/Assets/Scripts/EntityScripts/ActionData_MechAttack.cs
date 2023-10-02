using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AttackType {
    NONE = 0000,
    MELEE_MECH_BASE = 1000,
    AGILE_MECH_BASE = 2000,
    MELEE_MECH_SPECIAL = 3000,
    AGILE_MECH_SPECIAL = 4000,
    TANK_BASE = 5000,
    AIRSTRIKE = 6000,
    TRAP = 7000,
}

[CreateAssetMenu (fileName = "Data", menuName = "Actions/Mech Attack Data", order = 1)]
public class ActionData_MechAttack : EntityActionData {

    public AttackType m_type;

    public override ActionResultArgs Perform (ActionArgs args) {
        switch (m_type) {
            case AttackType.MELEE_MECH_BASE:
                {
                    return MeleeMechAttackBase (args);
                }
            case AttackType.AGILE_MECH_BASE:
                {
                    return AgileMechAttackBase (args);
                }
            case AttackType.AGILE_MECH_SPECIAL:
                {
                    return AgileMechAttackSpecial (args);
                }
            case AttackType.MELEE_MECH_SPECIAL:
                {
                    return MeleeMechAttackSpecial (args);
                }
            case AttackType.TANK_BASE:
                {
                    return TankAttackBase (args);
                }
            case AttackType.AIRSTRIKE:
                {
                    return AirstrikeBase (args);
                }
            case AttackType.TRAP:
                {
                    return TrapBase (args);
                }
        }
        return base.Perform (args);
    }

    ActionResultArgs MeleeMechAttackBase (ActionArgs args) {
        Entity target = args.target;
        ActionResultArgs returnArgs;
        // Stunned: 2 damage, no knockback
        if (target.entityEffects.HasEffect (EffectType.STUN)) {
            returnArgs = new ActionResultArgs (args, args.owner, target, this, "StunCritical", 2);
            target.AttackEntity (returnArgs, 0);
        } else { // Not stunned: 1 damage, knockback
            returnArgs = new ActionResultArgs (args, args.owner, target, this, "Knockback+Damage", 1);
            target.AttackEntity (returnArgs, 1);
        }
        return returnArgs;
    }
    ActionResultArgs AgileMechAttackBase (ActionArgs args) {
        Entity target = args.target;
        ActionResultArgs returnArgs;
        // Stunned: 2 damage, no knockback
        if (target.entityEffects.HasEffect (EffectType.STUN)) {
            returnArgs = new ActionResultArgs (args, args.owner, target, this, "StunCritical", 2);
            target.AttackEntity (returnArgs, 0);
        } else { // Not stunned: 0 damage, knockback
            returnArgs = new ActionResultArgs (args, args.owner, target, this, "Knockback", 0);
            target.AttackEntity (returnArgs, 1);
        }
        return returnArgs;
    }

    ActionResultArgs MeleeMechAttackSpecial (ActionArgs args) {
        Entity instigator = args.owner;
        MovementDirections direction = args.direction;
        GridLocations currentLocation = args.owner.Location;
        ActionResultArgs returnArgs;
        for (int i = 0; i < 10; i++) {
            District targetDistrict = DistrictManager.instance.GetDistrictInDirection (currentLocation, direction);
            if (targetDistrict.m_gridLocation != currentLocation) {
                foreach (Entity enemyEntity in targetDistrict.m_entitiesContained.FindAll ((x) => x.m_data.m_faction == EntityFaction.ENEMY)) {
                    returnArgs = new ActionResultArgs (args, args.owner, enemyEntity, this, "Success", 3);
                    enemyEntity.AttackEntity (returnArgs, 0);
                    return returnArgs;
                }
                currentLocation = targetDistrict.m_gridLocation;
            } else {
                break;
            }
        }
        return new ActionResultArgs (args, args.owner, null, this, "Fail", 0);
    }

    ActionResultArgs AgileMechAttackSpecial (ActionArgs args) {
        Entity mech = args.owner;
        MovementDirections direction = args.direction;
        GridLocations currentLocation = args.owner.Location;
        ActionResultArgs returnArgs = new ActionResultArgs ();
        int damageAmount = 0;
        District endDistrict = mech.entityMovement.m_currentDistrict;
        bool succeeded = false;
        for (int i = 0; i < 10; i++) {
            endDistrict = DistrictManager.instance.GetDistrictInDirection (currentLocation, direction);
            if (endDistrict.m_gridLocation != currentLocation) {
                damageAmount++;
                Entity enemyEntity = endDistrict.m_entitiesContained.Find ((x) => x.m_data.m_faction == EntityFaction.ENEMY);
                if (enemyEntity != null) {
                    returnArgs = new ActionResultArgs (args, args.owner, enemyEntity, this, "Success", damageAmount);
                    enemyEntity.AttackEntity (returnArgs, 0);
                    succeeded = true;
                    break;
                }
                currentLocation = endDistrict.m_gridLocation;
            } else {
                break;
            }
        }
        if (succeeded) {
            mech.entityMovement.MoveTo (endDistrict.m_gridLocation);
            return returnArgs;
        } else {
            mech.entityMovement.MoveInDirection (direction, 3);
            return new ActionResultArgs (args, args.owner, null, this, "Fail", 0);
        }
    }
    ActionResultArgs TankAttackBase (ActionArgs args) {
        args.target.AttackEntity (new ActionResultArgs (args, args.owner, args.target, this, "Tank Attack", 1), 0);
        return new ActionResultArgs (args, args.owner, null, this, "Tank Attack", 1);
    }
    ActionResultArgs AirstrikeBase (ActionArgs args) {
        List<District> attackedDistricts = new List<District> { };
        ActionResultArgs returnArgs = new ActionResultArgs (args, args.owner, null, this, "Airstrike Fail", 0);
        GridLocations currentLocation = args.owner.Location;
        for (int i = 1; i < 3; i++) {
            District nextDistrict = DistrictManager.instance.GetDistrictInDirection (currentLocation, args.direction);
            if (nextDistrict.m_gridLocation != currentLocation) {
                attackedDistricts.Add (nextDistrict);
                currentLocation = nextDistrict.m_gridLocation;
            } else {
                break;
            }
        }
        if (attackedDistricts.Count > 0) {
            foreach (District distr in attackedDistricts) {
                foreach (Entity entity in new List<Entity> (distr.m_entitiesContained)) {
                    if (entity.m_data.m_faction == EntityFaction.ENEMY) {
                        entity.AttackEntity (returnArgs, 0);
                        returnArgs = new ActionResultArgs (args, args.owner, entity, this, "Airstrike Success", 3);
                        return returnArgs;
                    }
                }
            }
        }
        return returnArgs;
    }

    ActionResultArgs TrapBase (ActionArgs args) {
        if (args.target.m_data.m_faction == EntityFaction.ENEMY) {
            args.target.entityEffects.AddEffect (EffectType.STUN);
            return new ActionResultArgs (args, args.owner, args.target, this, "Trap Hit", 0);
        }
        return new ActionResultArgs (args, args.owner, args.target, this, "Trap Missed", 0);
    }
}