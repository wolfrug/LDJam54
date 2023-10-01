using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityComponent : MonoBehaviour {
   public Entity Entity {
      get;
      set;
   }
   protected virtual void Awake () {
      Entity = GetComponent<Entity> ();
      Init ();
   }
   protected virtual void Init () {

   }
}