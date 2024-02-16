using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class FlockBehaviour : ScriptableObject
{
    public abstract Vector3 CalculateMove(Agent agent, List<Transform> context, Flock flock);
    public abstract Dictionary<string, float> WeightedBehaviours();
    public abstract void SetWeights(Dictionary<string, float> weights);
}
