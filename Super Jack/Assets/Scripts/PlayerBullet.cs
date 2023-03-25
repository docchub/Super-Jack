using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerBullet : Agent
{
    // Start is called before the first frame update
    void Awake()
    {
        Position = transform.position;
        Direction = transform.rotation * new Vector2(0, 1);
    }

    void Update()
    {
        // Update veloctity
        Velocity = Direction * Speed * Time.deltaTime;

        // Add velocity to our current position
        Position += Velocity;

        // Update drawn position
        transform.position = Position;

        // If there are nerves, check if this bullet collides with them
        if (nerveList.Count > 0)
        {
            foreach (Nerve nerve in nerveList)
            {
                if (BoxCollisions(gameObject, nerve.gameObject))
                {
                    bulletList.Remove(this);
                    //Destroy(gameObject);
                }
            }
        }
    }

    protected override void AgentUpdate()
    {
        throw new NotImplementedException();
    }
}