using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Flock/Behaviour/Composite")]
public class CompositeBehaviour : FlockBehaviour
{
    public FlockBehaviour[] behaviours;
    public float[] weights;
    public override Vector3 CalculateMove(Agent agent, List<Transform> context, Flock flock)
    {
        // Handle data mismatch
        if (weights.Length != behaviours.Length)
        {
            Debug.LogError("Data mismatch in " + name, this);
            return Vector3.zero;
        }

        // Set up move
        Vector3 move = Vector3.zero;

        // Iterate through behaviours
        for (int i = 0; i < behaviours.Length; i++)
        {
            Vector3 partialMove = behaviours[i].CalculateMove(agent, context, flock) * weights[i];

            if (partialMove != Vector3.zero)
            {
                if (partialMove.sqrMagnitude > weights[i] * weights[i])
                {

                    partialMove *= weights[i];
                }

                move += partialMove;
            }
        }
        return move;
    }

    public override Dictionary<string, float> WeightedBehaviours()
    {
        Dictionary<string, float> weightedBehaviours = new Dictionary<string, float>();
        for (int i = 0; i < behaviours.Length; i++)
        {
            weightedBehaviours.Add(behaviours[i].name, weights[i]);
        }
        return weightedBehaviours;
    }

    public override void SetWeights(Dictionary<string, float> newWeights)
    {
        for (int i = 0; i < behaviours.Length; i++)
        {
            weights[i] = newWeights[behaviours[i].name];
        }
    }
}
