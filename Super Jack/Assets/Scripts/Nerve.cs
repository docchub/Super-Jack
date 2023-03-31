using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Nerve : Agent
{
    public Animator animator;

    // Used for flipping sprites
    SpriteRenderer spriteRenderer;

    Vector2 vecToPlayer;
    bool onCooldown;

    [SerializeField]
    float initialTimeRemaining = 2f;
    float timeRemaining;

    private void Awake()
    {
        timeRemaining = initialTimeRemaining;

        // Initialize spriterenderer
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
    }

    protected override void AgentUpdate()
    {
        // Player can only be hit by the nerve once before a cooldown starts
        if (onCooldown)
        {
            totalSteeringForce += Flee(superJack.transform.position) * 10f;
            timeRemaining -= Time.deltaTime;
            if (timeRemaining <= 0)
            {
                onCooldown = false;
            }
        }
        else
        {
            totalSteeringForce += Seek(superJack.transform.position);
            totalSteeringForce += Separation();

            // Player collision
            if (BoxCollisions(gameObject, superJack.gameObject) && !onCooldown)
            {
                superJack.Health--;
                onCooldown = true;
                timeRemaining = initialTimeRemaining;
            }
        }

        // Animation Logic
        vecToPlayer = superJack.transform.position - Position;

        // Set animation based on which direction has more weight
        // --- HORIZONTAL ---
        if (Mathf.Abs(vecToPlayer.x) > Mathf.Abs(vecToPlayer.y))
        {
            animator.SetBool("horizontal", true);

            // Flip sprite logic
            if (vecToPlayer.x > 0)
            {
                spriteRenderer.flipX = true;
            }
            else
            {
                spriteRenderer.flipX = false;
            }

        }

        // --- VERTICAL ---
        else if (Mathf.Abs(vecToPlayer.x) < Mathf.Abs(vecToPlayer.y))
        {
            animator.SetBool("horizontal", false);

            // Flip vertically moving sprite
            if (vecToPlayer.y > 0)
            {
                spriteRenderer.flipY = true;
            }
            else
            {
                spriteRenderer.flipY = false;
            }
        }

        // If killed, delete the nerve
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
