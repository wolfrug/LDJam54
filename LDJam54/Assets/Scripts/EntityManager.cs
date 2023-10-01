using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityManager : MonoBehaviour {
    public static EntityManager instance;

    private List<EntityData> m_entityDatas = new List<EntityData> { };

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
        Object[] loadedDatas = Resources.LoadAll ("Data/Entitites", typeof (EntityData));
        foreach (Object obj in loadedDatas) {
            m_entityDatas.Add (obj as EntityData);
        }
    }
}