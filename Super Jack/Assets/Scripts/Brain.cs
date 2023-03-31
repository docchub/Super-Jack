using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    AudioSource audioSource;

    [SerializeField]
    GameObject brainEater;

    private void Awake()
    {
        healthIntervals = Health / 5;

        activeHitbox = hitBoxes[0];
        Instantiate(activeHitbox);
        phaseCompleted = false;
        brainDead = false;

        spriteRenderer = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();
    }

    protected override void AgentUpdate()
    {
        // Play braineater music on death
        if (!brainDead && Health == 0)
        {
            brainDead = true;
            audioSource.Stop();
            Instantiate(brainEater);
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
        }
        else if (Health <= healthIntervals)
        {
            spriteRenderer.sprite = sprites[4];
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
}
