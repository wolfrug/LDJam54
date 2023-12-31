using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable] public class ActionEvent : UnityEvent<ActionResultArgs> { }

[System.Serializable] public class GameStateEvent : UnityEvent<GameState> { }

[System.Serializable] public class EntityEvent : UnityEvent<EntityEventArgs> { }

[System.Serializable] public class DistrictEvent : UnityEvent<DistrictEventArgs> { }

[System.Serializable] public class MovementEvent : UnityEvent<MovementArgs> { }

public class GlobalEvents : MonoBehaviour {

    // Game State events
    public static GameStateEvent OnGameStateChanged = new GameStateEvent ();
    public static GameStateEvent OnSkipPlayerAttack = new GameStateEvent ();
    public static GameStateEvent OnEndPlayerTurn = new GameStateEvent ();
    // Action Events
    public static ActionEvent OnActionPerformed = new ActionEvent ();
    public static ActionEvent OnMonsterMovementCardDrawn = new ActionEvent ();
    public static ActionEvent OnMonsterAttackCardDrawn = new ActionEvent ();

    // Entity Events
    public static EntityEvent OnEntitySpawned = new EntityEvent ();
    public static EntityEvent OnEntityHurt = new EntityEvent ();
    public static EntityEvent OnEntityKilled = new EntityEvent ();

    public static EntityEvent OnEntitySelected = new EntityEvent ();
    public static EntityEvent OnEntityHoverOn = new EntityEvent ();
    public static EntityEvent OnEntityHoverOff = new EntityEvent ();

    // District events
    public static DistrictEvent OnDistrictEntered = new DistrictEvent ();
    public static DistrictEvent OnDistrictExited = new DistrictEvent ();
    public static DistrictEvent OnDistrictMoved = new DistrictEvent ();
    public static DistrictEvent OnDistrictWrecked = new DistrictEvent ();
    public static DistrictEvent OnDistrictMoveFail = new DistrictEvent ();

    // Movement events
    public static MovementEvent OnPlayerClickMovement = new MovementEvent ();

    public static MovementEvent OnPlayerClickAttack = new MovementEvent ();

    // Game state events
    public static void InvokeOnGameStateChanged (GameState newState) {
        OnGameStateChanged.Invoke (newState);
    }
    public static void InvokeOnSkipPlayerAttack (GameState newState) {
        OnSkipPlayerAttack.Invoke (newState);
    }
    public static void InvokeOnEndPlayerTurn (GameState newState) {
        OnEndPlayerTurn.Invoke (newState);
    }
    // Action event invokes
    public static void InvokeOnActionPerformed (ActionResultArgs args) {
        OnActionPerformed.Invoke (args);
    }
    public static void InvokeOnMonsterMovementCardDrawn (ActionResultArgs args) {
        OnMonsterMovementCardDrawn.Invoke (args);
    }
    public static void InvokeOnMonsterAttackCardDrawn (ActionResultArgs args) {
        OnMonsterAttackCardDrawn.Invoke (args);
    }
    // Entity event invokes
    public static void InvokeOnEntityHurt (EntityEventArgs args) {
        OnEntityHurt.Invoke (args);
    }
    public static void InvokeOnEntitySpawned (EntityEventArgs args) {
        OnEntitySpawned.Invoke (args);
    }
    public static void InvokeOnEntityKilled (EntityEventArgs args) {
        OnEntityKilled.Invoke (args);
    }
    public static void InvokeOnEntitySelected (EntityEventArgs args) {
        OnEntitySelected.Invoke (args);
    }
    public static void InvokeOnEntityHoverOn (EntityEventArgs args) {
        OnEntityHoverOn.Invoke (args);
    }
    public static void InvokeOnEntityHoverOff (EntityEventArgs args) {
        OnEntityHoverOff.Invoke (args);
    }

    // District invoke events
    public static void InvokeOnDistrictEntered (DistrictEventArgs args) {
        OnDistrictEntered.Invoke (args);
    }
    public static void InvokeOnDistrictExited (DistrictEventArgs args) {
        OnDistrictExited.Invoke (args);
    }
    public static void InvokeOnDistrictMoved (DistrictEventArgs args) {
        OnDistrictMoved.Invoke (args);
    }
    public static void InvokeOnDistrictWrecked (DistrictEventArgs args) {
        OnDistrictWrecked.Invoke (args);
    }
    public static void InvokeOnDistrictMoveFailed (DistrictEventArgs args) {
        OnDistrictMoveFail.Invoke (args);
    }

    // Movement invoke events
    public static void InvokeOnPlayerClickMovement (MovementArgs args) {
        OnPlayerClickMovement.Invoke (args);
    }
    public static void InvokeOnPlayerClickAttack (MovementArgs args) {
        OnPlayerClickAttack.Invoke (args);
    }
}