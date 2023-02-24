using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Nerve : Agent
{
    protected override void CalcSteeringForces()
    {
        totalSteeringForce += Seek(superJack.transform.position);
    }
}
