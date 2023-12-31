using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DistrictType {
    NONE = 0000,
    OCEAN = 1000,
    CITY = 2000,
    SPECIAL = 3000,
    ORBHQ = 3001,
}

[CreateAssetMenu (fileName = "Data", menuName = "District Data", order = 1)]
public class DistrictData : ScriptableObject {

    public string ID => this.name;
    public string m_displayName = "";
    public DistrictType m_type = DistrictType.NONE;
    public GameObject m_prefab;
    public GridLocations m_gridLocation = GridLocations.None;
    public bool m_wreckable = true;
    public bool m_autoWreckOnEnter = false;
    public bool m_stunOnEnter = false;
    public int m_damageMonsterPerTurn = 0;
    public bool m_doubleMonsterMovement = false;
    public List<MovementDirections> m_blockedDirectionsOut = new List<MovementDirections> { };
    public List<MovementDirections> m_blockedDirectionsIn = new List<MovementDirections> { };

}