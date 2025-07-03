using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testQuaternion : MonoBehaviour
{
    [SerializeField]
    private Transform trackerElbowRight; // The right elbow tracker
    [SerializeField]
    private Transform trackerElbowLeft; // The left elbow tracker
    [SerializeField]
    private Transform trackerChest; // The reference point, e.g., the spine or chest
    [SerializeField]
    private Transform controllerRight; // The right controller
    [SerializeField]
    private Transform controllerLeft; // The left controller

    private Quaternion initialRotation;

    private Vector3 previousEulerAngles;

    void Start()
    {
        // Store the initial rotation of the shoulder relative to the reference point
        initialRotation = Quaternion.Inverse(trackerChest.rotation) * trackerElbowRight.rotation;
        previousEulerAngles = trackerElbowRight.rotation.eulerAngles;

    }

    void Update()
    {
        RightShoulderRotation();
    }


    private void RightShoulderRotation()
    {
        // Calculate the current relative rotation
        // Quaternion currentRotation = Quaternion.Inverse(trackerChest.rotation) * trackerElbowRight.rotation;
        // Quaternion relativeRotation = currentRotation * Quaternion.Inverse(initialRotation);

        // // Decompose the quaternion to get the rotation around each axis
        // Vector3 eulerAngles = relativeRotation.eulerAngles;

        // if(eulerAngles.z > 180) eulerAngles.z -= 360;

        // Debug.Log("EulerAngles: " + eulerAngles);
        // Get the angle and axis of the relative rotation
        // Isolate the rotation around a specific axis (e.g., Z axis)
        // Calculate the current relative rotation
        // Get the current rotation in Euler angles
            // Calculate the current relative rotation as a quaternion
        Quaternion currentRotation = Quaternion.Inverse(trackerChest.rotation) * trackerElbowRight.rotation;
        Quaternion relativeRotation = currentRotation * Quaternion.Inverse(initialRotation);

        // Use the quaternion directly for any further calculations
        // (e.g., applying this rotation to a model, further processing, etc.)

        // Convert to Euler angles only for output or inspection
        Vector3 eulerAngles = relativeRotation.eulerAngles;

        // Optionally, clamp or adjust the Euler angles to avoid erratic behavior
        eulerAngles.x = Mathf.Clamp(eulerAngles.x, -90f, 90f); // Example clamping
        eulerAngles.y = Mathf.Clamp(eulerAngles.y, -90f, 90f);
        eulerAngles.z = Mathf.Clamp(eulerAngles.z, -90f, 90f);

        Debug.Log("EulerAngles: " + eulerAngles);
    }


    private void RightShoulderRotationFixed()
    {
        // Calculate the current relative rotation
        Quaternion currentRotation = Quaternion.Inverse(trackerChest.rotation) * trackerElbowRight.rotation;
        Quaternion relativeRotation = currentRotation * Quaternion.Inverse(initialRotation);

        // Extract the Euler angles from the relative rotation
        Vector3 eulerAngles = relativeRotation.eulerAngles;


        // Unity uses a left-handed coordinate system for Transformations
        // In your skizze, you might need to adjust the coordinate extraction according to your need

        // Assuming skizze corresponds to shoulder movement around different axes:
        float shoulderFlexion = eulerAngles.x;  // Flexion/Extension around X-axis
        float shoulderAbduction = eulerAngles.y; // Abduction/Adduction around Y-axis
        float shoulderRotation = eulerAngles.z; // Internal/External Rotation around Z-axis

        // Correct the angles if necessary based on the coordinate system differences
        shoulderFlexion = NormalizeAngle(shoulderFlexion);
        shoulderAbduction = NormalizeAngleY(shoulderAbduction);
        shoulderRotation = NormalizeAngleZ(shoulderRotation);

        Debug.Log("The Right shoulder flexion/extension (X-axis): " + shoulderFlexion + " degrees");
        // Debug.Log("Initial Rotation eulers X: " + initialRotation.eulerAngles.x + " Y: " +  initialRotation.eulerAngles.y + " Z: " + initialRotation.eulerAngles.z);
        Debug.Log("The Right shoulder abduction/adduction (Y-axis): " + shoulderAbduction + " degrees");
        Debug.Log("The Right shoulder internal/external rotation (Z-axis): " + shoulderRotation + " degrees");
    }

    private void RightShoulderRotationXX()
    {
        Quaternion currentRotation = Quaternion.FromToRotation(trackerChest.right, trackerElbowRight.right) ;

        Quaternion relativeRotation = currentRotation * Quaternion.Inverse(initialRotation);

        Vector3 eulerAngles = relativeRotation.eulerAngles;
        Debug.Log("The Right shoulder vertical rotation is: " + eulerAngles.x + " degrees");
        // Debug.Log("The Right shoulder horizontal rotation is: " + eulerAngles.y + " degrees");

    }

    private float NormalizeAngle(float angle) 
    {
        if (angle > 180) angle -= 360;
        if (angle < -180) angle += 360;
        return angle;
    }


    private float NormalizeAngleX(float angle) 
    {

        bool reached = false;

        float angleXplusInitial = angle + initialRotation.eulerAngles.x;


        if (angleXplusInitial > 180) angleXplusInitial -= 360;

        if(angleXplusInitial >= 90) reached = true;

        if(reached){
            print("90 degrees REACHED!!!!");
        }

        return angleXplusInitial;
    }

    private float NormalizeAngleXX(float angle) 
    {

        if (angle > 180) angle -= 360;

        return angle;
    }

    private float NormalizeAngleY(float angle) 
    {

        float angleXplusInitial = angle + initialRotation.eulerAngles.x;
        if (angle > 180) angle -= 360;
        return angle;
    }

    private float NormalizeAngleZ(float angle) 
    {
        float angleXplusInitial = angle + initialRotation.eulerAngles.x;

        if (angle > 180) angle -= 360;
        return angle;
    }


    private void LeftShoulderRotation(){
        // Calculate the current relative rotation
        Quaternion currentRotation = Quaternion.Inverse(trackerChest.rotation) * trackerElbowLeft.rotation;
        Quaternion relativeRotation = currentRotation * Quaternion.Inverse(initialRotation);

        // Extract the Euler angles from the relative rotation
        Vector3 eulerAngles = relativeRotation.eulerAngles;

        // Adjust for the specific axes if necessary
        float angleY = eulerAngles.y; // Y axis rotation
        float angleX = eulerAngles.x; // X axis rotation
        float angleZ = eulerAngles.z; // Z axis rotation

        Debug.Log("The Left shoulder angle around Y axis is: " + angleY + " degrees");
        Debug.Log("The Left shoulder angle around X axis is: " + angleX + " degrees");
        Debug.Log("The Left shoulder angle around Z axis is: " + angleZ + " degrees");
    }

    void ShowMessage(string message)
    {
        Debug.Log(message);
        DebugText.Instance.AppendLine(message);
    }

}
