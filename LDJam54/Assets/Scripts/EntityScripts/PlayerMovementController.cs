using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class MovementButton {
    public MovementDirections m_direction;
    public Button m_button;
    public Image m_buttonImage;
}

public class PlayerMovementController : MonoBehaviour {

    public Entity m_entity;
    public Canvas m_selfCanvas;
    public Color m_attackColor = Color.red;
    public GenericClickable m_clickable;
    public List<MovementButton> m_movementButtons = new List<MovementButton> { };

    private List<MovementButton> m_attackButtons = new List<MovementButton> { };

    void Awake () {
        foreach (MovementButton btn in m_movementButtons) {
            btn.m_button.onClick.AddListener (() => OnClickButton (btn));
            btn.m_button.gameObject.SetActive (false);
        }
        m_clickable.onMouseOverEvent.AddListener (HoverOn);
        m_clickable.onMouseExitEvent.AddListener (HoverOff);
        m_clickable.onMouseDownEvent.AddListener (OnClickEntity);
        GlobalEvents.OnPlayerClickMovement.AddListener ((x) => ResetArrows ());
    }
    void Start () {
        m_selfCanvas.worldCamera = GridCameraController.instance.mainCam;
    }

    void HoverOn (GenericClickable clickable) {
        //Debug.Log ("[PlayerMovementController] Unit hovered on");
        GlobalEvents.InvokeOnEntityHoverOn (new EntityEventArgs (m_entity));
    }
    void HoverOff (GenericClickable clickable) {
        //Debug.Log ("[PlayerMovementController] Unit hovered off");
        GlobalEvents.InvokeOnEntityHoverOff (new EntityEventArgs (m_entity));
    }

    void OnClickEntity (GenericClickable clickable) {
        GlobalEvents.InvokeOnEntitySelected (new EntityEventArgs (m_entity));
    }

    public void ResetArrows () {
        foreach (MovementButton btn in m_movementButtons) {
            btn.m_button.gameObject.SetActive (false);
            btn.m_button.interactable = false;
            btn.m_buttonImage.color = Color.white;
        }
    }
    public void SetActiveDirections (List<MovementDirections> allowedDirections) {
        foreach (MovementButton btn in m_movementButtons) {
            btn.m_button.gameObject.SetActive (true);
            btn.m_button.interactable = false;
        }
        foreach (MovementDirections dir in allowedDirections) {
            MovementButton btn = m_movementButtons.Find ((x) => x.m_direction == dir);
            if (btn != null) {
                btn.m_button.interactable = true;
            }
        }
    }
    public void SetAttackDirections (List<MovementDirections> attackDirections) {
        m_attackButtons.Clear ();
        foreach (MovementButton btn in m_movementButtons) {
            btn.m_buttonImage.color = Color.white;
        }
        foreach (MovementDirections dir in attackDirections) {
            MovementButton btn = m_movementButtons.Find ((x) => x.m_direction == dir);
            if (btn != null) {
                btn.m_buttonImage.color = m_attackColor;
                btn.m_button.interactable = true;
                m_attackButtons.Add (btn);
            }
        }
    }

    public void OnClickButton (MovementButton button) {
        Debug.Log ("[PlayerMovementController] Clicked movement button going " + button.m_direction.ToString ());
        if (m_attackButtons.Contains (button)) {
            GlobalEvents.InvokeOnPlayerClickAttack (new MovementArgs (m_entity, button.m_direction));
        } else {
            GlobalEvents.InvokeOnPlayerClickMovement (new MovementArgs (m_entity, button.m_direction));
        }
    }

}