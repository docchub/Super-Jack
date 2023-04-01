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

    AudioSource source;

    [SerializeField]
    AudioClip hurtSound;
    [SerializeField]
    AudioClip deathSound;

    Color nerveColor;

    [SerializeField]
    float flickerTime = 0.01f;

    private void Awake()
    {
        timeRemaining = initialTimeRemaining;

        // Initialize spriterenderer
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();

        source = gameObject.AddComponent<AudioSource>();

        // Damage effects
        nerveColor = gameObject.GetComponent<SpriteRenderer>().color;
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
                superJack.Hurt();
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
            timeRemaining -= Time.deltaTime;

            manager.Agents.Remove(this);
            superJack.nerveList.Remove(this);
            foreach (PlayerBullet b in bulletList)
            {
                b.nerveList.Remove(this);
            }

            // Pause before deleting to let death sound play
            if (timeRemaining <= 0)
            {
                Destroy(gameObject);
            }
        }
    }

    public void Hurt()
    {
        Health--;
        StopCoroutine(DamageEffect());
        StartCoroutine(DamageEffect());

        if (Health > 0)
        {
            source.clip = hurtSound;
            source.Play();
        }
        else
        {
            source.clip = deathSound;
            source.Play();
            timeRemaining = 0.5f;
        }
    }

    IEnumerator DamageEffect()
    {
        nerveColor.a = 0.5f;
        gameObject.GetComponent<SpriteRenderer>().color = nerveColor;
        yield return new WaitForSeconds(flickerTime);

        nerveColor.a = 1f;
        gameObject.GetComponent<SpriteRenderer>().color = nerveColor;
        yield return new WaitForSeconds(flickerTime);

        nerveColor.a = 0.5f;
        gameObject.GetComponent<SpriteRenderer>().color = nerveColor;
        yield return new WaitForSeconds(flickerTime);

        nerveColor.a = 1f;
        gameObject.GetComponent<SpriteRenderer>().color = nerveColor;
        yield return new WaitForSeconds(flickerTime);

        nerveColor.a = 0.5f;
        gameObject.GetComponent<SpriteRenderer>().color = nerveColor;
        yield return new WaitForSeconds(flickerTime);

        nerveColor.a = 1f;
        gameObject.GetComponent<SpriteRenderer>().color = nerveColor;

        StopCoroutine(DamageEffect());
    }
}
