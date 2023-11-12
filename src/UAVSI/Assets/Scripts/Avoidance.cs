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

            float yawAngle = 360f - agent.transform.rotation.eulerAngles.y;
            float cosYaw = Mathf.Cos(yawAngle);
            float sinYaw = Mathf.Sin(yawAngle);

            Matrix4x4 rotationMatrix = new Matrix4x4(
                new Vector4(cosYaw, 0, -sinYaw, 0),
                new Vector4(0, 1, 0, 0),
                new Vector4(sinYaw, 0, cosYaw, 0),
                new Vector4(0, 0, 0, 1)
            );

            // agent.transform.worldToLocalMatrix

            Vector3 localPosition = rotationMatrix.MultiplyPoint(avoidanceMove.normalized);
            Vector3 transf = agent.transform.InverseTransformVector(avoidanceMove);
            Debug.Log("unity:" + transf + "matrix:" + localPosition);


            return transf;

        
    }

}
