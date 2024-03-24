using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

[RequireComponent(typeof(Collider))]
public class Agent : MonoBehaviour
{
    Collider agentCollider;
    float timeReachedGoal;
    Boolean goalReached = false;

    bool isPressed = false;
    bool isChanged = false;
    float timePressed = 0;
    
    public Collider AgentCollider { get { return agentCollider; } }
    
    // Start is called before the first frame update
    void Start()
    {
        agentCollider = GetComponent<Collider>();
        agentCollider.isTrigger = true;
        // InvokeRepeating("ModifyMove", 0.0f, 1.0f);
    }

    void Update()
    {

        if (Input.GetKeyDown(KeyCode.W))
        {
            isPressed = true;
            timePressed = Time.timeSinceLevelLoad;
        }
        if (Input.GetKeyUp(KeyCode.W))
        {
            isChanged = true;
            using (StreamWriter sw = File.AppendText("pitchAngle.csv"))
            {
                sw.WriteLine("KEYPRESS," + (Time.timeSinceLevelLoad - timePressed).ToString());
            }
        }
        // open a csv file to write to which records the difference in angle of rotation about the pitch axis from 15 degrees and the time every update
        if(isPressed && !isChanged)
        {

            float angle = Mathf.DeltaAngle(transform.localRotation.eulerAngles.z, 345);
            float angle2 = transform.localRotation.eulerAngles.z;
            float aRad = angle * Mathf.Deg2Rad;
            float ang2dp = Mathf.Round(aRad * 10000) / 10000;
            using (StreamWriter sw = File.AppendText("pitchAngle.csv"))
            {
                sw.WriteLine(ang2dp.ToString() + "," + (Time.timeSinceLevelLoad - timePressed).ToString());
            }
        }
        else if(isPressed && isChanged)
        {
            float angle = Mathf.DeltaAngle(transform.localRotation.eulerAngles.z, 0);
            float angle2 = transform.localRotation.eulerAngles.z;
            float aRad = angle * Mathf.Deg2Rad;
            float ang2dp = Mathf.Round(aRad * 10000) / 10000;
            using (StreamWriter sw = File.AppendText("pitchAngle.csv"))
            {
                sw.WriteLine(ang2dp.ToString() + "," + (Time.timeSinceLevelLoad - timePressed).ToString());
            }
        }
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
        if(!goalReached)
        {
            timeReachedGoal = Time.timeSinceLevelLoad;
            goalReached = true;
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
            // Debug.Log("Collision!");
            transform.parent.GetComponent<Flock>().CollisionDetected(this);
        }
    }

    public float GetTimeReachedGoal()
    {
        return timeReachedGoal;
    }

    private void ModifyMove()
    {
        Movement movement = GetComponent<Movement>();
        movement.kprop_pitchroll += 1f;
    }
}
