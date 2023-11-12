using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Agent : MonoBehaviour
{
    Collider agentCollider;
    public Collider AgentCollider { get { return agentCollider; } }
    
    // Start is called before the first frame update
    void Start()
    {
        agentCollider = GetComponent<Collider>();
    }
    void OnDrawGizmosSelected(){
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 10);
    }
}
