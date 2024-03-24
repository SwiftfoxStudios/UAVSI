using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(menuName = "Flock/Behaviour/TerrainAvoidance")]
public class TerrainAvoidance : FlockBehaviour
{
    
    public override Vector3 CalculateMove(Agent agent, List<Transform> context, Flock flock)
    {
        Vector3 velocity = agent.GetComponent<Rigidbody>().velocity;
        float distanceToTerrain = SenseLidar(GameObject.Find("TerrainMesh").GetComponent<MeshCollider>(), agent.transform.position, velocity);
        if(distanceToTerrain < 10f){
            return 100 * agent.transform.up;
        }
        else{
            return Vector3.zero;
        }
    }

    public float SenseLidar(MeshCollider goal, Vector3 sensorPosition, Vector3 sensorDirection){
        Vector3 rayStart = sensorPosition;
        Quaternion angleOfattack = Quaternion.Euler(-80, 0, 0);
        Vector3 rayDirection = angleOfattack * new Vector3(0, sensorDirection.y, sensorDirection.z);
        float distance = Mathf.Infinity;
        if(rayDirection == Vector3.zero){
            return distance;
        }
    
        // Perform the raycast
        RaycastHit hit;
        if (goal.Raycast(new Ray(rayStart, rayDirection.normalized), out hit, Mathf.Infinity))
        {
            distance = hit.distance;
            Debug.DrawLine(rayStart, hit.point, Color.white);
        }

        return distance;
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