using System;
using Unity.VisualScripting;
using UnityEngine;
using Wave.Essence;
using Wave.Native;

public class UpperLimbAngleCalculator : MonoBehaviour
{

    // Singleton instance
    public static UpperLimbAngleCalculator Instance { get; private set; }

    // Variables for Chest
    public Transform trackerChest; // The reference point, e.g., the spine or chest
    private Quaternion initialRotation;

    // Variables for Right Upper Limb
    public Transform trackerElbowRight; // The right elbow tracker
    public Transform controllerRight; // The underlying tracker object of the right controller
    private float verticalShoulderAngleRightLastValid = 0f;
    private float horizontalShoulderAngleRightLastValid = 0f;
    private float shoulderRotationRightLastValid = 0f;
    private float elbowAngleRightLastValid = 0f;
    private float elbowSupinationRightLastValid = 0f;

    // Variables for Left Upper Limb
    public Transform trackerElbowLeft; // The left elbow tracker
    public Transform controllerLeft; // The underlying tracker object of the left controller
    private float verticalShoulderAngleLeftLastValid = 0f;
    private float horizontalShoulderAngleLeftLastValid = 0f;
    private float shoulderRotationLeftLastValid = 0f;
    private float elbowAngleLeftLastValid = 0f;
    private float elbowSupinationLeftLastValid = 0f;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this; // Assign the current object to the static Instance
            DontDestroyOnLoad(gameObject); // Keep this object across scenes
        }
        else
        {
            Destroy(gameObject); // Destroy duplicates if another instance already exists
        }
    }

    void Start()
    {
        // Chest
        if (trackerChest == null)
        {
            trackerChest = GameObject.FindGameObjectWithTag("ChestTracker").transform.childCount > 0 ? GameObject.FindGameObjectWithTag("ChestTracker").transform.GetChild(0).transform : GameObject.FindGameObjectWithTag("ChestTracker").transform;
        }

        // Right Upper Limb
        if (trackerElbowRight == null)
        {
            trackerElbowRight = GameObject.FindGameObjectWithTag("ElbowRightTracker").transform.childCount > 0 ? GameObject.FindGameObjectWithTag("ElbowRightTracker").transform.GetChild(0).transform : GameObject.FindGameObjectWithTag("ElbowRightTracker").transform;
        }
        if (controllerRight == null)
        {
            controllerRight = GameObject.FindGameObjectWithTag("DominantTracker").transform;
        }

        // Left Upper Limb
        if (trackerElbowLeft == null)
        {
            trackerElbowLeft = GameObject.FindGameObjectWithTag("ElbowLeftTracker").transform.childCount > 0 ? GameObject.FindGameObjectWithTag("ElbowLeftTracker").transform.GetChild(0).transform : GameObject.FindGameObjectWithTag("ElbowLeftTracker").transform;
        }
        if (controllerLeft == null)
        {
            controllerLeft = GameObject.FindGameObjectWithTag("NonDominantTracker").transform;
        }
    }

    void Update()
    {

        if (WXRDevice.ButtonPress(WVR_DeviceType.WVR_DeviceType_Controller_Left, WVR_InputId.WVR_InputId_Alias1_Y))
        {
            // Chest
            // ChestRotationMeasurement(trackerChest); 

            // Right Upper Limb
            // angleShoulderHorizontalRight(trackerElbowRight, trackerChest); // calculate shoulder angle - moving on z-axis (horizontal)
            // angleShoulderVerticalRight(trackerElbowRight, trackerChest); // calculate shoulder angle - moving on y-axis (vertical) - implemented
            // shoulderRotationRight(trackerElbowRight, trackerChest); // calculate shoulder rotation angle
            // angleElbowExtensionRight(trackerElbowRight, controllerRight); // calculate elbow angle
            // elbowSupinationRight(trackerElbowRight, controllerRight); // calculate elbow supination angle (rotation of the lower arm around the upper arm)

            // Left Upper Limb
            // angleShoulderHorizontalLeft(trackerElbowLeft, trackerChest); // calculate shoulder angle - moving on z-axis (horizontal)
            angleShoulderVerticalLeft(trackerElbowLeft, trackerChest); // calculate shoulder angle - moving on y-axis (vertical) - implemented
            // shoulderRotationLeft(trackerElbowLeft, trackerChest); // calculate shoulder rotation angle
            // angleElbowExtensionLeft(trackerElbowLeft, controllerLeft); // calculate elbow angle
            // elbowSupinationLeft(trackerElbowLeft, controllerLeft); // calculate elbow supination angle (rotation of the lower arm around the upper arm)
        }

        if (WXRDevice.ButtonPress(WVR_DeviceType.WVR_DeviceType_Controller_Right, WVR_InputId.WVR_InputId_Alias1_B))
        {
            // initialRotation = trackerChest.rotation;
            // Chest
            // ChestRotationMeasurement(trackerChest);

            // Right Upper Limb
            // angleShoulderHorizontalRight(trackerElbowRight, trackerChest); // calculate shoulder angle - moving on z-axis (horizontal)
            angleShoulderVerticalRight(trackerElbowRight, trackerChest); // calculate shoulder angle - moving on y-axis (vertical) - implemented
            // shoulderRotationRight(trackerElbowRight, trackerChest); // calculate shoulder rotation angle
            // angleElbowExtensionRight(trackerElbowRight, controllerRight); // calculate elbow angle
            // elbowSupinationRight(trackerElbowRight, controllerRight); // calculate elbow supination angle (rotation of the lower arm around the upper arm)

            // Left Upper Limb
            // angleShoulderHorizontalLeft(trackerElbowLeft, trackerChest); // calculate shoulder angle - moving on z-axis (horizontal)
            // angleShoulderVerticalLeft(trackerElbowLeft, trackerChest); // calculate shoulder angle - moving on y-axis (vertical) - implemented
            // shoulderRotationLeft(trackerElbowLeft, trackerChest); // calculate shoulder rotation angle
            // angleElbowExtensionLeft(trackerElbowLeft, controllerLeft); // calculate elbow angle
            // elbowSupinationLeft(trackerElbowLeft, controllerLeft); // calculate elbow supination angle (rotation of the lower arm around the upper arm)
        }
        
        
    }
