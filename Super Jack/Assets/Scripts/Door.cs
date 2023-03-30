using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : Agent
{
    protected override void AgentUpdate()
    {
        if (BoxCollisions(gameObject, superJack.gameObject))
        {
            Debug.Log("CHANGE ROOM");
            // go to next/prev room
        }
    }
}