using System.Collections.Generic;
using UnityEngine;

public class MinecraftWorld : MonoBehaviour
{
    private Dictionary<int, Entity> entities = new Dictionary<int, Entity>();
    public Queue<EntityInfoMessage> updates = new Queue<EntityInfoMessage>();
    
    public Entity entityObject;
    public Entity playerObject;
    public Light sun;
    private float time;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        sun.transform.rotation = Quaternion.identity;
        sun.transform.Rotate(Vector3.up, 90);
        sun.transform.Rotate(Vector3.right, time * 0.015f);

        while (updates.Count > 0)
        {
            EntityInfoMessage entityInfo = updates.Dequeue();
            
            Entity entity;
            if (!entities.ContainsKey(entityInfo.id))
            {
                if (entityInfo.type == "player")
                {
                    entity = Instantiate(playerObject, transform);
                }
                else
                {
                    entity = Instantiate(entityObject, transform);
                }

                entities.Add(entityInfo.id, entity);
                entity.id = entityInfo.id;
                entity.entityName = entityInfo.name;
                entity.type = entityInfo.type;
            }
            else
            {
                entity = entities[entityInfo.id];
            }

            entity.position = entityInfo.position;
            entity.pitch = entityInfo.pitch * 180f / Mathf.PI;
            entity.yaw = entityInfo.yaw * 180f / Mathf.PI;
        }
    }

    public void SetTimeOfDay(int ticks)
    {
        time = ticks;
    }
}
