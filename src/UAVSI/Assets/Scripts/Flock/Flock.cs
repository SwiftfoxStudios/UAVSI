using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Unity.VisualScripting;
using UnityEngine.Rendering;
using System.Linq;


public class Flock : MonoBehaviour
{
        public class DroneData
    {
        public float av,al,co,se,ta;
    }
    
    string filename = Application.dataPath + "/droneData.csv";
    bool goalReached = false;

    public Agent agentPrefab;
    List<Agent> agents = new List<Agent>();
    List<DroneData> droneData = new List<DroneData>();
    public FlockBehaviour behaviour;
    public Dictionary<string, float> lastChangeMade;
    
    public int numAgents = 2;
    public float spawnRadius = 5.0f;
    public float neighbourRadius = 10.0f;

    public float SquareAvoidanceRadius { get { return 12.0f; } }

    public float timeReached = 60.0f;

    public int collisionNumber = 0;
    // Start is called before the first frame update
    void Start()
    {
        Dictionary<string, float> behavs;
        // reset all weights to what was last recorded
        string prev = File.ReadLines("./simData.csv").Last();
        if(!Char.IsLetter(prev.ToCharArray()[0]))
        {
            float[] prevBehaviourList = Array.ConvertAll(prev.Split(','), float.Parse);
            behavs = new Dictionary<string, float>()
            {
                { "avoidance", prevBehaviourList[0] },
                { "alignment", prevBehaviourList[1] },
                { "cohesion", prevBehaviourList[2] },
                { "seeking", prevBehaviourList[3] },
                { "terrain", prevBehaviourList[4] }
            };
        }
        else
        {
            behavs = behaviour.WeightedBehaviours();
        }
        // get number of runs left
        StreamReader reader = new StreamReader("./sims.txt");
        string sims = reader.ReadLine();
        reader.Close();
        if(sims != null && int.Parse(sims) % 10 == 0)
        {
            behaviour.SetWeights(ModifyBehaviours(behavs));
        }
        



        // Dictionary<string, float> behavList;
        // // if not first run, get weights from file
        // if (new FileInfo("./weights.csv").Length != 0)
        // {
        //     StreamReader read = new StreamReader("./weights.csv");
        //     string line = read.ReadLine();
        //     behavList = new Dictionary<string, float>();
        //     while (line != null)
        //     {
        //         string[] values = line.Split(',');
        //         behavList.Add(values[0], float.Parse(values[1]));
        //         line = read.ReadLine();
        //     }
        //     read.Close();
        // }
        // // else, get weights from behaviour
        // else
        // {
        //     behavList = behaviour.WeightedBehaviours();
        // }
        


        
        // some annealing must be done here:

        // if the change is better, keep it
        // if the change is worse, keep it with a certain probability
        // repeat



        InvokeRepeating("UpdatePositions", 0.0f, 0.5f);
        Invoke("WriteCSV", 59f);

        for (int i=0; i < numAgents; i++){
            Vector3 randomPosition = new Vector3(
            UnityEngine.Random.Range(-spawnRadius, spawnRadius),
            GameObject.FindWithTag("Respawn").transform.position.y + 10f, // Y level (you can adjust this as needed)
            // 4.0f,
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
            foreach (Transform child in newAgent.transform)
            {
                child.gameObject.tag = "In Transit";
            }
            agents.Add(newAgent);
        }
    }

    public void CollisionDetected(Agent agent){
        collisionNumber++;
    }

    public void GoalReached(Agent agent){
        agent.tag = "Goal Reached";
        // Debug.Log("Goal Reached" + agent.name);
    }

    public void DroneLost(Agent agent){
        agent.tag = "Perished";
        // Debug.Log("Drone Lost" + agent.name);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        foreach (Agent agent in agents){
            // droneData.Add(new DroneData{droneID = agent.name, time = Time.time, x = agent.transform.position.x, z = agent.transform.position.z});
            List<Transform> context = GetNearbyObjects(agent);
            // agent.GetComponent<Rigidbody>().velocity = Vector3.zero;


            Vector3 rawMove = behaviour.CalculateMove(agent, context, this);
            // Debug.Log("Raw Move: " + rawMove);
            Vector3 move = new Vector3((float)(15f * Math.Tanh(0.05f * rawMove.x)), (float)(13.5f * Math.Tanh(0.48f + 0.1 * rawMove.y) + 18.5f), (float)(15f * Math.Tanh(0.05f * rawMove.z))); 
            // Debug.Log("Move: " + move);

            if(agent.tag == "Goal Reached"){
                if(!goalReached)
                {
                    timeReached = Time.timeSinceLevelLoad;
                    goalReached = true;
                }
                move = new Vector3(0, 24.5f, 0);
            }
            if(agent.tag == "Perished"){
                move = new Vector3(0, 0, 0);
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

        // foreach (Agent agent in agents){
        //     droneData.Add(new DroneData{
        //         av = behavList["avoidance"], 
        //         al = behavList["alignment"],
        //         co = behavList["cohesion"],
        //         se = behavList["seeking"],
        //         ta = behavList["terrain"],});
        //     // droneData.Add(new DroneData{droneID = agent.name, time = Time.time, x = agent.transform.position.x, z = agent.transform.position.z});
        // }
    }

    // Dictionary<string, float> ModifyBehaviours(Dictionary<string, float> behavList)
    // {
    //     Dictionary<string, float> newBehav = new Dictionary<string, float>(behavList);

    //     // Pick a random key from the dictionary
    //     List<string> keys = new List<string>(newBehav.Keys);
    //     string randomKey = keys[UnityEngine.Random.Range(0, keys.Count)];

    //     // Modify the weight of the randomly picked key
    //     float currentWeight = newBehav[randomKey];
    //     float modifiedWeight = currentWeight + (UnityEngine.Random.value < 0.5f ? 0.5f : -0.5f);
    //     newBehav[randomKey] = modifiedWeight;

    //     return newBehav;
    // }

    Dictionary<string, float> ModifyBehaviours(Dictionary<string, float> behavList)
    {
        Dictionary<string, float> newBehav = new Dictionary<string, float>(behavList);

        // Pick a random key from the dictionary
        List<string> keys = new List<string>(newBehav.Keys);
        string randomKey = keys[UnityEngine.Random.Range(0, keys.Count)];

        // Modify the weight of the randomly picked key
        float currentWeight = newBehav[randomKey];
        float modifiedWeight = currentWeight + (UnityEngine.Random.value < 0.5f ? 0.5f : -0.5f);
        newBehav[randomKey] = modifiedWeight;

        return newBehav;
    }

    Dictionary<string, float> GetDifferentValues(Dictionary<string, float> newBehav, Dictionary<string, float> behavList)
    {
        Dictionary<string, float> differences = new Dictionary<string, float>();
        foreach (KeyValuePair<string, float> entry in newBehav)
        {
            if (behavList.ContainsKey(entry.Key) && behavList[entry.Key] != entry.Value)
            {
                differences.Add(entry.Key, entry.Value - behavList[entry.Key]);
            }
        }
        return differences;
    }

    bool Anneal()
    {
        List <string> text = File.ReadLines("./simData.csv").Reverse().Take(2).ToList();
        for(int i = 0; i < text.Count; i++)
        {
            text[i] = text[i].Substring(text[i].LastIndexOf(',') + 1);
        }
        // text[1] is the cost of the initial run
        // text[0] is the cost of the annealed run
        if(float.Parse(text[0]) < float.Parse(text[1]))
        {
            return true;
        }
        else
        {
            // float probability = (float)Math.Exp((float.Parse(text[1]) - float.Parse(text[0])) / 100);
            // if(UnityEngine.Random.value < probability)
            // {
            //     return true;
            // }
            // else
            // {
            //     return false;
            // }
            return false;
        }
    }

    public void WriteCSV()
    {
        // if(droneData.Count > 0)
        // {
            TextWriter tw = new StreamWriter(filename, true);
            tw.WriteLine("Avoidance,Alignment,Cohesion,Seeking,TerrainAvoidance,NumberReached,NumberLostFlock,NumberDead,TimeForFirst,Collisions");
            tw.Close();
            Dictionary<string, float> behavList = behaviour.WeightedBehaviours();

            tw = new StreamWriter(filename, true);

            // for(int i = 0; i < droneData.Count; i++)
            // {
                tw.WriteLine(
                            behavList["avoidance"] + "," + 
                            behavList["alignment"] + "," + 
                            behavList["cohesion"] + "," + 
                            behavList["seeking"] + "," +
                            behavList["terrain"] + "," +
                            agents.FindAll(x => x.tag == "Goal Reached").Count + "," +
                            (numAgents - agents.FindAll(x => x.tag == "Goal Reached").Count - agents.FindAll(x => x.tag == "Perished").Count) + "," +
                            agents.FindAll(x => x.tag == "Perished").Count + "," +
                            timeReached + "," +
                            collisionNumber
                            );
            // }
            tw.Close();
        // }
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
