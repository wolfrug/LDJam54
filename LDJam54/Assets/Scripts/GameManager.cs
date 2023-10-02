using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState {
   NONE = 0000,
   GAME = 1000,
   NARRATIVE = 2000,
   ENEMY_TURN = 3000,
   PLAYER_TURN_ATTACK = 4000,
   PLAYER_TURN_AUTOATTACK = 4003,
   PLAYER_TURN_MOVEMENT = 4001,
   PLAYER_TURN_EVENT = 4002,
   CLEAN_UP = 5000,
   SET_UP = 6000,
}

public class GameManager : MonoBehaviour {

   public List<Entity> m_playerEntities = new List<Entity> { };
   public List<Entity> m_enemyEntities = new List<Entity> { };
   private static GameState m_currentState = GameState.SET_UP;
   public static Entity m_currentlySelectedEntity = null;
   public bool m_playerMechIsAlive = false;
   public Entity m_playerMech;
   public Transform cameraFocusPoint;
   void Start () {
      DistrictManager.instance.Init ();
      EntityManager.instance.Init ();
      GridCameraController.instance.mouseLeftClickEvent.AddListener (OnTileClick);
      GlobalEvents.OnEntityKilled.AddListener (OnEntityDeath);
      GlobalEvents.OnEntitySelected.AddListener (OnEntitySelected);
      GlobalEvents.OnEndPlayerTurn.AddListener (OnEndPlayerTurn);
      GlobalEvents.OnSkipPlayerAttack.AddListener (OnSkipAttack);
      GlobalEvents.OnDistrictEntered.AddListener (OnEnemyMoved);
      StartCoroutine (StartNewGame ());
   }

   public IEnumerator StartNewGame () {
      State = GameState.SET_UP;
      GridCameraController.instance.StartPanCamera ();
      yield return new WaitForSeconds (1f);
      // Place tanks
      // Random D
      GridLocations randomDColumn = new List<GridLocations> { GridLocations.D1, GridLocations.D2, GridLocations.D3, GridLocations.D4, GridLocations.D5 }[Random.Range (0, 4)];
      Entity tank1 = EntityManager.instance.SpawnEntity (EntityType.TANK);
      DistrictManager.instance.SetEntityLocation (tank1, randomDColumn);
      m_playerEntities.Add (tank1);
      // Random E
      GridLocations randomEColumn = new List<GridLocations> { GridLocations.E1, GridLocations.E2, GridLocations.E3, GridLocations.E4, GridLocations.E5 }[Random.Range (0, 4)];
      Entity tank2 = EntityManager.instance.SpawnEntity (EntityType.TANK);
      DistrictManager.instance.SetEntityLocation (tank2, randomEColumn);
      m_playerEntities.Add (tank2);
      // Random F
      GridLocations randomFColumn = new List<GridLocations> { GridLocations.F1, GridLocations.F2, GridLocations.F3, GridLocations.F4, GridLocations.F5 }[Random.Range (0, 4)];
      Entity tank3 = EntityManager.instance.SpawnEntity (EntityType.TANK);
      DistrictManager.instance.SetEntityLocation (tank3, randomFColumn);
      m_playerEntities.Add (tank3);
      // Random G
      GridLocations randomGColumn = new List<GridLocations> { GridLocations.G1, GridLocations.G2, GridLocations.G3, GridLocations.G4, GridLocations.G5 }[Random.Range (0, 4)];
      Entity tank4 = EntityManager.instance.SpawnEntity (EntityType.TANK);
      DistrictManager.instance.SetEntityLocation (tank4, randomGColumn);
      m_playerEntities.Add (tank4);

      // Place monster
      GridLocations randomMonsterSpawn = new List<GridLocations> { GridLocations.A1, GridLocations.A2, GridLocations.A3, GridLocations.A4, GridLocations.A5 }[Random.Range (0, 4)];
      Entity monster = EntityManager.instance.SpawnEntity (EntityType.MONSTER_CERATOLISK);
      DistrictManager.instance.SetEntityLocation (monster, randomMonsterSpawn);
      m_enemyEntities.Add (monster);
      yield return new WaitForSeconds (0.5f);
      // Change state

      StartCoroutine (EnemyTurn ());
   }

   void OnTileClick (TileInfo info) {
      Debug.Log ("[GameManager] Clicked tile " + info.location);
      foreach (Entity entity in m_playerEntities) {
         entity.m_playerMovementController.ResetArrows ();
      }

   }
   void OnEntitySelected (EntityEventArgs args) {

      if (State == GameState.PLAYER_TURN_ATTACK) {
         if (args.owner == m_playerMech) {
            args.owner.m_playerMovementController.SetAttackDirections (args.owner.entityAttack.GetPermittedAttackDirections ());
         }
      }

      if (State == GameState.PLAYER_TURN_MOVEMENT) {
         if (args.owner != m_currentlySelectedEntity && m_currentlySelectedEntity != null) {
            m_currentlySelectedEntity?.m_playerMovementController.ResetArrows ();
         }
         if (args.owner.m_data.m_faction == EntityFaction.PLAYER) {
            if (args.owner.entityMovement.MovementLeft > 0) {
               args.owner.m_playerMovementController.SetActiveDirections (args.owner.entityMovement.GetPermittedMovementDirections ());
               args.owner.m_playerMovementController.SetAttackDirections (args.owner.entityAttack.GetPermittedAttackDirections ());
               m_currentlySelectedEntity = args.owner;
            }
         }
      }
   }
   // For when spawning the mech on the monster first leaving the ocean
   void OnEnemyMoved (DistrictEventArgs args) {
      if (args.instigator.m_data.m_faction == EntityFaction.ENEMY && m_playerMech == null && args.owner.m_data.m_type != DistrictType.OCEAN) {
         SpawnMech (EntityType.MELEE_MECH);
      }
   }

