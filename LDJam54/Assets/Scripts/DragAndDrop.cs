using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Inventory {

    [System.Serializable]
    public class GenericDragAndDropEvents : UnityEvent<PointerEventData> { }

    [System.Serializable]
    public class GDDE_Enter : GenericDragAndDropEvents { }

    [System.Serializable]
    public class GDDE_Exit : GenericDragAndDropEvents { }

    [System.Serializable]
    public class GDDE_PointerDown : GenericDragAndDropEvents { }

    [System.Serializable]
    public class GDDE_PointerUp : GenericDragAndDropEvents { }

    [System.Serializable]
    public class DragStarted : UnityEvent<DragAndDrop> { }

    [System.Serializable]
    public class DragEnded : UnityEvent<DragAndDrop> { }

    public class DragAndDrop : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler, IBeginDragHandler, IDragHandler, IEndDragHandler {
        public Transform targetTransform;
        public Entity targetBox;
        public Canvas targetCanvas;
        public CanvasGroup targetCanvasGroup;
        public RectTransform limiter;
        private int defaultLayer;
        public bool interactable = true;
        //public CustomAudioSource audioSource;

        //public List<GameStates> interactableStates = new List<GameStates> { GameStates.DRAFT };
        public bool dragWithLeft = true;
        public bool pointerIsOver = false;
        public bool pointerIsDown = false;

        [Tooltip ("Drag using Unity's pointer system vs. simulated dragging (start/stop on mouse click OR command)")]
        public bool realDragging = true;
        private bool dragging = false;
        private Vector3 pointerClickLocation = Vector3.zero;
        private GameObject dragPointObj;
        public GDDE_Enter pointerEntered;
        public GDDE_Exit pointerExited;
        public GDDE_PointerDown pointerDown;
        public GDDE_PointerDown pointerDoubleClick;
        public GDDE_PointerDown rightClick;
        public GDDE_PointerUp pointerUp;
        public DragStarted dragStarted;
        public DragEnded dragEnded;

        // Since you can only ever drag one thing, this is a static!
        public static DragAndDrop currentDragTarget;
        public static DragAndDrop lastDragTarget;

        void Start () {
            if (targetCanvas != null) { defaultLayer = targetCanvas.sortingOrder; };
            if (targetCanvasGroup == null) {
                targetCanvasGroup = GetComponent<CanvasGroup> ();
            }
            if (targetTransform == null) {
                if (transform.childCount > 0) {
                    targetTransform = transform.GetChild (0);
                }
            }
            if (targetBox == null) {
                targetBox = targetTransform.GetComponent<Entity> ();
            }
            UpdateInteractability ();
        }
        Transform DragPoint {
            get {
                if (dragPointObj == null) {
                    dragPointObj = new GameObject (gameObject.name + "DragPoint", typeof (RectTransform));
                    dragPointObj.transform.SetParent (targetCanvas.transform);
                }
                return dragPointObj.transform;
            }
        }
        public void OnPointerEnter (PointerEventData coll) {
            if (interactable) {
                //   Debug.Log("Pointer enter" + coll);
                pointerIsOver = true;
                pointerEntered.Invoke (coll);
                //audioSource?.PlayRandomType (SFXType.UI_HOVERON);
            };
        }
        public void OnPointerExit (PointerEventData coll) {
            if (interactable) {
                //  Debug.Log("Pointer exit " + coll);
                pointerIsOver = false;
                pointerExited.Invoke (coll);
                //audioSource?.PlayRandomType (SFXType.UI_HOVEROFF);
            };
        }
        public void OnPointerDown (PointerEventData coll) {
            if (interactable) {
                Debug.Log ("Pointer down" + coll);
                pointerClickLocation = Input.mousePosition;
                if (coll.button == PointerEventData.InputButton.Left && dragWithLeft) {
                    pointerIsDown = true;
                    pointerDown.Invoke (coll);
                    //audioSource?.PlayRandomType (SFXType.UI_CLICK);
                } else if (dragWithLeft && coll.button != PointerEventData.InputButton.Left) {
                    //   Debug.Log("Right clicked");
                    rightClick.Invoke (coll);
                    //audioSource?.PlayRandomType (SFXType.UI_CLICK);
                } else {
                    pointerIsDown = true;
                    pointerDown.Invoke (coll);
                    //audioSource?.PlayRandomType (SFXType.UI_CLICK);
                }
                //        Debug.Log(coll.clickCount);
                if (coll.clickCount > 1) {
                    pointerDoubleClick.Invoke (coll);
                }
            };
        }
        public void OnPointerUp (PointerEventData coll) {
            if (interactable) {
                // Debug.Log("Pointer up" + coll);
                if (dragWithLeft && coll.button == PointerEventData.InputButton.Left) {
                    pointerClickLocation = Vector3.zero;
                    pointerIsDown = false;
                    if (dragging && !realDragging) {
                        dragging = false;
                    };
                    pointerUp.Invoke (coll);
                };
            };
        }

        public void OnBeginDrag (PointerEventData eventData) {
            //Debug.Log ("OnBeginDrag: ");
            if (interactable && !dragging) {
                pointerClickLocation = Input.mousePosition;
                StartDrag ();
            };
        }

        // Drag the selected item.
        public void OnDrag (PointerEventData data) {
            if (data.dragging && interactable) {
                // Object is being dragged.
                DragAction ();
            }
        }

        public void OnEndDrag (PointerEventData eventData) {
            //Debug.Log ("OnEndDrag: ");
            if (interactable) {
                StopDrag ();
            }
        }

        Vector2 DraggedPosition (Vector2 desiredPosition, Vector2 currentPosition) {
            if (RectTransformUtility.RectangleContainsScreenPoint (limiter, desiredPosition) || limiter == null) {
                return desiredPosition;
            } else {
                /* none of this works
                Vector2 xInsidePos = new Vector2 (desiredPosition.x, limiter.rect.yMax * 2f);
                if (RectTransformUtility.RectangleContainsScreenPoint (limiter, xInsidePos)) {
                    Debug.Log ("X inside ok (y max)");
                    return xInsidePos;
                }
                xInsidePos = new Vector2 (desiredPosition.x, limiter.rect.yMin * 2);
                if (RectTransformUtility.RectangleContainsScreenPoint (limiter, xInsidePos)) {
                    Debug.Log ("X inside ok (y min)");
                    return xInsidePos;
                }
                Vector2 yInsidePos = new Vector2 (limiter.rect.xMax * 2, desiredPosition.y);
                if (RectTransformUtility.RectangleContainsScreenPoint (limiter, yInsidePos)) {
                    Debug.Log ("Y inside ok (x max)");
                    return yInsidePos;
                }
                yInsidePos = new Vector2 (limiter.rect.xMin * 2, desiredPosition.y);
                if (RectTransformUtility.RectangleContainsScreenPoint (limiter, yInsidePos)) {
                    Debug.Log ("Y inside ok (x min)");
                    return yInsidePos;
                }
                */
                //Debug.Log ("Neither inside");
                return currentPosition;
            }
        }

        public void StartDrag (bool simulatePointerDownAndOver = false) {
            if (interactable) {
                if (simulatePointerDownAndOver) {
                    pointerIsOver = true;
                    pointerIsDown = true;
                }
                DragPoint.position = pointerClickLocation;
                targetTransform.SetParent (DragPoint, true);
                dragging = true;
                currentDragTarget = this;
                dragStarted.Invoke (this);
                targetCanvas.overrideSorting = true;
                targetCanvas.sortingOrder = 9999;
                targetCanvasGroup.blocksRaycasts = false;
            }
            //if (audioSource != null) {
                //audioSource.PlayRandomType (SFXType.UI_DRAGSTART);
            //}
        }
        public void DragAction () {
            if (interactable) {
                DragPoint.position = DraggedPosition (Input.mousePosition, DragPoint.position);
            }
        }
        public void StopDrag () {
            if (interactable) {
                targetTransform.SetParent (targetCanvas.transform, true);
                dragging = false;
                currentDragTarget = null;
                lastDragTarget = this;
                dragEnded.Invoke (this);
                targetCanvas.overrideSorting = false;
                targetCanvas.sortingOrder = defaultLayer + 1;
                targetCanvasGroup.blocksRaycasts = true;
            }
            /*if (audioSource != null) {
                audioSource.PlayRandomType (SFXType.UI_DRAGCANCEL);
            }*/
        }

        public void UpdateInteractability () {
            interactable = targetBox.draggable;
        }

        public void ResetDragTargetPosition () {
            targetTransform.localPosition = Vector2.zero;
        }

        void Update () {
            if (!realDragging && dragging) {
                DragAction ();
            }
        }

        void OnDestroy () {
            if (dragPointObj != null) {
                Destroy (dragPointObj);
            }
        }
    }
}