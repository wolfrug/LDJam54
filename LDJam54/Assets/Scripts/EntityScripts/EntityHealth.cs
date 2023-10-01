using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityHealth : EntityComponent {

    private int m_currentHealth = 1;
    private int m_maxHealth = 1;

    protected override void Init () {
        base.Init ();
        Health = Entity.m_data.m_health;
        m_maxHealth = Health;
    }

    public bool IsDead {
        get {
            return m_currentHealth <= 0;
        }
    }

    public int Health {
        get {
            return m_currentHealth;
        }
        set {
            Debug.Log ("[EntityHealth] New health value: " + value);
            if (!IsDead && value <= 0) {
                Debug.Log ("[EntityHealth] " + Entity.m_data.ID + " killed!");
                GlobalEvents.InvokeOnEntityKilled (new EntityEventArgs (Entity));
            }
            if (value < m_currentHealth) {
                GlobalEvents.InvokeOnEntityHurt (new EntityEventArgs (Entity, null, this, m_currentHealth - value));
            }
            m_currentHealth = Mathf.Clamp (value, 0, m_maxHealth);
        }
    }

}