#region Chest Methods

    public Vector3 ChestRotationMeasurement(Transform trackerChest)
    {
        // Get the relative rotation of the chest compared to the initial rotation
        Quaternion chestRotation = Quaternion.Inverse(initialRotation) * trackerChest.rotation;
        Vector3 eulerChest = chestRotation.eulerAngles;

        // Ensure that each angle is within the -180 to 180 degree range
        if (eulerChest.x > 180) eulerChest.x -= 360; // Turning body to the left == positive rotation around y-axis; Turning body to the right == negative rotation around y-axis
        if (eulerChest.y > 180) eulerChest.y -= 360; // Leaning body forward == negative rotation around x-axis; Leaning body backward == positive rotation around x-axis
        if (eulerChest.z > 180) eulerChest.z -= 360; // Tilting head to the left == negative rotation around z-axis; Tilting head to the right == positive rotation around z-axis

        // Return the angles as a Vector3
        // ShowMessage("Chest Rotation normal:" + eulerChest);
        return eulerChest;
    }

#endregion


#region Right Upper Limb Methods

    public (float angle, Vector3 horizontalChestDirection, Vector3 horizontalElbowDirection) angleShoulderHorizontalRight(Transform trackerElbowRight, Transform trackerChest)
    {
        
        // Define the body forward vector based on the chest tracker's orientation - Directions to compute the horizontal shoulder angle (movement on Z-axis)
        Vector3 horizontalChestDirection = -trackerChest.up; 
        Vector3 horizontalElbowDirection = trackerElbowRight.right;

        // Project the vectors onto the x-z plane (ignore y-component)
        horizontalChestDirection.y = 0;
        horizontalElbowDirection.y = 0;

        // Normalize the vectors to ensure consistent angle calculation
        // horizontalChestDirection.Normalize();
        // horizontalElbowDirection.Normalize();


        float horizontalShoulderAngleRight = Vector3.Angle(horizontalChestDirection, horizontalElbowDirection); // Declares also the direction of the angle and divides the body parts in positive and negative angles

        // Use the cross product to determine the direction of the angle
        Vector3 reference = Vector3.Cross(horizontalChestDirection, horizontalElbowDirection);
        
        if (reference.y >= 0)
        {
            horizontalShoulderAngleRight = -horizontalShoulderAngleRight;
        }

        // ShowMessage("Right Horizontal Shoulder Angle: " + horizontalShoulderAngleRight + ", Reference: " + reference);
        return (horizontalShoulderAngleRight, horizontalChestDirection, horizontalElbowDirection);
    }

    public (float angle, Vector3 verticalChestDirection, Vector3 verticalElbowDirection) angleShoulderVerticalRight(Transform trackerElbowRight, Transform trackerChest)
    {

        // Define the body forward vector based on the chest tracker's orientation - Directions to compute the vertical shoulder angle (movement on y-axis)
        Vector3 verticalChestDirection = trackerChest.right;
        Vector3 verticalElbowDirection = trackerElbowRight.right;

        // verticalChestDirection.Normalize();
        // verticalElbowDirection.Normalize();

        float verticalShoulderAngleRight = Vector3.Angle(verticalChestDirection, verticalElbowDirection); // Declares also the direction of the angle and divides the body parts in positive and negative angles
        // float reference = trackerElbowRight.rotation.eulerAngles.x;
        Vector3 reference = Vector3.Cross(verticalChestDirection, verticalElbowDirection); // reference not needed for the calculation of the angle

        // if (reference.z <= 0)
        // {
        //     verticalShoulderAngleRight = -verticalShoulderAngleRight;
        // }

        // ShowMessage("Vertical Shoulder Angle: " + verticalShoulderAngleRight + ", Reference: " + reference);
        return (verticalShoulderAngleRight, verticalChestDirection, verticalElbowDirection);


    }

    public (float angle, Vector3 shoulderDirection, Vector3 elbowDirection) shoulderRotationRight(Transform trackerElbowRight, Transform trackerChest)
    {
        Vector3 shoulderDirection = -trackerChest.forward;
        Vector3 elbowDirection = trackerElbowRight.up;

        float shoulderRotationRight = Vector3.Angle(shoulderDirection, elbowDirection);
        // float reference = trackerElbowRight.rotation.eulerAngles.z;
        Vector3 reference = Vector3.Cross(shoulderDirection, elbowDirection);

        if(reference.x >= 0)
        {
            shoulderRotationRight = -shoulderRotationRight;
        }

        // ShowMessage("ShoulderRotationRight: " + shoulderRotationRight + ", Reference: " + reference);
        return (shoulderRotationRight, shoulderDirection, elbowDirection);
    }


    public (float angle, Vector3 upperLimbDirection, Vector3 lowerLimbDirection) angleElbowExtensionRight(Transform trackerElbowRight, Transform controllerRight)
    {
        Vector3 upperLimbDirection = trackerElbowRight.up;
        Vector3 lowerLimbDirection = controllerRight.right;

        float elbowAngleRight = Vector3.Angle(upperLimbDirection, lowerLimbDirection);
        float reference = lowerLimbDirection.x;
        Vector3 reference2 = Vector3.Cross(upperLimbDirection, lowerLimbDirection); // Reference not needed here

        if(elbowAngleRight <= 180 && elbowAngleRight >= 0 ) // && reference < 0
        {
            // ShowMessage("ElbowAngleRight: " + elbowAngleRight + ", Reference: " + reference + ", Reference2 cross: " + reference2);
            elbowAngleRightLastValid = elbowAngleRight;
            return (elbowAngleRight, upperLimbDirection, lowerLimbDirection);
        }
        else
        {
            // ShowMessage("ElbowAngleRight: " + elbowAngleRightLastValid + ", Reference: " + reference + ", Reference2 cross: " + reference2);
            return (elbowAngleRightLastValid, upperLimbDirection, lowerLimbDirection);
            throw new InvalidOperationException("The elbow extension angle can only be measured between 0 and 180 degrees.");
        }
    }

    public (float angle, Vector3 upperLimbDirection, Vector3 lowerLimbDirection ) elbowSupinationRight(Transform trackerElbowRight, Transform controllerRight) // to implement - or to check if it is correct
    {
        float elbowSupinationRightOutOfBoundsSmall = 0f;
        float elbowSupinationRightOutOfBoundsLarge = 180f;
        Vector3 upperLimbDirection = trackerElbowRight.forward;
        Vector3 lowerLimbDirection = -controllerRight.up;



        float elbowSupinationRight = Vector3.Angle(upperLimbDirection, lowerLimbDirection);
        Vector3 reference = Vector3.Cross(upperLimbDirection, lowerLimbDirection);

        if(reference.x >= 0)
        {
            elbowSupinationRight = -elbowSupinationRight;
        }

        if(elbowSupinationRight < 0 )
        {
            if(elbowSupinationRight < -90f)
            {
                // ShowMessage("ElbowSupinationRight: " + elbowSupinationRightOutOfBoundsLarge + ", Reference: " + reference);
                return (elbowSupinationRightOutOfBoundsLarge, upperLimbDirection, lowerLimbDirection);
            }else 
            {
                // ShowMessage("ElbowSupinationRight: " + elbowSupinationRightOutOfBoundsSmall + ", Reference: " + reference);
                return (elbowSupinationRightOutOfBoundsSmall, upperLimbDirection, lowerLimbDirection);
            }
        }
        else 
        {
            // ShowMessage("ElbowSupinationRight: " + elbowSupinationRight + ", Reference: " + reference);
            return (elbowSupinationRight, upperLimbDirection, lowerLimbDirection);
        }
    }

