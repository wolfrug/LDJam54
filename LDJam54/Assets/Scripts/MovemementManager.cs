using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MovementDirections {
    NONE = 0000,
    UP = 1000,
    DIAGONAL_UP_RIGHT = 1001,
    DIAGONAL_UP_LEFT = 1002,
    DOWN = 2000,
    DIAGONAL_DOWN_RIGHT = 2001,
    DIAGONAL_DOWN_LEFT = 2002,
    RIGHT = 3000,
    LEFT = 4000,
}

public struct MovementArgs {
    public Entity owner;

    public MovementDirections direction;
    public District targetDistrict;
    public District originDistrict;
    public bool wreckOnEntry;
    public bool attackMove;

    public MovementArgs (Entity setOwner = null, MovementDirections setDirection = MovementDirections.NONE, District setTarget = null, District setOrigin = null, bool setWreckOnEntry = false, bool setAttackMove = false) {
        owner = setOwner;
        direction = setDirection;
        targetDistrict = setTarget;
        originDistrict = setOrigin;
        wreckOnEntry = setWreckOnEntry;
        attackMove = setAttackMove;
    }
}

public class MovemementManager : MonoBehaviour {

}