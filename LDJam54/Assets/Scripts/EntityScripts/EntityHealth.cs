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
        Debug.Log ("[EntityHealth] Set entity " + Entity.m_data.ID + " to " + Health);
    }

    public bool IsDead {
        get {
            return m_currentHealth <= 0;
        }
    }

    public void Damage (ActionResultArgs args) {
        int damage = args.intVal;
        Health -= damage;
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
                Entity.m_avatarDestroyed?.SetActive (true); // this is a horrible way of doing this
            }
            if (value < m_currentHealth) {
                GlobalEvents.InvokeOnEntityHurt (new EntityEventArgs (Entity, null, this, m_currentHealth - value));
            }
            m_currentHealth = value;
        }
    }

}