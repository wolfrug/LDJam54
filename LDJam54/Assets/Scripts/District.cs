using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class District : MonoBehaviour {
    public DistrictData m_data;
    public GridLocations m_gridLocation = GridLocations.None;
    public Vector3Int m_gridPosition;
    public List<Entity> m_entitiesContained = new List<Entity> { };

    public List<Transform> m_entityPositions = new List<Transform> { };
    private Dictionary<Transform, Entity> m_occupancyDictionary = new Dictionary<Transform, Entity> { };

    void Awake () {
        foreach (Transform tra in m_entityPositions) {
            m_occupancyDictionary.Add (tra, null);
        }
    }

    public Vector3 AddEntity (Entity newEntity) {
        if (!m_entitiesContained.Contains (newEntity)) {
            m_entitiesContained.Add (newEntity);
            Transform randomFreePosition = GetRandomFreeEntityPosition ();
            m_occupancyDictionary[randomFreePosition] = newEntity;
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
        }
    }

    Transform GetRandomFreeEntityPosition () {
        foreach (KeyValuePair<Transform, Entity> kvp in m_occupancyDictionary) {
            if (kvp.Value == null) {
                return kvp.Key;
            }
        }
        return transform;
    }
}