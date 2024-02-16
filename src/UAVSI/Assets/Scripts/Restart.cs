using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using Unity.VisualScripting;
using System;
using System.Linq;
using UnityEngine.UIElements;


public class Restart : MonoBehaviour
{
    public int simulationsToRun;
    public int simulationTime;
    public int timescale;
    public float framerate;
    public float temp;
void Start()
    {
        Invoke("Reload", simulationTime);
        Time.timeScale = timescale;
        Time.fixedDeltaTime = framerate;
        
        if (Input.GetKeyDown(KeyCode.R)){
            SceneManager.LoadScene( SceneManager.GetActiveScene().name );
        }
    }

    void Reload()
    {
        
        int simulationsToGo = int.Parse(GetSimulations()) - 1;
        // Debug.Log(simulationsToGo);
        if(simulationsToGo != 0)
        {
            if(simulationsToGo % 10 == 0)
            {
                temp = simulationsToRun / 10 - simulationsToGo / 10;
                AggregateSimulationData(simulationsToRun - 10 == simulationsToGo);
            }
            SetSimulations(simulationsToGo);
            SceneManager.LoadScene( SceneManager.GetActiveScene().name );
        }
        else
        {
            AggregateSimulationData(false);
            File.WriteAllText("./sims.txt", string.Empty);
            // AggregateSimulationData();
            UnityEditor.EditorApplication.isPlaying = false;
        }
        
    }

    public string GetSimulations()
    {
        if(new FileInfo("./sims.txt").Length == 0)
        {
            SetSimulations(simulationsToRun);
        }
        StreamReader reader = new StreamReader("./sims.txt");
        string sims = reader.ReadLine();
        reader.Close();
        return sims;
    }


    void SetSimulations(int simulationsLeft)
    {
        StreamWriter writer = new StreamWriter("./sims.txt", false);
        writer.WriteLine(simulationsLeft);
        writer.Close();
    }

    float costFunction(float firsttime, float w1, float reached, float total, float w2, float collisions, float w3)
    {
        // this function assumes the three parameters are bounded by (0,1)
        float w1n = w1 / (w1 + w2 + w3);
        float w2n = w2 / (w1 + w2 + w3);
        float w3n = w3 / (w1 + w2 + w3);

        float p1 = w1n * (firsttime / simulationTime);
        float p2 = w2n * (reached / total);
        float p3 = (float)(w3n * (1 / (1 + Math.Exp(-0.1 *(collisions - 40)))));
        return p1 + p2 + p3;
    }

    void AggregateSimulationData(bool firstRun)
    {
        int averageDroneLost = 0;
        int averageDroneDead = 0;
        int averageDroneReached = 0;
        int averageCollisions = 0;
        float averageTimeReached = 0;

        float timeWeightedCost = 0;
        float collisionWeightedCost = 0;
        float droneLostWeightedCost = 0;

        float al = 0, av = 0, co = 0, se = 0, ta = 0;

        StreamReader reader = new StreamReader("./Assets/droneData.csv");
        for(int i = 0; i < 10; i++)
        {
            reader.ReadLine();
            string line = reader.ReadLine();
            string[] values = line.Split(',');
            av = float.Parse(values[0]);
            al = float.Parse(values[1]);
            co = float.Parse(values[2]);
            se = float.Parse(values[3]);
            ta = float.Parse(values[4]);
            averageDroneReached += int.Parse(values[5]);
            averageDroneLost += int.Parse(values[6]);
            averageDroneDead += int.Parse(values[7]);
            averageTimeReached += float.Parse(values[8]);
            averageCollisions += int.Parse(values[9]);
        }
        averageDroneLost /= 10;
        averageDroneDead /= 10;
        averageDroneReached /= 10;
        averageTimeReached /= 10;
        averageCollisions /= 10;


        timeWeightedCost = costFunction(averageTimeReached, 2, averageDroneReached, 8, 1, averageCollisions, 0);
        collisionWeightedCost = costFunction(averageTimeReached, 1, averageDroneReached, 8, 1, averageCollisions, 2);
        droneLostWeightedCost = costFunction(averageTimeReached, 1, averageDroneReached, 8, 2, averageCollisions, 0);
        reader.Close();


        File.WriteAllText("./Assets/droneData.csv", string.Empty);
        string prevRaw = File.ReadLines("./simData.csv").Last();

        float[] currBehaviourList = new float[] {av, al, co, se, ta};

        string prev = prevRaw.Substring(prevRaw.LastIndexOf(',') + 1);
        Debug.Log(firstRun);

        if(!firstRun && float.Parse(prev) < timeWeightedCost && !Anneal(float.Parse(prev) - timeWeightedCost, temp))
        {
            float[] prevBehaviourList = Array.ConvertAll(prevRaw.Split(','), float.Parse);
            Debug.Log("reverting " + timeWeightedCost + currBehaviourList[0] + currBehaviourList[1] + currBehaviourList[2] + currBehaviourList[3] + currBehaviourList[4]);
            Flock flock = GameObject.Find("Flock").GetComponent<Flock>();
            FlockBehaviour behav = flock.behaviour;
            Dictionary<string, float> newBehav = new Dictionary<string, float>()
            {
                { "avoidance", prevBehaviourList[0] },
                { "alignment", prevBehaviourList[1] },
                { "cohesion", prevBehaviourList[2] },
                { "seeking", prevBehaviourList[3] },
                { "terrain", prevBehaviourList[4] }
            };
            behav.SetWeights(newBehav);

            // StreamWriter writer2 = new StreamWriter("./weights.csv", false);
            // foreach (KeyValuePair<string, float> entry in newBehav)
            // {
            //     writer2.WriteLine(entry.Key + "," + entry.Value);
            // }
            // writer2.Close();
        }
        else
        {
            Debug.Log("keeping");
            StreamWriter writer = new StreamWriter("./simData.csv", true);
            writer.WriteLine(av + "," + al + "," + co + "," + se + "," + ta + "," + timeWeightedCost);
            writer.Close();
        }

    }

    bool Anneal(float delta, float temperature)
    {
        temperature = 1 / Mathf.Log(1 + temperature);
        float probability = Mathf.Exp(-delta / temperature);
        return UnityEngine.Random.value < probability;
    }

}

