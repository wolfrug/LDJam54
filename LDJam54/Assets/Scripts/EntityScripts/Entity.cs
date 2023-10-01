using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour {

    public EntityData m_data;
    public EntityMovement entityMovement;
    public virtual void Start () {

    }

    [NaughtyAttributes.Button]
    public virtual void Init () {
        entityMovement = gameObject.AddComponent<EntityMovement> ();
    }

    [NaughtyAttributes.Button]
    void MoveToTest () {
        entityMovement.MoveTo (GridLocations.D3);
    }
}