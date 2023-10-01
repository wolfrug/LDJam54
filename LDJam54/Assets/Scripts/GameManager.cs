using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState {
   NONE = 0000,
   GAME = 1000,
   NARRATIVE = 2000,
   ENEMY_TURN = 3000,
   PLAYER_TURN = 4000,
   CLEAN_UP = 5000,
   SET_UP = 6000,
}

public class GameManager : MonoBehaviour {

   public List<Entity> m_playerEntities = new List<Entity> { };
   public List<Entity> m_enemyEntities = new List<Entity> { };
   private static GameState m_currentState = GameState.PLAYER_TURN;
   void Start () {
      DistrictManager.instance.Init ();
      EntityManager.instance.Init ();
      GridCameraController.instance.mouseLeftClickEvent.AddListener (OnTileClick);
      GlobalEvents.OnEntityKilled.AddListener (OnEntityDeath);
      StartNewGame ();
   }

   public void StartNewGame () {
      State = GameState.SET_UP;
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

   }

   void OnTileClick (TileInfo info) {

   }

   public void SpawnMech (EntityType type) {
      // Place Mech
      Entity mech = EntityManager.instance.SpawnEntity (type);
      DistrictManager.instance.SetEntityLocation (mech, GridLocations.B2);
      m_playerEntities.Add (mech);
   }

   void OnEntityDeath (EntityEventArgs entity) {
      if (m_playerEntities.Contains (entity.owner)) {
         m_playerEntities.Remove (entity.owner);
      }

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