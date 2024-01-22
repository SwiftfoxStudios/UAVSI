using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public int mapWidth;
    public int mapHeight;
    public float scale;
    public int octaves;
    [Range(0, 1)]
    public float persistance;
    public float lacunarity;   
    public int seed;

    public float multiplier;

    public void GenerateMap()
    {
        float[,] noiseMap = Noise.GenerateNoiseMap(mapWidth, mapHeight, seed, scale, octaves, persistance, lacunarity);

        MapDisplay display = FindObjectOfType<MapDisplay>(); 
        display.DrawMesh(MeshGenerator.GenerateTerrainMesh(noiseMap, multiplier));
    }
}
