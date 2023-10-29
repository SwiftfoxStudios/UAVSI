using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEditor.Callbacks;
using UnityEngine;

public class movement : MonoBehaviour
{
    public Rigidbody uav;

    public GameObject thrustD1;
    public GameObject thrustD2;
    public GameObject thrustD3;
    public GameObject thrustD4;

    public float kprop_altitude = 6f;
    public float kinteg_altitude = 5f;
    public float kderiv_altitude = 2f;
    public float kprop_pitchroll= 7f;
    public float kinteg_pitchroll= 6f;
    public float kderiv_pitchroll= 0.5f;
    private float integral_altitude = 0f;
    private float integral_pitch = 0f;
    private float integral_yaw = 0f;
    private float integral_roll = 0f;
    private float lastError_altitude = 0f;
    private float lastError_pitch= 0f;
    private float lastError_roll = 0f;
    private float lastError_yaw = 0f;

    private bool firstFrame = true;
    private float targetAltitude = 0;

    float desiredPitchRate = 0;
    float pitchRate = 0;
    float pitchError = 0;
    float pitchInput = 0;

    float desiredRollRate = 0;
    float rollRate = 0;
    float rollError = 0;
    float rollInput = 0;

    float desiredYawRate = 0;
    float yawRate = 0;
    float yawError = 0;
    float yawInput = 0;

