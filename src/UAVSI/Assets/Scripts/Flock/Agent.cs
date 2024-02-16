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
    
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 10);
        
        // Cast a ray in the direction of the velocity vector
        Gizmos.color = Color.blue;
        Vector3 velocityDirection = transform.forward;
        Gizmos.DrawRay(transform.position, velocityDirection * 1f);
    }
    
    void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.tag == "Goal Reached")
        {
            transform.parent.GetComponent<Flock>().GoalReached(this);
        }
        
        // Debug.Log("Collision Detected");
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.gameObject.tag == "Perished")
        {
            transform.parent.GetComponent<Flock>().DroneLost(this);
        }
        else if (collision.collider.gameObject.tag == "In Transit")
        {
            Debug.Log("Collision!");
            transform.parent.GetComponent<Flock>().CollisionDetected(this);
        }
    }
}