#endregion


#region Left Upper Limb Methods
    
    public (float angle, Vector3 horizontalChestDirection, Vector3 horizontalElbowDirection) angleShoulderHorizontalLeft( Transform trackerElbowLeft, Transform trackerChest)
    {
        
        // Define the body forward vector based on the chest tracker's orientation - Directions to compute the horizontal shoulder angle (movement on Z-axis)
        Vector3 horizontalChestDirection = trackerChest.up; 
        Vector3 horizontalElbowDirection = trackerElbowLeft.right;

        // Project the vectors onto the x-z plane (ignore y-component)
        horizontalChestDirection.y = 0;
        horizontalElbowDirection.y = 0;

        // Normalize the vectors to ensure consistent angle calculation
        // horizontalChestDirection.Normalize();
        // horizontalElbowDirection.Normalize();

        float horizontalShoulderAngleLeft = Vector3.Angle(horizontalChestDirection, horizontalElbowDirection); // Declares also the direction of the angle and divides the body parts in positive and negative angles
        Vector3 reference = Vector3.Cross(horizontalChestDirection, horizontalElbowDirection);

        if (reference.y <= 0)
        {
            horizontalShoulderAngleLeft = -horizontalShoulderAngleLeft;
        }

        // ShowMessage("Left Horizontal Shoulder Angle: " + horizontalShoulderAngleLeft + ", Reference: " + reference);
        return (horizontalShoulderAngleLeft, horizontalChestDirection, horizontalElbowDirection);
    }

    public (float angle, Vector3 verticalChestDirection, Vector3 verticalElbowDirection) angleShoulderVerticalLeft(Transform trackerElbowLeft, Transform trackerChest)
    {
        // Save last value of the vertical shoulder angle and if indexout of bounds save last valid value in the db
        // Define the body forward vector based on the chest tracker's orientation - Directions to compute the vertical shoulder angle (movement on y-axis)
        Vector3 verticalChestDirection = trackerChest.right;
        Vector3 verticalElbowDirection = trackerElbowLeft.right;

        // verticalChestDirection.Normalize();
        // verticalElbowDirection.Normalize();

        float verticalShoulderAngleLeft = Vector3.Angle(verticalChestDirection, verticalElbowDirection); // Declares also the direction of the angle and divides the body parts in positive and negative angles
        // float reference = trackerElbowLeft.rotation.eulerAngles.x;
        Vector3 reference = Vector3.Cross(verticalChestDirection, verticalElbowDirection); // reference not needed for the calculation of the angle
        
        // if(reference.z >= 0)
        // {
        //     verticalShoulderAngleLeft = -verticalShoulderAngleLeft;
        // }

        // ShowMessage("Left Vertical Shoulder Angle: " + verticalShoulderAngleLeft+ ", Reference: " + reference);
        return (verticalShoulderAngleLeft, verticalChestDirection, verticalElbowDirection);
    }

    public (float angle, Vector3 shoulderDirection, Vector3 elbowDirection) shoulderRotationLeft(Transform trackerElbowLeft, Transform trackerChest)
    {
        Vector3 shoulderDirection = trackerChest.forward;
        Vector3 elbowDirection = trackerElbowLeft.up;

        float shoulderRotationLeft = Vector3.Angle(shoulderDirection, elbowDirection);
        float reference = trackerElbowLeft.rotation.eulerAngles.z;
        Vector3 reference2 = Vector3.Cross(shoulderDirection, elbowDirection);

        if(reference2.x >= 0)
        {
            shoulderRotationLeft = -shoulderRotationLeft;
        }

        // ShowMessage("Left ShoulderRotation: " + shoulderRotationLeft + ", Reference: " + reference + ", Reference2 cross: " + reference2);
        return (shoulderRotationLeft, shoulderDirection, elbowDirection);
    }



    public (float angle,  Vector3 upperLimbDirection, Vector3 lowerLimbDirection) angleElbowExtensionLeft(Transform trackerElbowLeft, Transform controllerLeft)
    {
        Vector3 upperLimbDirection = trackerElbowLeft.up;
        Vector3 lowerLimbDirection = controllerLeft.right;

        float elbowAngleLeft = Vector3.Angle(upperLimbDirection, lowerLimbDirection);
        float reference = lowerLimbDirection.x;
        Vector3 reference2 = Vector3.Cross(upperLimbDirection, lowerLimbDirection); // Reference not needed here

        if(elbowAngleLeft <= 180 && elbowAngleLeft >= 0 ) // && reference < 0
        {
            elbowAngleLeftLastValid = elbowAngleLeft;   
            // ShowMessage("Left ElbowAngle: " + elbowAngleLeft + ", Reference: " + reference + ", Reference2 cross: " + reference2);
            return (elbowAngleLeft, upperLimbDirection, lowerLimbDirection);
        }
        else
        {
            // ShowMessage("Left ElbowAngle: " + elbowAngleLeftLastValid + ", Reference: " + reference + ", Reference2 cross: " + reference2);
            return (elbowAngleLeftLastValid, upperLimbDirection, lowerLimbDirection);
            throw new InvalidOperationException("The elbow extension angle can only be measured between 0 and 180 degrees.");
        }
    }