    // Start is called before the first frame update
    void Awake(){
        uav = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate(){
        VerticalAxis();
        PitchAxis();
        RollAxis();
        YawAxis();
    }

    // Pitch forward/backward
    void PitchAxis(){
        float thrust1;
        float thrust3;
        // Set pitch angle to 15 degrees
        if(Input.GetKey(KeyCode.W)){
            desiredPitchRate = -15f;
        }
        else if(Input.GetKey(KeyCode.S)){
            desiredPitchRate = 15f;
        }
        else{
            desiredPitchRate = 0;
        }
        // Wraps pitch angle s.t. centred around 0
        if(uav.rotation.eulerAngles.z > 180f){
            pitchRate = uav.rotation.eulerAngles.z - 360f;
        }
        else{
            pitchRate = uav.rotation.eulerAngles.z;
        }
        pitchError = desiredPitchRate - pitchRate;
        // Reset integral term if drone is tilted
        if (Mathf.Abs(pitchRate) > 3f){
            integral_pitch = 0;
        }

        // PID controller
        pitchInput = kprop_pitchroll* pitchError + integral_pitch + kinteg_pitchroll* (pitchError + lastError_pitch)/2 + kderiv_pitchroll* (pitchError - lastError_pitch) / Time.deltaTime;
        lastError_pitch= pitchError;
        
        // Clamp pitch input for safety
        pitchInput = Mathf.Clamp(pitchInput, -20f, 20f);
        integral_pitch = pitchInput;

        // thrust in the direction of the pitch
        thrust1 = pitchInput;
        thrust3 = -pitchInput;
        // add given thrust
        uav.AddForceAtPosition(transform.up * thrust1, thrustD1.transform.position);
        uav.AddForceAtPosition(transform.up * thrust3, thrustD3.transform.position);
    }

    // Roll left/right
    void RollAxis(){
        float thrust2;
        float thrust4;
        // Set roll angle to 15 degrees
        if(Input.GetKey(KeyCode.A)){
            desiredRollRate = 15f;
        }
        else if(Input.GetKey(KeyCode.D)){
            desiredRollRate = -15f;
        }
        else{
            desiredRollRate = 0;
        }
        // Wraps roll angle s.t. centred around 0
        if(uav.rotation.eulerAngles.x > 180f){
            rollRate = uav.rotation.eulerAngles.x - 360f;
        }
        else{
            rollRate = uav.rotation.eulerAngles.x;
        }
        rollError = desiredRollRate - rollRate;
        // Reset integral term if drone is tilted
        if (Mathf.Abs(rollRate) > 3f){
            integral_roll = 0;
        }

        // PID controller
        rollInput = kprop_pitchroll* rollError + integral_roll + kinteg_pitchroll* (rollError + lastError_roll)/2 + kderiv_pitchroll* (rollError - lastError_roll) / Time.deltaTime;
        lastError_roll = rollError;

        // Clamp roll input for safety
        rollInput = Mathf.Clamp(rollInput, -20f, 20f);
        integral_roll = rollInput;

        // thrust in the direction of the roll
        thrust2 = rollInput;
        thrust4 = -rollInput;
        // add given thrust
        uav.AddForceAtPosition(transform.up * thrust2, thrustD2.transform.position);
        uav.AddForceAtPosition(transform.up * thrust4, thrustD4.transform.position);
    }

    // Yaw left/right
    void YawAxis(){
        float thrust1;
        float thrust2;
        float thrust3;
        float thrust4;
        // Set yaw rate to 2 rad/s
        if(Input.GetKey(KeyCode.J)){
            desiredYawRate = 2f;
        }
        else if(Input.GetKey(KeyCode.L)){
            desiredYawRate = -2f;
        }
        else{
            desiredYawRate = 0;
        }

        yawRate = uav.angularVelocity.y;
        yawError = desiredYawRate - yawRate;

        // PID controller
        yawInput = kprop_pitchroll * yawError + integral_yaw + kinteg_pitchroll* (yawError + lastError_yaw)/2 + kderiv_pitchroll* (yawError - lastError_yaw) / Time.deltaTime;
        lastError_yaw = yawError;

        // Clamp yaw input for safety
        yawInput = Mathf.Clamp(yawInput, -2f, 2f);
        integral_yaw = yawInput;

        // thrust in the direction of the yaw
        thrust1 = -yawInput;
        thrust2 = -yawInput;
        thrust3 = +yawInput;
        thrust4 = +yawInput;
        // add given thrust
        uav.AddForceAtPosition(transform.forward * thrust1, thrustD1.transform.position);
        uav.AddForceAtPosition(transform.right * thrust2, thrustD2.transform.position);
        uav.AddForceAtPosition(transform.forward * thrust3, thrustD3.transform.position);
        uav.AddForceAtPosition(transform.right * thrust4, thrustD4.transform.position);
    }

    // Move up/down
    void VerticalAxis(){
        float thrust_all;
        if(Input.GetKey(KeyCode.I)){
            firstFrame = true;
            targetAltitude = transform.position.y;
            thrust_all = 34; 
        }
        else if(Input.GetKey(KeyCode.K)){
            firstFrame = true;
            targetAltitude = transform.position.y;
            thrust_all = 5;
        }
        // Set altitude when key is released
        else{
            if (firstFrame){
                targetAltitude = transform.position.y;
                firstFrame = false;
            }
            thrust_all = 24.5f;
        }
        float error = targetAltitude - transform.position.y;
        // Reset integral term if drone is tilted or if error is too small
        if (Mathf.Abs(error) < 0.01f || Mathf.Abs(error) > 0.5f){
            integral_altitude = 0;
        }
        // PID controller
        float throttleInput = kprop_altitude * error + kinteg_altitude * (error + lastError_altitude)/2 + kderiv_altitude * ((error - lastError_altitude) / Time.deltaTime);
        lastError_altitude = error; 

        // Clamp throttle input for safety
        throttleInput = Mathf.Clamp(throttleInput, -40f, 40f);
        integral_altitude = throttleInput;

        float thrust = thrust_all + throttleInput;
        // add given thrust
        uav.AddForceAtPosition(transform.up * thrust, thrustD1.transform.position);
        uav.AddForceAtPosition(transform.up * thrust, thrustD2.transform.position);
        uav.AddForceAtPosition(transform.up * thrust, thrustD3.transform.position);
        uav.AddForceAtPosition(transform.up * thrust, thrustD4.transform.position);
    }
}
