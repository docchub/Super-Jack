using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Nerve : Agent
{
    protected override void AgentUpdate()
    {
        totalSteeringForce += Seek(superJack.transform.position);

        // Player collision
        if (BoxCollisions(gameObject, superJack.gameObject))
        {
            // nothing yet
        }

        if (Health <= 0)
        {
            manager.Agents.Remove(this);
            superJack.nerveList.Remove(this);
            foreach (PlayerBullet b in bulletList)
            {
                b.nerveList.Remove(this);
            }
            Destroy(gameObject);
        }
    }
}
