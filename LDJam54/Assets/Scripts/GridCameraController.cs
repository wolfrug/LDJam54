using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;

public struct TileInfo {
    public TileBase tile;
    public Tilemap tilemap;
    public Vector3Int location;
}

[System.Serializable]
public class GridMouseClickEvent : UnityEvent<TileInfo> { }

[System.Serializable]
public class GridMouseOverEvent : GridMouseClickEvent { }

[System.Serializable]
public class GridMouseExitEvent : GridMouseClickEvent { }

[System.Serializable]
public class GridMouseLeftClickEvent : GridMouseClickEvent { }

[System.Serializable]
public class GridMouseRightClickEvent : GridMouseClickEvent { }

[System.Serializable]
public class GridMouseMiddleClickEvent : GridMouseClickEvent { }
public class GridCameraController : MonoBehaviour {

    public static GridCameraController instance;
    // Start is called before the first frame update
    public Camera mainCam;
    public float cameraDistance = 500f;

    public float defaultZoomLevel = 7.5f;
    private Transform cameraTarget;
    public Transform moveClickLocation;
    public CinemachineVirtualCamera mainvcam;
    public CinemachineVirtualCamera panCamera;
    public MouseRts panCameraScript;
    public LayerMask hitMask;
    public GridMouseOverEvent mouseOverEvent;
    public GridMouseExitEvent mouseExitEvent;
    public GridMouseLeftClickEvent mouseLeftClickEvent;
    public GridMouseRightClickEvent mouseRightClickEvent;
    public GridMouseMiddleClickEvent mouseMiddleClickEvent;
    public TileInfo mouseOverTile;
    private TileInfo previousMouseOverTile;

    [SerializeField]
    private Grid maingrid;

    void Awake () {
        if (instance == null) {
            instance = this;
        } else {
            Destroy (this);
        }

    }

    void Start () {
        if (mainCam == null) {
            mainCam = Camera.main;
        }
        previousMouseOverTile = TileInfoConstructor (null, null, Vector3Int.zero);
        if (maingrid == null) { // maybe no grid manager around? Just grab the first grid you can find!
            maingrid = GridManager.instance.grid;
        }
    }

    public void SetCamTarget (Transform newtarget) {
        mainvcam.m_LookAt = newtarget;
        mainvcam.m_Follow = newtarget;
    }

    public void StartPanCamera () {
        //  Debug.Log ("Starting camera pan");
        panCameraScript.target.transform.position = mainvcam.m_Follow.position;
        panCamera.Priority = 11;
        DefaultZoom ();
    }
    public void StopPanCamera () {
        //  Debug.Log ("Stopping camera pan");
        panCameraScript.target.transform.position = mainvcam.m_Follow.position;
        panCamera.Priority = 9;
        DefaultZoom ();
    }
    public void SetPanCameraTarget (Transform target) {
        panCameraScript.target.position = target.position;
    }
    public void PanToCameraTarget () {
        panCamera.Priority = 11;
        DefaultZoom ();
    }

    public void SetZoomLevel (float zoomLevel) {
        panCamera.m_Lens.OrthographicSize = zoomLevel;
    }

    public void DefaultZoom () {
        panCamera.m_Lens.OrthographicSize = defaultZoomLevel;
    }
    public float GetCurrentZoom () {
        return panCamera.m_Lens.OrthographicSize;
    }

    TileInfo TileInfoConstructor (TileBase tile, Tilemap tilemap, Vector3Int location) {
        TileInfo newinfo;
        newinfo.tile = tile;
        newinfo.tilemap = tilemap;
        newinfo.location = location;
        return newinfo;
    }
    void NullPreviousTile () {
        previousMouseOverTile.tile = null;
        previousMouseOverTile.tilemap = null;
        previousMouseOverTile.location = Vector3Int.zero;
    }

    TileInfo DidHit (RaycastHit2D hit, GridMouseClickEvent clickEvent) {
        TileInfo info = TileInfoConstructor (null, null, Vector3Int.zero);
        var pointerEventData = new PointerEventData (EventSystem.current);
        pointerEventData.position = Input.mousePosition;
        var raycastResults = new List<RaycastResult> ();
        EventSystem.current.RaycastAll (pointerEventData, raycastResults);

        if (raycastResults.Count > 0) {
            //Debug.LogWarning ("Hit UI, quitting.");
            return info;
        }
        if (hit.collider != null) {
            //Debug.Log (hit.collider);
            GridManager manager = GridManager.instance;
            if (manager != null) {
                TileBase tile = manager.GetTile (hit.point);
                if (tile != null) {
                    info.tile = tile;
                    info.tilemap = manager.tilemap;
                    info.location = manager.WorldSpaceToGrid (hit.point);
                    clickEvent.Invoke (info);
                    //             Debug.Log (controller, controller.gameObject);
                    //                    Debug.Log ("Tile: " + info.location + " Tilemap: " + controller.tilemap.name);
                    return info;
                }
            }
        }
        return info;
    }

    // Update is called once per frame
    void Update () {
        if (GameManager.State == GameState.PLAYER_TURN) {
            RaycastHit2D hit = Physics2D.Raycast (mainCam.ScreenToWorldPoint (Input.mousePosition), Vector2.zero, cameraDistance, hitMask);
            if (Input.GetMouseButtonDown (0)) {
                //Debug.Log ("Clicked");
                //Debug.DrawRay (mainCam.ScreenToWorldPoint (Input.mousePosition), Vector2.zero, Color.red, 3f);
                DidHit (hit, mouseLeftClickEvent);
            }
            if (Input.GetMouseButtonDown (1)) { // we don't actually do anything with this but run the event lol
                //Debug.Log ("Clicked");
                //Debug.DrawRay (mainCam.ScreenToWorldPoint (Input.mousePosition), Vector2.zero, Color.red, 3f);
                mouseRightClickEvent.Invoke (TileInfoConstructor (null, null, new Vector3Int (0, 0, 0)));
            }

            // Mouse over
            //Debug.DrawRay (mainCam.ScreenToWorldPoint (Input.mousePosition), Vector2.zero, Color.red, 3f);
            mouseOverTile = DidHit (hit, mouseOverEvent);
            if (previousMouseOverTile.tile == null && mouseOverTile.tile != null) {
                //Debug.Log ("PreviousHighlightTile: " + mouseOverTile.tile);
                previousMouseOverTile.tile = mouseOverTile.tile;
                previousMouseOverTile.tilemap = mouseOverTile.tilemap;
                previousMouseOverTile.location = mouseOverTile.location;
            } else if (mouseOverTile.tile != previousMouseOverTile.tile && previousMouseOverTile.tile != null) {
                //Debug.Log ("MouseExitFrom " + previousMouseOverTile.tile);
                mouseExitEvent.Invoke (previousMouseOverTile);
                NullPreviousTile ();
                //Debug.Log (previousMouseOverTile.tile);
            }
            // Snap the transform to the appropriate grid location  
        };
        moveClickLocation.transform.position = maingrid.CellToWorld (maingrid.WorldToCell (mainCam.ScreenToWorldPoint (Input.mousePosition))) + (GridManager.instance.grid.cellSize / 2f);
    }
}