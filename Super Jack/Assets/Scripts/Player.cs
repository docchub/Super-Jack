using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;
using UnityEngine.Playables;
using UnityEngine.UIElements;

enum SuperJack
{
    FaceUp,
    FaceDown,
    FaceLeft,
    FaceRight
}

public class Player : Agent
{
    public Animator animator;

    SuperJack currentState;

    [SerializeField]
    PlayerBullet bullet;

    [SerializeField]
    float fireRate = 1f;
    float fireRateTimer;
    bool reloading;
    bool fireButtonReleased;
    Quaternion bulletRotation = Quaternion.identity;

    // Used for flipping player sprite
    SpriteRenderer spriteRenderer;

    AudioSource source;
    public AudioClip bulletSound;
    public List<AudioClip> walkSounds;

    [SerializeField]
    float playerStepVolume = 0.4f;

    void Awake()
    {
        fireRateTimer = fireRate;
        reloading = false;

        // Initialize
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        source = gameObject.AddComponent<AudioSource>();
        source.volume = playerStepVolume;
    }

    protected override void AgentUpdate()
    {
        BulletControl();
        CleanStrayBullets();
        SetWalkType();
        PlayerMovement();

        if (reloading)
        {
            fireRateTimer -= Time.deltaTime;

            // Drop synth when button is held
            if (fireRateTimer <= (fireRate/5) && fireButtonReleased)
            {
                animator.SetBool("Firing", false);
            }

            // Become able to fire again
            if (fireRateTimer <= 0)
            {
                reloading = false;
            }

            // Fire bullet if fire button is held
            if (!reloading && !fireButtonReleased)
            {
                FireBullet();
            }
        }
    }

    /// <summary>
    /// Checks for player inputs
    /// </summary>
    /// <param name="context"></param>
    public void OnMove(InputAction.CallbackContext context)
    {
        Vector2 playerDirection = context.ReadValue<Vector2>();
        ApplyDirection(playerDirection);

        // If speed = 0 --> idle
        animator.SetFloat("Speed", 1f);
        if (context.canceled)
        {
            animator.SetFloat("Speed", 0f);
        }
    }

    /// <summary>
    /// Spawns a bullet
    /// </summary>
    public void Shoot(InputAction.CallbackContext context)
    {
        if (Health > 0)
        {
            fireButtonReleased = false;

            // Animation Logic
            animator.SetBool("Firing", true);

            // Fire button pressed
            if (context.performed && !reloading)
            {
                FireBullet();
            }

            // Fire button released
            if (context.canceled)
            {
                fireButtonReleased = true;

                // Animation Logic
                animator.SetBool("Firing", false);
            }
        }
    }

    void FireBullet()
    {
        // Set reload to true and reset timer
        reloading = true;
        fireRateTimer = fireRate;

        // Create bullet
        manager.Agents.Add(Instantiate(bullet, transform.position, bulletRotation, transform));
        manager.InitAgent(manager.Agents[manager.Agents.Count - 1]);

        // Audio
        source.clip = bulletSound;
        source.Play();
    }

    void CleanStrayBullets()
    {
        // Clean up stray bullets
        if (bulletList.Count > 0)
        {
            foreach (PlayerBullet b in bulletList)
            {
                if (b.transform.position.y > screenHeight ||
                    b.transform.position.y < -screenHeight ||
                    b.transform.position.x > screenWidth ||
                    b.transform.position.x < -screenWidth)
                {
                    Destroy(b.gameObject);
                    bulletList.Remove(b);
                    manager.Agents.Remove(b);
                    return;
                }
            }
        }
    }

    void BulletControl()
    {
        // Enum control
        if (Direction.y == 1)
        {
            currentState = SuperJack.FaceUp;
            SetBulletDirection(new Vector2(0, 1));
        }
        else if (Direction.y == -1)
        {
            currentState = SuperJack.FaceDown;
            SetBulletDirection(new Vector2(0, -1));
        }
        else if (Direction.x == -1)
        {
            currentState = SuperJack.FaceLeft;
            SetBulletDirection(new Vector2(-1, 0));
        }
        else if (Direction.x == 1)
        {
            currentState = SuperJack.FaceRight;
            SetBulletDirection(new Vector2(1, 0));
        }
    }

    void SetBulletDirection(Vector2 vector)
    {
        bulletRotation = Quaternion.LookRotation(Vector3.forward, vector);
    }

    /// <summary>
    /// Controls animation variable for walk cycles
    /// </summary>
    void SetWalkType()
    {
        // Animation state logic
        // 0 = Left/Right
        // 1 = Down
        // 2 = Up
        if (currentState == SuperJack.FaceUp)
        {
            animator.SetInteger("WalkType", 2);
        }
        else if (currentState == SuperJack.FaceDown)
        {
            animator.SetInteger("WalkType", 1);
        }
        else if (currentState == SuperJack.FaceLeft)
        {
            animator.SetInteger("WalkType", 0);
            spriteRenderer.flipX = false;
        }
        else if (currentState == SuperJack.FaceRight)
        {
            animator.SetInteger("WalkType", 0);
            spriteRenderer.flipX = true;
        }
    }

    void PlayWalkClip(int index)
    {
        source.clip = walkSounds[index];
        source.Play();
    }
}