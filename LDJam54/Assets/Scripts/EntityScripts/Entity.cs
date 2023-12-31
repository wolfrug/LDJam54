using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct EntityEventArgs {
    public Entity owner;
    public Entity instigator;

    public EntityComponent targetComponent;

    public int intVal;

    public string stringVal;

    public EntityEventArgs (Entity setOwner = null, Entity setInstigator = null, EntityComponent setComponent = null, int setInt = 0, string setString = "") {
        owner = setOwner;
        instigator = setInstigator;
        targetComponent = setComponent;
        intVal = setInt;
        stringVal = setString;
    }
}

public class Entity : MonoBehaviour {

    public EntityData m_data;
    public PlayerMovementController m_playerMovementController;
    public GameObject m_avatarDestroyed;
    public EntityMovement entityMovement;
    public EntityAttack entityAttack;
    public EntityHealth entityHealth;
    public EntityEffects entityEffects;
    public bool draggable = true;
    public virtual void Start () {

    }

    [NaughtyAttributes.Button]
    public virtual void Init (EntityData newData = null) {
        if (newData != null) {
            m_data = newData;
        }
        entityMovement = gameObject.AddComponent<EntityMovement> ();
        entityAttack = gameObject.AddComponent<EntityAttack> ();
        entityHealth = gameObject.AddComponent<EntityHealth> ();
        entityEffects = gameObject.AddComponent<EntityEffects> ();
    }
    public GridLocations Location {
        get {
            return entityMovement.m_currentDistrict.m_gridLocation;
        }
    }
    public void AttackEntity (ActionResultArgs args, int knockBackSquares = 0) {
        if (!entityHealth.IsDead && args.target == this) {
            entityHealth.Damage (args);
            if (knockBackSquares > 0) {
                MovementDirections relativeMovement = DistrictManager.instance.GetRelativeDirection (args.owner.Location, Location);
                entityMovement.MoveInDirection (relativeMovement, knockBackSquares);
            }
        }
    }
    public void MoveIntoEntity (MovementArgs args) {
        if (m_data.m_type == EntityType.TANK && args.owner.m_data.m_type == EntityType.MONSTER_CERATOLISK) {
            // Destroy tank!
            entityHealth.Health = -1;
        }
        if (m_data.m_type == EntityType.AGILE_MECH && args.owner.m_data.m_type == EntityType.MONSTER_CERATOLISK) {
            entityMovement.MoveTo (args.originDistrict.m_gridLocation);
        }
        if (m_data.m_type == EntityType.MELEE_MECH && args.owner.m_data.m_type == EntityType.MONSTER_CERATOLISK) {
            List<MovementDirections> availableSpots = entityMovement.GetPermittedMovementDirections ();
            MovementDirections randomDirection = availableSpots[Random.Range (0, availableSpots.Count - 1)];
            args.owner.entityMovement.MoveInDirection (randomDirection);
            args.owner.entityEffects.AddEffect (EffectType.STUN);
        }
    }

    [NaughtyAttributes.Button]
    void MoveToTest () {
        entityMovement.MoveTo (GridLocations.D3);
    }
}