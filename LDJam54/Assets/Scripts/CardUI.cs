using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardUI : MonoBehaviour {

    void Start () {
        GlobalEvents.OnMonsterMovementCardDrawn.AddListener (SetTestText);
    }

    void SetTestText (ActionResultArgs args) {
        //m_testText.SetText (args.owner.m_data.ID + " drew card " + args.performedAction.ID);
    }

}