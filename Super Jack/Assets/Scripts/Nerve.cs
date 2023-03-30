using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Nerve : Agent
{
    public Animator animator;

    // Used for flipping sprites
    SpriteRenderer spriteRenderer;

    private void Awake()
    {
        // Initialize spriterenderer
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
    }

    protected override void AgentUpdate()
    {
        totalSteeringForce += Seek(superJack.transform.position);

        // Player collision
        if (BoxCollisions(gameObject, superJack.gameObject))
        {
            // nothing yet
        }

        // Animation Logic
        Vector2 desiredVelocity = superJack.transform.position - Position;
        if (Mathf.Abs(desiredVelocity.x) > Mathf.Abs(desiredVelocity.y))
        {
            animator.SetBool("horizontal", true);

            // Flip sprite logic
            if (desiredVelocity.x > 0)
            {
                spriteRenderer.flipX = true;
            }
            else
            {
                spriteRenderer.flipX = false;
            }

        }
        else if (Mathf.Abs(desiredVelocity.x) < Mathf.Abs(desiredVelocity.y))
        {
            animator.SetBool("horizontal", false);

            // Flip vertically moving sprite
            if (desiredVelocity.y > 0)
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
