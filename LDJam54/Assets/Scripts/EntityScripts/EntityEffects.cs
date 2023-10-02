using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;

public class EntityEffects : EntityComponent {

    public Animator animator;
    public List<EntityEffectData> m_effects = new List<EntityEffectData> { };
    private Dictionary<EntityEffectData, GameObject> m_effectDictionary = new Dictionary<EntityEffectData, GameObject> { };

    protected override void Init () {
        base.Init ();
        animator = Entity.gameObject.GetComponent<Animator> ();
        if (animator == null) {
            animator = Entity.gameObject.AddComponent<Animator> ();
        }
        if (Entity.m_data.m_animator != null) {
            animator.runtimeAnimatorController = Entity.m_data.m_animator;
        }
        GlobalEvents.OnEntityHoverOn.AddListener (OnEntityHoverOn);
        GlobalEvents.OnEntityHoverOff.AddListener (OnEntityHoverOff);
    }

    void OnEntityHoverOn (EntityEventArgs args) {
        if (args.owner == Entity) {
            animator?.SetBool ("isHighlighted", true);
        }
    }
    void OnEntityHoverOff (EntityEventArgs args) {
        if (args.owner == Entity) {
            animator?.SetBool ("isHighlighted", false);
        }
    }

    public void AddEffect (EntityEffectData data) {
        if (!m_effects.Contains (data)) {
            m_effects.Add (data);
            if (m_effectDictionary.ContainsKey (data)) {
                m_effectDictionary[data].SetActive (true);
            } else {
                m_effectDictionary.Add (data, data.SpawnVisualEffect (new EntityEffectArgs (Entity)));
            }

        }
    }
    public void AddEffect (EffectType type) {
        EntityEffectData data = EntityManager.instance.GetEffectData (type);
        if (data != null) {
            AddEffect (data);
        }
    }
    public void RemoveEffect (EntityEffectData data) {
        if (m_effects.Contains (data)) {
            m_effects.Remove (data);
            m_effectDictionary[data].SetActive (false);
        }
    }
    public void RemoveEffect (EffectType type) {
        EntityEffectData data = EntityManager.instance.GetEffectData (type);
        if (data != null) {
            RemoveEffect (data);
        }
    }
    public bool HasEffect (EffectType type) {
        if (m_effects.Find ((x) => x.m_type == type) != null) {
            return true;
        } else {
            return false;
        }
    }
    public void ClearAllEffects () {
        foreach (EntityEffectData data in new List<EntityEffectData> (m_effects)) {
            RemoveEffect (data);
        }
    }

    [NaughtyAttributes.Button]
    void TestStun () {
        AddEffect (EffectType.STUN);
    }
}