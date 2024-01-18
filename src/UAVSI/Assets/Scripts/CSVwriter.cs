using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class CSVwriter : MonoBehaviour
{
    string filename = "";

    [System.Serializable]
    public class DroneData
    {
        public int droneID;
        public float time;
        public float x;
        public float z;
        public float avoidance;
        public float alignment;
        public float cohesion;
    }
    [System.Serializable]
    public class DroneDataList
    {
        public DroneData[] droneData;
    }
    public DroneDataList mydroneData = new DroneDataList();
    // Start is called before the first frame update
    void Start()
    {
       filename = Application.dataPath + "/droneData.csv";
    }

    // Update is called once per frame
    void Update()
    {
        
    }


}
