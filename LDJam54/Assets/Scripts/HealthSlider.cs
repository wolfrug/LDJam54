using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthSlider : MonoBehaviour {
    public Image m_healthSlider;
    public Entity m_targetEntity;
    // Start is called before the first frame update
    void Start () {
        GlobalEvents.OnEntityHurt.AddListener (UpdateSlider);
    }
    void UpdateSlider (EntityEventArgs args) {
        if (args.owner == m_targetEntity) {
            m_healthSlider.fillAmount = (float) m_targetEntity.entityHealth.Health / (float) m_targetEntity.entityHealth.m_maxHealth;
        }
    }

}