using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [SerializeField]
    float speed = 5f;
    public int health;

    [SerializeField]
    GameObject bullet;
    List<GameObject> bullets = new List<GameObject>();

    public Vector3 playerPos = Vector3.zero;
    Vector3 direction = Vector3.zero;
    Vector3 velocity = Vector3.zero;

    [SerializeField]
    float fireRate = 1f;
    bool rapidFire;
    bool reloading;

    const float screenWidth = 5f;
    const float screenHeight = 5f;

    //const float hitStateLength = 0.2f;

    // Start is called before the first frame update
    void Start()
    {
        playerPos = transform.position;
        rapidFire = false;
        reloading = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (health > 0)
        {
            // Update veloctity
            velocity = direction * speed * Time.deltaTime;

            // Add velocity to our current position
            playerPos += velocity;

            // Draw at calculated position
            transform.position = playerPos;

            // Prevent moving outside the bounds
            if (playerPos.x >= screenWidth)
            {
                playerPos = new Vector3(screenWidth, playerPos.y, 0);
                transform.position = new Vector3(screenWidth, playerPos.y, 0);
            }
            else if (playerPos.x <= -screenWidth)
            {
                playerPos = new Vector3(-screenWidth, playerPos.y, 0);
                transform.position = new Vector3(-screenWidth, playerPos.y, 0);
            }
            if (playerPos.y >= screenHeight)
            {
                playerPos = new Vector3(playerPos.x, screenHeight, 0);
                transform.position = new Vector3(playerPos.x, screenHeight, 0);
            }
            else if (playerPos.y <= -screenHeight)
            {
                playerPos = new Vector3(playerPos.x, -screenHeight, 0);
                transform.position = new Vector3(playerPos.x, -screenHeight, 0);
            }

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
        else
        {
            StopAllCoroutines();
            this.GetComponent<SpriteRenderer>().color = Color.red;
        }
    }

    /// <summary>
    /// Checks for player inputs
    /// </summary>
    /// <param name="context"></param>
    public void OnMove(InputAction.CallbackContext context)
    {
        direction = context.ReadValue<Vector2>();
    }

    /// <summary>
    /// Spawns a bullet
    /// </summary>
    public void Shoot(InputAction.CallbackContext context)
    {
        if (health > 0)
        {
            if (context.performed && !reloading)
            {
                StopCoroutine(Reload());

                // Start firing
                rapidFire = true;
                StartCoroutine(RapidFire());

                // Start timer before next shot
                // Prevents button mashing
                StartCoroutine(Reload());
            }

            if (context.canceled)
            {
                // Stop firing
                rapidFire = false;
                StopCoroutine(RapidFire());
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
            bullets.Add(Instantiate(bullet, transform.position, Quaternion.identity, transform));
            yield return new WaitForSeconds(fireRate);
        }
    }
}