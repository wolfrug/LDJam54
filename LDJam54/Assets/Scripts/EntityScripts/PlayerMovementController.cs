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
    public Color m_attackColor = Color.red;
    public List<MovementButton> m_movementButtons = new List<MovementButton> { };

    void Awake () {
        foreach (MovementButton btn in m_movementButtons) {
            btn.m_button.onClick.AddListener (() => OnClickButton (btn));
        }
    }

    public void SetActiveDirections (List<MovementDirections> allowedDirections) {
        foreach (MovementButton btn in m_movementButtons) {
            btn.m_button.gameObject.SetActive (false);
        }
        foreach (MovementDirections dir in allowedDirections) {
            MovementButton btn = m_movementButtons.Find ((x) => x.m_direction == dir);
            if (btn != null) {
                btn.m_button.gameObject.SetActive (true);
            }
        }
    }
    public void SetAttackDirections (List<MovementDirections> attackDirections) {
        foreach (MovementButton btn in m_movementButtons) {
            btn.m_buttonImage.color = Color.white;
        }
        foreach (MovementDirections dir in attackDirections) {
            MovementButton btn = m_movementButtons.Find ((x) => x.m_direction == dir);
            if (btn != null) {
                btn.m_buttonImage.color = m_attackColor;
            }
        }
    }

    public void OnClickButton (MovementButton button) {
        Debug.Log ("[PlayerMovementController] Clicked movement button going " + button.m_direction.ToString ());
        GlobalEvents.InvokeOnPlayerClickMovement(new MovementArgs(m_entity, button.m_direction));
    }

}