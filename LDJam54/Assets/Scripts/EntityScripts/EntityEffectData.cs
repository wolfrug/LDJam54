using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public enum EffectType {
    NONE = 0000,
    STUN = 1000,
    CLIMBED_BUILDING = 2000,
    REAR_BACK = 3000,
    HEATED = 4000,
    FRENZIED = 5000,
    HASTED = 6000,
    DAMAGE_AT_END_OF_TURN = 7000,
}

public struct EntityEffectArgs {
    public Entity owner;
    public Entity instigator;
    public EntityEffectData source;
    public VisualEffectAsset vfxAsset;

    public EntityEffectArgs (Entity setOwner = null, Entity setInstigator = null, EntityEffectData setData = null, VisualEffectAsset setVfxAsset = null) {
        owner = setOwner;
        instigator = setInstigator;
        source = setData;
        vfxAsset = setVfxAsset;
    }
}

[CreateAssetMenu (fileName = "Data", menuName = "Effect Data", order = 1)]
public class EntityEffectData : ScriptableObject {

    public string ID => this.name;
    public string m_displayName;
    public GameObject fXAsset;
    public EffectType m_type = EffectType.NONE;

    public virtual EntityEffectArgs Perform (EntityEffectArgs args) {
        return new EntityEffectArgs ();
    }

    public GameObject SpawnVisualEffect (EntityEffectArgs args) {
        GameObject spawnedAsset = null;
        if (fXAsset != null) {
            spawnedAsset = Instantiate (fXAsset, args.owner.transform);
        }
        return spawnedAsset;
    }

}