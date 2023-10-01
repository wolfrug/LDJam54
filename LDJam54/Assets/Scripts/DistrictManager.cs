using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DistrictManager : MonoBehaviour {
    public static DistrictManager instance;
    public GameObject m_districtPrefab;
    public List<District> m_spawnedDistricts = new List<District> { };

    private Dictionary<GridLocations, DistrictData> m_districtDatas = new Dictionary<GridLocations, DistrictData> { };

    private Vector2Int m_gridSize;

    void Awake () {
        if (instance == null) {
            instance = this;
            LoadAllDistrictDatas ();
        } else {
            Destroy (gameObject);
        }
    }

    void LoadAllDistrictDatas () {
        // Load all inventory datas!
        m_districtDatas.Clear ();
        Object[] loadedDatas = Resources.LoadAll ("Data/Districts", typeof (DistrictData));
        foreach (Object obj in loadedDatas) {
            m_districtDatas.Add ((obj as DistrictData).m_gridLocation, obj as DistrictData);
        }
        // Debug.Log ("Inventories loaded: " + AllInventoryData.Count);
    }

    public void Init () {
        m_gridSize = GridManager.instance.m_gridSize;
        for (int y = 0; y > -m_gridSize.y; y--) {
            for (int x = 0; x < m_gridSize.x; x++) {
                m_spawnedDistricts.Add (SpawnDistrict (x, y));
            }
        }
    }

    District SpawnDistrict (int x, int y) {
        Vector3Int districtLoc = new Vector3Int (x, y, 0);
        DistrictData tryGetData;
        GameObject prefab = m_districtPrefab;
        GridLocations gridLoc = GridManager.instance.GetEnum (districtLoc);
        m_districtDatas.TryGetValue (gridLoc, out tryGetData);
        if (tryGetData != null) {
            if (tryGetData.m_prefab != null) {
                prefab = tryGetData.m_prefab;
            }
        };
        GameObject newDistrict = Instantiate (prefab, transform);
        District district = newDistrict.GetComponent<District> ();
        if (tryGetData != null) { district.m_data = tryGetData; }
        district.m_gridPosition = districtLoc;
        district.m_gridLocation = gridLoc;
        newDistrict.transform.position = GridManager.instance.GridToWorldSpace (districtLoc) + (GridManager.instance.grid.cellSize / 2f);
        newDistrict.name = district.m_data.m_displayName + "(" + gridLoc.ToString () + ")";
        return district;
    }

    public District GetDistrict (Vector3Int location) {
        GridLocations enumLoc = GridManager.instance.GetEnum (location);
        if (enumLoc != GridLocations.None) {
            return GetDistrict (enumLoc);
        }
        return null;
    }
    public District GetDistrict (GridLocations location) {
        if (location != GridLocations.None) {
            District foundDistrict = m_spawnedDistricts.Find ((x) => x.m_gridLocation == location);
            return foundDistrict;
        }
        return null;
    }

    public MovementDirections GetRelativeDirection (GridLocations startLocation, GridLocations endLocation) {
        District startDistrict = GetDistrict (startLocation);
        District endDistrict = GetDistrict (endLocation);
        if (startDistrict.m_gridPosition.x > endDistrict.m_gridPosition.x) {
            if (startDistrict.m_gridPosition.y == endDistrict.m_gridPosition.y) {
                return MovementDirections.LEFT;
            } else {
                if (startDistrict.m_gridPosition.y < endDistrict.m_gridPosition.y) {
                    return MovementDirections.DIAGONAL_UP_LEFT;
                } else {
                    return MovementDirections.DIAGONAL_DOWN_LEFT;
                }
            }
        } else if (startDistrict.m_gridPosition.x < endDistrict.m_gridPosition.x) {
            if (startDistrict.m_gridPosition.y == endDistrict.m_gridPosition.y) {
                return MovementDirections.RIGHT;
            } else {
                if (startDistrict.m_gridPosition.y < endDistrict.m_gridPosition.y) {
                    return MovementDirections.DIAGONAL_UP_RIGHT;
                } else {
                    return MovementDirections.DIAGONAL_DOWN_RIGHT;
                }
            }
        } else if (startDistrict.m_gridPosition.x == endDistrict.m_gridPosition.x) {
            if (startDistrict.m_gridPosition.y > endDistrict.m_gridPosition.y) {
                return MovementDirections.DOWN;
            } else {
                return MovementDirections.UP;
            }
        }
        return MovementDirections.NONE;
    }

    public bool IsAdjacent (GridLocations startLocation, GridLocations endLocation, bool allowDiagonal = false) {
        District startDistrict = GetDistrict (startLocation);
        District endDistrict = GetDistrict (endLocation);
        if (Mathf.Abs (startDistrict.m_gridPosition.x - endDistrict.m_gridPosition.x) > 1) {
            return false;
        }
        if (Mathf.Abs (startDistrict.m_gridPosition.y - endDistrict.m_gridPosition.y) > 1) {
            return false;
        }
        if (allowDiagonal) {
            return true;
        }
        MovementDirections relativeDirection = GetRelativeDirection (startLocation, endLocation);
        if (!new List<MovementDirections> { MovementDirections.DOWN, MovementDirections.UP, MovementDirections.RIGHT, MovementDirections.LEFT }.Contains (relativeDirection)) {
            return false;
        }
        return true;

    }

    [NaughtyAttributes.Button]
    void TestDirections () {
        Debug.Log ("test 1 (A1, A2) " + IsAdjacent (GridLocations.A1, GridLocations.A2));
        Debug.Log ("test 2 (D4, E4) " + IsAdjacent (GridLocations.D4, GridLocations.E4));
        Debug.Log ("test 3 (G1, E1) " + IsAdjacent (GridLocations.G1, GridLocations.E1));
        Debug.Log ("test 4 (B2, C3) " + IsAdjacent (GridLocations.B2, GridLocations.C3));
        Debug.Log ("test 5 (B2, C3) w/diagonal" + IsAdjacent (GridLocations.B2, GridLocations.C3, true));
    }

}