using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalJack : Agent
{
    SpriteRenderer spriteRenderer;

    [SerializeField]
    Sprite sprite;

    public bool isSuperJack;

    private void Awake()
    {
        Health = 1;
        isSuperJack = false;
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    protected override void AgentUpdate()
    {
        if (Health <= 0)
        {
            spriteRenderer.sprite = sprite;
            isSuperJack = true;
        }

        // Dont do while eating
        if (!superJack.eating)
        {
            // Move to face super jack
            if (superJack.Position.x > 0)
            {
                spriteRenderer.flipX = true;
            }
            else
            {
                spriteRenderer.flipX = false;
            }
        }
    }
}
