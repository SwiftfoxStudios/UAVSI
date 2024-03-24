using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
public GameObject terrain;
public Collider meshCollider; 
public GameObject obstacle;

    void Start()
    {
        terrain = GameObject.Find("TerrainMesh");
        meshCollider = terrain.GetComponent<MeshCollider>();
        for(int i = 0; i < 125; i++)
        {
            // Starting point of the ray (make sure it's above the highest point of the mesh)
            Vector3 rayStart = new Vector3(Random.Range(-125, 125), 1000.0f, Random.Range(-125, 125));

            // Ray direction (downwards)
            Vector3 rayDirection = Vector3.down;

            // Perform the raycast
            RaycastHit hit;
            if (meshCollider.Raycast(new Ray(rayStart, rayDirection), out hit, Mathf.Infinity))
            {
                Vector3 spawnPosition = new Vector3(hit.point.x, hit.point.y + 5.0f, hit.point.z);
                GameObject spawnedObs = Instantiate(obstacle, spawnPosition, Quaternion.identity);
                spawnedObs.transform.localScale = new Vector3(Random.Range(3,8), Random.Range(5,20), Random.Range(3,8));
            }
        }
    }

}
