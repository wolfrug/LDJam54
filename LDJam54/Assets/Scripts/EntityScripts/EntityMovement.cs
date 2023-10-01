using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class EntityMovement : EntityComponent {

    public District m_currentDistrict;
    protected override void Init () {
        base.Init ();
        Vector3Int currenLocation = GridManager.instance.WorldSpaceToGrid (Entity.transform.position);
        m_currentDistrict = DistrictManager.instance.GetDistrict (currenLocation);
        MoveTo (m_currentDistrict.m_gridLocation);
    }

    public bool MoveTo (Vector3Int targetLocation) {
        if (GridManager.instance.HasTile (targetLocation)) { // Tile available check
            District targetDistrict = DistrictManager.instance.GetDistrict (targetLocation);
            if (targetDistrict != null) { // Just in case, although this shouldn't be a problem
                m_currentDistrict.RemoveEntity (Entity);
                Vector3 targetPos = targetDistrict.AddEntity (Entity);
                transform.DOMove (targetPos, 1);
                m_currentDistrict = targetDistrict;
                return true;
            }
        }
        return false;
    }
    public bool MoveTo (GridLocations location) {
        if (location != GridLocations.None) {
            if (DistrictManager.instance.GetDistrict (location) != null) {
                return MoveTo (DistrictManager.instance.GetDistrict (location).m_gridPosition);
            }
        }
        return false;
    }
}