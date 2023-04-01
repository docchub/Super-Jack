using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    AudioSource source;

    [SerializeField]
    AudioClip startTheme;
    [SerializeField]
    AudioClip buttonClicked;

    // Start is called before the first frame update
    void Start()
    {
        source = gameObject.AddComponent<AudioSource>();
        source.clip = startTheme;
        source.volume = 0.8f;
        source.loop = true;
        source.Play();
    }

    public void StartGame()
    {
        source.clip = buttonClicked;
        source.loop = false;
        source.Play();

        FindObjectOfType<MenuJack>().StartAnimating();
    }
}
