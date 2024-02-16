using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishSpawner : MonoBehaviour
{
public GameObject terrain;
public Collider meshCollider; 
public GameObject goalPlatform;

    void Start()
    {
        Vector2 range = Random.insideUnitCircle.normalized * 100;
        // Debug.Log(range);
        terrain = GameObject.Find("TerrainMesh");
        meshCollider = terrain.GetComponent<MeshCollider>();
        // Starting point of the ray (make sure it's above the highest point of the mesh)
        Vector3 rayStart = new Vector3(range.x, 1000.0f, range.y);

        // Ray direction (downwards)
        Vector3 rayDirection = Vector3.down;

        // Perform the raycast
        RaycastHit hit;
        if (meshCollider.Raycast(new Ray(rayStart, rayDirection), out hit, Mathf.Infinity))
        {
            Vector3 spawnPosition = new Vector3(range.x, hit.point.y + 5.0f, range.y);
            Instantiate(goalPlatform, spawnPosition, Quaternion.identity);
            // Debug.Log(hit.point.y);
        }
    }

}