// TODO implement reference
    public (float angle, Vector3 upperLimbDirection, Vector3 lowerLimbDirection) elbowSupinationLeft(Transform trackerElbowLeft, Transform controllerLeft) // to implement - or to check if it is correct
    {

        float elbowSupinationLeftOutOfBoundsSmall = 0f;
        float elbowSupinationLeftOutOfBoundsLarge = 180f;
        Vector3 upperLimbDirection = trackerElbowLeft.forward;
        Vector3 lowerLimbDirection = -controllerLeft.up;

        float elbowSupinationLeft = Vector3.Angle(upperLimbDirection, lowerLimbDirection);
        Vector3 reference = Vector3.Cross(upperLimbDirection, lowerLimbDirection);

        if(reference.x >= 0)
        {
            elbowSupinationLeft = -elbowSupinationLeft;
        }

        if(elbowSupinationLeft < 0 )
        {
            if(elbowSupinationLeft < -90f)
            {
                // ShowMessage("ElbowSupinationLeft: " + elbowSupinationLeftOutOfBoundsLarge + ", Reference: " + reference);
                return (elbowSupinationLeftOutOfBoundsLarge, upperLimbDirection, lowerLimbDirection);
            }else 
            {
                // ShowMessage("ElbowSupinationLeft: " + elbowSupinationLeftOutOfBoundsSmall + ", Reference: " + reference);
                return (elbowSupinationLeftOutOfBoundsSmall, upperLimbDirection, lowerLimbDirection);
            }
        }
        else 
        {
            // ShowMessage("ElbowSupinationLeft: " + elbowSupinationLeft + ", Reference: " + reference);
            return (elbowSupinationLeft, upperLimbDirection, lowerLimbDirection);
        }
    }

#endregion

    public void SetInitialChestRotation(Quaternion rotation)
    {
        initialRotation = rotation;
    }


    void ShowMessage(string message)
    {

        Debug.Log(message);
        try
        {
            DebugText.Instance.AppendLine(message);
        }
        catch
        {
            Debug.Log("DebugText not found or disabled");
        }

    }

}
