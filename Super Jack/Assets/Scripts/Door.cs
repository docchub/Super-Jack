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
            // Lock door if it requires a key
            if (requiresKey)
            {
                // Open if Jack has the key
                if (superJack.hasKey)
                {
                    SceneManager.LoadScene(sceneIndex);
                }
            }
            else
            {
                SceneManager.LoadScene(sceneIndex);
            }
        }
    }
}