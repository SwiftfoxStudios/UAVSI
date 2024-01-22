using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

[CreateAssetMenu(menuName = "Flock/Behaviour/Seeking")]
public class Seeking : FlockBehaviour

{
    public override Vector3 CalculateMove(Agent agent, List<Transform> context, Flock flock)
    {
        GameObject finish = GameObject.FindWithTag("Finish");
        Vector3 goalPos = finish.transform.position;
        Vector3 currentPos = agent.transform.position;
        Vector3 errorVector = goalPos - currentPos;
        Vector3 localError = agent.transform.InverseTransformVector(errorVector);

        Debug.Log(agent.name + "localError: " + localError);
        return localError;
    }
}

