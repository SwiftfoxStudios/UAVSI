using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartSpawner : MonoBehaviour
{
public GameObject terrain;
public Collider meshCollider; 
public GameObject startPlatform;

    void Start()
    {
        terrain = GameObject.Find("TerrainMesh");
        meshCollider = terrain.GetComponent<MeshCollider>();
        // Starting point of the ray (make sure it's above the highest point of the mesh)
        Vector3 rayStart = new Vector3(0, 1000.0f, 0);

        // Ray direction (downwards)
        Vector3 rayDirection = Vector3.down;

        // Perform the raycast
        RaycastHit hit;
        if (meshCollider.Raycast(new Ray(rayStart, rayDirection), out hit, Mathf.Infinity))
        {
            Vector3 spawnPosition = new Vector3(0, hit.point.y + 5.0f, 0);
            Instantiate(startPlatform, spawnPosition, Quaternion.identity);
        }
    }

}
