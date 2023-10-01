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
}

public class GameManager : MonoBehaviour {

   private static GameState m_currentState = GameState.PLAYER_TURN;
   void Start () {
      DistrictManager.instance.Init ();

   }

   public static GameState State {
      get {
         return m_currentState;
      }
      set {
         m_currentState = value;
      }
   }
}