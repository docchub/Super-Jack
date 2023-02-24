using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Agent : MonoBehaviour
{
    protected AgentManager manager;

    Vector3 direction = Vector3.zero;
    Vector3 position = Vector3.zero;
    Vector3 velocity = Vector3.zero;

    Vector3 force = Vector3.zero;

    protected Vector3 totalSteeringForce;

    public Vector3 Direction { get { return direction; } }
    public Vector3 Position { get { return position; } }
    public Vector3 Velocity { get { return velocity; } }

    protected float screenHeight;
    protected float screenWidth;

    protected Player superJack;
    protected List<Nerve> nerveList;

    [SerializeField]
    int health = 4;
    public int Health { get { return health; } }

    [SerializeField]
    float speed = 1f;
    public float Speed { get { return speed; } }

    [SerializeField]
    float maxForce = 1f;

    // Start is called before the first frame update
    void Start()
    {
        position = transform.position;

        screenHeight = Camera.main.orthographicSize;
        screenWidth = Camera.main.aspect * screenHeight;

        InitializeAgents();
    }

    // Update is called once per frame
    void Update()
    {
        totalSteeringForce = Vector3.zero;

        CalcSteeringForces();

        totalSteeringForce = Vector3.ClampMagnitude(totalSteeringForce, maxForce);

        ApplyForce(totalSteeringForce);

        // Stay in the screen bounds
        StayInBounds();

        // Physics object movement
        if (health > 0)
        {
            velocity += force * Time.deltaTime;
            position += velocity * Time.deltaTime;
            direction = velocity.normalized;
            transform.position = position;
        }

        force = Vector3.zero;
    }

    protected abstract void CalcSteeringForces();

    public void ApplyForce(Vector3 addedForce)
    {
        force += addedForce;
    }

    public void ApplyDirection(Vector2 playerDirection)
    {
        direction = playerDirection;
    }

    public Vector3 Seek(Vector3 targetPos)
    {
        Vector2 desiredVelocity = targetPos - position;
        desiredVelocity = desiredVelocity.normalized * speed;
        Vector2 seekForce = desiredVelocity - (Vector2)velocity;
        return seekForce;
    }

    /// <summary>
    /// Stay within the bounds of the screen
    /// </summary>
    void StayInBounds()
    {
        // Prevent moving outside the bounds
        if (position.x >= screenWidth)
        {
            position = new Vector3(screenWidth, position.y, 0);
            transform.position = new Vector3(screenWidth, position.y, 0);
        }
        else if (position.x <= -screenWidth)
        {
            position = new Vector3(-screenWidth, position.y, 0);
            transform.position = new Vector3(-screenWidth, position.y, 0);
        }
        if (position.y >= screenHeight)
        {
            position = new Vector3(position.x, screenHeight, 0);
            transform.position = new Vector3(position.x, screenHeight, 0);
        }
        else if (position.y <= -screenHeight)
        {
            position = new Vector3(position.x, -screenHeight, 0);
            transform.position = new Vector3(position.x, -screenHeight, 0);
        }
    }

    /// <summary>
    /// Used to initialize objects under the manager
    /// </summary>
    /// <param name="manager"></param>
    public void Init(AgentManager manager)
    {
        this.manager = manager;
    }

    void InitializeAgents()
    {
        string sJackPlayer = "superJack(Clone)";
        string nerve = "nerve(Clone)";

        superJack = new Player();
        nerveList = new List<Nerve>();

        foreach (Agent agent in manager.Agents)
        {
            if (agent.name == sJackPlayer)
            {
                superJack = (Player)agent;
            }
            else if (agent.name == nerve)
            {
                nerveList.Add((Nerve)agent);
            }
        }
    }
}