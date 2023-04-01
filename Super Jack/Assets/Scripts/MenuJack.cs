using System.Collections;
using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class MenuJack : MonoBehaviour
{
    public Animator animator;

    AudioSource source;

    [SerializeField]
    List<AudioClip> walkSound;
    [SerializeField]
    AudioClip chompSound;

    Vector3 velocity = Vector2.zero;
    Vector3 direction = Vector2.zero;
    Vector3 position = Vector2.zero;

    [SerializeField]
    float speed = 1f;

    bool moving;
    bool hitPlayed;

    [SerializeField]
    GameObject blackout;

    [SerializeField]
    float pauseTimer;

    // Start is called before the first frame update
    void Start()
    {
        moving = false;
        hitPlayed = false;
        source = gameObject.AddComponent<AudioSource>();

        position = transform.position;
        direction = new Vector2(1, 0);
    }

    private void Update()
    {
        if (moving)
        {
            // Update veloctity
            velocity = direction * speed * Time.deltaTime;

            // Add velocity to our current position
            position += velocity;

            // Update drawn position
            transform.position = position;

            // Chomp and transition when makes contacts normalJack
            if (position.x >= 5.2 && !hitPlayed)
            {
                PlayChompSound();
                Instantiate(blackout);
                hitPlayed = true;
            }

            // Transition
            if (position.x >= 5.2)
            {
                pauseTimer -= Time.deltaTime;
                if (pauseTimer <= 0)
                {
                    SceneManager.LoadScene(5);
                }
            }
        }
    }

    void PlayWalkSound(int index)
    {
        source.clip = walkSound[index];
        source.Play();
    }

    void PlayChompSound()
    {
        animator.SetBool("gameStarted", false);
        source.clip = chompSound;
        source.Play();
    }

    public void StartAnimating()
    {
        animator.SetBool("gameStarted", true);
        moving = true;
    }
}
