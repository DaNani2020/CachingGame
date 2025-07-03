using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text;
using Wave.Native;
using Wave.Essence.BodyTracking;

public class InitTracking : MonoBehaviour
{
    public HumanoidTracking humanoidTracking = null;
    public Button beginBtn = null;
    public GameObject[] doNotRenderTheseObjects = null;

    void Start()
    {
        // BeginTracking();
    }

    void Update()
    {

    }

    public void BeginTracking()
    {
        if (humanoidTracking != null)
        {
            if (beginBtn != null) { beginBtn.interactable = false; }
            if (doNotRenderTheseObjects != null)
            {
                for (int i = 0; i < doNotRenderTheseObjects.Length; i++)
                {
                    doNotRenderTheseObjects[i].GetComponent<SkinnedMeshRenderer>().enabled = false;
                    Debug.Log($"Skinned Mesh Renderer at gameobject {doNotRenderTheseObjects[i].name} is active: {doNotRenderTheseObjects[i].GetComponent<SkinnedMeshRenderer>().enabled}.");
                }
            }
            humanoidTracking.Tracking = HumanoidTracking.TrackingMode.Arm;
            // humanoidTracking.Tracking = HumanoidTracking.TrackingMode.UpperBody;
            // humanoidTracking.Tracking = HumanoidTracking.TrackingMode.FullBody;
            humanoidTracking.BeginTracking();
            Debug.Log("Tracking Started.");
        }
    }
    public void EndTracking()
    {
        if (humanoidTracking != null)
        {
            humanoidTracking.StopTracking();
        }
    }
}

