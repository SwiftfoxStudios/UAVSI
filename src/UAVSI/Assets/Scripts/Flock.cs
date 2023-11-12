using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flock : MonoBehaviour
{
    public Agent agentPrefab;
    List<Agent> agents = new List<Agent>();
    public FlockBehaviour behaviour;
    public int numAgents = 2;
    public float spawnRadius = 5.0f;
    public float neighbourRadius = 10.0f;

    public float SquareAvoidanceRadius { get { return 12.0f; } }
    // Start is called before the first frame update
    void Start()
    {
        for (int i=0; i < numAgents; i++){
            Vector3 randomPosition = new Vector3(
            UnityEngine.Random.Range(-spawnRadius, spawnRadius),
            4.0f, // Y level (you can adjust this as needed)
            UnityEngine.Random.Range(-spawnRadius, spawnRadius)
            );
            Agent newAgent = Instantiate(
                agentPrefab,
                randomPosition,
                Quaternion.identity,
                transform
            );
            newAgent.name = "Agent " + i;
            agents.Add(newAgent);
        }
    }

    // Update is called once per frame
    void Update()
    {
        foreach (Agent agent in agents){
            List<Transform> context = GetNearbyObjects(agent);

            Vector3 move = behaviour.CalculateMove(agent, context, this).normalized;
            float rollAngle = Mathf.Atan2(-move.y, move.z); // Calculate roll angle (φ)
            float pitchAngle = Mathf.Atan2(move.x, Mathf.Sqrt(move.y * move.y + move.z * move.z)); // Calculate pitch angle (θ)

            // move.x = Mathf.Clamp(move.x, -4f, 4f);
            // move.z = Mathf.Clamp(move.z, -4f, 4f);



            if(move.x > 0.5){
                agent.GetComponent<Rigidbody>().AddForceAtPosition(4 * agent.transform.up * move.x,agent.transform.position + new Vector3(-0.5f,0,0));
                Debug.Log("pitch down");
            }
            else if(move.x < -0.5){
                agent.GetComponent<Rigidbody>().AddForceAtPosition(4 * agent.transform.up * Math.Abs(move.x),agent.transform.position + new Vector3(0.5f,0,0));
            }
            if(move.z > 0.5){
                agent.GetComponent<Rigidbody>().AddForceAtPosition(4 * agent.transform.up * move.z,agent.transform.position + new Vector3(0,0,-0.5f));
                Debug.Log("roll left");
            }
            else if(move.z < -0.5){
                agent.GetComponent<Rigidbody>().AddForceAtPosition(4 * agent.transform.up * Math.Abs(move.z),agent.transform.position + new Vector3(0,0,0.5f));
            }

            // agent.GetComponent<Rigidbody>().AddForceAtPosition(agent.transform.up * move.x,agent.transform.position + new Vector3(0.5f,0,0));
            // agent.GetComponent<Rigidbody>().AddForceAtPosition(agent.transform.up * move.z,agent.transform.position + new Vector3(0,0,0.5f));

        }
    }

    List<Transform> GetNearbyObjects(Agent agent){
        List<Transform> context = new List<Transform>();

        Collider[] contextColliders = Physics.OverlapSphere(agent.transform.position, neighbourRadius);
        // print name and length of colliders
        // Debug.Log("Name:" + agent.transform.name + "Colliders: " + contextColliders.Length);

        foreach (Collider c in contextColliders){
            if (c != agent.AgentCollider){
                context.Add(c.transform);
            }
        }
        return context;
    }
}
