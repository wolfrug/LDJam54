using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;

public class EntityEffects : EntityComponent {

    public Animator animator;

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
}