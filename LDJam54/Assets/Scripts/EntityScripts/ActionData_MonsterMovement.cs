using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (fileName = "Data", menuName = "Actions/Monster Move Data", order = 1)]
public class ActionData_MonsterMovement : EntityActionData {
    public MovementDirections m_direction;

    public override ActionResultArgs Perform (ActionArgs args) {

        if (m_direction == MovementDirections.NONE) {
            m_resultArgs = new ActionResultArgs (args, args.owner, args.target, this, "Shuffle");
            return base.Perform (args);
        }
        District targetDistrict = DistrictManager.instance.GetDistrictInDirection (args.owner.Location, m_direction);
        if (targetDistrict == args.owner.entityMovement.m_currentDistrict) {
            m_resultArgs = new ActionResultArgs (args, args.owner, args.target, this, "Fail");
            return base.Perform (args);
        }
        Vector3Int targetLocation = targetDistrict.m_gridPosition;
        args.owner.entityMovement.MoveTo (targetLocation);
        if (args.owner.entityMovement.m_currentDistrict == targetDistrict) {
            m_resultArgs = new ActionResultArgs (args, args.owner, args.target, this, "Success");
            return base.Perform (args);
        } else {
            m_resultArgs = new ActionResultArgs (args, args.owner, args.target, this, "Fail");
            return base.Perform (args);
        }
    }
}