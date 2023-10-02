using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct ActionArgs {
    public Entity owner;
    public Entity target;

    public MovementDirections direction;

    public ActionArgs (Entity setOwner, Entity setTarget, MovementDirections setDirection = MovementDirections.NONE) {
        owner = setOwner;
        target = setTarget;
        direction = setDirection;
    }
}
public struct ActionResultArgs {
    public ActionArgs originalActionArgs;
    public Entity owner;
    public Entity target;
    public EntityActionData performedAction;
    public string stringVal;
    public int intVal;

    public ActionResultArgs (ActionArgs setOriginalArgs, Entity setOwner = null, Entity setTarget = null, EntityActionData setPerformedAction = null, string setStringVal = "", int setIntVal = 0) {
        originalActionArgs = setOriginalArgs;
        owner = setOwner;
        target = setTarget;
        performedAction = setPerformedAction;
        stringVal = setStringVal;
        intVal = setIntVal;
    }
}

[CreateAssetMenu (fileName = "Data", menuName = "Actions/Base Action Data", order = 1)]
public class EntityActionData : ScriptableObject {
    public string ID => this.name;
    public string m_displayName;

    [NaughtyAttributes.ShowAssetPreview]
    public Sprite m_displayIcon;

    protected ActionResultArgs m_resultArgs;

    public virtual ActionResultArgs Perform (ActionArgs args) {
        GlobalEvents.InvokeOnActionPerformed (m_resultArgs);
        return m_resultArgs;
    }

}