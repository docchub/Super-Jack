using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;
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

    public bool hasKey;
    bool prevHasKey;

    Quaternion bulletRotation = Quaternion.identity;

    // Used for flipping player sprite
    SpriteRenderer spriteRenderer;

    AudioSource source;
    public AudioClip bulletSound;
    public AudioClip hurtSound;
    public AudioClip keySound;
    public AudioClip chompSound;
    public List<AudioClip> walkSounds;

    [SerializeField]
    float playerSfxVolume = 0.4f;

    Color playerColor;

    [SerializeField]
    float flickerTime = 0.01f;

    bool eating;
    bool canEat;

    [SerializeField]
    float eatingTime = 8f;

    void Awake()
    {
        fireRateTimer = fireRate;
        reloading = false;
        hasKey = false;
        prevHasKey = false;
        eating = false;
        canEat = false;

        // Initialize
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        source = gameObject.AddComponent<AudioSource>();
        source.volume = playerSfxVolume;

        // Damage effects
        playerColor = gameObject.GetComponent<SpriteRenderer>().color;
    }

    protected override void AgentUpdate()
    {
        if (!eating)
        {
            BulletControl();
            CleanStrayBullets();
            SetWalkType();
            PlayerMovement();

            if (reloading)
            {
                fireRateTimer -= Time.deltaTime;

                // Drop synth when button is held
                if (fireRateTimer <= (fireRate / 5) && fireButtonReleased)
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

            // If player picks up the key, play a ring
            if (hasKey && !prevHasKey)
            {
                source.clip = keySound;
                source.Play();
            }

            prevHasKey = hasKey;

            // If player dies go to GameOver scene
            if (Health <= 0)
            {
                GameOver();
            }

            // Can start eating when the brain dies
            if (brain.Health <= 0)
            {
                canEat = true;
            }
        }

        // JACK EATS THE BRAIN
        else
        {
            // Play credits after X seconds
            eatingTime -= Time.deltaTime;
            if (eatingTime <= 0)
            {
                SceneManager.LoadScene(6);
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
        if (Health > 0 && !eating)
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

    /// <summary>
    /// Start eating brain on key press
    /// </summary>
    public void EatBrain(InputAction.CallbackContext context)
    {
        // Start brain eating animation
        if (canEat && !eating)
        {
            eating = true;
            animator.SetBool("Eating", true);
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

    /// <summary>
    /// Controls walk sounds
    /// </summary>
    /// <param name="index"></param>
    void PlayWalkClip(int index)
    {
        source.clip = walkSounds[index];
        source.Play();
    }

    public void Hurt()
    {
        Health--;
        StopCoroutine(DamageEffect());
        StartCoroutine(DamageEffect());
        source.clip = hurtSound;
        source.Play();
    }

    void GameOver()
    {
        // Implement death sound + pause

        SceneManager.LoadScene(5);
    }

    IEnumerator DamageEffect()
    {
        playerColor.a = 0.5f;
        gameObject.GetComponent<SpriteRenderer>().color = playerColor;
        yield return new WaitForSeconds(flickerTime);

        playerColor.a = 1f;
        gameObject.GetComponent<SpriteRenderer>().color = playerColor;
        yield return new WaitForSeconds(flickerTime);

        playerColor.a = 0.5f;
        gameObject.GetComponent<SpriteRenderer>().color = playerColor;
        yield return new WaitForSeconds(flickerTime);

        playerColor.a = 1f;
        gameObject.GetComponent<SpriteRenderer>().color = playerColor;
        yield return new WaitForSeconds(flickerTime);

        playerColor.a = 0.5f;
        gameObject.GetComponent<SpriteRenderer>().color = playerColor;
        yield return new WaitForSeconds(flickerTime);

        playerColor.a = 1f;
        gameObject.GetComponent<SpriteRenderer>().color = playerColor;

        StopCoroutine(DamageEffect());
    }

    void Chomp()
    {
        source.clip = chompSound; 
        source.Play();
    }

    void ChompingTeleport()
    {
        Position = brain.Position;
        Position += RandomVector();
        spriteRenderer.flipX = !spriteRenderer.flipX;
    }

    Vector3 RandomVector()
    {
        return new Vector2(Random.Range(-2.0f, 1.0f), Random.Range(-2.0f, 1.0f));
    }
}