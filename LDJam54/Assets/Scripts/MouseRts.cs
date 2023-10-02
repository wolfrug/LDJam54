using UnityEngine;
//From https://www.andrejeworutzki.de/game-developement/unity-realtime-strategy-camera/
public class MouseRts : MonoBehaviour {
    public Transform target;
    public Collider2D boundingArea;

    public bool active = true;
    public int LevelArea = 100;

    public int ScrollArea = 30;
    public int ScrollSpeed = 25;
    public int DragSpeed = 100;

    public int ZoomSpeed = 25;
    public int ZoomMin = 3;
    public int ZoomMax = 15;

    public int PanSpeed = 50;
    public int PanAngleMin = 25;
    public int PanAngleMax = 80;

    // Update is called once per frame
    void FixedUpdate () {
        if (GameManager.State != GameState.ENEMY_TURN && active) { // only allow panning camera in game mode
            // Init camera translation for this frame.
            var translation = Vector3.zero;

            // Zoom in or out
            var zoomDelta = Input.GetAxis ("Mouse ScrollWheel") * ZoomSpeed * Time.deltaTime;
         //   Debug.Log ("Zoom delta: " + zoomDelta);
            if (zoomDelta != 0) {
                zoomDelta += GridCameraController.instance.GetCurrentZoom ();
                if (zoomDelta < ZoomMax && zoomDelta > ZoomMin) {
                    GridCameraController.instance.SetZoomLevel (zoomDelta);
                }
            }
            /*
        // Start panning camera if zooming in close to the ground or if just zooming out.
        var pan = camera.transform.eulerAngles.x - zoomDelta * PanSpeed;
        pan = Mathf.Clamp (pan, PanAngleMin, PanAngleMax);
        if (zoomDelta < 0 || camera.transform.position.y < (ZoomMax / 2)) {
            camera.transform.eulerAngles = new Vector3 (pan, 0, 0);
        }
*/
            // Move camera with arrow keys
            translation += new Vector3 (Input.GetAxis ("Horizontal") * ScrollSpeed * Time.deltaTime, Input.GetAxis ("Vertical") * ScrollSpeed * Time.deltaTime, 0);

            // Move camera with mouse
            if (Input.GetMouseButton (2) || Input.touchCount > 0) // MMB
            {
                // Hold button and drag camera around
                translation -= new Vector3 (Input.GetAxis ("Mouse X") * DragSpeed * Time.deltaTime,
                    Input.GetAxis ("Mouse Y") * DragSpeed * Time.deltaTime, 0f);
            }
#if !UNITY_EDITOR
            else {

                // Move camera if mouse pointer reaches screen borders
                if (Input.mousePosition.x < ScrollArea) {
                    translation += Vector3.right * -ScrollSpeed * Time.deltaTime;
                }

                if (Input.mousePosition.x >= Screen.width - ScrollArea) {
                    translation += Vector3.right * ScrollSpeed * Time.deltaTime;
                }

                if (Input.mousePosition.y < ScrollArea) {
                    translation += Vector3.up * -ScrollSpeed * Time.deltaTime;
                }

                if (Input.mousePosition.y > Screen.height - ScrollArea) {
                    translation += Vector3.up * ScrollSpeed * Time.deltaTime;
                }
            }
            Vector3 desiredPosition = target.transform.position + translation;
            if (!boundingArea.bounds.Contains (desiredPosition)) {
                Debug.Log ("Trying to leave bounding area");
                Debug.Log ("Boundingarea extents X: " + boundingArea.bounds.extents.x);
                Debug.Log ("Boundingarea extents Y: " + boundingArea.bounds.extents.y);
                if (desiredPosition.x < boundingArea.bounds.extents.x) {
                    translation.x = 0f;
                }
                if (desiredPosition.x > boundingArea.bounds.extents.x) {
                    translation.x = 0f;
                }
                if (desiredPosition.y < boundingArea.bounds.extents.y) {
                    translation.y = 0f;
                }
                if (desiredPosition.y > boundingArea.bounds.extents.y) {
                    translation.y = 0f;
                }
            }
#endif
            /*
                    // Keep camera within level and zoom area
                    var desiredPosition = camera.transform.position + translation;
                    if (desiredPosition.x < -LevelArea || LevelArea < desiredPosition.x) {
                        translation.x = 0;
                    }
                    if (desiredPosition.y < ZoomMin || ZoomMax < desiredPosition.y) {
                        translation.y = 0;
                    }
                    if (desiredPosition.z < -LevelArea || LevelArea < desiredPosition.z) {
                        translation.z = 0;
                    }
            */
            // Finally move camera parallel to world axis
            target.transform.position += translation;
        };
    }
}