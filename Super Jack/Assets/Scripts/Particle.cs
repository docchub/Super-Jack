using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Particle : MonoBehaviour
{
    void DestroyParticles()
    {
        Destroy(gameObject);
    }
}