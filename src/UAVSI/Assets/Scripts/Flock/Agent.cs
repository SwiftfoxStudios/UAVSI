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
        agentCollider.isTrigger = true;
    }
    void OnDrawGizmosSelected(){
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 10);
    }
	void OnTriggerEnter(Collider collider)
	{
        if(collider.gameObject.tag == "Finish"){
            transform.parent.GetComponent<Flock>().CollisionDetected(this);
        }
        Debug.Log("Collision Detected");
	}
}
