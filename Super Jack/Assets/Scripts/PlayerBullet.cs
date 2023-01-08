using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBullet : MonoBehaviour
{
    [SerializeField]
    float speed = 1f;

    Vector3 bulletPosition;
    Vector3 direction;
    Vector3 velocity = Vector3.zero;

    // Start is called before the first frame update
    void Start()
    {
        bulletPosition = transform.position;
        direction = new Vector2(1,0);
    }

    // Update is called once per frame
    void Update()
    {
        // Update veloctity
        velocity = direction * speed * Time.deltaTime;

        // Add velocity to our current position
        bulletPosition += velocity;

        transform.position = bulletPosition;
    }
}
