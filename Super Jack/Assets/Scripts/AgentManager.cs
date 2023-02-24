using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentManager : MonoBehaviour
{
    [SerializeField]
    Player player;

    [SerializeField]
    Nerve nervePrefab;

    List<Agent> agents = new List<Agent>();
    public List<Agent> Agents { get { return agents; } }

    // Start is called before the first frame update
    void Start()
    {
        agents.Add(Instantiate(player));
        agents[0].Init(this);
        agents.Add(Instantiate(nervePrefab));
        agents[1].Init(this);
    }
}
