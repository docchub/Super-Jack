using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;

public abstract class Agent : MonoBehaviour
{
    public AgentManager manager;

    Vector3 direction = Vector3.zero;
    Vector3 position = Vector3.zero;
    Vector3 velocity = Vector3.zero;

    Vector3 force = Vector3.zero;

    SpriteRenderer agent1Sprite;
    SpriteRenderer agent2Sprite;

    float aWidth;
    float aHeight;
    float bWidth;
    float bHeight;

    float aMaxX;
    float aMinX;
    float aMaxY;
    float aMinY;

    float bMaxX;
    float bMinX;
    float bMaxY;
    float bMinY;

    protected Vector3 totalSteeringForce;

    public Vector3 Direction { get { return direction; } set { direction = value; } }
    public Vector3 Position { get { return position; } set { position = value; } }
    public Vector3 Velocity { get { return velocity; } set { velocity = value; } }

    protected float screenHeight;
    protected float screenWidth;

    public float screenHeightBounds = 0.7f;
    public float screenWidthBounds = 0.9f;

    public Player superJack;
    public List<Nerve> nerveList;
    public Brain brain;
    public List<PlayerBullet> bulletList;

    [SerializeField]
    int health = 4;
    public int Health
    {
        get { return health; }
        set
        {
            if (value >= 0)
            {
                health = value;
            }
        }
    }

    [SerializeField]
    float speed = 1f;
    public float Speed { get { return speed; } }

    [SerializeField]
    float maxForce = 1f;

    // Start is called before the first frame update
    void Start()
    {
        position = transform.position;

        screenHeight = Camera.main.orthographicSize * screenHeightBounds;
        screenWidth = Camera.main.aspect * screenHeight * screenWidthBounds;

        InitializeAgents();
    }

    // Update is called once per frame
    void Update()
    {
        totalSteeringForce = Vector3.zero;

        AgentUpdate();

        totalSteeringForce = Vector3.ClampMagnitude(totalSteeringForce, maxForce);

        ApplyForce(totalSteeringForce);

        // Physics object movement
        if (health > 0)
        {
            velocity += force * Time.deltaTime;
            position += velocity * Time.deltaTime;
            direction = velocity.normalized;
            transform.position = position;
        }

        // Stay in the screen bounds
        StayInBounds();

        force = Vector3.zero;
    }

    protected abstract void AgentUpdate();

    /// <summary>
    /// Movement for the Player agent
    /// </summary>
    protected void PlayerMovement()
    {
        velocity = direction * speed * Time.deltaTime;
        position += velocity;
        transform.position = position;
    }

    /// <summary>
    /// Apply a force to an agent
    /// </summary>
    /// <param name="addedForce"></param>
    public void ApplyForce(Vector3 addedForce)
    {
        force += addedForce;
    }

    /// <summary>
    /// Apply a direction to an agent
    /// </summary>
    /// <param name="pDirection"></param>
    public void ApplyDirection(Vector3 pDirection)
    {
        direction = pDirection;
    }

    /// <summary>
    /// Seek an agent
    /// </summary>
    /// <param name="targetPos"></param>
    /// <returns></returns>
    public Vector3 Seek(Vector3 targetPos)
    {
        Vector2 desiredVelocity = targetPos - position;
        desiredVelocity = desiredVelocity.normalized * speed;
        Vector2 seekForce = desiredVelocity - (Vector2)velocity;
        return seekForce;
    }

    /// <summary>
    /// Flee an agent
    /// </summary>
    /// <param name="targetPos"></param>
    /// <returns></returns>
    public Vector3 Flee(Vector3 targetPos)
    {
        Vector2 desiredVelocity =  position - targetPos;
        desiredVelocity = desiredVelocity.normalized * speed;
        Vector2 fleeForce = desiredVelocity - (Vector2)velocity;
        return fleeForce;
    }

    /// <summary>
    /// Don't touch other agents
    /// </summary>
    /// <returns></returns>
    public Vector3 Separation()
    {
        Vector3 separateForce = Vector3.zero;
        float sqrDist;

        foreach (Agent other in manager.Agents)
        {
            sqrDist = Vector3.SqrMagnitude(Position - other.Position);

            // Flee the closest agent
            if (sqrDist != 0)
            {
                separateForce += Flee(other.Position) * (1f / sqrDist);
            }
        }

        return separateForce;
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

    /// <summary>
    /// Allow agents to reference other agents
    /// </summary>
    void InitializeAgents()
    {
        string sJackPlayer = "superJack(Clone)";
        string nerve = "nerve(Clone)";
        string brainString = "brain(Clone)";
        string bullet = "bullet(Clone)";

        nerveList = new List<Nerve>();
        bulletList = new List<PlayerBullet>();

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
            else if (agent.name == brainString)
            {
                brain = (Brain)agent;
            }
            else if (agent.name == bullet)
            {
                bulletList.Add((PlayerBullet)agent);

                // Give each existing agent that needs it a reference to the bullet
                superJack.bulletList = bulletList;
                foreach (Nerve n in nerveList)
                {
                    n.bulletList = bulletList;
                }
            }
        }
    }

    /// <summary>
    /// Detect AABB collisions between 2 agents
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    public bool BoxCollisions(GameObject a, GameObject b)
    {
        // Get both objects' spriterenders
        agent1Sprite = a.GetComponent<SpriteRenderer>();
        agent2Sprite = b.GetComponent<SpriteRenderer>();

        // Determine object widths and heights
        aWidth = agent1Sprite.bounds.size.x / 2;
        aHeight = agent1Sprite.bounds.size.y / 2;
        bWidth = agent2Sprite.bounds.size.x / 2;
        bHeight = agent2Sprite.bounds.size.y / 2;

        // Max width and height
        aMaxX = a.transform.position.x + aWidth;
        aMinX = a.transform.position.x - aWidth;
        aMaxY = a.transform.position.y + aHeight;
        aMinY = a.transform.position.y - aHeight;
        bMaxX = b.transform.position.x + bWidth;
        bMinX = b.transform.position.x - bWidth;
        bMaxY = b.transform.position.y + bHeight;
        bMinY = b.transform.position.y - bHeight;

        if (aMaxX > bMinX && aMinX < bMaxX && aMinY < bMaxY && aMaxY > bMinY)
        {
            // Collision is detected
            return true;
        }

        // No collision
        return false;
    }
}