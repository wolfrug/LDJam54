using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public enum GridLocations {
    None,
    A1,
    A2,
    A3,
    A4,
    A5,
    B1,
    B2,
    B3,
    B4,
    B5,
    C1,
    C2,
    C3,
    C4,
    C5,
    D1,
    D2,
    D3,
    D4,
    D5,
    E1,
    E2,
    E3,
    E4,
    E5,
    F1,
    F2,
    F3,
    F4,
    F5,
    G1,
    G2,
    G3,
    G4,
    G5,

}
public class GridManager : MonoBehaviour {
    public static GridManager instance;
    private Grid gridObj;
    [SerializeField] private Tilemap tilemapobj;

    public Vector2Int m_gridSize = new Vector2Int (7, 5);
    void Awake () {
        if (instance == null) {
            instance = this;
            if (gridObj == null) {
                gridObj = GetComponentInParent<Grid> ();
            }
        } else {
            Destroy (gameObject);
        }
    }
    public Grid grid {
        get {
            if (gridObj == null) {
                gridObj = GetComponentInParent<Grid> ();
            }
            return gridObj;
        }
        set { // Don't do this
            gridObj = value;
        }
    }

    public virtual Tilemap tilemap {
        get {
            if (tilemapobj == null) {
                tilemapobj = GetComponentInChildren<Tilemap> ();
            }
            return tilemapobj;
        }
    }

    public Vector3 GridToWorldSpace (Vector3Int gridLoc) {
        Vector3 returnVal = grid.CellToWorld (gridLoc);
        return returnVal;
    }

    public Vector3Int WorldSpaceToGrid (Vector3 worldspaceLoc) {
        Vector3Int returnVal = grid.WorldToCell (worldspaceLoc);
        return returnVal;
    }
    public TileBase GetTile (Vector3Int targetLoc) {
        return tilemap.GetTile (targetLoc);
    }
    public TileBase GetTile (Vector3 worldSpaceLoc) {
        return tilemap.GetTile (WorldSpaceToGrid (worldSpaceLoc));
    }
    public bool HasTile (Vector3Int targetLoc) {
        return tilemap.HasTile (targetLoc);
    }
    public bool HasTile (Vector3 worldspaceLoc) {
        return tilemap.HasTile (WorldSpaceToGrid (worldspaceLoc));
    }

    public GridLocations GetEnum (Vector3Int location) {
        string X = "None";
        string Y = (Mathf.Clamp (Mathf.Abs (location.y) + 1, 1, m_gridSize.y)).ToString ();
        GridLocations returnVal = GridLocations.None;
        switch (location.x) {
            case 0:
                {
                    X = "A";
                    break;
                }
            case 1:
                {
                    X = "B";
                    break;
                }
            case 2:
                {
                    X = "C";
                    break;
                }
            case 3:
                {
                    X = "D";
                    break;
                }
            case 4:
                {
                    X = "E";
                    break;
                }
            case 5:
                {
                    X = "F";
                    break;
                }
            case 6:
                {
                    X = "G";
                    break;
                }
            default:
                {
                    return GridLocations.None;
                }
        }
        returnVal = (GridLocations) Enum.Parse (typeof (GridLocations), X + Y);
        return returnVal;
    }

}