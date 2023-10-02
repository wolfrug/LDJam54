using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerControlUI : MonoBehaviour {
    // Start is called before the first frame update

    private Coroutine m_playerTurn = null;
    void Start () {
        GlobalEvents.OnGameStateChanged.AddListener (OnGameStateChanged);
    }

    void OnGameStateChanged (GameState newstate) {
        if (newstate == GameState.PLAYER_TURN_MOVEMENT) {
            m_playerTurn = StartCoroutine (PlayerTurn ());
        } else {
            if (m_playerTurn != null) {
                {
                    StopCoroutine (m_playerTurn);
                    m_playerTurn = null;
                }
            }
        }
    }

    IEnumerator PlayerTurn () {
        yield return null;
    }

}