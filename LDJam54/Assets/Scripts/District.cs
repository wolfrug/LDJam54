using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct DistrictEventArgs {
    public District owner;
    public District target;
    public Entity instigator;

    public DistrictEventArgs (District setOwner = null, District setTarget = null, Entity setInstigator = null) {
        owner = setOwner;
        target = setTarget;
        instigator = setInstigator;
    }
}

public class District : MonoBehaviour {
    public DistrictData m_data;
    public GridLocations m_gridLocation = GridLocations.None;
    public Vector3Int m_gridPosition;
    public GameObject m_wreckedIcon;
    public List<Entity> m_entitiesContained = new List<Entity> { };
    public List<Transform> m_entityPositions = new List<Transform> { };
    private Dictionary<Transform, Entity> m_occupancyDictionary = new Dictionary<Transform, Entity> { };
    private bool m_wrecked = false;

    void Awake () {
        foreach (Transform tra in m_entityPositions) {
            m_occupancyDictionary.Add (tra, null);
        }
        GlobalEvents.OnGameStateChanged.AddListener (OnEnemyTurnStart);
        GlobalEvents.OnDistrictEntered.AddListener (OnEnemyEnterDistrict);
    }

    public void OnEnemyTurnStart (GameState state) {
        if (state == GameState.ENEMY_TURN) {
            if (m_data.m_damageMonsterPerTurn > 0) {
                foreach (Entity entity in m_entitiesContained) {
                    if (entity.m_data.m_faction == EntityFaction.ENEMY) {
                        entity.AttackEntity (new ActionResultArgs (new ActionArgs (), null, entity, null, "", m_data.m_damageMonsterPerTurn), 0);
                    }
                }
            }
        }
    }
    public void OnEnemyEnterDistrict (DistrictEventArgs args) {
        if (args.owner == this) {
            if (m_data.m_stunOnEnter && args.instigator.m_data.m_faction == EntityFaction.ENEMY) {
                args.instigator.entityEffects.AddEffect (EffectType.STUN);
            }
            if (m_data.m_autoWreckOnEnter && args.instigator.m_data.m_faction == EntityFaction.ENEMY) {
                AttemptWreckDistrict ();
            }
        }
    }

    public Vector3 AddEntity (Entity newEntity) {
        if (!m_entitiesContained.Contains (newEntity)) {
            m_entitiesContained.Add (newEntity);
            Transform randomFreePosition = GetRandomFreeEntityPosition ();
            m_occupancyDictionary[randomFreePosition] = newEntity;
            newEntity.entityMovement.m_currentDistrict = this;
            GlobalEvents.InvokeOnDistrictEntered (new DistrictEventArgs (this, null, newEntity));
            return randomFreePosition.position;
        }
        return new Vector3 (-1, -1, -1);
    }
    public void RemoveEntity (Entity newEntity) {
        if (m_entitiesContained.Contains (newEntity)) {
            m_entitiesContained.Remove (newEntity);
            if (m_occupancyDictionary.ContainsValue (newEntity)) {
                Transform targetPosition = transform;
                foreach (KeyValuePair<Transform, Entity> kvp in m_occupancyDictionary) {
                    if (kvp.Value == newEntity) {
                        targetPosition = kvp.Key;
                        break;
                    }
                }
                if (targetPosition != transform) {
                    m_occupancyDictionary[targetPosition] = null;
                }
            }
            GlobalEvents.InvokeOnDistrictExited (new DistrictEventArgs (this, null, newEntity));
        }
    }

    public Vector3 MoveEntity (MovementArgs args) {
        foreach (Entity entity in new List<Entity> (m_entitiesContained)) {
            entity.MoveIntoEntity (args);
            if (entity.entityHealth.IsDead) {
                RemoveEntity (entity);
            }
        }
        Vector3 returnVal = AddEntity (args.owner);
        args.originDistrict.RemoveEntity (args.owner);
        GlobalEvents.InvokeOnDistrictMoved (new DistrictEventArgs (this, args.originDistrict, args.owner));
        return returnVal;
    }

    Transform GetRandomFreeEntityPosition () {
        foreach (KeyValuePair<Transform, Entity> kvp in m_occupancyDictionary) {
            if (kvp.Value == null) {
                return kvp.Key;
            }
        }
        return transform;
    }

    public bool Wrecked {
        get {
            return m_wrecked;
        }
        set {
            if (value && !m_wrecked) {
                GlobalEvents.InvokeOnDistrictWrecked (new DistrictEventArgs (this));
            }
            m_wrecked = value;
            m_wreckedIcon.SetActive (value);
        }
    }

    public bool AttemptWreckDistrict () {
        if (m_data.m_wreckable) {
            Entity tank = m_entitiesContained.Find ((x) => x.m_data.m_type == EntityType.TANK);
            if (tank != null) {
                tank.AttackEntity (new ActionResultArgs (new ActionArgs (), null, tank, null, "", 1), 0);
            } else {
                Wrecked = true;
            }
            return true;
        } else {
            return false;
        }
    }
}