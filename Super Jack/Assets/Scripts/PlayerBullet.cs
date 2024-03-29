using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerBullet : Agent
{
    [SerializeField]
    GameObject particles;

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
                    SpawnParticles(particles);

                    bulletList.Remove(this);
                    superJack.bulletList.Remove(this);
                    manager.Agents.Remove(this);
                    Destroy(gameObject);

                    nerve.Hurt();
                }
            }
        }

        // If there's a brain, check for collisions against it
        if (brain)
        {
            // Check for collisions against the brain
            if (BoxCollisions(gameObject, brain.ActiveHitbox()))
            {
                // Spawn hit particles
                SpawnParticles(particles);

                bulletList.Remove(this);
                superJack.bulletList.Remove(this);
                manager.Agents.Remove(this);
                Destroy(gameObject);

                // Subtract brain health
                brain.Hurt();
            }
        }

        // If there's a normal jack check for collisions against it
        if (normalJack)
        {
            if (BoxCollisions(gameObject, normalJack.gameObject) && normalJack.Health > 0)
            {
                // Spawn hit particles
                SpawnParticles(particles);

                bulletList.Remove(this);
                superJack.bulletList.Remove(this);
                manager.Agents.Remove(this);
                Destroy(gameObject);

                normalJack.Health--;
            }
        }
    }

    /// <summary>
    /// Drop particles on hit
    /// </summary>
    /// <param name="particles"></param>
    void SpawnParticles(GameObject particles)
    {
        Instantiate(particles, Position, Quaternion.identity);
    }

    protected override void AgentUpdate()
    {
        throw new NotImplementedException();
    }
}