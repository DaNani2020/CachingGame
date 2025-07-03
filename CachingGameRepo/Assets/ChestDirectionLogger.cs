using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestDirectionLogger : MonoBehaviour
{
    public Transform trackerChest;

    private Quaternion initialRotation;
    private Quaternion thisInitialRotation;

    // Start is called before the first frame update
    void Start()
    {
        if (trackerChest == null)
        {
            trackerChest = GameObject.FindGameObjectWithTag("ChestTracker").transform.childCount > 0 ? GameObject.FindGameObjectWithTag("ChestTracker").transform.GetChild(0).transform : GameObject.FindGameObjectWithTag("ChestTracker").transform;
        }
        initialRotation = trackerChest.rotation;
        thisInitialRotation = transform.rotation;
        
    }

    // Update is called once per frame
    void Update()
    {
        Quaternion chestRotation = Quaternion.Inverse(initialRotation) * trackerChest.rotation;
        Quaternion thisRotation = Quaternion.Inverse(thisInitialRotation) * transform.rotation;

        //ShowMessage("Chest Rotation: "+ thisRotation.eulerAngles + ", Chest Rotation normal:" + trackerChest.rotation.eulerAngles);
        
    }


    public void measureChestRotation(Transform trackerChest)
    {
        Vector3 angle = trackerChest.transform.eulerAngles;
        float x = angle.x;
        float y = angle.y;
        float z = angle.z;

        if (Vector3.Dot(transform.up, Vector3.up) >= 0f)
        {
            if (angle.x >= 0f && angle.x <= 90f)
            {
                x = angle.x;
            }
            if (angle.x >= 270f && angle.x <= 360f)
            {
                x = angle.x - 360f;
            }
        }
        if (Vector3.Dot(transform.up, Vector3.up) < 0f)
        {
            if (angle.x >= 0f && angle.x <= 90f)
            {
                x = 180 - angle.x;
            }
            if (angle.x >= 270f && angle.x <= 360f)
            {
                x = 180 - angle.x;
            }
        }
        if (angle.y > 180)
        {
            y = angle.y - 360f;
        }
        if (angle.z > 180)
        {
            z = angle.z - 360f;
        }

        Debug.Log(angle + " :::: " + Mathf.Round(x) + " , " + Mathf.Round(y) + " , " + Mathf.Round(z));
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
