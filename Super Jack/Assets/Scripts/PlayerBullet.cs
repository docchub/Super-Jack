using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBullet : Agent
{
    // Start is called before the first frame update
    void Start()
    {
        Position = transform.position;
        Direction = transform.rotation * new Vector2(0, 1);
    }

    // Update is called once per frame
    void Update()
    {
        // Update veloctity
        Velocity = Direction * Speed * Time.deltaTime;

        // Add velocity to our current position
        Position += Velocity;

        transform.position = Position;
    }

    protected override void CalcSteeringForces()
    {
        throw new System.NotImplementedException();
    }
}
