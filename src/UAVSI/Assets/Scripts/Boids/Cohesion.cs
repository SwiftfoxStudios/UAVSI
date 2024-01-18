using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Flock/Behaviour/Cohesion")]
public class Cohesion : FlockBehaviour
{
    public override Vector3 CalculateMove(Agent agent, List<Transform> context, Flock flock)
    {
        if (context.Count == 0){
            return Vector3.zero;
        }
        Vector3 cohesionMove = Vector3.zero;
        foreach (Transform item in context){
            cohesionMove += item.position;

        }
        cohesionMove /= context.Count;


        cohesionMove -= agent.transform.position;
        Vector3 cohesionMoveTransform = agent.transform.InverseTransformVector(cohesionMove);

        return cohesionMoveTransform;
    }
}
