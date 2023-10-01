using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EntityType {
    NONE = 0000,
    MELEE_MECH = 1000,
    AGILE_MECH = 2000,
    TANK = 3000,
    MONSTER_CERATOLISK = 4000,
    TRAP = 5000,
    AIRSTRIKE_HORIZONTAL = 6000,
    AIRSTRIKE_VERTICAL = 6001,

}
public enum EntityFaction {
    NONE = 0000,
    PLAYER = 1000,
    ENEMY = 2000,
}

[CreateAssetMenu (fileName = "Data", menuName = "Entity Data", order = 1)]
public class EntityData : ScriptableObject {
    public string ID => this.name;
    public string m_displayName = "";
    public EntityType m_type = EntityType.NONE;
    public EntityFaction m_faction = EntityFaction.NONE;
    public int m_value = 1; // Used to determine which is destroyed when there are too many
    public GameObject m_prefab;
    public int m_health = 1;
    public int m_movementPoints = 1;
    public bool m_canMoveInOcean = true;
    public List<EntityActionData> m_entityMovementActions = new List<EntityActionData> { };
    public List<EntityActionData> m_entityAttackActions = new List<EntityActionData> { };
}