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
        Vector3 goalPos = finish.transform.position + (finish.transform.localScale.y * new Vector3(0, 1, 0)) + new Vector3(0, 10, 0);
        Vector3 currentPos = agent.transform.position;
        Vector3 errorVector = goalPos - currentPos;
        Vector3 localError = agent.transform.InverseTransformVector(errorVector);

        // Debug.Log(agent.name + "localError: " + localError);
        return localError;
    }

    public override Dictionary<string, float> WeightedBehaviours()
    {
        throw new System.NotImplementedException();
    }

    public override void SetWeights(Dictionary<string, float> weights)
    {
        throw new System.NotImplementedException();
    }
}

