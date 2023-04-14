using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Scene = UnityEngine.SceneManagement.Scene;

public class OverworldMusic : MonoBehaviour
{
    Scene currentScene;
    Scene prevScene;

    Player superJack;
    Vector2 pos;

    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(gameObject);
        SceneManager.LoadScene(1);
    }

    // Update is called once per frame
    void Update()
    {
        currentScene = SceneManager.GetActiveScene();

        if (currentScene.name == "BrainFight" || currentScene.name == "GameOver")
        {
            Destroy(gameObject);
        }

        if (!superJack)
        {
            superJack = FindObjectOfType<Player>();
            if (superJack)
            {
                MovePlayer();
            }
        }
        else if (superJack)
        {
            pos = superJack.Position;
        }

        // Set sj null after scene change
        if (prevScene != currentScene)
        {
            superJack = null;
        }

        prevScene = currentScene;
    }

    void MovePlayer()
    {
        if (pos.y < 0 && currentScene.buildIndex == 1)
        {
            superJack.Position = new Vector3(0, 3, 0);
        }
        else if (pos.y < 0)
        {
            superJack.Position = new Vector3(4, 3, 0);
        }
    }
}
