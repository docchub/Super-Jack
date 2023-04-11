using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using TMPro;
using UnityEngine;

public class JackText : MonoBehaviour
{
    Brain brain;
    NormalJack jack;
    TextMeshProUGUI textMesh;

    // Start is called before the first frame update
    void Start()
    {
        textMesh = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        // Get a reference to the brain and normal jack
        if (!brain)
        {
            brain = FindObjectOfType<Brain>();
        }
        if (!jack)
        {
            jack = FindObjectOfType<NormalJack>();
        }

        // Change the text if normal jack has transformed into super jack
        if (jack.isSuperJack)
        {
            textMesh.text = "\"I'm alright. I'm ok.\"";
            textMesh.fontSize = 23;
            textMesh.color = UnityEngine.Color.blue;

            if (brain.Health <= 0)
            {
                textMesh.fontSize = 36;
                textMesh.text = "EAT THE BRAIN";
                textMesh.color = UnityEngine.Color.red;
            }
        }
        else
        {
            // Remove text after damaging brain
            if (brain.Health <= 97 && brain.Health > 80)
            {
                textMesh.text = "";
            }

            // Unmasked Jack Text
            else if (brain.Health <= 80 && brain.Health > 40)
            {
                textMesh.text = "\"What are you doing? Stop!\"";
                textMesh.fontSize = 23;
                textMesh.color = UnityEngine.Color.blue;
            }
            else if (brain.Health <= 40 && brain.Health > 0)
            {
                textMesh.text = "...";
            }

            // Brain Eater
            else if (brain.Health <= 0)
            {
                textMesh.fontSize = 36;
                textMesh.text = "EAT THE BRAIN";
                textMesh.color = UnityEngine.Color.red;
            }
        }
    }
}
