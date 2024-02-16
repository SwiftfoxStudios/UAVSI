using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Flock/Behaviour/Alignment")]
public class Alignment : FlockBehaviour
{
    public override Vector3 CalculateMove(Agent agent, List<Transform> context, Flock flock)
    {
        // If no neighbors, maintain current alignment
        if (context.Count == 0){
            return agent.transform.forward;
        }
        Vector3 alignmentMove = Vector3.zero;
        foreach (Transform item in context){
            alignmentMove += item.transform.forward;

        }
        alignmentMove /= context.Count;


        return alignmentMove;
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
