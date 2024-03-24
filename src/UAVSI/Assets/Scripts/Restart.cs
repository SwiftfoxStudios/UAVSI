using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using Unity.VisualScripting;
using System;
using System.Linq;
using UnityEngine.UIElements;
using UnityEditor;


public class Restart : MonoBehaviour
{
    public int simulationsToRun;
    public int simulationTime;
    public int timescale;
    public float framerate;
    public float temp;

    public int simsPerRun = 10;
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
            if(simulationsToGo % simsPerRun == 0)
            {
                temp = (float)simulationsToGo / simulationsToRun;
                AggregateSimulationData(simulationsToRun - simsPerRun == simulationsToGo);
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

    float costFunction(float firsttime, float w1, float reached, float total, float w2, float collisions, float w3, float sigma, float w4)
    {
        // this function assumes the three parameters are bounded by (0,1)
        float w1n = w1 / (w1 + w2 + w3 + w4);
        float w2n = w2 / (w1 + w2 + w3 + w4);
        float w3n = w3 / (w1 + w2 + w3 + w4);
        float w4n = w4 / (w1 + w2 + w3 + w4);

        // higher = worse
        float p1 = w1n * (firsttime / simulationTime);
        float p2 = w2n * ((total - reached) / total);
        float p3 = (float)(w3n * (1 / (1 + Math.Exp(-0.1 *(collisions - 40)))));
        float p4;
        if(sigma == 0 && firsttime == simulationTime)
        {
            // no drones have arrived, so standard deviation is 0, but the time is the maximum.
            p4 = w4n * 1;
        }
        else
        {
            p4 = w4n * (sigma / simulationTime);
        }
        return p1 + p2 + p3 + p4;
    }

    void AggregateSimulationData(bool firstRun)
    {
        int averageDroneLost = 0;
        int averageDroneDead = 0;
        int averageDroneReached = 0;
        int averageCollisions = 0;
        float averageTimeReached = 0;
        float averageSigma = 0;


        float timeWeightedCost = 0;
        float collisionWeightedCost = 0;
        float droneProportionWeightedCost = 0;

        float al = 0, av = 0, co = 0, se = 0, ta = 0;

        StreamReader reader = new StreamReader("./Assets/droneData.csv");
        for(int i = 0; i < simsPerRun; i++)
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
            // split a string by spaces into a list
            float[] timesList = Array.ConvertAll(values[10].Split(' '), float.Parse);
            // calculate the standard deviation of the list
            averageSigma += StandardDeviation(timesList);

        }
        averageDroneLost /= simsPerRun;
        averageDroneDead /= simsPerRun;
        averageDroneReached /= simsPerRun;
        averageTimeReached /= simsPerRun;
        averageCollisions /= simsPerRun;
        averageSigma /= simsPerRun;


        // get parsedTimesList, in the form '[0.22,0.34,0.44]' back into a list format. Calculate the standard deviation
        


        timeWeightedCost = costFunction(averageTimeReached, 2, averageDroneReached, 8, 1, averageCollisions, 0, averageSigma, 0);
        // collisionWeightedCost = costFunction(averageTimeReached, 1, averageDroneReached, 8, 1, averageCollisions, 2, averageSigma, 0);
        // droneProportionWeightedCost = costFunction(averageTimeReached, 1, averageDroneReached, 8, 2, averageCollisions, 0, averageSigma, 2);
        reader.Close();

        float usedCost = timeWeightedCost;


        File.WriteAllText("./Assets/droneData.csv", string.Empty);
        string prevRaw = File.ReadLines("./simData.csv").Last();

        float[] currBehaviourList = new float[] {av, al, co, se, ta};

        string prev = prevRaw.Substring(prevRaw.LastIndexOf(',') + 1);
        Debug.Log(firstRun);

        if(!firstRun && float.Parse(prev) < usedCost && !Anneal(float.Parse(prev) - usedCost, temp)) //
        {
            float[] prevBehaviourList = Array.ConvertAll(prevRaw.Split(','), float.Parse);
            Debug.Log("reverting " + usedCost + currBehaviourList[0] + currBehaviourList[1] + currBehaviourList[2] + currBehaviourList[3] + currBehaviourList[4]);
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
            writer.WriteLine(av + "," + al + "," + co + "," + se + "," + ta + "," + usedCost);
            writer.Close();

            StreamWriter writer2 = new StreamWriter("./values.csv", true);
            writer2.WriteLine(averageDroneReached + "," + averageDroneLost + "," + averageDroneDead + "," + averageTimeReached + "," + averageCollisions + "," + averageSigma); 
            writer2.Close();
        }

    }

    bool Anneal(float delta, float temperature)
    {
        float probability = Mathf.Exp(1 * delta / temperature);
        Debug.Log("probability to keep: " + probability + " with delta: " + delta);
        return UnityEngine.Random.value < probability;
    }

    // create a function that calulates the standard deviation of a list of floats
    float StandardDeviation(float[] values)
    {
        float mean = values.Average();
        float sumOfSquares = 0;
        foreach(float value in values)
        {
            sumOfSquares += (value - mean) * (value - mean);
        }
        return Mathf.Sqrt(sumOfSquares / values.Length);
    }

}