   public void SpawnMech (EntityType type) {
      // Place Mech
      Entity mech = EntityManager.instance.SpawnEntity (type);
      DistrictManager.instance.SetEntityLocation (mech, GridLocations.B2);
      m_playerEntities.Add (mech);
      m_playerMech = mech;
      m_playerMechIsAlive = true;
   }

   void OnEntityDeath (EntityEventArgs entity) {
      if (m_playerEntities.Contains (entity.owner)) {
         m_playerEntities.Remove (entity.owner);
         if (entity.owner == m_playerMech) {
            m_playerMechIsAlive = false;
         }
      }
   }

   void OnSkipAttack (GameState newState) {
      State = GameState.PLAYER_TURN_AUTOATTACK;
   }

   void OnEndPlayerTurn (GameState newState) {
      // For now we just do this
      foreach (Entity playerEntity in m_playerEntities) {
         playerEntity.entityMovement.ResetMovement ();
         playerEntity.entityAttack.ResetAttacksLeft ();
         playerEntity.entityEffects.ClearAllEffects ();
      }
      foreach (Entity enemyEntity in m_enemyEntities) {
         enemyEntity.entityMovement.ResetMovement ();
         enemyEntity.entityAttack.ResetAttacksLeft ();
         enemyEntity.entityEffects.ClearAllEffects ();
      }

      StartCoroutine (EnemyTurn ());
   }

   IEnumerator PlayerTurn () {
      State = GameState.PLAYER_TURN_ATTACK;
      GridCameraController.instance.SetPanCameraTarget (cameraFocusPoint, true);
      GridCameraController.instance.SetZoomLevel (25f);
      if (m_playerMechIsAlive) {
         yield return new WaitUntil (() => State == GameState.PLAYER_TURN_AUTOATTACK);
      } else {
         State = GameState.PLAYER_TURN_AUTOATTACK;
      }
      // Do airstrikes here
      yield return new WaitForSeconds (1f);
      // Auto-attack with tanks
      foreach (Entity tank in m_playerEntities.FindAll ((x) => x.m_data.m_type == EntityType.TANK)) {
         if (tank.entityAttack.AttacksLeft > 0 && tank.entityAttack.GetPermittedAttackDirections ().Count > 0) {
            tank.entityAttack.AutoAttackRandomAdjacentEnemy ();
            yield return new WaitForSeconds (1f);
         }
      }
      // Movemement phase
      State = GameState.PLAYER_TURN_MOVEMENT;
   }

   IEnumerator EnemyTurn () {
      State = GameState.ENEMY_TURN;
      foreach (Entity enemyEntity in m_enemyEntities) {
         //Camera
         GridCameraController.instance.SetPanCameraTarget (enemyEntity.transform, true);
         GridCameraController.instance.SetZoomLevel (15f);
         yield return new WaitForSeconds (0.5f);
         if (enemyEntity.entityEffects.HasEffect (EffectType.STUN)) { // Skip all if stunned
            enemyEntity.entityEffects.RemoveEffect (EffectType.STUN);
            break;
         }
         // Ceratolisk has 2x movement in Ocean
         if (enemyEntity.m_data.m_type == EntityType.MONSTER_CERATOLISK) {
            if (enemyEntity.entityMovement.m_currentDistrict.m_data.m_type == DistrictType.OCEAN) {
               enemyEntity.entityMovement.MovementLeft++;
            }
         }
         // Movememnt
         while (enemyEntity.entityMovement.MovementLeft > 0 && enemyEntity.entityMovement.CanMove) {

            enemyEntity.entityMovement.PerformRandomMovementAction ();
            enemyEntity.entityMovement.MovementLeft--;
            yield return new WaitForSeconds (1f);
         }
         // Attack
         while (enemyEntity.entityAttack.AttacksLeft > 0) {
            enemyEntity.entityAttack.PerformRandomAttackAction ();
            enemyEntity.entityAttack.AttacksLeft--;
            yield return new WaitForSeconds (1f);
         }
      }
      yield return new WaitForSeconds (1f);
      // All done, change turn!
      StartCoroutine (PlayerTurn ());
   }

   public static GameState State {
      get {
         return m_currentState;
      }
      set {
         if (m_currentState != value) {
            GlobalEvents.InvokeOnGameStateChanged (value);
         }
         m_currentState = value;
      }
   }
}