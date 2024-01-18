using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishSpawner : MonoBehaviour
{
    public GameObject finishPrefab;

    // Update is called once per frame
    void Start()
    {
        Vector2 range = Random.insideUnitCircle * 50;
        Vector3 spawnPosition = new Vector3(range.x, 4.0f, range.y);
        Instantiate(finishPrefab, spawnPosition, Quaternion.identity);
    }
}
