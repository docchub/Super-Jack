using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Brain : Agent
{
    public List<Sprite> sprites; 

    SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    protected override void AgentUpdate()
    {
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
    }
}
