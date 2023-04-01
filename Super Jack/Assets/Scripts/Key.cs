using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Key : Agent
{
    protected override void AgentUpdate()
    {
        if (BoxCollisions(gameObject, superJack.gameObject))
        {
            superJack.hasKey = true;
            manager.Agents.Remove(this);
            Destroy(gameObject);
        }
    }
}
