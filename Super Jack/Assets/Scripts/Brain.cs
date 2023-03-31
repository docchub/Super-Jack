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

    SpriteRenderer spriteRenderer;

    private void Awake()
    {
        activeHitbox = hitBoxes[0];
        Instantiate(activeHitbox);
        phaseCompleted = false;

        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    protected override void AgentUpdate()
    {
        // Sprite manager
        prevSprite = spriteRenderer.sprite;
        if (Health <= 40 && Health > 30)
        {
            spriteRenderer.sprite = sprites[1];
        }
        else if (Health <= 30 && Health > 20)
        {
            spriteRenderer.sprite = sprites[2];
        }
        else if (Health <= 20 && Health > 10)
        {
            spriteRenderer.sprite = sprites[3];
        }
        else if (Health <= 10)
        {
            spriteRenderer.sprite = sprites[4];
        }

        if (prevSprite != spriteRenderer.sprite)
        {
            phaseCompleted = true;
        }

        // Instantiate hitboxes
        if (Health == 40 && phaseCompleted)
        {
            activeHitbox = hitBoxes[1];
            Instantiate(activeHitbox);
            phaseCompleted = false;
        }
        else if (Health == 30 && phaseCompleted)
        {
            activeHitbox = hitBoxes[2];
            Instantiate(activeHitbox);
            phaseCompleted = false;
        }
        else if (Health == 20 && phaseCompleted)
        {
            activeHitbox = hitBoxes[3];
            Instantiate(activeHitbox);
            phaseCompleted = false;
        }
        else if (Health == 10 && phaseCompleted)
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
