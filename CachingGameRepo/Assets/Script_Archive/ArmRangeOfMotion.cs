using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using Wave.Essence;
using Wave.Native;

public class ArmRangeOfMotion : MonoBehaviour
{

    public GameObject trackerElbowRight = null;
    public GameObject trackerElbowLeft = null;
    public GameObject trackerChest = null;
    public GameObject controllerRight = null;
    public GameObject controllerLeft = null;

    private bool isShoulderVisualized = false;

    private Vector3 chestPosition;
    private Vector3 rightElbowPosition;
    private Vector3 rightControllerPosition;
    private Vector3 rightShoulderPosition;

    private float rightMinAngle = float.MaxValue;
    private float rightMaxAngle = float.MinValue;


    public GameObject[] trackerElbowRightt;

    void Start()
    {
        trackerElbowRightt = GameObject.FindGameObjectsWithTag("ElbowRightTracker");
        foreach (GameObject tracker in trackerElbowRightt)
        {
            ShowMessage("Elbow tracker in ArmRangeOfMotion: "+tracker.name);
        }
        
    }

    void Update()
    {
        // AngleByRotation();
    }

    public void AngleByRotation()
    {
        // Definiere die Referenzrotation basierend auf der Ausrichtung des Brusttrackers
        Quaternion chestRotation = trackerChest ? trackerChest.transform.rotation : Quaternion.identity;

        // Erfasse die Rotationswerte des Oberarmtrackers
        Quaternion upperRightRotation = trackerElbowRight ? trackerElbowRight.transform.rotation : Quaternion.identity;

        // Berechne die relative Rotation zwischen dem Brusttracker und dem Oberarmtracker
        Quaternion relativeRotation = Quaternion.Inverse(chestRotation) * upperRightRotation;

        // Extrahiere die Euler-Winkel aus der relativen Rotation
        Vector3 eulerAngles = relativeRotation.eulerAngles;

        // Je nach deiner Anforderung kannst du den Winkel um die x-, y- oder z-Achse verwenden
        float shoulderX = eulerAngles.x; // Roll
        float shoulderY = eulerAngles.y; // Pitch
        float shoulderZ = eulerAngles.z; // Yaw

        print("shoulderX: " + shoulderX);
        
    }

    private void StaticPositions()
    {
        // Define the body forward vector based on the chest tracker's orientation - take up vector because the top of the tracker is l
        Vector3 chestDirection = trackerChest.transform ? trackerChest.transform.up : Vector3.up;

        // Define the upper arm direction based on the upper arm tracker's orientation
        Vector3 upperArmDirection = trackerElbowRight.transform ? trackerElbowRight.transform.right : Vector3.right;

        // Berechne den Winkel zwischen den beiden Vektoren
        float shoulderAngle = Vector3.Angle(chestDirection, upperArmDirection);
    }


    void OnGripButtonPressed()
    {
        if(trackerChest != null){
            chestPosition = trackerChest.transform.position;
        }
        if (trackerElbowRight != null)
        {
            rightElbowPosition = trackerElbowRight.transform.position;
        }
        if(controllerRight != null)
        {
            rightControllerPosition = controllerRight.transform.position;
        }
    }

    void ShowMessage(string message)
    {
        Debug.Log(message);
        DebugText.Instance.AppendLine(message);
    }

}
