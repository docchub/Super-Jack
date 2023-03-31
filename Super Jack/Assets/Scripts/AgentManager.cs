using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentManager : MonoBehaviour
{
    [SerializeField]
    Player player;

    [SerializeField]
    Nerve nervePrefab;

    [SerializeField]
    Brain brainPrefab;

    [SerializeField]
    Door doorPrefab;

    [SerializeField]
    Door doorKeyPrefab;

    List<Agent> agents = new List<Agent>();
    public List<Agent> Agents { get { return agents; } set { agents = value; } }

    [SerializeField]
    Vector2 doorPos1 = new Vector2(-4.0f, -4.7f);
    [SerializeField]
    Vector2 doorPos2 = new Vector2(4.0f, 4.7f);

    [SerializeField]
    List<Vector2> nervePosList;

    [SerializeField]
    Vector2 playerStartPos = new Vector2(-4, -3);

    [SerializeField]
    int roomNumber;

    // Start is called before the first frame update
    void Start()
    {
        // Create agents based on the room value
        if (roomNumber == 0)
        {

        }
        else if (roomNumber == 1)
        {
            NerveRoom();
            PlaceDoors();
        }
        else if (roomNumber == 2)
        {
            BrainRoom();
        }

        // Initialize all created agents
        foreach (Agent a in agents)
        {
            InitAgent(a);
        }
    }

    void StartingRoom()
    {

    }

    void NerveRoom()
    {
        agents.Add(Instantiate(player, playerStartPos, Quaternion.identity));

        for (int i = 0; i < nervePosList.Count; i++)
        {
            agents.Add(Instantiate(nervePrefab, nervePosList[i], Quaternion.identity));
        }
    }

    void BrainRoom()
    {
        agents.Add(Instantiate(player, playerStartPos, Quaternion.identity));
        agents.Add(Instantiate(brainPrefab));
    }

    void PlaceDoors()
    {
        agents.Add(Instantiate((Agent)doorPrefab, doorPos1, Quaternion.identity));
        agents.Add(Instantiate((Agent)doorPrefab, doorPos2, Quaternion.identity));
    }

    public void InitAgent(Agent agent)
    {
        agent.Init(this);
    }
}
