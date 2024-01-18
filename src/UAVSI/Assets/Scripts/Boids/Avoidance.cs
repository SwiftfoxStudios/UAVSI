using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Flock/Behaviour/Avoidance")]
public class Avoidance : FlockBehaviour
{
    public override Vector3 CalculateMove(Agent agent, List<Transform> context, Flock flock)
    {
        if (context.Count == 0){
            return Vector3.zero;
        }
        Vector3 avoidanceMove = Vector3.zero;
        int nAvoid = 0;
        foreach (Transform item in context){
            if (Vector3.SqrMagnitude(item.position - agent.transform.position) < flock.SquareAvoidanceRadius){
                nAvoid++;
                avoidanceMove += agent.transform.position - item.position;
            }
        }
       if (nAvoid > 0){
           avoidanceMove /= nAvoid;
       }

            // float yawAngle = 360f - agent.transform.rotation.eulerAngles.y;
            // float cosYaw = Mathf.Cos(yawAngle);
            // float sinYaw = Mathf.Sin(yawAngle);

            Vector3 transf = agent.transform.InverseTransformVector(avoidanceMove);
            return transf;

        
    }

}
