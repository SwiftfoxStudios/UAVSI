using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class motor : MonoBehaviour
{
    Rigidbody uav;
    // Start is called before the first frame update
    void Awake(){
        uav = GetComponent<Rigidbody>();
    }

    void FixedUpdate(){
        MovementUpDown();
        MovementForward();
        uav.AddRelativeForce(Vector3.up * upForce);
    }

    public float upForce;
    void MovementUpDown(){
        if(Input.GetKey(KeyCode.I)){
            upForce = 450;
        }
        else if(Input.GetKey(KeyCode.K)){
            upForce = -200;
        }
        else if(!Input.GetKey(KeyCode.I) && !Input.GetKey(KeyCode.K)){
            upForce = -98.1f;
        }
    }

    private float movementForwardSpeed = 500.0f;
    private float tiltAmountForward = 0;
    private float tiltVelocityForward;
    void MovementForward(){
        if(Input.GetAxis("Vertical") != 0){
            uav.AddRelativeForce(Vector3.forward * Input.GetAxis("Vertical") * movementForwardSpeed);
            tiltAmountForward = Mathf.SmoothDamp(tiltAmountForward, 20 * Input.GetAxis("Vertical"), ref tiltVelocityForward, 0.1f);
        }
    }


}
