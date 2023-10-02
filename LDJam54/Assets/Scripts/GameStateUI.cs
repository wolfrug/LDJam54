using System.Collections;
using System.Collections.Generic;
using InkEngine;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameStateUI : MonoBehaviour {
    // Start is called before the first frame update
    public InkWriter m_inkWriter;
    public TextMeshProUGUI m_wreckedDistrictsText;
    public Button m_endTurnButton;
    private int m_wreckedDistricts = 0;
    void Start () {
        GlobalEvents.OnGameStateChanged.AddListener (OnGameStateChanged);
        GlobalEvents.OnDistrictWrecked.AddListener (OnDistrictWrecked);
        m_endTurnButton.onClick.AddListener (OnClickEndTurn);
    }

    void OnGameStateChanged (GameState newState) {
        m_endTurnButton.interactable = (newState == GameState.PLAYER_TURN);
        m_inkWriter.PlayKnot (newState.ToString ());
    }

    void OnDistrictWrecked (DistrictEventArgs args) {
        m_wreckedDistricts++;
        m_wreckedDistrictsText.SetText (m_wreckedDistricts.ToString ());
    }

    void OnClickEndTurn () {
        GlobalEvents.InvokeOnEndPlayerTurn (GameState.PLAYER_TURN);
    }
}