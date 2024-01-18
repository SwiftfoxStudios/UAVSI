using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Unity.VisualScripting;


public class Flock : MonoBehaviour
{
        public class DroneData
    {
        public string droneID;
        public float time;
        public float x;
        public float z;
    }
    string filename = Application.dataPath + "/droneData.csv";
    bool goalReached = false;

    public Agent agentPrefab;
    List<Agent> agents = new List<Agent>();
    List<DroneData> droneData = new List<DroneData>();
    public FlockBehaviour behaviour;
    public int numAgents = 2;
    public float spawnRadius = 5.0f;
    public float neighbourRadius = 10.0f;

    public float SquareAvoidanceRadius { get { return 12.0f; } }
    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("UpdatePositions", 0.0f, 0.5f);
        Invoke("WriteCSV", 20.0f);

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
            newAgent.tag = "In Transit";
            agents.Add(newAgent);
        }
    }

    public void CollisionDetected(Agent agent){
        agent.tag = "Goal Reached";
        Debug.Log("Collision Detected with agent" + agent.name);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        foreach (Agent agent in agents){
            // droneData.Add(new DroneData{droneID = agent.name, time = Time.time, x = agent.transform.position.x, z = agent.transform.position.z});
            List<Transform> context = GetNearbyObjects(agent);
            // agent.GetComponent<Rigidbody>().velocity = Vector3.zero;


            Vector3 rawMove = behaviour.CalculateMove(agent, context, this);
            Vector3 move = new Vector3((float)(15f * Math.Tanh(0.3f * rawMove.x)), (float)(13.5f * Math.Tanh(0.48f + 0.1 * rawMove.y) + 18.5f), (float)(15f * Math.Tanh(0.3f * rawMove.z))); 
            Debug.Log("Move: " + move);

            if(agent.tag == "Goal Reached"){
                move = new Vector3(0, 24.5f, 0);
            }

            // Vector3 move = new Vector3(Mathf.Clamp(rawMove.x, -clampingFactor, clampingFactor), Mathf.Clamp(rawMove.y, -clampingFactor, clampingFactor), Mathf.Clamp(rawMove.z, -clampingFactor, clampingFactor));
            // float rollAngle = Mathf.Atan2(-move.y, move.z); // Calculate roll angle (φ)
            // float pitchAngle = Mathf.Atan2(move.x, Mathf.Sqrt(move.y * move.y + move.z * move.z)); // Calculate pitch angle (θ)

            // move.x = Mathf.Clamp(move.x, -4f, 4f);
            // move.z = Mathf.Clamp(move.z, -4f, 4f);

            // Increase/Decrease Height
            agent.GetComponent<Rigidbody>().AddForceAtPosition(move.y * agent.transform.up, agent.transform.position + new Vector3(0,0,0.5f));
            agent.GetComponent<Rigidbody>().AddForceAtPosition(move.y * agent.transform.up,agent.transform.position + new Vector3(0,0,-0.5f));
            agent.GetComponent<Rigidbody>().AddForceAtPosition(move.y * agent.transform.up,agent.transform.position + new Vector3(-0.5f,0,0));
            agent.GetComponent<Rigidbody>().AddForceAtPosition(move.y * agent.transform.up, agent.transform.position + new Vector3(0.5f,0,0));
            

            if(move.x > 0.1){
                agent.GetComponent<Rigidbody>().AddForceAtPosition(1 * agent.transform.up * move.x,agent.transform.position + new Vector3(-0.5f,0,0));
                agent.GetComponent<Rigidbody>().AddForceAtPosition(1 * agent.transform.up * -move.x,agent.transform.position + new Vector3(0.5f,0,0));

            }
            else if(move.x < -0.1){
                agent.GetComponent<Rigidbody>().AddForceAtPosition(1 * agent.transform.up * Math.Abs(move.x),agent.transform.position + new Vector3(0.5f,0,0));
                agent.GetComponent<Rigidbody>().AddForceAtPosition(1 * agent.transform.up * move.x,agent.transform.position + new Vector3(-0.5f,0,0));
            }
            if(move.z > 0.1){
                agent.GetComponent<Rigidbody>().AddForceAtPosition(1 * agent.transform.up * move.z,agent.transform.position + new Vector3(0,0,-0.5f));
                agent.GetComponent<Rigidbody>().AddForceAtPosition(1 * agent.transform.up * -move.z,agent.transform.position + new Vector3(0,0,0.5f));
            }
            else if(move.z < -0.1){
                agent.GetComponent<Rigidbody>().AddForceAtPosition(1 * agent.transform.up * Math.Abs(move.z),agent.transform.position + new Vector3(0,0,0.5f));
                agent.GetComponent<Rigidbody>().AddForceAtPosition(1 * agent.transform.up * move.z,agent.transform.position + new Vector3(0,0,-0.5f));
            }

            // agent.GetComponent<Rigidbody>().AddForceAtPosition(agent.transform.up * move.x,agent.transform.position + new Vector3(0.5f,0,0));
            // agent.GetComponent<Rigidbody>().AddForceAtPosition(agent.transform.up * move.z,agent.transform.position + new Vector3(0,0,0.5f));

        }
    }

    public void UpdatePositions()
    {
        foreach (Agent agent in agents){
            droneData.Add(new DroneData{droneID = agent.name, time = Time.time, x = agent.transform.position.x, z = agent.transform.position.z});
        }
    }

    public void WriteCSV()
    {
        if(droneData.Count > 0)
        {
            TextWriter tw = new StreamWriter(filename, false);
            tw.WriteLine("DroneID,Time,X,Z");
            tw.Close();

            tw = new StreamWriter(filename, true);

            for(int i = 0; i < droneData.Count; i++)
            {
                tw.WriteLine(droneData[i].droneID + "," + droneData[i].time + "," + droneData[i].x + "," + droneData[i].z);
            }
            tw.Close();
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
