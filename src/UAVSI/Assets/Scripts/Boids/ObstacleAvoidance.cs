using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(menuName = "Flock/Behaviour/ObstacleAvoidance")]
public class ObstacleAvoidance : FlockBehaviour
{
    
    public override Vector3 CalculateMove(Agent agent, List<Transform> context, Flock flock)
    {
        GameObject[] obstacles = GameObject.FindGameObjectsWithTag("Obstacle");
        if (context.Count == 0){
            return Vector3.zero;
        }
        Vector3 avoidanceMove = Vector3.zero;
        int nAvoid = 0;
        foreach (GameObject item in obstacles){
            Transform obsTransform = item.transform;
            if (Vector3.SqrMagnitude(obsTransform.position - agent.transform.position - obsTransform.localScale) < flock.SquareAvoidanceRadius){
                nAvoid++;
                avoidanceMove += agent.transform.position - obsTransform.position;
            }
        }
       if (nAvoid > 0){
           avoidanceMove /= nAvoid;
       }

            Vector3 transf = agent.transform.InverseTransformVector(avoidanceMove);
            return transf;

        
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