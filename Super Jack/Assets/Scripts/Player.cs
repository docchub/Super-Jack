using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
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
    GameObject bullet;
    List<GameObject> bullets = new List<GameObject>();

    [SerializeField]
    float fireRate = 1f;
    bool rapidFire;
    bool reloading;
    Quaternion bulletRotation = Quaternion.identity;

    // Used for flipping player sprite
    SpriteRenderer spriteRenderer;

    // Start is called before the first frame update
    void Start()
    {
        rapidFire = false;
        reloading = false;

        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        BulletControl();
        CleanStrayBullets();
        SetWalkType();
        PlayerMovement();

        // Stop on death
        if (Health <= 0)
        {
            StopAllCoroutines();
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
            if (context.performed && !reloading)
            {
                animator.SetBool("Firing", true);

                // Start firing
                rapidFire = true;
                StartCoroutine(RapidFire());

                // Start timer before next shot
                // Prevents button mashing
                StartCoroutine(Reload());
            }

            if (context.canceled)
            {
                animator.SetBool("Firing", false);

                // Stop firing
                rapidFire = false;
                StopCoroutine(RapidFire());
                StopCoroutine(Reload());
            }
        }
    }

    /// <summary>
    /// Reference to the player bullets
    /// </summary>
    /// <returns></returns>
    public List<GameObject> GetPlayerBullets()
    {
        if (bullets != null && bullets.Count > 0)
        {
            return bullets;
        }
        else
        {
            return null;
        }
    }

    /// <summary>
    /// Prevent button mashing to fire faster
    /// </summary>
    /// <returns></returns>
    IEnumerator Reload()
    {
        reloading = true;
        yield return new WaitForSeconds(fireRate);
        reloading = false;
    }

    /// <summary>
    /// Fire bullets in succession while holding the fire button
    /// </summary>
    /// <returns></returns>
    IEnumerator RapidFire()
    {
        while (rapidFire)
        {
            Instantiate(bullet, transform.position, bulletRotation, transform);
            yield return new WaitForSeconds(fireRate);
        }
    }

    void CleanStrayBullets()
    {
        // Clean up stray bullets
        foreach (GameObject b in bullets)
        {
            if (b.transform.position.y > screenHeight ||
                b.transform.position.y < -screenHeight ||
                b.transform.position.x > screenWidth ||
                b.transform.position.x < -screenWidth)
            {
                Destroy(b);
                bullets.Remove(b);
                return;
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

    protected override void CalcSteeringForces()
    {
        throw new System.NotImplementedException();
    }
}