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
    AIRSTRIKE = 6000,

}

[CreateAssetMenu (fileName = "Data", menuName = "Entity Data", order = 1)]
public class EntityData : ScriptableObject {
    public string ID => this.name;
    public string m_displayName = "";
    public EntityType m_type = EntityType.NONE;
    public GameObject m_prefab;
}