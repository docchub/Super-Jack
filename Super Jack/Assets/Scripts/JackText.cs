using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using TMPro;
using UnityEngine;

public class JackText : MonoBehaviour
{
    Brain brain;
    TextMeshProUGUI textMesh;

    // Start is called before the first frame update
    void Start()
    {
        textMesh = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!brain)
        {
            brain = FindObjectOfType<Brain>();
        }
        else
        {
            if (brain.Health <= 0)
            {
                textMesh.text = "EAT THE BRAIN";
                textMesh.color = UnityEngine.Color.red;
            }
        }
    }
}
