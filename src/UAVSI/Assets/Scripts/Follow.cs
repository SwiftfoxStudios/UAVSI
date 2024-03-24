using UnityEngine;

public class Follow : MonoBehaviour
{
    public GameObject flock;
    public float smoothSpeed = 0.000005f;
    public Vector3 offset;

    void FixedUpdate()
    {
        GameObject target = GameObject.FindWithTag("Finish");
        Vector3 averagePosition = new Vector3(0, 0, 0);
        int validChildrenCount = 0; // Keep track of children without the 'perished' tag

        for (int i = 0; i < flock.transform.childCount; i++)
        {
            GameObject child = flock.transform.GetChild(i).gameObject;
            // Only consider the child if it doesn't have the 'perished' tag
            if (child.tag != "Perished")
            {
                Vector3 desiredPosition = child.transform.position;
                averagePosition += desiredPosition;
                validChildrenCount++; // Only count this child if it's not 'perished'
            }
        }

        // Ensure we have at least one valid child to avoid division by zero
        if (validChildrenCount > 0)
        {
            averagePosition /= validChildrenCount;

            // Calculate distance ignoring the y-axis
            Vector3 targetPositionXZ = new Vector3(target.transform.position.x, 0, target.transform.position.z);
            Vector3 agentPositionXZ = new Vector3(averagePosition.x, 0, averagePosition.z);
            float distance = Vector3.Distance(targetPositionXZ, agentPositionXZ);

            // Check if the distance is greater than or equal to 30
            if (distance >= 15)
            {
                // Face the target (rotation)
                transform.LookAt(target.transform.position);

                // Move the camera backwards a bit
                Vector3 desiredPosition = averagePosition - transform.forward * 20;
                Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
                transform.position = smoothedPosition;
            }
            else if (distance > 10)
            {
                transform.position += transform.forward * 0.1f;
            }
        }
    }
}
