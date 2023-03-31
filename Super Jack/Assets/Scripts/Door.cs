using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Door : Agent
{
    [SerializeField]
    int sceneIndex;

    [SerializeField]
    bool requiresKey;

    protected override void AgentUpdate()
    {
        if (BoxCollisions(gameObject, superJack.gameObject))
        {
            if (requiresKey && superJack.hasKey)
            {
                SceneManager.LoadScene(sceneIndex);
            }
            else
            {
                SceneManager.LoadScene(sceneIndex);
            }
        }
    }
}