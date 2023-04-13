using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Brain : Agent
{
    public List<Sprite> sprites;
    Sprite prevSprite;

    public List<GameObject> hitBoxes;
    GameObject activeHitbox;

    bool phaseCompleted;
    bool brainDead;

    int healthIntervals;

    SpriteRenderer spriteRenderer;
    AudioSource source;
    AudioSource bgMusic;

    public AudioClip hurtSound;
    public AudioClip brainFight;
    public AudioClip brainEater;

    [SerializeField]
    float volume = 0.5f;

    Color brainColor;

    [SerializeField]
    float flickerTime = 0.01f;

    int randomNumber;

    private void Awake()
    {
        healthIntervals = Health / 5;

        activeHitbox = hitBoxes[0];
        Instantiate(activeHitbox);
        phaseCompleted = false;
        brainDead = false;

        spriteRenderer = GetComponent<SpriteRenderer>();

        source = gameObject.AddComponent<AudioSource>();
        source.volume = volume;

        bgMusic = gameObject.AddComponent<AudioSource>();
        bgMusic.clip = brainFight;
        bgMusic.volume = 0.6f;
        bgMusic.Play();

        // Damage effects
        brainColor = gameObject.GetComponent<SpriteRenderer>().color;
    }

    protected override void AgentUpdate()
    {
        superJack.brainExists = true;

        // Play braineater music on death
        if (!brainDead && Health == 0)
        {
            // Jitter until end of game
            StartCoroutine(EndlessDamage());

            // Music
            brainDead = true;
            bgMusic.clip = brainEater;
            bgMusic.loop = true;
            bgMusic.Play();
        }

        // Player sprite overlap
        if (superJack.Position.y < -1.35)
        {
            spriteRenderer.sortingOrder = 0;
        }
        else
        {
            spriteRenderer.sortingOrder = 2;
        }

        // Sprite manager
        prevSprite = spriteRenderer.sprite;
        if (Health <= 4 * healthIntervals && Health > 3 * healthIntervals)
        {
            spriteRenderer.sprite = sprites[1];
        }
        else if (Health <= 3 * healthIntervals && Health > 2 * healthIntervals)
        {
            spriteRenderer.sprite = sprites[2];
        }
        else if (Health <= 2 * healthIntervals && Health > healthIntervals)
        {
            spriteRenderer.sprite = sprites[3];

            // adjust for sprite
            Position = new Vector3(-0.5f, 0f);
        }
        else if (Health <= healthIntervals)
        {
            spriteRenderer.sprite = sprites[4];
            Position = Vector3.zero;
        }

        if (prevSprite != spriteRenderer.sprite)
        {
            phaseCompleted = true;
        }

        // Instantiate hitboxes
        if (Health == 4 * healthIntervals && phaseCompleted)
        {
            activeHitbox = hitBoxes[1];
            Instantiate(activeHitbox);
            phaseCompleted = false;
        }
        else if (Health == 3 * healthIntervals && phaseCompleted)
        {
            activeHitbox = hitBoxes[2];
            Instantiate(activeHitbox);
            phaseCompleted = false;
        }
        else if (Health == 2 * healthIntervals && phaseCompleted)
        {
            activeHitbox = hitBoxes[3];
            Instantiate(activeHitbox);
            phaseCompleted = false;
        }
        else if (Health == 1 * healthIntervals && phaseCompleted)
        {
            activeHitbox = hitBoxes[4];
            Instantiate(activeHitbox);
            phaseCompleted = false;
        }
    }

    public GameObject ActiveHitbox()
    {
        return activeHitbox;
    }

    public void Hurt()
    {
        Health--;
        StopCoroutine(DamageEffect());
        StartCoroutine(DamageEffect());
        source.clip = hurtSound;
        source.Play();
    }

    IEnumerator DamageEffect()
    {
        randomNumber = Random.Range(1, 101);

        // Normal
        if (randomNumber < 66)
        {
            brainColor.a = 0.7f;
            gameObject.GetComponent<SpriteRenderer>().color = brainColor;
            yield return new WaitForSeconds(flickerTime);

            brainColor.a = 1f;
            gameObject.GetComponent<SpriteRenderer>().color = brainColor;
            yield return new WaitForSeconds(flickerTime);

            brainColor.a = 0.7f;
            gameObject.GetComponent<SpriteRenderer>().color = brainColor;
            yield return new WaitForSeconds(flickerTime);

            brainColor.a = 1f;
            gameObject.GetComponent<SpriteRenderer>().color = brainColor;
            yield return new WaitForSeconds(flickerTime);
        }

        // Grotesque
        else 
        {
            Vector2 ogPos = gameObject.transform.position;

            for (int i = 0; i < 3; i++)
            {
                spriteRenderer.transform.position = new Vector3(Random.Range(-1f, 1f), Random.Range(-0.3f, 0.3f));
                brainColor.a = 0.7f;
                gameObject.GetComponent<SpriteRenderer>().color = brainColor;
                yield return new WaitForSeconds(flickerTime);

                spriteRenderer.transform.position = new Vector3(Random.Range(-1f, 1f), Random.Range(-0.3f, 0.3f));
                brainColor.a = 1f;
                gameObject.GetComponent<SpriteRenderer>().color = brainColor;
                yield return new WaitForSeconds(flickerTime);
            }

            spriteRenderer.transform.position = ogPos;
            yield return new WaitForSeconds(flickerTime);
        }

        StopCoroutine(DamageEffect());
    }

    IEnumerator EndlessDamage()
    {
        while (true)
        {
            spriteRenderer.transform.position = new Vector3(Random.Range(-0.6f, 0.6f), Random.Range(-0.2f, 0.2f));
            brainColor.a = 0.7f;
            gameObject.GetComponent<SpriteRenderer>().color = brainColor;
            yield return new WaitForSeconds(flickerTime);

            spriteRenderer.transform.position = new Vector3(Random.Range(-0.6f, 0.6f), Random.Range(-0.2f, 0.2f));
            brainColor.a = 1f;
            gameObject.GetComponent<SpriteRenderer>().color = brainColor;
            yield return new WaitForSeconds(flickerTime);
        }
    }
}
