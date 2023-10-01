using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityManager : MonoBehaviour {
    public static EntityManager instance;
    private List<EntityData> m_entityDatas = new List<EntityData> { };
    public List<Entity> m_spawnedEntities = new List<Entity> { };

    void Awake () {
        if (instance == null) {
            instance = this;
            LoadAllEntityDatas ();
        } else {
            Destroy (gameObject);
        }
    }

    void LoadAllEntityDatas () {
        // Load all inventory datas!
        m_entityDatas.Clear ();
        Object[] loadedDatas = Resources.LoadAll ("Data/Entities", typeof (EntityData));
        foreach (Object obj in loadedDatas) {
            m_entityDatas.Add (obj as EntityData);
        }
        Debug.Log ("Loaded entitydatas: " + m_entityDatas.Count);
    }

    public Entity SpawnEntity (EntityData data) {
        GameObject spawnedGO = Instantiate (data.m_prefab, transform);
        Entity entity = spawnedGO.GetComponent<Entity> ();
        entity.Init (data);
        m_spawnedEntities.Add (entity);
        GlobalEvents.InvokeOnEntitySpawned (new EntityEventArgs (entity));
        return entity;
    }
    public Entity SpawnEntity (EntityType type) {
        EntityData dataForType = m_entityDatas.Find ((x) => x.m_type == type);
        if (dataForType != null) {
            return SpawnEntity (dataForType);
        }
        Debug.LogWarning ("Could not find entity data of type " + type);
        return null;
    }

    public void Init () {

    }
}