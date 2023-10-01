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
    private int m_wreckedDistricts = 0;
    void Start () {
        GlobalEvents.OnGameStateChanged.AddListener (OnGameStateChanged);
        GlobalEvents.OnDistrictWrecked.AddListener (OnDistrictWrecked);
    }

    void OnGameStateChanged (GameState newState) {
        m_inkWriter.PlayKnot (newState.ToString ());
    }

    void OnDistrictWrecked (DistrictEventArgs args) {
        m_wreckedDistricts++;
        m_wreckedDistrictsText.SetText (m_wreckedDistricts.ToString ());
    }
}