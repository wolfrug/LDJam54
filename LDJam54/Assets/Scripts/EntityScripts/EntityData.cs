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

[CreateAssetMenu (fileName = "Data", menuName = "Entity Data", order = 1)]
public class EntityData : ScriptableObject {
    public string ID => this.name;
    public string m_displayName = "";
    public EntityType m_type = EntityType.NONE;
    public int m_value = 1; // Used to determine which is destroyed when there are too many
    public GameObject m_prefab;
}